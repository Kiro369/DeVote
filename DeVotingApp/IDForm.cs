using DeVote.PyRecognition;
using MetroFramework.Forms;
using OpenCvSharp.Extensions;
using System.Drawing.Text;

namespace DeVotingApp
{
    public partial class IDForm : MetroForm
    {
        readonly PrivateFontCollection KMRFont = new();
        readonly OpenCvSharp.VideoCapture _capture = new(0); //new("http://192.168.1.2:4747/video");
        private Thread _cameraThread;
        readonly OpenCvSharp.Mat _image = new();

        public IDForm()
        {
            InitializeComponent();
            Bounds = Screen.PrimaryScreen.Bounds;
            ControlBox = false;
            AutoScaleMode = AutoScaleMode.Dpi;
            CustomHeader();
            AdjustComponents();
        }

        private void AdjustComponents()
        {
            titleLabel.Location = new Point(Width / 2 - titleLabel.Width / 2, Height / 15 / 3);

            pictureBox1.Size = new Size(Width / 2, (int)((Width / 2) * 0.635));
            pictureBox1.Location = new Point(Width / 3 - pictureBox1.Width / 2, (int)(Height / 6.75));


            pictureBox2.Size = new Size((int)((Width / 3) * 0.635), (int)(pictureBox1.Height * 0.80));
            pictureBox2.Location = new Point(pictureBox1.Location.X + pictureBox1.Width / 2 + Width / 3, Height / 10);

            label1.Font = new Font(KMRFont.Families[0], 30);
            label1.Location = new Point(pictureBox2.Location.X, pictureBox2.Location.Y + Height / 10 + pictureBox2.Height);
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
        private void metroButton1_Click(object sender, EventArgs e)
        {
            _capture.Read(_image);
            if (_image.Empty()) return;
            pictureBox1.BackgroundImage = BitmapConverter.ToBitmap(_image);
            pictureBox1.BackgroundImageLayout = ImageLayout.Stretch;
        }

        private void IDForm_Load(object sender, EventArgs e)
        {
            _cameraThread = new Thread(new ThreadStart(CaptureCameraCallback));
            _cameraThread.Start();

        }

        private void CaptureCameraCallback()
        {
            while (true)
            {
                try
                {
                    _capture.Read(_image);
                    if (_image.Empty()) return;
                    var imageRes = new OpenCvSharp.Mat();
                    OpenCvSharp.Cv2.Resize(_image, imageRes, new OpenCvSharp.Size(pictureBox1.Height, pictureBox1.Width));
                    var bmpWebCam = BitmapConverter.ToBitmap(imageRes);
                    pictureBox1.Image = bmpWebCam;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void ExtractInfo(out dynamic info)
        {
            info = null;
            //Recognition.Current.ExtractIDInfo
        }
    }
}
