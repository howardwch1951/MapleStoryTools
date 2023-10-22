using AutoHotkey.Interop;
using log4net;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MapleStoryTools
{
    public partial class frmMouse : Form
    {
        int TotalSeconds = 0;
        int TimerTotalSeconds = 0;
        int LoopTime = 1000;
        int WaitTime = 0;
        bool isSingle = false;
        bool isScriptStart = false;
        bool isSetControlKey = false;
        string keyUp;
        string[][] keyPress = new string[][] { };
        Point point = new Point();
        StringBuilder keysPressed = new StringBuilder();
        DateTime nextRumTime;
        Mouse mouse = new Mouse();
        AutoHotkeyEngine ahk = new AutoHotkeyEngine();
        KeyboardHook kbHook = new KeyboardHook();
        TextBox textBox = new TextBox();
        List<TextBox> txtControlKey = new List<TextBox>();
        public ILog log;
        public frmMain frm;

        public frmMouse()
        {
            this.Icon = new Icon(Path.Combine(Application.StartupPath, "Icon.ico"));

            InitializeComponent();

            kbHook = new KeyboardHook();
            kbHook.KeyDown += OnKeyDown;
            kbHook.KeyUp += OnKeyUp;
        }

        private void frmMouse_Load(object sender, EventArgs e)
        {
            try
            {
                txtControlKey.Add(txtStart);
                txtControlKey.Add(txtStop);
                txtControlKey.Add(txtSetPoint);

                string path = Path.Combine(Application.StartupPath, "Mouse");

                if (!File.Exists(path))
                    Directory.CreateDirectory(path);

                kbHook.Start();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private delegate void DelSetMouseScript();
        /// <summary>
        /// 滑鼠指令執行緒
        /// </summary>
        private void SetMouseScript()
        {
            while (isScriptStart)
            {
                try
                {
                    DateTime now = DateTime.Now;
                    if (now >= nextRumTime)
                    {
                        nextRumTime = now.AddSeconds(TotalSeconds);

                        Thread.Sleep(WaitTime);

                        mouse.LeftClick(point);

                        if (isSingle)
                        {
                            new Thread(SetSingleEnd).Start();
                            break;
                        }

                        //Thread.Sleep(LoopTime);
                    }
                    Thread.Sleep(1);
                }
                catch (Exception ex)
                {
                    log.Error(ex);
                }
            }
        }

        /// <summary>
        /// 設定Enable
        /// </summary>
        /// <param name="enable"></param>
        private void SetEnable(bool enable)
        {
            txtHours.Enabled = txtMinutes.Enabled = txtSeconds.Enabled = txtStart.Enabled = 
            txtStop.Enabled = txtSetPoint.Enabled = rdbtnSingle.Enabled = rdbtnLoop.Enabled = enable;
        }

        /// <summary>
        /// 設定Label顯示文字和顏色
        /// </summary>
        /// <param name="lab"></param>
        /// <param name="msg"></param>
        /// <param name="color"></param>
        private void SetLabel(Label lab, string msg, Color color)
        {
            lab.Text = msg;
            lab.ForeColor = color;
            Refresh();
        }

        #region 循環模式RadioButton
        private void rdbtnSingle_CheckedChanged(object sender, EventArgs e)
        {
            if (rdbtnSingle.Checked)
            {
                rdbtnLoop.Checked = false;
                isSingle = true;

                LoopTime = 0;
                WaitTime = TotalSeconds * 1000;

                labLoopCaption.Text = "下次執行";
            }
        }

        private void rdbtnLoop_CheckedChanged(object sender, EventArgs e)
        {
            if (rdbtnLoop.Checked)
            {
                rdbtnSingle.Checked = false;
                isSingle = false;

                LoopTime = TotalSeconds * 1000;
                WaitTime = 0;

                labLoopCaption.Text = "執行間隔";
            }
        }

        private void SetSingleEnd()
        {
            if (this.InvokeRequired)
            {

                DelSetMouseScript del = new DelSetMouseScript(SetSingleEnd);
                this.Invoke(del);
            }
            else
            {
                nextRumTime = new DateTime();
                timerUpdateLabel.Enabled = false;
                isScriptStart = false;
                SetEnable(true);

                SetLabel(labStatus, "未開始", Color.Blue);
            }
        }
        #endregion

        #region 設定時間控件
        private void KeyPressOnlyNumber(object sender, KeyPressEventArgs e)
        {
            try
            {
                // e.KeyChar == (Char)48 ~ 57 -----> 0~9
                // e.KeyChar == (Char)8 -----------> Backpace
                // e.KeyChar == (Char)13-----------> Enter
                if (e.KeyChar == (Char)48 || e.KeyChar == (Char)49 ||
                   e.KeyChar == (Char)50 || e.KeyChar == (Char)51 ||
                   e.KeyChar == (Char)52 || e.KeyChar == (Char)53 ||
                   e.KeyChar == (Char)54 || e.KeyChar == (Char)55 ||
                   e.KeyChar == (Char)56 || e.KeyChar == (Char)57 ||
                   e.KeyChar == (Char)13)
                {
                    e.Handled = false;
                }
                else
                {
                    e.Handled = true;
                    return;
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }

        }

        private void txtSetTime_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(((TextBox)sender).Text))
                {
                    int hours = Convert.ToInt32(txtHours.Text); // 小時數
                    int minutes = Convert.ToInt32(txtMinutes.Text); // 分鐘數
                    int seconds = Convert.ToInt32(txtSeconds.Text); // 秒數
                    TotalSeconds = hours * 3600 + minutes * 60 + seconds;

                    labFormatTime.Text = $"(總共 {TotalSeconds} 秒)";

                    LoopTime = TotalSeconds * 1000;

                    if (rdbtnSingle.Checked)
                    {
                        LoopTime = 0;
                        WaitTime = TotalSeconds * 1000;
                    }
                    else
                        WaitTime = 0;
                }
                else
                    labFormatTime.Text = "";
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
        }
        #endregion

        #region 開始、結束、設定座標TextBox控制
        private void txtControlKey_Enter(object sender, EventArgs e)
        {
            try
            {
                textBox = (System.Windows.Forms.TextBox)sender;
                isSetControlKey = true;
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
        }

        private void txtControlKey_Leave(object sender, EventArgs e)
        {
            textBox = null;
            isSetControlKey = false;
        }

        private void txtControlKey_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;
        }

        private void ScriptStart()
        {
            try
            {
                if (string.IsNullOrEmpty(txtPointX.Text) || string.IsNullOrEmpty(txtPointY.Text))
                {
                    frm.ShowMessageBox("錯誤", "座標尚未設定!", MessageBoxIcon.Error, MessageBoxButtons.OK);
                    return;
                }

                if (txtHours.Text == "0" && txtMinutes.Text == "0" && txtSeconds.Text == "0")
                {
                    frm.ShowMessageBox("錯誤", "執行間隔時間尚未設定!", MessageBoxIcon.Error, MessageBoxButtons.OK);
                    return;
                }

                SetEnable(false);
                isScriptStart = frm.isMouseScriptRun = true;
                
                TimerTotalSeconds = TotalSeconds;

                SetLabel(labStatus, "執行中", Color.DarkGreen);

                new Thread(SetMouseScript).Start();
                timerUpdateLabel.Enabled = true;
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
        }

        private void ScriptStop()
        {
            try
            {
                SetEnable(true);
                nextRumTime = new DateTime();
                timerUpdateLabel.Enabled = false;
                isScriptStart = frm.isMouseScriptRun = false;
                labFormatTime.Text = $"(總共 {TotalSeconds} 秒)";

                SetLabel(labStatus, "未開始", Color.Blue);
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
        }

        private void SetPoint()
        {
            try
            {
                point = mouse.CurrentPosition();
                txtPointX.Text = point.X.ToString();
                txtPointY.Text = point.Y.ToString();
                this.Refresh();
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
        }
        #endregion

        #region 鍵盤偵測
        /// <summary>
        /// 鍵盤按下
        /// </summary>
        /// <param name="key"></param>
        private void OnKeyDown(string key)
        {
            try
            {

            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
        }

        /// <summary>
        /// 鍵盤彈起
        /// </summary>
        /// <param name="key"></param>
        private void OnKeyUp(string key)
        {
            try
            {
                if (isSetControlKey)
                {
                    #region 設定開始、結束
                    keyUp = key;
                    if (textBox != null)
                    {
                        bool canSet = true;
                        foreach (TextBox txt in txtControlKey)
                        {
                            if (textBox != txt && keyUp == txt.Text)
                            {
                                canSet &= false;
                            }
                            else if (textBox != txt && textBox.Text != txt.Text)
                            {
                                canSet &= true;
                            }
                        }

                        if (canSet)
                            textBox.Text = keyUp;
                        else
                            frm.ShowMessageBox("錯誤", "該按鍵已被設定!", MessageBoxIcon.Error, MessageBoxButtons.OK);

                        labStatus.Focus();
                    }
                    #endregion
                }
                else
                {
                    if (key == txtStart.Text)//開始
                    {
                        ScriptStart();
                    }
                    else if (key == txtStop.Text)//結束
                    {
                        ScriptStop();
                    }
                    else if (key == txtSetPoint.Text)//設定座標
                    {
                        SetPoint();
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
        }
        #endregion

        private void timerUpdateLabel_Tick(object sender, EventArgs e)
        {
            try
            {
                if (nextRumTime >= DateTime.Now)
                {
                    string hour = (nextRumTime - DateTime.Now).Hours + "時";
                    string minute = (nextRumTime - DateTime.Now).Minutes + "分";
                    string second = (nextRumTime - DateTime.Now).Seconds + "秒";
                    labFormatTime.Text = $"(下次執行倒數：{hour} {minute} {second})";
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
        }

        private void labStart_Click(object sender, EventArgs e)
        {
            ScriptStart();
        }

        private void labStop_Click(object sender, EventArgs e)
        {
            ScriptStop();
        }

        private void frmMouse_FormClosing(object sender, FormClosingEventArgs e)
        {
            frm.SaveFormLocation(this);
        }

        private void frmMouse_FormClosed(object sender, FormClosedEventArgs e)
        {
            kbHook.Stop();
            this.Close();
        }
    }
}
