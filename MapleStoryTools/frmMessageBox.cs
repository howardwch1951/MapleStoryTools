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
    public partial class frmMessageBox : Form
    {
        public string title = "";
        public string message = "";
        public string formName = "";
        public MessageBoxIcon icon = new MessageBoxIcon();
        public MessageBoxButtons buttons = new MessageBoxButtons();
        public frmMessageBox()
        {
            this.Icon = new Icon(Path.Combine(Application.StartupPath, "Icon.ico"));

            InitializeComponent();
        }

        public override void Refresh()
        {
            this.Text = title;
            labMessage.Text = message;
            Button btnOK = new Button();
            Button btnCancel = new Button();
            Button btnYes = new Button();
            Button btnNo = new Button();

            // 將 MessageBoxIcon 的圖示轉換為 Image
            Image image = SystemIcons.Information.ToBitmap();
            switch (icon)
            {
                case MessageBoxIcon.Information:
                    image = SystemIcons.Information.ToBitmap();
                    break;
                case MessageBoxIcon.Warning:
                    image = SystemIcons.Warning.ToBitmap();
                    break;
                case MessageBoxIcon.Error:
                    image = SystemIcons.Error.ToBitmap();
                    break;
                case MessageBoxIcon.Question:
                    image = SystemIcons.Question.ToBitmap();
                    break;
            }

            // 將圖像顯示在 PictureBox 上
            pictureBox1.Image = image;

            switch (buttons)
            {
                case MessageBoxButtons.OK:
                    btnOK = new Button();
                    btnOK.Name = "btnOK";
                    btnOK.Text = "確認";
                    btnOK.Location = new Point(74, 124);
                    btnOK.Size = new Size(140, 30);
                    btnOK.Font = new Font("微軟正黑體", 12, FontStyle.Regular);
                    btnOK.DialogResult = DialogResult.OK;
                    this.Controls.Add(btnOK);
                    break;
                case MessageBoxButtons.OKCancel:
                    btnOK = new Button();
                    btnOK.Name = "btnOK";
                    btnOK.Text = "確認";
                    btnOK.Location = new Point(47, 124);
                    btnOK.Size = new Size(89, 30);
                    btnOK.Font = new Font("微軟正黑體", 12, FontStyle.Regular);
                    btnOK.DialogResult= DialogResult.OK;
                    this.Controls.Add(btnOK);

                    btnCancel = new Button();
                    btnCancel.Name = "btnCancel";
                    btnCancel.Text = "取消";
                    btnCancel.Location = new Point(148, 124);
                    btnCancel.Size = new Size(89, 30);
                    btnCancel.Font = new Font("微軟正黑體", 12, FontStyle.Regular);
                    btnCancel.DialogResult= DialogResult.Cancel;
                    this.Controls.Add(btnCancel);
                    break;
                case MessageBoxButtons.YesNo:
                    btnYes = new Button();
                    btnYes.Name = "btnYes";
                    btnYes.Text = "是";
                    btnYes.Location = new Point(47, 124);
                    btnYes.Size = new Size(89, 30);
                    btnYes.Font = new Font("微軟正黑體", 12, FontStyle.Regular);
                    btnYes.DialogResult= DialogResult.Yes;
                    this.Controls.Add(btnYes);

                    btnNo = new Button();
                    btnNo.Name = "btnNo";
                    btnNo.Text = "否";
                    btnNo.Location = new Point(148, 124);
                    btnNo.Size = new Size(89, 30);
                    btnNo.Font = new Font("微軟正黑體", 12, FontStyle.Regular);
                    btnNo.DialogResult= DialogResult.No;
                    this.Controls.Add(btnNo);
                    break;
            }

            base.Refresh();
        }

        public void Show(string title, string message, MessageBoxIcon icon)
        {

            Refresh();
        }
    }
}
