using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MapleStoryTools
{
    public partial class frmPoint : Form
    {
        public Point point = new Point();

        public frmPoint()
        {
            InitializeComponent();

            ShowInTaskbar = false;
        }

        /// <summary>
        /// 右上 = "🡵", 右下 = "🡶", 左上 = "🡴", 左下 = "🡷"
        /// </summary>
        /// <param name="pArrow"></param>
        public void SetPoint(string pArrow)
        {
            switch (pArrow)
            {
                case "右上":
                    labArrow.Text = "🡵";
                    labArrow.Location= new Point(137, -9);

                    labPoint.Text = $"({point.X}, {point.Y})";
                    labPoint.BackColor = SystemColors.Control;
                    labPoint.Location = new Point(3, 0);
                    break;
                case "右下":
                    labArrow.Text = "🡶";
                    labArrow.Location = new Point(137, -3);

                    labPoint.Text = $"({point.X}, {point.Y})";
                    labPoint.BackColor = SystemColors.Control;
                    labPoint.Location = new Point(3, 0);
                    break;
                case "左上":
                    labArrow.Text = "🡴";
                    labArrow.Location = new Point(-8, -9);

                    labPoint.Text = $"({point.X}, {point.Y})";
                    labPoint.BackColor = SystemColors.Control;
                    labPoint.Location = new Point(32, 0);
                    break;
                case "左下":
                    labArrow.Text = "🡷";
                    labArrow.Location = new Point(-8, -3);

                    labPoint.Text = $"({point.X}, {point.Y})";
                    labPoint.BackColor = SystemColors.Control;
                    labPoint.Location = new Point(32, 0);
                    break;
            }
            this.Refresh();
            //🡵🡶🡴🡷
            if (true)
            {

            }
        }        
    }
}
