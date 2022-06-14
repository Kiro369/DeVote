using MetroFramework.Forms;
using OpenCvSharp.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DeVotingApp
{
    public partial class FaceRecognition : MetroForm
    {
        readonly PrivateFontCollection KMRFont = new();
        readonly OpenCvSharp.VideoCapture _capture = new("http://192.168.1.2:8080/video");
        private Thread _cameraThread;
        readonly OpenCvSharp.Mat _image = new();
        BitmapContainer ImageContainer;
        byte Verifications, Tries, RecognitionState;
        IDInfo Info;
        public FaceRecognition(IDInfo info)
        {
            Info = info;
            InitializeComponent();
            Bounds = Screen.PrimaryScreen.Bounds;
            ControlBox = false;
            AutoScaleMode = AutoScaleMode.Dpi;

            firstName.Text = info.Name;
            lastName.Text = info.LastName;
            address.Text = info.Address;
            national_id.Text = info.ID;

            CustomHeader();
            AdjustComponents();
        }
        private void AdjustComponents()
        {
            titleLabel.Location = new Point(Width / 2 - titleLabel.Width / 2, Height / 15 / 3);

            pictureBox1.Size = new Size(Width / 2, (int)((Width / 2) * 0.635));
            pictureBox1.Location = new Point(Width / 3 - pictureBox1.Width / 2, (int)(Height / 6.75));

            //pictureBox2.Size = new Size(Width / 4, Height / 10 * 3);
            pictureBox2.Size = new Size(200, 200);
            pictureBox2.Location = new Point(Width / 2 - pictureBox1.Width / 2 - pictureBox2.Width / 2, pictureBox1.Location.Y + pictureBox1.Height);

            var labels = new Label[] { label1, firstName, label2, lastName, label3, address, label4, national_id };

            var lastY = pictureBox1.Location.Y - Height / 20;
            foreach (var label in labels)
            {
                if (label.Name.Contains("label"))
                    label.ForeColor = Color.Gray;
                label.RightToLeft = RightToLeft.Yes;
                label.Font = new Font(KMRFont.Families[0], 30);
                lastY = lastY + Height / 15;
                label.Location = new Point(Width / 10 * 8 + 15 - label.Width, lastY);
            }
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            using Brush b = new SolidBrush(Color.Red);
            int borderWidth = Height / 15;
            e.Graphics.FillRectangle(b, 0, 0, Width, borderWidth);
        }
        public void CustomHeader()
        {
            titleLabel.BackColor = Color.Red;
            titleLabel.ForeColor = Color.White;

            int fontLength = Properties.Resources.KMR.Length;

            // create a buffer to read in to
            byte[] fontdata = Properties.Resources.KMR;

            // create an unsafe memory block for the font data
            System.IntPtr data = System.Runtime.InteropServices.Marshal.AllocCoTaskMem(fontLength);

            // copy the bytes to the unsafe memory block
            System.Runtime.InteropServices.Marshal.Copy(fontdata, 0, data, fontLength);

            // pass the font to the font collection
            KMRFont.AddMemoryFont(data, fontLength);

            titleLabel.Font = new Font(KMRFont.Families[0], 18);
        }

        private void CaptureCameraCallback()
        {
            while (true)
            {
                try
                {
                    _capture.Read(_image);
                    if (_image.Empty()) return;
                    ImageContainer = new BitmapContainer(BitmapConverter.ToBitmap(_image));
                    pictureBox1.Image = ImageContainer.ToBitmap();
                    //pictureBox1.Image.RotateFlip(RotateFlipType.RotateNoneFlipX);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void FaceRecognition_Load(object sender, EventArgs e)
        {
            _cameraThread = new Thread(new ThreadStart(CaptureCameraCallback));
            _cameraThread.Start();

            if (true)
            {
                new Thread(new ThreadStart(Reco)).Start();
            }
            else
                RecognitionState = 1;

        }
        void Reco()
        {
            try
            {
                Paths = DeVote.PyRecognition.Recognition.Current.VerifyPerson(0, Info.FrontIDPath, 10);
                if (Paths.Count > 0)
                {
                    RecognitionState = 2;
                }
                else RecognitionState = 3;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }
        bool CanVerify = true;
        System.Diagnostics.Stopwatch sp = new();
        List<string> Paths = new List<string>();
        void Recognize()
        {
            if (CanVerify)
            {
                CanVerify = false;
                if (Tries >= 10 || Verifications >= 6)
                {
                    if (Verifications >= 6)
                        RecognitionState = 2;
                    else
                        RecognitionState = 3;
                }
                else
                {
                    var path = Directory.GetCurrentDirectory() + "\\" + $"CurrentImage{Tries}.jpg";
                    while (true)
                    {
                        try
                        {
                            ImageContainer.ToBitmap().Save(path, ImageFormat.Jpeg);
                            break;
                        }
                        catch { }
                    }
                    sp.Restart();
                    var verified = DeVote.PyRecognition.Recognition.Current.VerifyVoter(Info.FrontIDPath, path);
                    sp.Stop();
                    if (verified) {
                        Verifications++;
                        Paths.Add(path);
                    }
                    Tries++;
                    CanVerify = true;
                }
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (RecognitionState == 1)
            {
                Task.Factory.StartNew(() => Recognize());
                titleLabel.Text = $"Decentralized Voting Application (DeVoting) | {Verifications} / {Tries} in {sp.Elapsed.TotalSeconds}";

            }
            else if (RecognitionState == 2)
            {
                Hide();
                var form2 = new VotingForm(Info, Paths);
                form2.Closed += (s, args) => Close();
                form2.Show();
                form2.Activate();
                timer1.Enabled = false;
            }
            else if (RecognitionState == 3)
            {
                MessageBox.Show("Recognition failed, you can't vote", "Recognition Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Close();
            }
        }
    }
}