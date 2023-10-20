namespace MapleStoryTools
{
    partial class frmPoint
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
            this.labPoint = new System.Windows.Forms.Label();
            this.labArrow = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // labPoint
            // 
            this.labPoint.AutoSize = true;
            this.labPoint.BackColor = System.Drawing.Color.IndianRed;
            this.labPoint.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.labPoint.ForeColor = System.Drawing.Color.Red;
            this.labPoint.Location = new System.Drawing.Point(3, 0);
            this.labPoint.Margin = new System.Windows.Forms.Padding(0);
            this.labPoint.Name = "labPoint";
            this.labPoint.Size = new System.Drawing.Size(132, 25);
            this.labPoint.TabIndex = 0;
            this.labPoint.Text = "(0000, 0000)";
            this.labPoint.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // labArrow
            // 
            this.labArrow.AutoSize = true;
            this.labArrow.BackColor = System.Drawing.Color.IndianRed;
            this.labArrow.Font = new System.Drawing.Font("微軟正黑體", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.labArrow.ForeColor = System.Drawing.Color.Red;
            this.labArrow.Location = new System.Drawing.Point(137, -9);
            this.labArrow.Margin = new System.Windows.Forms.Padding(0);
            this.labArrow.Name = "labArrow";
            this.labArrow.Size = new System.Drawing.Size(38, 38);
            this.labArrow.TabIndex = 0;
            this.labArrow.Text = "🡵";
            this.labArrow.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // frmPoint
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.IndianRed;
            this.ClientSize = new System.Drawing.Size(164, 25);
            this.ControlBox = false;
            this.Controls.Add(this.labArrow);
            this.Controls.Add(this.labPoint);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmPoint";
            this.Text = "🡵";
            this.TopMost = true;
            this.TransparencyKey = System.Drawing.Color.IndianRed;
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labPoint;
        private System.Windows.Forms.Label labArrow;
    }
}