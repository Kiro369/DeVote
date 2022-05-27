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
    public partial class VotingForm : MetroForm
    {
        readonly PrivateFontCollection KMRFont = new();
        public VotingForm()
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

            pictureBox1.Size = new Size(Width / 3, (int)((Width / 2) * 0.635));
            pictureBox1.Location = new Point((Width - pictureBox1.Width + pictureBox2.Width + Width / 10) / 2 - pictureBox1.Width, (int)(Height / 6.75));
            
            pictureBox2.Size = pictureBox1.Size;
            pictureBox2.Location = new Point(pictureBox1.Location.X + pictureBox1.Width + Width / 10, pictureBox1.Location.Y);

            metroButton1.Size = metroButton2.Size = new Size(Width / 15, Height / 15);

            metroButton1.Location = new Point(pictureBox1.Location.X + pictureBox1.Width / 2 - metroButton1.Width / 2, pictureBox1.Location.Y + pictureBox1.Height + Height / 30);
            metroButton2.Location = new Point(pictureBox2.Location.X + pictureBox2.Width / 2 - metroButton2.Width / 2, pictureBox2.Location.Y + pictureBox2.Height + Height / 30);

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
    }
}
