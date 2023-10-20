namespace MapleStoryTools
{
    partial class frmColorSelecter
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
            this.picRgb = new System.Windows.Forms.PictureBox();
            this.labRgbValue = new System.Windows.Forms.Label();
            this.labSmallScreen = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.pnlSelectedScreen = new System.Windows.Forms.Panel();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.txtRedValue = new System.Windows.Forms.TextBox();
            this.labRedValue = new System.Windows.Forms.Label();
            this.labGreenValue = new System.Windows.Forms.Label();
            this.labBlueValue = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.picRgb)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // picRgb
            // 
            this.picRgb.Image = global::MapleStoryTools.Properties.Resources.rgbSpectrum;
            this.picRgb.Location = new System.Drawing.Point(12, 12);
            this.picRgb.Name = "picRgb";
            this.picRgb.Size = new System.Drawing.Size(226, 156);
            this.picRgb.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.picRgb.TabIndex = 0;
            this.picRgb.TabStop = false;
            this.picRgb.MouseDown += new System.Windows.Forms.MouseEventHandler(this.picRGB_MouseDown);
            this.picRgb.MouseMove += new System.Windows.Forms.MouseEventHandler(this.picRGB_MouseMove);
            // 
            // labRgbValue
            // 
            this.labRgbValue.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.labRgbValue.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.labRgbValue.Location = new System.Drawing.Point(12, 231);
            this.labRgbValue.Name = "labRgbValue";
            this.labRgbValue.Size = new System.Drawing.Size(147, 35);
            this.labRgbValue.TabIndex = 1;
            this.labRgbValue.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // labSmallScreen
            // 
            this.labSmallScreen.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.labSmallScreen.Location = new System.Drawing.Point(12, 180);
            this.labSmallScreen.Name = "labSmallScreen";
            this.labSmallScreen.Size = new System.Drawing.Size(147, 35);
            this.labSmallScreen.TabIndex = 1;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.pnlSelectedScreen);
            this.groupBox1.Controls.Add(this.txtRedValue);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.labBlueValue);
            this.groupBox1.Controls.Add(this.labGreenValue);
            this.groupBox1.Controls.Add(this.labRedValue);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Location = new System.Drawing.Point(12, 269);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(226, 122);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            // 
            // pnlSelectedScreen
            // 
            this.pnlSelectedScreen.Location = new System.Drawing.Point(144, 14);
            this.pnlSelectedScreen.Name = "pnlSelectedScreen";
            this.pnlSelectedScreen.Size = new System.Drawing.Size(76, 99);
            this.pnlSelectedScreen.TabIndex = 2;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label5.ForeColor = System.Drawing.Color.Blue;
            this.label5.Location = new System.Drawing.Point(6, 88);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(43, 21);
            this.label5.TabIndex = 0;
            this.label5.Text = "Blue";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label4.ForeColor = System.Drawing.Color.Green;
            this.label4.Location = new System.Drawing.Point(6, 53);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(56, 21);
            this.label4.TabIndex = 0;
            this.label4.Text = "Green";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label3.ForeColor = System.Drawing.Color.Red;
            this.label3.Location = new System.Drawing.Point(6, 18);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(40, 21);
            this.label3.TabIndex = 0;
            this.label3.Text = "Red";
            // 
            // button1
            // 
            this.button1.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.button1.Location = new System.Drawing.Point(80, 397);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(95, 29);
            this.button1.TabIndex = 3;
            this.button1.Text = "確認";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // txtRedValue
            // 
            this.txtRedValue.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtRedValue.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.txtRedValue.Location = new System.Drawing.Point(68, 14);
            this.txtRedValue.Name = "txtRedValue";
            this.txtRedValue.ReadOnly = true;
            this.txtRedValue.Size = new System.Drawing.Size(0, 29);
            this.txtRedValue.TabIndex = 1;
            // 
            // labRedValue
            // 
            this.labRedValue.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.labRedValue.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.labRedValue.ForeColor = System.Drawing.Color.Red;
            this.labRedValue.Location = new System.Drawing.Point(66, 14);
            this.labRedValue.Name = "labRedValue";
            this.labRedValue.Size = new System.Drawing.Size(73, 29);
            this.labRedValue.TabIndex = 0;
            this.labRedValue.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // labGreenValue
            // 
            this.labGreenValue.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.labGreenValue.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.labGreenValue.ForeColor = System.Drawing.Color.Green;
            this.labGreenValue.Location = new System.Drawing.Point(65, 49);
            this.labGreenValue.Name = "labGreenValue";
            this.labGreenValue.Size = new System.Drawing.Size(73, 29);
            this.labGreenValue.TabIndex = 0;
            this.labGreenValue.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // labBlueValue
            // 
            this.labBlueValue.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.labBlueValue.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.labBlueValue.ForeColor = System.Drawing.Color.Blue;
            this.labBlueValue.Location = new System.Drawing.Point(65, 84);
            this.labBlueValue.Name = "labBlueValue";
            this.labBlueValue.Size = new System.Drawing.Size(73, 29);
            this.labBlueValue.TabIndex = 0;
            this.labBlueValue.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // frmColorSelecter
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(353, 461);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.labSmallScreen);
            this.Controls.Add(this.labRgbValue);
            this.Controls.Add(this.picRgb);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmColorSelecter";
            this.Text = "frmColorSelecter";
            ((System.ComponentModel.ISupportInitialize)(this.picRgb)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox picRgb;
        private System.Windows.Forms.Label labRgbValue;
        private System.Windows.Forms.Label labSmallScreen;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Panel pnlSelectedScreen;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TextBox txtRedValue;
        private System.Windows.Forms.Label labBlueValue;
        private System.Windows.Forms.Label labGreenValue;
        private System.Windows.Forms.Label labRedValue;
    }
}