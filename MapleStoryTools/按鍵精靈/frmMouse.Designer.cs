
namespace MapleStoryTools
{
    partial class frmMouse
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
            this.rdbtnLoop = new System.Windows.Forms.RadioButton();
            this.rdbtnSingle = new System.Windows.Forms.RadioButton();
            this.txtSeconds = new System.Windows.Forms.TextBox();
            this.txtMinutes = new System.Windows.Forms.TextBox();
            this.txtStop = new System.Windows.Forms.TextBox();
            this.txtStart = new System.Windows.Forms.TextBox();
            this.txtHours = new System.Windows.Forms.TextBox();
            this.txtPointY = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.txtPointX = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.labFormatTime = new System.Windows.Forms.Label();
            this.labLoopCaption = new System.Windows.Forms.Label();
            this.labStatus = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.labStop = new System.Windows.Forms.Label();
            this.labStart = new System.Windows.Forms.Label();
            this.timerUpdateLabel = new System.Windows.Forms.Timer(this.components);
            this.label10 = new System.Windows.Forms.Label();
            this.txtSetPoint = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // rdbtnLoop
            // 
            this.rdbtnLoop.AutoSize = true;
            this.rdbtnLoop.Checked = true;
            this.rdbtnLoop.Font = new System.Drawing.Font("微軟正黑體", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.rdbtnLoop.Location = new System.Drawing.Point(97, 294);
            this.rdbtnLoop.Name = "rdbtnLoop";
            this.rdbtnLoop.Size = new System.Drawing.Size(66, 28);
            this.rdbtnLoop.TabIndex = 20;
            this.rdbtnLoop.TabStop = true;
            this.rdbtnLoop.Text = "循環";
            this.rdbtnLoop.UseVisualStyleBackColor = true;
            this.rdbtnLoop.CheckedChanged += new System.EventHandler(this.rdbtnLoop_CheckedChanged);
            // 
            // rdbtnSingle
            // 
            this.rdbtnSingle.AutoSize = true;
            this.rdbtnSingle.Font = new System.Drawing.Font("微軟正黑體", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.rdbtnSingle.Location = new System.Drawing.Point(9, 294);
            this.rdbtnSingle.Name = "rdbtnSingle";
            this.rdbtnSingle.Size = new System.Drawing.Size(66, 28);
            this.rdbtnSingle.TabIndex = 19;
            this.rdbtnSingle.Text = "單次";
            this.rdbtnSingle.UseVisualStyleBackColor = true;
            this.rdbtnSingle.CheckedChanged += new System.EventHandler(this.rdbtnSingle_CheckedChanged);
            // 
            // txtSeconds
            // 
            this.txtSeconds.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.txtSeconds.ImeMode = System.Windows.Forms.ImeMode.Off;
            this.txtSeconds.Location = new System.Drawing.Point(280, 221);
            this.txtSeconds.MaxLength = 2;
            this.txtSeconds.Name = "txtSeconds";
            this.txtSeconds.Size = new System.Drawing.Size(48, 29);
            this.txtSeconds.TabIndex = 16;
            this.txtSeconds.TabStop = false;
            this.txtSeconds.Text = "0";
            this.txtSeconds.TextChanged += new System.EventHandler(this.txtSetTime_TextChanged);
            this.txtSeconds.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.KeyPressOnlyNumber);
            // 
            // txtMinutes
            // 
            this.txtMinutes.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.txtMinutes.ImeMode = System.Windows.Forms.ImeMode.Off;
            this.txtMinutes.Location = new System.Drawing.Point(187, 221);
            this.txtMinutes.MaxLength = 2;
            this.txtMinutes.Name = "txtMinutes";
            this.txtMinutes.Size = new System.Drawing.Size(48, 29);
            this.txtMinutes.TabIndex = 14;
            this.txtMinutes.TabStop = false;
            this.txtMinutes.Text = "0";
            this.txtMinutes.TextChanged += new System.EventHandler(this.txtSetTime_TextChanged);
            this.txtMinutes.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.KeyPressOnlyNumber);
            // 
            // txtStop
            // 
            this.txtStop.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.txtStop.ImeMode = System.Windows.Forms.ImeMode.Off;
            this.txtStop.Location = new System.Drawing.Point(197, 79);
            this.txtStop.MaxLength = 5;
            this.txtStop.Name = "txtStop";
            this.txtStop.Size = new System.Drawing.Size(48, 29);
            this.txtStop.TabIndex = 4;
            this.txtStop.TabStop = false;
            this.txtStop.Text = "F2";
            this.txtStop.Enter += new System.EventHandler(this.txtControlKey_Enter);
            this.txtStop.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtControlKey_KeyPress);
            this.txtStop.Leave += new System.EventHandler(this.txtControlKey_Leave);
            // 
            // txtStart
            // 
            this.txtStart.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.txtStart.ImeMode = System.Windows.Forms.ImeMode.Off;
            this.txtStart.Location = new System.Drawing.Point(78, 79);
            this.txtStart.MaxLength = 5;
            this.txtStart.Name = "txtStart";
            this.txtStart.Size = new System.Drawing.Size(48, 29);
            this.txtStart.TabIndex = 2;
            this.txtStart.TabStop = false;
            this.txtStart.Text = "F1";
            this.txtStart.Enter += new System.EventHandler(this.txtControlKey_Enter);
            this.txtStart.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtControlKey_KeyPress);
            this.txtStart.Leave += new System.EventHandler(this.txtControlKey_Leave);
            // 
            // txtHours
            // 
            this.txtHours.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.txtHours.ImeMode = System.Windows.Forms.ImeMode.Off;
            this.txtHours.Location = new System.Drawing.Point(94, 221);
            this.txtHours.MaxLength = 2;
            this.txtHours.Name = "txtHours";
            this.txtHours.Size = new System.Drawing.Size(48, 29);
            this.txtHours.TabIndex = 12;
            this.txtHours.TabStop = false;
            this.txtHours.Text = "0";
            this.txtHours.TextChanged += new System.EventHandler(this.txtSetTime_TextChanged);
            this.txtHours.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.KeyPressOnlyNumber);
            // 
            // txtPointY
            // 
            this.txtPointY.BackColor = System.Drawing.SystemColors.ControlLight;
            this.txtPointY.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtPointY.Font = new System.Drawing.Font("微軟正黑體", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.txtPointY.Location = new System.Drawing.Point(196, 151);
            this.txtPointY.Name = "txtPointY";
            this.txtPointY.ReadOnly = true;
            this.txtPointY.Size = new System.Drawing.Size(46, 26);
            this.txtPointY.TabIndex = 10;
            this.txtPointY.TabStop = false;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("微軟正黑體", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label8.Location = new System.Drawing.Point(336, 223);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(29, 24);
            this.label8.TabIndex = 17;
            this.label8.Text = "秒";
            // 
            // txtPointX
            // 
            this.txtPointX.BackColor = System.Drawing.SystemColors.ControlLight;
            this.txtPointX.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtPointX.Font = new System.Drawing.Font("微軟正黑體", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.txtPointX.Location = new System.Drawing.Point(72, 151);
            this.txtPointX.Name = "txtPointX";
            this.txtPointX.ReadOnly = true;
            this.txtPointX.Size = new System.Drawing.Size(46, 26);
            this.txtPointX.TabIndex = 8;
            this.txtPointX.TabStop = false;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("微軟正黑體", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label7.Location = new System.Drawing.Point(243, 223);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(29, 24);
            this.label7.TabIndex = 15;
            this.label7.Text = "分";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("微軟正黑體", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label3.Location = new System.Drawing.Point(134, 152);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(59, 24);
            this.label3.TabIndex = 9;
            this.label3.Text = "Y座標";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("微軟正黑體", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label5.Location = new System.Drawing.Point(150, 223);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(29, 24);
            this.label5.TabIndex = 13;
            this.label5.Text = "時";
            // 
            // labFormatTime
            // 
            this.labFormatTime.AutoSize = true;
            this.labFormatTime.Font = new System.Drawing.Font("微軟正黑體", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.labFormatTime.Location = new System.Drawing.Point(9, 252);
            this.labFormatTime.Name = "labFormatTime";
            this.labFormatTime.Size = new System.Drawing.Size(100, 24);
            this.labFormatTime.TabIndex = 18;
            this.labFormatTime.Text = "(總共 0 秒)";
            // 
            // labLoopCaption
            // 
            this.labLoopCaption.AutoSize = true;
            this.labLoopCaption.Font = new System.Drawing.Font("微軟正黑體", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.labLoopCaption.Location = new System.Drawing.Point(9, 223);
            this.labLoopCaption.Name = "labLoopCaption";
            this.labLoopCaption.Size = new System.Drawing.Size(86, 24);
            this.labLoopCaption.TabIndex = 11;
            this.labLoopCaption.Text = "執行間隔";
            // 
            // labStatus
            // 
            this.labStatus.AutoSize = true;
            this.labStatus.Font = new System.Drawing.Font("微軟正黑體", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.labStatus.ForeColor = System.Drawing.Color.Blue;
            this.labStatus.Location = new System.Drawing.Point(9, 9);
            this.labStatus.Margin = new System.Windows.Forms.Padding(3);
            this.labStatus.Name = "labStatus";
            this.labStatus.Size = new System.Drawing.Size(96, 35);
            this.labStatus.TabIndex = 0;
            this.labStatus.Text = "未開始";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("微軟正黑體", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label2.Location = new System.Drawing.Point(9, 152);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(60, 24);
            this.label2.TabIndex = 7;
            this.label2.Text = "X座標";
            // 
            // labStop
            // 
            this.labStop.AutoSize = true;
            this.labStop.Font = new System.Drawing.Font("微軟正黑體", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.labStop.Location = new System.Drawing.Point(128, 81);
            this.labStop.Name = "labStop";
            this.labStop.Size = new System.Drawing.Size(67, 24);
            this.labStop.TabIndex = 3;
            this.labStop.Text = "停止：";
            this.labStop.Click += new System.EventHandler(this.labStop_Click);
            // 
            // labStart
            // 
            this.labStart.AutoSize = true;
            this.labStart.Font = new System.Drawing.Font("微軟正黑體", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.labStart.Location = new System.Drawing.Point(9, 81);
            this.labStart.Name = "labStart";
            this.labStart.Size = new System.Drawing.Size(67, 24);
            this.labStart.TabIndex = 1;
            this.labStart.Text = "開始：";
            this.labStart.Click += new System.EventHandler(this.labStart_Click);
            // 
            // timerUpdateLabel
            // 
            this.timerUpdateLabel.Tick += new System.EventHandler(this.timerUpdateLabel_Tick);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("微軟正黑體", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label10.Location = new System.Drawing.Point(247, 81);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(105, 24);
            this.label10.TabIndex = 5;
            this.label10.Text = "設定座標：";
            // 
            // txtSetPoint
            // 
            this.txtSetPoint.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.txtSetPoint.ImeMode = System.Windows.Forms.ImeMode.Off;
            this.txtSetPoint.Location = new System.Drawing.Point(354, 79);
            this.txtSetPoint.MaxLength = 5;
            this.txtSetPoint.Name = "txtSetPoint";
            this.txtSetPoint.Size = new System.Drawing.Size(48, 29);
            this.txtSetPoint.TabIndex = 6;
            this.txtSetPoint.TabStop = false;
            this.txtSetPoint.Text = "F3";
            this.txtSetPoint.Enter += new System.EventHandler(this.txtControlKey_Enter);
            this.txtSetPoint.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtControlKey_KeyPress);
            this.txtSetPoint.Leave += new System.EventHandler(this.txtControlKey_Leave);
            // 
            // frmMouse
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(413, 356);
            this.Controls.Add(this.rdbtnLoop);
            this.Controls.Add(this.rdbtnSingle);
            this.Controls.Add(this.txtSeconds);
            this.Controls.Add(this.txtMinutes);
            this.Controls.Add(this.txtSetPoint);
            this.Controls.Add(this.txtStop);
            this.Controls.Add(this.txtStart);
            this.Controls.Add(this.txtHours);
            this.Controls.Add(this.txtPointY);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.txtPointX);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.labFormatTime);
            this.Controls.Add(this.labLoopCaption);
            this.Controls.Add(this.labStatus);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.labStop);
            this.Controls.Add(this.labStart);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmMouse";
            this.Text = "土炮版滑鼠點擊";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmMouse_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.frmMouse_FormClosed);
            this.Load += new System.EventHandler(this.frmMouse_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RadioButton rdbtnLoop;
        private System.Windows.Forms.RadioButton rdbtnSingle;
        private System.Windows.Forms.TextBox txtSeconds;
        private System.Windows.Forms.TextBox txtMinutes;
        private System.Windows.Forms.TextBox txtStop;
        private System.Windows.Forms.TextBox txtStart;
        private System.Windows.Forms.TextBox txtHours;
        private System.Windows.Forms.TextBox txtPointY;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox txtPointX;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label labFormatTime;
        private System.Windows.Forms.Label labLoopCaption;
        private System.Windows.Forms.Label labStatus;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label labStop;
        private System.Windows.Forms.Label labStart;
        private System.Windows.Forms.Timer timerUpdateLabel;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox txtSetPoint;
    }
}