using MetroFramework.Forms;
using System.Drawing.Text;

namespace DeVotingApp
{
    public partial class Main : MetroForm
    {
        readonly PrivateFontCollection KMRFont = new();

        public Main()
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

            pictureBox1.Size = new Size(Width / 5, Height / 3);
            pictureBox1.Location = new Point(Width / 2 - pictureBox1.Width / 2, pictureBox1.Location.Y);

            processLabel.Font = new Font(KMRFont.Families[0], 30);
            processLabel.Location = new Point(Width / 2 - processLabel.Width / 2, pictureBox1.Location.Y + (int)(Height / 2.75));

            startButton.Font = new Font(KMRFont.Families[0], 30);
            startButton.Size = new Size(Width / 10, Height / 20);
            startButton.Location = new Point(Width / 2 - startButton.Width / 2, processLabel.Location.Y + (int)(Height / 15));
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

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            using Brush b = new SolidBrush(Color.Red);
            int borderWidth = Height / 15;
            e.Graphics.FillRectangle(b, 0, 0, Width, borderWidth);
        }

        private void startButton_Click(object sender, EventArgs e)
        {
            Hide();
            var form2 = new IDForm();
            form2.Closed += (s, args) => Show();
            form2.Show();
            form2.Activate();
        }
    }
}