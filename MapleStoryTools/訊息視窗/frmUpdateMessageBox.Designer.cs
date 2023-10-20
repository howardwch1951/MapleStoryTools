namespace MapleStoryTools
{
    partial class frmUpdateMessageBox
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.labMessage = new System.Windows.Forms.Label();
            this.timerUpdateLabel = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // labMessage
            // 
            this.labMessage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labMessage.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.labMessage.Location = new System.Drawing.Point(0, 0);
            this.labMessage.Name = "labMessage";
            this.labMessage.Size = new System.Drawing.Size(244, 91);
            this.labMessage.TabIndex = 4;
            this.labMessage.Text = "測試文字";
            this.labMessage.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // timerUpdateLabel
            // 
            this.timerUpdateLabel.Enabled = true;
            this.timerUpdateLabel.Interval = 1000;
            this.timerUpdateLabel.Tick += new System.EventHandler(this.timerUpdateLabel_Tick);
            // 
            // frmUpdateMessageBox
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(244, 91);
            this.ControlBox = false;
            this.Controls.Add(this.labMessage);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmUpdateMessageBox";
            this.Text = "檢查更新";
            this.Move += new System.EventHandler(this.frmUpdateMessageBox_Move);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Label labMessage;
        private System.Windows.Forms.Timer timerUpdateLabel;
    }
}