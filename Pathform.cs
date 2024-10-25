using System;
using System.Drawing;
using System.Windows.Forms;

namespace math8
{
    public partial class PathForm : Form
    {
        private Bitmap pathMap;
        public PathForm(Bitmap map)
        {
            this.pathMap = map;
            this.DoubleBuffered = true;
            this.Width = 450;
            this.Height = 450;
            this.Text = "Path Trace";
            this.Paint += new PaintEventHandler(OnPaint);
        }
        private void OnPaint(object sender, PaintEventArgs e)
        {
            e.Graphics.DrawImage(pathMap, 0, 0);
        }
    }
}