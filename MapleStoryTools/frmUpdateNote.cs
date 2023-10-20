using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MapleStoryTools
{
    public partial class frmUpdateNote : Form
    {
        public string note = "";
        public frmUpdateNote()
        {
            this.Icon = new Icon(Path.Combine(Application.StartupPath, "Icon.ico"));
            InitializeComponent();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        public override void Refresh()
        {
            richTextBox1.Text = note + Environment.NewLine + Environment.NewLine;
            base.Refresh();
        }
    }
}
