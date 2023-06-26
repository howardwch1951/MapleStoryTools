using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MapleStoryTools
{
    public partial class frmColorSelecter : Form
    {
        public Color color;
        public frmColorSelecter()
        {
            InitializeComponent();
        }

        private void picRGB_MouseMove(object sender, MouseEventArgs e)
        {
            try
            {
                Bitmap pixelData = (Bitmap)picRgb.Image;
                Color clr = pixelData.GetPixel(e.X, e.Y);
                labSmallScreen.BackColor = clr;
            }
            catch
            {

            }
        }

        private void picRGB_MouseDown(object sender, MouseEventArgs e)
        {
            Bitmap pixelData = (Bitmap)picRgb.Image;
            Color clr = pixelData.GetPixel(e.X, e.Y);
            labRgbValue.Text = $"#{clr.R:X}{clr.G:X}{clr.B:X}";
            labRedValue.Text = clr.R.ToString();
            labGreenValue.Text = clr.G.ToString();
            labBlueValue.Text = clr.B.ToString();
            pnlSelectedScreen.BackColor = clr;
        }
    }
}
