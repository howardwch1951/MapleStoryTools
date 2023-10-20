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
    public partial class frmUpdateMessageBox : Form
    {
        public string message = "";
        public frmUpdateMessageBox()
        {
            InitializeComponent();
        }

        public override void Refresh()
        {
            labMessage.Text = message;
            base.Refresh();
        }

        private void timerUpdateLabel_Tick(object sender, EventArgs e)
        {
            switch (DateTime.Now.Second % 6)
            {
                case 0:
                    labMessage.Text = message;
                    break;
                case 1:
                    labMessage.Text = $"{message}.";
                    break;
                case 2:
                    labMessage.Text = $"{message}..";
                    break;
                case 3:
                    labMessage.Text = $"{message}....";
                    break;
                case 4:
                    labMessage.Text = $"{message}.....";
                    break;
                case 5:
                    labMessage.Text = $"{message}......";
                    break;
            }
        }

        private void frmUpdateMessageBox_Move(object sender, EventArgs e)
        {
            if (Application.OpenForms["frmMain"] != null)
            {
                Application.OpenForms["frmMain"].StartPosition = FormStartPosition.Manual;
                Application.OpenForms["frmMain"].Location = new Point(
                    this.Location.X - (Application.OpenForms["frmMain"].Size.Width / 2 - this.Width / 2),
                    this.Location.Y - (Application.OpenForms["frmMain"].Size.Height / 2 - this.Height / 2));
            }
        }
    }
}
