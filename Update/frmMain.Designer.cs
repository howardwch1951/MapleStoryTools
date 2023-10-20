namespace Update
{
    partial class frmMain
    {
        /// <summary>
        /// 設計工具所需的變數。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清除任何使用中的資源。
        /// </summary>
        /// <param name="disposing">如果應該處置受控資源則為 true，否則為 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 設計工具產生的程式碼

        /// <summary>
        /// 此為設計工具支援所需的方法 - 請勿使用程式碼編輯器修改
        /// 這個方法的內容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.labUpdateText = new System.Windows.Forms.Label();
            this.timerUpdateLabel = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // labUpdateText
            // 
            this.labUpdateText.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labUpdateText.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.labUpdateText.Location = new System.Drawing.Point(0, 0);
            this.labUpdateText.Name = "labUpdateText";
            this.labUpdateText.Size = new System.Drawing.Size(314, 65);
            this.labUpdateText.TabIndex = 0;
            this.labUpdateText.Text = "程式更新中，請稍後......";
            this.labUpdateText.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.labUpdateText.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Form_MouseDown);
            this.labUpdateText.MouseMove += new System.Windows.Forms.MouseEventHandler(this.Form_MouseMove);
            this.labUpdateText.MouseUp += new System.Windows.Forms.MouseEventHandler(this.Form_MouseUp);
            // 
            // timerUpdateLabel
            // 
            this.timerUpdateLabel.Enabled = true;
            this.timerUpdateLabel.Interval = 1000;
            this.timerUpdateLabel.Tick += new System.EventHandler(this.timerUpdateLabel_Tick);
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(314, 65);
            this.Controls.Add(this.labUpdateText);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmMain";
            this.Text = "Form1";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.frmMain_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label labUpdateText;
        private System.Windows.Forms.Timer timerUpdateLabel;
    }
}

