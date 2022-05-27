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
        readonly OpenCvSharp.VideoCapture _capture = new(0); //new("http://192.168.1.2:4747/video");
        private Thread _cameraThread;
        readonly OpenCvSharp.Mat _image = new();
        BitmapContainer ImageContainer;
        string FrontIDPath;
        byte Verifications, Tries, RecognitionState;
        public FaceRecognition(string FrontIDPath)
        {
            InitializeComponent();
            Bounds = Screen.PrimaryScreen.Bounds;
            ControlBox = false;
            AutoScaleMode = AutoScaleMode.Dpi;
            CustomHeader();
            // test data

            firstName.Text = "كيرلس";
            lastName.Text = "تادرس";
            address.Text = "عين شمس";
            national_id.Text = "123456";

            AdjustComponents();

            this.FrontIDPath = FrontIDPath;

            //var verified = DeVote.PyRecognition.Recognition.Current.VerifyVoter(FrontIDPath, Directory.GetCurrentDirectory() + "\\" + $"CurrentImage{0}.jpg");
            //var verified2 = DeVote.PyRecognition.Recognition.Current.VerifyVoter(FrontIDPath, Directory.GetCurrentDirectory() + "\\" + $"CurrentImage{1}.jpg");


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
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            //Close();
        }

        private void FaceRecognition_Load(object sender, EventArgs e)
        {
            _cameraThread = new Thread(new ThreadStart(CaptureCameraCallback));
            _cameraThread.Start();
        }

        bool CanVerify = true;
        System.Diagnostics.Stopwatch sp = new();
        void Recognize()
        {
            if (CanVerify)
            {
                CanVerify = false;
                if (Tries >= 10)
                {
                    if (Verifications >= 6)
                        RecognitionState = 1;
                    else
                        RecognitionState = 2;
                }
                else
                {
                    var path = $"CurrentImage{Tries}.jpg";
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
                    using var _ = DeVote.PyRecognition.Recognition.Current.GIL();
                    var verified = DeVote.PyRecognition.Recognition.Current.VerifyVoter(FrontIDPath, Directory.GetCurrentDirectory() + "\\" + path);
                    sp.Stop();
                    if (verified)
                        Verifications++;
                    Tries++;
                    CanVerify = true;
                }
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (RecognitionState == 0)
            {
                Task.Factory.StartNew(() => Recognize());
                titleLabel.Text = $"{Verifications} / {Tries} in {sp.Elapsed.TotalSeconds.ToString()}";

            }
            else if (RecognitionState == 1)
            {
                Hide();
                var form2 = new VotingForm();
                form2.Closed += (s, args) => Close();
                form2.Show();
                form2.Activate();
                timer1.Enabled = false;
            }
            else
            {
                MessageBox.Show("Recognition failed, you can't vote", "Recognition Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Close();
            }
        }
    }
}
