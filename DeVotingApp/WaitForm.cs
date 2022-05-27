using MetroFramework.Forms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DeVotingApp
{
    public partial class WaitForm : MetroForm
    {
        readonly PrivateFontCollection KMRFont = new();
        public WaitForm()
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


            pictureBox1.Size = new Size(Width / 3, (int)(pictureBox1.Height / 1.5));
            pictureBox1.Location = new Point(Width / 2 - pictureBox1.Width / 2, Height / 5);

            label1.Font = new Font(KMRFont.Families[0], 30);
            label1.Location = new Point(Width / 2 - label1.Width / 2, pictureBox1.Location.Y + Height / 10 + pictureBox1.Height);
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

        DateTime d;
        private void WaitForm_Load(object sender, EventArgs e)
        {
            d = DateTime.Now.AddSeconds(5);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (DateTime.Now > d)
            {
                Hide();
                var form2 = new FaceRecognition("E:/front_id.jpg");
                form2.Closed += (s, args) => Close();
                form2.Show();
                form2.Activate();
                timer1.Enabled = false;
            }
        }
    }
}
