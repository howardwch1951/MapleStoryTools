using AutoHotkey.Interop;
using log4net;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Automation;
using System.Windows.Forms;
using Point = System.Drawing.Point;
using Notion.Client;
using Color = System.Drawing.Color;
using System.Windows.Interop;
using Newtonsoft.Json.Linq;
using System.Data.Common;
using System.Runtime.ConstrainedExecution;
using System.Text.Json.Nodes;
using System.Runtime.CompilerServices;
using System.Collections;
using System.Security.Policy;

namespace MapleStoryTools
{
    public partial class frmAutoScripts : Form
    {
        #region 引用Win32
        [DllImport("User32.dll")]
        private static extern bool ShowWindowAsync(IntPtr hWnd, int cmdShow);
        // 導入取得視窗控制項的 Process 函式
        [DllImport("user32.dll", SetLastError = true)]
        public static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        // 導入取得目前活躍視窗函式
        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        // 導入取得視窗控制項標題函式
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);
        //正常
        private const int WS_SHOWNORMAL = 1;
        //最大
        private const int SW_MAXIMIZE = 3;

        private const int SW_MINIMIZE = 6;
        #endregion

        #region 宣告
        int TotalSeconds = 0;
        int TotalEndSeconds = 0;
        int oriTotalEndSeconds = 0;
        int LoopTime = 1000;
        int WaitTime = 0;
        int nextRunTime = 0;
        double TotalSleepSeconds = 0;
        double currendWaitTime = 0;
        string ProcessName = "";
        string mouseType = "";
        string keyDown = "";
        string keyUp = "";
        string keyType = "";
        string pathHotKey = Path.Combine(Application.StartupPath, "Config", "HotKey.json");
        public bool isFormOpen = false;
        bool isSingle = false;
        bool isSetControlKey = false;
        bool isSetKeyCommand = false;
        bool isScriptStart = false;
        bool isCheckProcessOn = false;
        DateTime nextRumTime;
        DateTime LoopEndTime;
        System.Windows.Rect WindowRect;
        Point MousePoint = new Point();
        Point ScreenPoint = new Point();
        DataTable dtCommand = new DataTable();
        Mouse mouse = new Mouse();
        StringBuilder keysPressed = new StringBuilder();
        AutoHotkeyEngine ahk = new AutoHotkeyEngine();
        KeyboardHook kbHook = new KeyboardHook();
        TextBox textBox = new TextBox();
        ScriptCommand currentCommand = new ScriptCommand();
        List<TextBox> txtControlKey = new List<TextBox>();
        List<oldScriptCommand> listOldCommand = new List<oldScriptCommand>();
        BindingList<ScriptCommand> listCommand = new BindingList<ScriptCommand>();
        List<HotKey> listHotKey = new List<HotKey>();
        List<frmPoint> listFrmPoint = new List<frmPoint>();
        InputLanguage currentInputLanguage;
        public ILog log;
        public frmMain frm;
        #endregion

        public frmAutoScripts()
        {
            // 滑鼠點擊事件處理
            MouseHook.MouseClick += MouseHook_MouseClick;

            this.Icon = new Icon(Path.Combine(Application.StartupPath, "Icon.ico"));

            InitializeComponent();

            kbHook = new KeyboardHook();
            kbHook.KeyDown += OnKeyDown;
            kbHook.KeyUp += OnKeyUp;
            kbHook.Start();
        }

        private void frmAutoScripts_Load(object sender, EventArgs e)
        {
            cmbMouseType.SelectedIndex = 0;
            cmbKeyType.SelectedIndex = 0;

            txtControlKey.Add(txtStart);
            txtControlKey.Add(txtStop);

            listHotKey = new List<HotKey>();

            string path = Path.Combine(Application.StartupPath, "ScriptsCommand");

            if (!File.Exists(path))
                Directory.CreateDirectory(path);

            if (chbLockWindow.Checked && txtWindowName.Text != "")
                chbLockPoint.Enabled = true;
            else
                chbLockPoint.Enabled = false;

            dtCommand.Columns.Add("CommandType");
            dtCommand.Columns.Add("Command");
            dtCommand.Columns.Add("CommandInfo");
            dtCommand.Columns.Add("Note");

            SetHotKey("開始", txtStart);
            SetHotKey("停止", txtStop);
            SetHotKey("設定座標", txtSetPoint);

            SetHotKey("執行間隔(時)", txtHours);
            SetHotKey("執行間隔(分)", txtMinutes);
            SetHotKey("執行間隔(秒)", txtSeconds);
        }

        private void frmAutoScripts_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (isFormOpen)
            {
                isFormOpen = false;
                kbHook.Stop();
                this.Close();
            }
        }

        /// <summary>
        /// 設定Enable狀態
        /// </summary>
        /// <param name="enable"></param>
        private void SetEnable(bool enable)
        {
            txtStart.Enabled = txtStop.Enabled = 
            txtHours.Enabled = txtMinutes.Enabled = txtSeconds.Enabled =
            txtEndHours.Enabled = txtEndMinutes.Enabled = txtEndSeconds.Enabled = 
            btnLoad.Enabled = btnSave.Enabled = 
            cmbMouseType.Enabled = txtSetPoint.Enabled = chbLockPoint.Enabled = btnAddMouse.Enabled =
            cmbKeyType.Enabled = txtKeyCommand.Enabled = btnAddKey.Enabled =
            txtSleepHours.Enabled = txtSleepMinutes.Enabled = 
            txtSleepSeconds.Enabled = txtSleepMilliseconds.Enabled = btnAddSleepTime.Enabled =
            btnMoveUp.Enabled = btnMoveDown.Enabled = btnRemove.Enabled = btnColorSelector.Enabled = 
            chbLockWindow.Enabled = chbStop.Enabled = rdbtnLoop.Enabled = rdbtnSingle.Enabled = rdbtnLoopInTime.Enabled = 
            txtKeyinText.Enabled = btnAddText.Enabled = enable;
        }

        /// <summary>
        /// 設定Label文字和顏色
        /// </summary>
        /// <param name="lab"></param>
        /// <param name="msg"></param>
        /// <param name="color"></param>
        private void SetLabel(Label lab, string msg, Color color)
        {
            lab.Text = msg;
            lab.ForeColor = color;
            this.Refresh();
        }

        /// <summary>
        /// 鍵盤腳本執行緒
        /// </summary>
        private async void RunScript()
        {
            bool flag = true;
            int index = 0;
            //紀錄腳本結束時間
            if (rdbtnLoopInTime.Checked)
            {
                oriTotalEndSeconds = TotalEndSeconds;
                LoopEndTime = DateTime.Now.AddSeconds(TotalEndSeconds);
            }
            while (isScriptStart)
            {
                try
                {
                    DateTime now = DateTime.Now;

                    if (now >= nextRumTime)
                    {
                        if (isSingle && flag)
                        {
                            flag = false;
                            nextRumTime = DateTime.Now.AddSeconds(TotalSeconds);
                            continue;
                        }

                        if (index < listCommand.Count)
                        {
                            if (chbLockPoint.Checked)
                                GetWindowRect(ProcessName);
                            else
                                WindowRect = new System.Windows.Rect();

                            if (!isScriptStart)
                                return;

                            currentCommand = listCommand[index];

                            if (listCommand[index].Type.Contains("其他"))
                            {
                                if (listCommand[index].Function.Contains("等待"))
                                {
                                    await Task.Delay(Convert.ToInt32(Convert.ToDecimal(listCommand[index].Command) * 1000));
                                }
                                else if (listCommand[index].Function.Contains("文字"))
                                {
                                    SetClipboardText(listCommand[index].Command);

                                    // 檢查剪貼簿是否包含文字資料
                                    if (CheckClipboard())
                                    {
                                        // 取得剪貼簿中的文字資料
                                        string clipboardText = GetClipboardText();

                                        //ahk.ExecRaw("Send, ^v");
                                        SendKeys.SendWait("^{v}");

                                        ClearClipboard();
                                    }
                                }
                            }

                            if (listCommand[index].Type.Contains("滑鼠"))
                            {
                                if (listCommand[index].Function.Contains("左鍵"))
                                {
                                    Point point = ConvertStringToPoint(listCommand[index].Command.Split(',')[0], listCommand[index].Command.Split(',')[1]);
                                    if (chbLockPoint.Checked)
                                    {
                                        mouse.LeftClick(new Point((point.X + (int)(WindowRect.Left)), (point.Y + (int)(WindowRect.Top))));
                                    }
                                    else
                                        mouse.LeftClick(point);
                                }
                                else if (listCommand[index].Function.Contains("右鍵"))
                                {
                                    Point point = ConvertStringToPoint(listCommand[index].Command.Split(',')[0], listCommand[index].Command.Split(',')[1]);
                                    if (chbLockPoint.Checked)
                                    {
                                        mouse.RightClick(new Point((point.X + (int)(WindowRect.Left)), (point.Y + (int)(WindowRect.Top))));
                                    }
                                    else
                                        mouse.RightClick(point);
                                }
                            }

                            if (listCommand[index].Type.Contains("鍵盤"))
                            {
                                if (listCommand[index].Command.Length > 1)
                                {
                                    string cmd = listCommand[index].Function == "按鍵" ? "Send, {" + listCommand[index].Command + "}" :
                                                 listCommand[index].Function == "按下" ? "Send, {" + listCommand[index].Command + " down}" :
                                                                         "Send, {" + listCommand[index].Command + " up}";
                                    ahk.ExecRaw(cmd);
                                }
                                else
                                {
                                    string cmd = listCommand[index].Function == "按鍵" ? "Send, " + listCommand[index].Command.ToLower() :
                                                 listCommand[index].Function == "按下" ? "Send, " + listCommand[index].Command.ToLower() :
                                                                         "Send, " + listCommand[index].Command.ToLower();
                                    ahk.ExecRaw("Send, " + listCommand[index].Command.ToLower());
                                }
                            }

                            Console.WriteLine($"[{DateTime.Now:HH:mm:dd}] {listCommand[index].Info}");

                            if (isSingle)
                            {
                                new Thread(SetSingleEnd).Start();
                                break;
                            }

                            index++;
                        }
                        else
                        {
                            index = 0;
                            nextRumTime = DateTime.Now.AddSeconds(TotalSeconds);
                        }
                        //Thread.Sleep(WaitTime);

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

        #region 剪貼簿
        private void SetClipboardText(string pText)
        {
            Thread STAThread = new Thread(
                delegate ()
                {
                    System.Windows.Forms.Clipboard.SetText(pText);
                });
            STAThread.SetApartmentState(ApartmentState.STA);
            STAThread.Start();
            STAThread.Join();
        }

        private string GetClipboardText()
        {
            string ReturnValue = string.Empty;
            Thread STAThread = new Thread(
                delegate ()
                {
                    ReturnValue = System.Windows.Forms.Clipboard.GetText();
                });
            STAThread.SetApartmentState(ApartmentState.STA);
            STAThread.Start();
            STAThread.Join();

            return ReturnValue;
        }

        private bool CheckClipboard()
        {
            bool isNotEmty = true;
            Thread STAThread = new Thread(
                delegate ()
                {
                    IDataObject clipboardDataObject = Clipboard.GetDataObject();
                    isNotEmty = clipboardDataObject.GetDataPresent(DataFormats.Text);
                });
            STAThread.SetApartmentState(ApartmentState.STA);
            STAThread.Start();
            STAThread.Join();
            return isNotEmty;
        }

        private void ClearClipboard()
        {
            Thread STAThread = new Thread(
                delegate ()
                {
                    Clipboard.Clear();
                });
            STAThread.SetApartmentState(ApartmentState.STA);
            STAThread.Start();
            //STAThread.Join();
        }
        #endregion

        /// <summary>
        /// 字串轉換為Poit
        /// </summary>
        /// <param name="X"></param>
        /// <param name="Y"></param>
        /// <returns></returns>
        private Point ConvertStringToPoint(string X, string Y)
        {
            Point point = new Point(Convert.ToInt32(X), Convert.ToInt32(Y));
            return point;
        }

        /// <summary>
        /// 轉換為AutoHotKey格式
        /// </summary>
        /// <param name="type"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        private string GetAutohotkeyCommand(string type, string key)
        {
            switch (type)
            {
                case "KeyPress":
                    return key;
                case "KeyDown":
                    return $"{key} Down";
                case "KeyUp":
                    return $"{key} Up";
                default: 
                    return key;
            }
        }

        /// <summary>
        /// 滑鼠模式
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmbMouseType_SelectedIndexChanged(object sender, EventArgs e)
        {
            mouseType = cmbMouseType.Text;
            /*
            switch (cmbMouseType.Text)
            {
                case "左鍵":
                    mouseType = "Left Click";
                    break;
                case "右鍵":
                    mouseType = "Right Click";
                    break;
            }
            */
        }

        /// <summary>
        /// 按鍵模式
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmbKeyType_SelectedIndexChanged(object sender, EventArgs e)
        {

            keyType = cmbKeyType.Text;
            /*
            switch (cmbKeyType.Text)
            {
                case "按鍵":
                    keyType = "KeyPress";
                    break;
                case "按下":
                    keyType = "KeyDown";
                    break;
                case "彈起":
                    keyType = "KeyUp";
                    break;
            }
            */
        }

        /// <summary>
        /// 載入腳本
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnLoad_Click(object sender, EventArgs e)
        {
            LoacScript();
        }

        /// <summary>
        /// 載入腳本
        /// </summary>
        public void LoacScript()
        {

            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();

                // 設定預設的起始目錄
                openFileDialog.InitialDirectory = Path.Combine(Application.StartupPath, "ScriptsCommand");

                // 設定篩選檔案的類型，例如只顯示文字檔案
                openFileDialog.Filter = "腳本設定檔 (*.json)|*.json";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    txtScriptFileName.Text = openFileDialog.SafeFileName;
                    txtScriptFileName.SelectionStart = txtScriptFilePath.Text.Length;

                    txtScriptFilePath.Text = openFileDialog.FileName;
                    txtScriptFilePath.SelectionStart = txtScriptFilePath.Text.Length;

                    listCommand.Clear();

                    // 取得選擇的檔案路徑或多個檔案路徑（若有啟用多選）
                    string filepath = openFileDialog.FileName;

                    List<oldScriptCommand> oldCommand = new List<oldScriptCommand>();
                    BindingList<ScriptCommand> newCommand = new BindingList<ScriptCommand>();

                    string fileData = File.ReadAllText(filepath);

                    try
                    {
                        JArray jsonObject = JArray.Parse(fileData);
                        if (jsonObject[0].SelectToken("Name") != null && jsonObject[0].SelectToken("Command") != null &&
                            jsonObject[0].SelectToken("Point") != null)
                        {
                            JsonConvert.PopulateObject(fileData, oldCommand);

                            foreach (oldScriptCommand item in oldCommand)
                            {
                                ScriptCommand sc = new ScriptCommand();
                                sc.Type = item.Name.Contains("Click") ? "滑鼠" : item.Name.Contains("Key") ? "鍵盤" : "其他";
                                sc.Function = item.Name.Contains("Left Click") ? "左鍵" :
                                              item.Name.Contains("Right Click") ? "右鍵" :
                                              item.Name.Contains("KeyPress") ? "按鍵" :
                                              item.Name.Contains("KeyDown") ? "按下" :
                                              item.Name.Contains("KeyUp") ? "彈起" :
                                              item.Name.Contains("Sleep") ? "等待" : "";
                                sc.Command = item.Name.Contains("Left Click") || item.Name.Contains("Right Click") ? $"{item.Point.X},{item.Point.Y}" :
                                             item.Name.Contains("KeyPress") || item.Name.Contains("KeyDown") || item.Name.Contains("KeyUp") ? item.Name.Split(' ')[1] :
                                             item.Name.Contains("Sleep") ? item.Command : "";
                                sc.Info = item.LockWindow && (item.Name.Contains("Left Click") || item.Name.Contains("Right Click")) ? $"視窗內座標({item.Point.X}, {item.Point.Y})" :
                                sc.Info = !item.LockWindow && (item.Name.Contains("Left Click") || item.Name.Contains("Right Click")) ? $"座標({item.Point.X}, {item.Point.Y})" :
                                          item.Name.Contains("KeyPress") ? $"按鍵 {item.Name.Split(' ')[1]}" :
                                          item.Name.Contains("KeyDown") ? $"按下 {item.Name.Split(' ')[1]}" :
                                          item.Name.Contains("KeyUp") ? $"彈起 {item.Name.Split(' ')[1]}" :
                                          item.Name.Contains("Sleep") ? $"等待 {item.Name.Split(' ')[1]} 秒" : "";
                                sc.IsLockPoint = item.LockWindow;
                                newCommand.Add(sc);
                            }
                            //JsonConvert.PopulateObject(fileData, data);
                            frm.ShowMessageBox("提示", "載入腳本為舊版本格式，已轉換為新版本腳本格式!", MessageBoxIcon.Information, MessageBoxButtons.OK);

                            txtScriptFileName.Text = txtScriptFilePath.Text = "";
                        }
                        else if (jsonObject[0].SelectToken("Type") != null && jsonObject[0].SelectToken("Function") != null &&
                                 jsonObject[0].SelectToken("Command") != null && jsonObject[0].SelectToken("Info") != null &&
                                 jsonObject[0].SelectToken("IsLockPoint") != null && jsonObject[0].SelectToken("Note") != null &&
                                 jsonObject[0].SelectToken("RowColor") != null)
                        {
                            JsonConvert.PopulateObject(fileData, newCommand);
                        }
                        else
                        {
                            frm.ShowMessageBox("錯誤", "腳本格式錯誤!", MessageBoxIcon.Error, MessageBoxButtons.OK);
                        }
                    }
                    catch (Exception ex)
                    {
                        frm.ShowMessageBox("錯誤", "腳本格式錯誤!", MessageBoxIcon.Error, MessageBoxButtons.OK);
                        Console.WriteLine("設定檔內容有誤，請確認!\n" + ex.Message, "設定檔內容有誤");
                    }

                    listCommand = newCommand;

                    dgvCommand.DataSource = listCommand;

                    dgvCommand.Rows.Cast<DataGridViewRow>()
                        .Where(t => !string.IsNullOrEmpty(t.Cells["colRowColor"].Value.ToString())).ToList()
                        .ForEach(k => k.DefaultCellStyle.BackColor = ConvertArgbToColor(k.Cells["colRowColor"].Value.ToString()));
                    
                    ShowPointOnScreen();
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
        }

        /// <summary>
        /// 儲存腳本按鈕
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSave_Click(object sender, EventArgs e)
        {
            SaveScripts();
        }

        /// <summary>
        /// 儲存腳本
        /// </summary>
        private DialogResult SaveScripts()
        {
            DialogResult result = new DialogResult();
            try
            {
                if (listCommand.Count > 0)
                {
                    SaveFileDialog saveFileDialog = new SaveFileDialog();

                    // 設定對話方塊的屬性
                    saveFileDialog.Filter = "腳本設定檔 (*.json)|*.json";
                    saveFileDialog.Title = "儲存檔案";
                    saveFileDialog.InitialDirectory = Path.Combine(Application.StartupPath, "ScriptsCommand");
                    saveFileDialog.FileName = "ScriptsCommand";

                    // 顯示儲存檔案的對話方塊
                    result = saveFileDialog.ShowDialog();

                    // 如果使用者選擇了檔案位置並按下儲存按鈕
                    if (result == DialogResult.OK)
                    {
                        // 取得使用者選擇的檔案路徑
                        string filePath = saveFileDialog.FileName;

                        string output = JsonConvert.SerializeObject(listCommand, Formatting.Indented);
                        File.WriteAllText(filePath, output);


                        txtScriptFilePath.Text = saveFileDialog.FileName;
                        txtScriptFilePath.SelectionStart = txtScriptFilePath.Text.Length;
                    }
                }
                else
                {
                    frm.ShowMessageBox("錯誤", "尚未設定腳本!", MessageBoxIcon.Error, MessageBoxButtons.OK);
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
            return result;
        }

        #region 循環模式RadioButton
        private void rdbtnSingle_CheckedChanged(object sender, EventArgs e)
        {
            if (rdbtnSingle.Checked)
            {
                txtEndHours.Enabled = txtEndMinutes.Enabled = txtEndSeconds.Enabled = false;
                rdbtnLoop.Checked = rdbtnLoopInTime.Checked = false;
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
                txtEndHours.Enabled = txtEndMinutes.Enabled = txtEndSeconds.Enabled = false;
                rdbtnSingle.Checked = rdbtnLoopInTime.Checked = false;
                isSingle = false;

                LoopTime = TotalSeconds * 1000;
                WaitTime = 0;

                labLoopCaption.Text = "執行間隔";
            }
        }

        private void rdbtnLoopInTime_CheckedChanged(object sender, EventArgs e)
        {
            if (rdbtnLoopInTime.Checked)
            {
                txtEndHours.Enabled = txtEndMinutes.Enabled = txtEndSeconds.Enabled = true;
                rdbtnSingle.Checked = rdbtnLoop.Checked = false;
                isSingle = false;

                LoopTime = TotalSeconds * 1000;
                WaitTime = 0;

                labLoopCaption.Text = "執行間隔";
            }
        }

        private delegate void DelSetMouseScript();
        private void SetSingleEnd()
        {
            if (this.InvokeRequired)
            {
                DelSetMouseScript del = new DelSetMouseScript(SetSingleEnd);
                this.Invoke(del);
            }
            else
            {
                nextRumTime = DateTime.Now;

                ScriptStop();
            }
        }
        #endregion

        #region 快捷鍵設定
        private void txtControl_Enter(object sender, EventArgs e)
        {
            textBox = (TextBox)sender;
            isSetControlKey = true;
        }

        private void txtControl_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;
        }

        private void txtControl_Leave(object sender, EventArgs e)
        {
            textBox = null;
            isSetControlKey = false;
        }
        #endregion

        #region 鍵盤指令開始設定和結束設定
        private void txtKeyCommand_Enter(object sender, EventArgs e)
        {
            isSetKeyCommand = true;
        }

        private void txtKeyCommand_Leave(object sender, EventArgs e)
        {
            isSetKeyCommand = false;
        }

        private void ScriptStart()
        {
            if (listCommand.Count == 0)
            {
                frm.ShowMessageBox("錯誤", "腳本尚未設定!", MessageBoxIcon.Error, MessageBoxButtons.OK);
                return;
            }

            SetLoopTime();

            SetEnable(false);
            isScriptStart = frm.isKeyboardScriptRun = timerUpdateLabel.Enabled =
            labCurrentCommandLabel.Visible = labCurrentCommand.Visible = true;

            SetLabel(labStatus, "執行中", Color.DarkGreen);

            if (chbStop.Checked)
            {
                isCheckProcessOn = true;
                new Thread(CheckProcess) { IsBackground = true }.Start();
            }

            Task.Run(RunScript);
        }
        private void ScriptStop()
        {
            isScriptStart = frm.isKeyboardScriptRun = timerUpdateLabel.Enabled =
            labCurrentCommandLabel.Visible = labCurrentCommand.Visible = false;
            if (chbStop.Checked)
                isCheckProcessOn = false;
            SetEnable(true);
            labFormatTime.Text = $"(總共 {TotalSeconds} 秒)";
            SetLabel(labStatus, "未開始", Color.Blue);

            if (rdbtnLoopInTime.Checked)
            {
                int hours = oriTotalEndSeconds / 3600; // 取得小時數
                int minutes = (oriTotalEndSeconds % 3600) / 60; // 取得分鐘數
                int seconds = oriTotalEndSeconds % 60; // 取得秒數

                txtEndHours.Text = hours.ToString();
                txtEndMinutes.Text = minutes.ToString();
                txtEndSeconds.Text = seconds.ToString();
            }
        }
        #endregion

        #region 設定循環時間TextBox
        private void txtRunTime_TextChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(((TextBox)sender).Text))
            {
                SetLoopTime();
            }
            else
                labFormatTime.Text = "";
        }

        private void SetLoopTime()
        {
            int hours = Convert.ToInt32(txtHours.Text); // 小時數
            int minutes = Convert.ToInt32(txtMinutes.Text); // 分鐘數
            int seconds = Convert.ToInt32(txtSeconds.Text); // 秒數
            TotalSeconds = hours * 3600 + minutes * 60 + seconds;

            labFormatTime.Text = $"(總共 {TotalSeconds} 秒)";

            LoopTime = TotalSeconds * 1000;

            nextRumTime = DateTime.Now.AddSeconds(-10);
        }
        #endregion

        /// <summary>
        /// 設定TextBox只能輸入數字
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBoxOnlyNumber_KeyPress(object sender, KeyPressEventArgs e)
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

            if (((TextBox)sender).Text.Length > 1)
            {
                ((TextBox)sender).Text = ((TextBox)sender).Text.Substring(((TextBox)sender).Text.Length - 1);
                ((TextBox)sender).SelectionStart = ((TextBox)sender).Text.Length;
            }
            ((TextBox)sender).Text = ((TextBox)sender).Text.TrimStart('0');
        }

        #region 腳本設定
        #region 設定等待時間
        /// <summary>
        /// 加入等待時間
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAddSleepTime_Click(object sender, EventArgs e)
        {
            if (txtSleepHours.Text == "0" && txtSleepMinutes.Text == "0" && txtSleepSeconds.Text == "0" && txtSleepMilliseconds.Text == "0")
            {
                frm.ShowMessageBox("錯誤", "等待時間不能為「0」!", MessageBoxIcon.Error, MessageBoxButtons.OK);
            }
            else
            {
                int index = 0;
                ScriptCommand sc = new ScriptCommand();
                sc.Type = "其他";
                sc.Function = "等待";
                sc.Command = TotalSleepSeconds.ToString(); ;
                sc.Info = $"等待 {TotalSleepSeconds} 秒";

                if (dgvCommand.SelectedRows.Count > 0)
                    index = dgvCommand.CurrentCell.RowIndex;

                if (index > 0)
                    listCommand.Insert(index + 1, sc);
                else
                    listCommand.Add(sc);

                dgvCommand.DataSource = listCommand;
                if (listCommand.Count > 0)
                    dgvCommand.CurrentCell = dgvCommand.Rows[index + 1].Cells[0];
            }
        }

        /// <summary>
        /// 等待時間TextBox TextChanged
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtSleepTime_TextChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(((TextBox)sender).Text))
            {
                int hours = Convert.ToInt32(txtSleepHours.Text); // 小時數
                int minutes = Convert.ToInt32(txtSleepMinutes.Text); // 分鐘數
                int seconds = Convert.ToInt32(txtSleepSeconds.Text); // 秒數
                int milliseconds = Convert.ToInt32(txtSleepMilliseconds.Text); // 秒數
                TotalSleepSeconds = hours * 3600 + minutes * 60 + seconds + (double)milliseconds / (double)1000;

                labTotalSleepSeconds.Text = $"(總共 {TotalSleepSeconds} 秒)";

                WaitTime = (int)(TotalSleepSeconds * 1000);
            }
        }

        /// <summary>
        /// 等待時間TextBox KeyPress
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtSleepTime_KeyPress(object sender, KeyPressEventArgs e)
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

            if (((TextBox)sender).Name == "txtSleepMilliseconds")
            {
                if (((TextBox)sender).Text.Length > 2)
                {
                    ((TextBox)sender).Text = ((TextBox)sender).Text.Substring(((TextBox)sender).Text.Length - 1);
                    ((TextBox)sender).SelectionStart = ((TextBox)sender).Text.Length;
                }
            }
            else 
            {
                if (((TextBox)sender).Text.Length > 1)
                {
                    ((TextBox)sender).Text = ((TextBox)sender).Text.Substring(((TextBox)sender).Text.Length - 1);
                    ((TextBox)sender).SelectionStart = ((TextBox)sender).Text.Length;
                }
            }
            ((TextBox)sender).Text = ((TextBox)sender).Text.TrimStart('0');
        }
        #endregion

        #region 設定鍵盤指令
        /// <summary>
        /// 設定按鍵TextBox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtKeyCommand_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;
        }

        /// <summary>
        /// 加入按鍵指令
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAddKey_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtKeyCommand.Text))
            {
                frm.ShowMessageBox("錯誤", "按鍵指令不得為空!", MessageBoxIcon.Error, MessageBoxButtons.OK);
            }
            else
            {
                int index = 0;
                ScriptCommand sc = new ScriptCommand();
                sc.Type = "鍵盤";
                sc.Function = keyType;

                sc.Command = GetAutohotkeyCommand(keyType, keyUp);
                sc.Info = $"{keyType} {keyUp}";

                if (dgvCommand.SelectedRows.Count > 0)
                    index = dgvCommand.CurrentCell.RowIndex;

                if (index > 0)
                    listCommand.Insert(index + 1, sc);
                else
                    listCommand.Add(sc);

                dgvCommand.DataSource = listCommand;

                if (listCommand.Count > 1)
                    dgvCommand.CurrentCell = dgvCommand.Rows[index + 1].Cells[0];
            }

            //txtKeyCommand.Focus();
        }
        #endregion

        #region 設定滑鼠指令
        /// <summary>
        /// 加入滑鼠指令
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAddMouse_Click(object sender, EventArgs e)
        {
            try
            {
                int index = 0;
                if (string.IsNullOrEmpty(cmbMouseType.Text))
                {
                    frm.ShowMessageBox("錯誤", "滑鼠模式不得為空!", MessageBoxIcon.Error, MessageBoxButtons.OK);
                }
                else if (string.IsNullOrEmpty(txtPointX.Text) || string.IsNullOrEmpty(txtPointY.Text))
                {
                    frm.ShowMessageBox("錯誤", "滑鼠座標不得為空!", MessageBoxIcon.Error, MessageBoxButtons.OK);
                }
                else
                {
                    ScriptCommand sc = new ScriptCommand();

                    sc.Type = "滑鼠";
                    sc.Function = mouseType;
                    sc.Command = $"{txtPointX.Text},{txtPointY.Text}";
                    if (chbLockPoint.Checked)
                        sc.Info = $"視窗內座標({txtPointX.Text}, {txtPointY.Text})";
                    else
                        sc.Info = $"座標({txtPointX.Text}, {txtPointY.Text})";

                    sc.IsLockPoint = chbLockPoint.Checked;

                    if (dgvCommand.SelectedRows.Count > 0)
                        index = dgvCommand.CurrentCell.RowIndex;

                    if (index > 0)
                        listCommand.Insert(index + 1, sc);
                    else
                        listCommand.Add(sc);

                    dgvCommand.DataSource = listCommand;
                    if (listCommand.Count > 1)
                        dgvCommand.CurrentCell = dgvCommand.Rows[index + 1].Cells[0];
                    else
                        dgvCommand.CurrentCell = dgvCommand.Rows[index].Cells[0];

                    ShowPointOnScreen();
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
        }

        /// <summary>
        /// 設定滑鼠座標
        /// </summary>
        private void SetPoint()
        {
            try
            {

                //取得滑鼠座標
                MousePoint = mouse.CurrentPosition();

                if (chbLockPoint.Checked)
                {
                    GetWindowRect(ProcessName);

                    if (MousePoint.X - WindowRect.Left > WindowRect.Width ||
                        MousePoint.X - WindowRect.Left < 0 ||
                        MousePoint.Y - WindowRect.Top > WindowRect.Height ||
                        MousePoint.Y - WindowRect.Top < 0)
                    {
                        frm.ShowMessageBox("錯誤", "目前為鎖定視窗內座標模式，無法設定已鎖定視窗以外的座標!", MessageBoxIcon.Error, MessageBoxButtons.OK);
                        return;
                    }
                }
                else
                    WindowRect = new System.Windows.Rect();

                txtPointX.Text = (MousePoint.X  - WindowRect.Left).ToString();
                txtPointY.Text = (MousePoint.Y - WindowRect.Top).ToString();
                this.Refresh();
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
        }

        /// <summary>
        /// 取得視窗座標
        /// </summary>
        /// <param name="_ProcessName"></param>
        /// <returns></returns>
        private void GetWindowRect(string _ProcessName)
        {
            Process process = Process.GetProcessesByName(_ProcessName)[0];

            // 取得應用程式主視窗的 AutomationElement
            var mainWindow = AutomationElement.RootElement.FindFirst(
                TreeScope.Children,
                new PropertyCondition(AutomationElement.ProcessIdProperty, process.Id));

            // 獲取主視窗的標題
            string title = mainWindow.Current.Name;
            Console.WriteLine("Title: " + title);

            // 獲取主視窗的大小和位置
            WindowRect = mainWindow.Current.BoundingRectangle;
            Console.WriteLine("Left: " + WindowRect.Left);
            Console.WriteLine("Top: " + WindowRect.Top);
            Console.WriteLine("Width: " + WindowRect.Width);
            Console.WriteLine("Height: " + WindowRect.Height);
        }
        #endregion

        /// <summary>
        /// 加入輸入文字內容
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAddText_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtKeyinText.Text))
            {
                frm.ShowMessageBox("錯誤", "輸入文字不得為空!", MessageBoxIcon.Error, MessageBoxButtons.OK);
            }
            else
            {
                int index = 0;
                ScriptCommand sc = new ScriptCommand();
                sc.Type = "其他";
                sc.Function = "輸入文字";

                sc.Command = txtKeyinText.Text;
                sc.Info = $"{txtKeyinText.Text}";

                if (dgvCommand.SelectedRows.Count > 0)
                    index = dgvCommand.CurrentCell.RowIndex;

                if (index > 0)
                    listCommand.Insert(index + 1, sc);
                else
                    listCommand.Add(sc);

                dgvCommand.DataSource = listCommand;

                if (listCommand.Count > 1)
                    dgvCommand.CurrentCell = dgvCommand.Rows[index + 1].Cells[0];

                txtKeyinText.Text = "";
            }
        }

        /// <summary>
        /// 點擊腳本清單空白區域
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvCommand_MouseClick(object sender, MouseEventArgs e)
        {
            // 檢查滑鼠左鍵點擊
            if (e.Button == MouseButtons.Left)
            {
                // 取得滑鼠點擊的位置
                Point clickLocation = e.Location;

                // 檢查是否在空白區域
                DataGridView.HitTestInfo hitTestInfo = dgvCommand.HitTest(clickLocation.X, clickLocation.Y);
                if (hitTestInfo.Type == DataGridViewHitTestType.None)
                {
                    dgvCommand.ClearSelection();
                }
            }
        }

        /// <summary>
        /// 腳本清單備註編輯
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvCommand_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
            {
                DataGridView dgv = (DataGridView)sender;
                DataGridViewCell editedCell = dgv.Rows[e.RowIndex].Cells[e.ColumnIndex];

                // 編輯的儲存格的位置
                int rowIndex = e.RowIndex;
                int columnIndex = e.ColumnIndex;

                string editedValue = editedCell.Value is null ? "" : editedCell.Value.ToString();

                listCommand[e.RowIndex].Note = editedValue;
                dgvCommand.DataSource = listCommand;
            }
        }

        /// <summary>
        /// 開啟顏色選擇器
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnColorSelector_Click(object sender, EventArgs e)
        {
            try
            {
                string ARGB = "";
                string json = "";
                string pathConfig = "";
                string pathRowColor = "";
                Form main = Application.OpenForms["frmMain"];
                List<RowColorARGB> listARGB = new List<RowColorARGB>();
                ColorDialogExtension colorDialog = new ColorDialogExtension(main.Location.X + main.Size.Width, main.Location.Y);

                pathConfig = Path.Combine(Application.StartupPath, "Config");
                pathRowColor = Path.Combine(Application.StartupPath, "Config", "RowColorARGB.json");

                if (!Directory.Exists(pathConfig))
                    Directory.CreateDirectory(Path.Combine(Application.StartupPath, "Config"));
                if (!File.Exists(pathRowColor))
                    using (FileStream fileStream = File.Create(pathRowColor)) { }

                json = File.ReadAllText(pathRowColor);

                if (json.Length > 0)
                {
                    // 將 JSON 字串轉換為物件結構
                    listARGB = JsonConvert.DeserializeObject<List<RowColorARGB>>(json);

                    //讀取顏色
                    if (listARGB.Count > 0)
                    {
                        colorDialog.CustomColors = listARGB.Select(t => Color.FromArgb(t.A, t.R, t.G, t.B).ToArgb()).ToArray(); ;
                    }
                }

                if (listCommand.Count > 0)
                {
                    int index = dgvCommand.SelectedCells[0].RowIndex;

                    if (index != -1)
                    {
                        if (colorDialog.ShowDialog() == DialogResult.OK)
                        {
                            Color clr = colorDialog.Color;
                            ARGB = $"{clr.A},{clr.R},{clr.G},{clr.B}";
                            int[] customColors = colorDialog.CustomColors;

                            listCommand[index].RowColor = ARGB;
                            dgvCommand.Rows[index].DefaultCellStyle.BackColor = Color.FromArgb(clr.A, clr.R, clr.G, clr.B);

                            listARGB = new List<RowColorARGB>(
                                colorDialog.CustomColors.Select(t => Color.FromArgb(t))
                                    .Select(k => new RowColorARGB
                                    {
                                        A = k.A,
                                        R = k.R,
                                        G = k.G,
                                        B = k.B
                                    })
                            );

                            json = JsonConvert.SerializeObject(listARGB, Formatting.Indented);
                            File.WriteAllText(pathRowColor, json);
                        }
                    }
                    else
                        frm.ShowMessageBox("錯誤", "無可更改顏色的項目!", MessageBoxIcon.Error, MessageBoxButtons.OK);
                }
                else
                    frm.ShowMessageBox("錯誤", "無可更改顏色的項目!", MessageBoxIcon.Error, MessageBoxButtons.OK);
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
        }
        #endregion

        #region 鍵盤偵測
        /// <summary>
        /// 按下
        /// </summary>
        /// <param name="key"></param>
        private void OnKeyDown(string key)
        {
            if (key != keyDown)
            {
                keyDown = key;
                if (isSetKeyCommand)
                {
                }
                Console.WriteLine($"Key down: {key}");
            }
        }

        /// <summary>
        /// 彈起
        /// </summary>
        /// <param name="key"></param>
        private void OnKeyUp(string key)
        {
            if (isSetControlKey)
            {
                #region 設定開始、結束快捷鍵
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
            else if (isSetKeyCommand)
            {
                if (key != keyUp)
                {
                    keyUp = key;

                    txtKeyCommand.Text = keyUp;
                }
            }
            else
            {
                if (key == txtStart.Text && !isScriptStart)//開始
                {
                    ScriptStart();
                }
                else if (key == txtStop.Text && isScriptStart)//結束
                {
                    ScriptStop();
                }
                else if (key == txtSetPoint.Text)
                {
                    if (isScriptStart)
                    {
                        int index = 0;

                        if (dgvCommand.SelectedRows.Count > 0)
                            index = dgvCommand.CurrentCell.RowIndex;
                        if (index > 0)
                        {
                            SetPoint();
                            if (listCommand[index].Command.Contains("Left Click") ||
                                listCommand[index].Command.Contains("Right Click"))
                            {
                                listCommand[index].Command = $"{txtPointX.Text},{txtPointY.Text}";
                                if (chbLockPoint.Checked)
                                    switch (listCommand[index].Type)
                                    {
                                        case "Left Click":
                                            listCommand[index].Info = $"左鍵 視窗內座標({txtPointX.Text}, {txtPointY.Text})";
                                            break;
                                        case "Right Click":
                                            listCommand[index].Info = $"右鍵 視窗內座標({txtPointX.Text}, {txtPointY.Text})";
                                            break;
                                    }
                                else
                                    switch (listCommand[index].Type)
                                    {
                                        case "Left Click":
                                            listCommand[index].Info = $"左鍵 座標({txtPointX.Text}, {txtPointY.Text})";
                                            break;
                                        case "Right Click":
                                            listCommand[index].Info = $"右鍵 座標({txtPointX.Text}, {txtPointY.Text})";
                                            break;
                                    }
                            }

                            dgvCommand.DataSource = listCommand;
                        }
                    }
                    else
                    {
                        SetPoint();
                    }
                }
            }
            Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Key up: {key}");
        }
        #endregion

        #region 控制指令清單按鈕
        /// <summary>
        /// 上移
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnMoveUp_Click(object sender, EventArgs e)
        {
            try
            {
                if (listCommand.Count > 0)
                {
                    int index = dgvCommand.SelectedCells[0].RowIndex;

                    if (index != -1)
                    {
                        int newIndex = index == 0 ? 0 : index - 1;

                        if (index == newIndex)
                            return;
                        if (index >= listCommand.Count || newIndex >= listCommand.Count)
                            return;

                        ScriptCommand sc = listCommand[index];
                        listCommand.RemoveAt(index);
                        listCommand.Insert(newIndex, sc);

                        dgvCommand.DataSource = listCommand;

                        Color clr = ConvertArgbToColor(listCommand[index].RowColor);
                        dgvCommand.Rows[index].DefaultCellStyle.BackColor = Color.FromArgb(clr.A, clr.R, clr.G, clr.B);

                        clr = ConvertArgbToColor(listCommand[index - 1].RowColor);
                        dgvCommand.Rows[index - 1].DefaultCellStyle.BackColor = Color.FromArgb(clr.A, clr.R, clr.G, clr.B);

                        if (listCommand.Count > 0)
                        {
                            dgvCommand.ClearSelection();
                            dgvCommand.Rows[index - 1].Selected = true;
                        }
                    }
                    else
                        frm.ShowMessageBox("錯誤", "無可移動的項目!", MessageBoxIcon.Error, MessageBoxButtons.OK);
                }
                else
                    frm.ShowMessageBox("錯誤", "無可移動的項目!", MessageBoxIcon.Error, MessageBoxButtons.OK);
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
        }

        /// <summary>
        /// 下移
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnMoveDown_Click(object sender, EventArgs e)
        {
            try
            {
                if (listCommand.Count > 0)
                {
                    int index = dgvCommand.SelectedCells[0].RowIndex;

                    if (index != -1)
                    { 
                        int newIndex = index == listCommand.Count ? 0 : index + 1;

                        if (index == newIndex)
                            return;
                        if (index >= listCommand.Count || newIndex >= listCommand.Count)
                            return;

                        ScriptCommand sc = listCommand[index];
                        listCommand.RemoveAt(index);
                        listCommand.Insert(newIndex, sc);

                        dgvCommand.DataSource = listCommand;

                        Color clr = ConvertArgbToColor(listCommand[index].RowColor);
                        dgvCommand.Rows[index].DefaultCellStyle.BackColor = Color.FromArgb(clr.A, clr.R, clr.G, clr.B);

                        clr = ConvertArgbToColor(listCommand[index + 1].RowColor);
                        dgvCommand.Rows[index + 1].DefaultCellStyle.BackColor = Color.FromArgb(clr.A, clr.R, clr.G, clr.B);

                        if (listCommand.Count > 0)
                        {
                            dgvCommand.ClearSelection();
                            dgvCommand.Rows[index + 1].Selected = true;
                        }
                    }
                    else
                        frm.ShowMessageBox("錯誤", "無可移動的項目!", MessageBoxIcon.Error, MessageBoxButtons.OK);
                }
                else
                    frm.ShowMessageBox("錯誤", "無可移動的項目!", MessageBoxIcon.Error, MessageBoxButtons.OK);
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
        }

        /// <summary>
        /// 移除
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRemove_Click(object sender, EventArgs e)
        {
            try
            {
                if (listCommand.Count > 0)
                {
                    int index = dgvCommand.SelectedCells[0].RowIndex;

                    if (index != -1)
                    {
                        listCommand.RemoveAt(index);

                        dgvCommand.DataSource = listCommand;


                        //Color clr = ConvertArgbToColor(listCommand[index].RowColor);
                        //dgvCommand.Rows[index].DefaultCellStyle.BackColor = Color.FromArgb(clr.A, clr.R, clr.G, clr.B);

                        //clr = ConvertArgbToColor(listCommand[index - 1].RowColor);
                        //dgvCommand.Rows[index - 1].DefaultCellStyle.BackColor = Color.FromArgb(clr.A, clr.R, clr.G, clr.B);

                        if (listCommand.Count > 0)
                        {
                            dgvCommand.ClearSelection();
                            dgvCommand.Rows[index - 1].Selected = true;
                        }

                    }
                    else
                        frm.ShowMessageBox("錯誤", "無可刪除的項目!", MessageBoxIcon.Error, MessageBoxButtons.OK);
                }
                else
                    frm.ShowMessageBox("錯誤", "無可刪除的項目!", MessageBoxIcon.Error, MessageBoxButtons.OK);
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
        }
        #endregion

        #region 鎖定視窗
        /// <summary>
        /// 滑鼠掛勾(偵測Mouse Click)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MouseHook_MouseClick(object sender, MouseEventArgs e)
        {
            timerFindWindow.Enabled = true;
        }

        /// <summary>
        /// 取得目前活躍視窗的控制項
        /// </summary>
        /// <returns></returns>
        private IntPtr GetActiveWindow()
        {
            const int nChars = 256;
            StringBuilder sb = new StringBuilder(nChars);

            IntPtr handle = GetForegroundWindow();
            if (GetWindowText(handle, sb, nChars) > 0)
                return handle;

            return IntPtr.Zero;
        }

        /// <summary>
        /// 尋找視窗Timer
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timerFindWindow_Tick(object sender, EventArgs e)
        {
            // 取得目前活躍視窗的控制項
            IntPtr activeWindowHandle = GetActiveWindow();

            // 取得該視窗的 Process
            uint processId;
            GetWindowThreadProcessId(activeWindowHandle, out processId);
            Process process = Process.GetProcessById((int)processId);

            if (process.ProcessName != "MapleStoryTools")
            {
                Console.WriteLine("Process Name: " + process.ProcessName);
                Console.WriteLine("Process ID: " + process.Id);

                ProcessName = txtWindowName.Text = process.ProcessName;

                //取消訂閱滑鼠事件
                MouseHook.Stop();
                timerFindWindow.Enabled = false;
                btnLock.Enabled = chbLockWindow.Enabled = true;
                btnLock.Text = "鎖定";
                frm.ShowMessageBox("提示", $"已鎖定視窗｢{ProcessName}｣，請確認名稱是否正確，若有錯誤請重新鎖定!", MessageBoxIcon.Information, MessageBoxButtons.OK);
            }
        }

        /// <summary>
        /// 鎖定視窗按鈕
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnLock_Click(object sender, EventArgs e)
        {
            btnLock.Enabled = chbLockWindow.Enabled = false;
            btnLock.Text = "鎖定中";
            //訂閱滑鼠事件
            MouseHook.Start();
        }

        /// <summary>
        /// 鎖定視窗CheckBox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void chbLock_CheckedChanged(object sender, EventArgs e)
        {
            if (chbLockWindow.Checked)
            {
                gpbLockWindows.Enabled = true;
                if (txtWindowName.Text != "")
                    chbLockPoint.Enabled = chbStop.Enabled = true;
                else
                    chbLockPoint.Enabled = chbStop.Enabled = false;
            }
            else
            {
                txtWindowName.Text = ProcessName = "";
                gpbLockWindows.Enabled = chbLockPoint.Enabled = chbStop.Checked = false;
            }
        }

        /// <summary>
        /// 鎖定視窗座標CheckBox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void chbLockPoint_CheckedChanged(object sender, EventArgs e)
        {
            if (chbLockPoint.Checked)
            {
                if (frm.ShowMessageBox("提示", "已開啟鎖定視窗內座標功能，已設定的座標將自動轉換為視窗內座標!", MessageBoxIcon.Information, MessageBoxButtons.OK) == DialogResult.OK)
                {
                    string Lock = chbLockPoint.Checked ? "視窗內座標" : "座標";

                    foreach (ScriptCommand item in listCommand)
                    {
                        if (item.Type == "滑鼠")
                        {
                            item.Command = $"{Convert.ToInt32(item.Command.Split(',')[0]) - WindowRect.Left},{Convert.ToInt32(item.Command.Split(',')[1]) - WindowRect.Top}";
                            item.Info = $"{Lock}({Convert.ToInt32(item.Command.Split(',')[0]) - WindowRect.Left}, {Convert.ToInt32(item.Command.Split(',')[1]) - WindowRect.Top})";
                        }

                    }
                    dgvCommand.DataSource = listCommand;
                    this.Refresh();
                }
            }
            else
            {
                foreach (ScriptCommand item in listCommand)
                {
                    if (item.Type == "滑鼠")
                    {
                        item.Command = $"{Convert.ToInt32(item.Command.Split(',')[0]) - WindowRect.Left},{Convert.ToInt32(item.Command.Split(',')[1]) - WindowRect.Top}";
                        item.Info = $"({Convert.ToInt32(item.Command.Split(',')[0]) + WindowRect.Left}, {Convert.ToInt32(item.Command.Split(',')[1]) + WindowRect.Top})";
                    }
                }
                dgvCommand.DataSource = listCommand;
                this.Refresh();
            }
        }

        /// <summary>
        /// 應用程式名稱
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtWindowName_TextChanged(object sender, EventArgs e)
        {
            if (chbLockWindow.Checked && txtWindowName.Text != "")
            {
                chbLockPoint.Enabled = chbStop.Enabled = true;
                chbLockPoint.Checked = chbStop.Checked = false;
            }
            else
                chbLockPoint.Enabled = chbStop.Enabled = false;
        }
        #endregion

        #region 視窗關閉時停止腳本
        /// <summary>
        /// 檢查視窗是否關閉
        /// </summary>
        private void CheckProcess()
        {
            DateTime next = DateTime.Now;
            while (isCheckProcessOn) 
            {
                DateTime now = DateTime.Now;

                if (now >= next)
                {
                    try
                    {
                        next = DateTime.Now.AddMinutes(1);
                        Process[] _process = Process.GetProcessesByName(ProcessName);
                        if (_process == null || _process.Length == 0)
                            new Thread(SetSingleEnd).Start();
                    }
                    catch (Exception ex)
                    {
                        log.Error(ex);
                        throw;
                    }
                }
                Thread.Sleep(100);
            }
        }
        #endregion

        /// <summary>
        /// 更新倒數時間Timer
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timerUpdateLabel_Tick(object sender, EventArgs e)
        {
            try
            {
                if (isScriptStart)
                {
                    if (currentCommand.Function == "等待")
                    {
                        if (currendWaitTime == 0)
                            currendWaitTime = Convert.ToDouble(currentCommand.Command);
                        else
                        {
                            currendWaitTime -= 0.1;
                            if (currendWaitTime < 0)
                                currendWaitTime = 0;
                        }
                        labCurrentCommand.Text = $"等待 {Math.Round(currendWaitTime, 1, MidpointRounding.AwayFromZero):0.0} 秒";
                    }
                    else
                    {
                        currendWaitTime = 0;
                        labCurrentCommand.Text = currentCommand.Info;
                    }

                    if (nextRumTime >= DateTime.Now)
                    {
                        string hour = (nextRumTime - DateTime.Now).Hours + "時";
                        string minute = (nextRumTime - DateTime.Now).Minutes + "分";
                        string second = (nextRumTime - DateTime.Now).Seconds + "秒";
                        labFormatTime.Text = $"(下次執行倒數：{hour} {minute} {second})";

                        if (rdbtnLoopInTime.Checked)
                        {
                            if (DateTime.Now <= LoopEndTime)
                            {
                                txtEndHours.Text = (LoopEndTime - DateTime.Now).Hours.ToString();
                                txtEndMinutes.Text = (LoopEndTime - DateTime.Now).Minutes.ToString();
                                txtEndSeconds.Text = (LoopEndTime - DateTime.Now).Seconds.ToString();
                            }
                            else
                                ScriptStop();
                        }
                    }
                    else
                    {
                        labFormatTime.Text = "(腳本執行中...)";
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
        }

        /// <summary>
        /// 開始Label
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void labStart_Click(object sender, EventArgs e)
        {
            ScriptStart();
        }

        /// <summary>
        /// 停止Label
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void labStop_Click(object sender, EventArgs e)
        {
            ScriptStop();
        }

        /// <summary>
        /// 程式關閉
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void frmAutoScripts_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                if (isFormOpen)
                {
                    Console.WriteLine("frmAutoScripts_FormClosing");

                    //frm.SaveFormLocation(this);

                    if ((string.IsNullOrEmpty(txtScriptFileName.Text) && listCommand.Count > 0) ||
                        (!string.IsNullOrEmpty(txtScriptFileName.Text) && listCommand.Count == 0))
                    {
                        if (frm.ShowMessageBox("提示", "目前腳本尚未存檔，請問是否需儲存?", MessageBoxIcon.Question, MessageBoxButtons.YesNo) == DialogResult.Yes)
                        {
                            if (SaveScripts() == DialogResult.Cancel)
                                e.Cancel = true;
                        }
                    }
                    else if (!string.IsNullOrEmpty(txtScriptFileName.Text))
                    {
                        BindingList<ScriptCommand> checkCommand = new BindingList<ScriptCommand>();
                        string fileData = File.ReadAllText(txtScriptFilePath.Text);
                        JsonConvert.PopulateObject(fileData, checkCommand);
                        bool isErual = true;
                        if (listCommand.Count == checkCommand.Count)
                        {
                            for (int i = 0; i < listCommand.Count; i++)
                            {

                                //isErual &= listCommand[i].Type == checkCommand[i].Type;
                                //isErual &= listCommand[i].Function == checkCommand[i].Function;
                                //isErual &= listCommand[i].Command == checkCommand[i].Command;
                                //isErual &= listCommand[i].Info == checkCommand[i].Info;
                                //isErual &= listCommand[i].IsLockPoint == checkCommand[i].IsLockPoint;
                                //isErual &= listCommand[i].Note == checkCommand[i].Note;
                                //isErual &= listCommand[i].RowColor == checkCommand[i].RowColor;

                                isErual = listCommand[i].Equals(checkCommand[i]);
                                
                                if (!isErual)
                                    break;
                            }
                        }
                        else
                            isErual = false;

                        if (!isErual)
                        {
                            if (frm.ShowMessageBox("提示", "腳本內容有更新，請問是否需儲存?", MessageBoxIcon.Question, MessageBoxButtons.YesNo) == DialogResult.Yes)
                            {
                                if (SaveScripts() == DialogResult.Cancel)
                                    e.Cancel = true;
                            }
                        }
                    }

                    SaveHotKey("開始", txtStart.Text);
                    SaveHotKey("停止", txtStop.Text);
                    SaveHotKey("設定座標", txtSetPoint.Text);

                    SaveHotKey("執行間隔(時)", txtHours.Text);
                    SaveHotKey("執行間隔(分)", txtMinutes.Text);
                    SaveHotKey("執行間隔(秒)", txtSeconds.Text);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 儲存快捷鍵
        /// </summary>
        /// <param name="pKeyName"></param>
        /// <param name="pHotKey"></param>
        public void SaveHotKey(string pKeyName, string pHotKey )
        {
            try
            {
                if (!Directory.Exists(Path.Combine(Application.StartupPath, "Config")))
                    Directory.CreateDirectory(Path.Combine(Application.StartupPath, "Config"));

                if (!File.Exists(pathHotKey))
                    using (FileStream fileStream = File.Create(pathHotKey)) { }

                string fileData = File.ReadAllText(pathHotKey);
                List<HotKey> data = new List<HotKey>();

                if (fileData.Length > 0)
                {
                    // 將 JSON 字串轉換為物件結構
                    data = JsonConvert.DeserializeObject<List<HotKey>>(fileData);

                    data.RemoveAll(t => t.Key == pKeyName);
                }

                data.Add(new HotKey
                {
                    Key = pKeyName,
                    Command = pHotKey
                });

                string json = JsonConvert.SerializeObject(data, Formatting.Indented);

                File.WriteAllText(pathHotKey, json);
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
        }

        /// <summary>
        /// 設定快捷鍵
        /// </summary>
        /// <param name="pKeyName"></param>
        /// <param name="ptextbox"></param>
        private void SetHotKey(string pKeyName, TextBox ptextbox)
        {
            try
            {
                if (!File.Exists(Path.Combine(Application.StartupPath, "Config")))
                    Directory.CreateDirectory(Path.Combine(Application.StartupPath, "Config"));

                if (!File.Exists(pathHotKey))
                    using (FileStream fileStream = File.Create(pathHotKey)) { }
                else
                {
                    string fileData = File.ReadAllText(pathHotKey);

                    if (fileData.Length > 0)
                    {
                        // 將 JSON 字串轉換為物件結構
                        listHotKey = JsonConvert.DeserializeObject<List<HotKey>>(fileData);
                        
                        if (listHotKey.AsEnumerable().Count(t => t.Key == pKeyName) > 0)
                            ptextbox.Text = listHotKey.FirstOrDefault(t => t.Key == pKeyName).Command;
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
        }

        /// <summary>
        /// 將ARGB轉換為Color
        /// </summary>
        /// <param name="argb"></param>
        /// <returns></returns>
        private Color ConvertArgbToColor(string argb)
        {
            Color clr = new Color();
            int A = Convert.ToInt32(argb.Split(',')[0]);
            int R = Convert.ToInt32(argb.Split(',')[1]);
            int G = Convert.ToInt32(argb.Split(',')[2]);
            int B = Convert.ToInt32(argb.Split(',')[3]);
            clr = Color.FromArgb(A, R, G, B);
            return clr;
        }

        /// <summary>
        /// 防止畫面閃爍
        /// </summary>
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x02000000; // Turn on WS_EX_COMPOSITED 
                return cp;
            }
        }

        #region 設定循環結束時間
        private void txtLoopEndTime_TextChanged(object sender, EventArgs e)
        {
            if (rdbtnLoopInTime.Checked && !string.IsNullOrEmpty(((TextBox)sender).Text))
            {
                SetLoopEndTime();
            }
        }

        private void SetLoopEndTime()
        {
            int hours = Convert.ToInt32(txtEndHours.Text); // 小時數
            int minutes = Convert.ToInt32(txtEndMinutes.Text); // 分鐘數
            int seconds = Convert.ToInt32(txtEndSeconds.Text); // 秒數
            TotalEndSeconds = hours * 3600 + minutes * 60 + seconds;
        }

        #endregion

        private void chbShowPoint_CheckedChanged(object sender, EventArgs e)
        {
            ShowPointOnScreen();
        }

        private void ShowPointOnScreen()
        {
            if (chbShowPoint.Checked)
            {
                foreach (ScriptCommand sc in listCommand)
                {
                    if (sc.Type == "滑鼠")
                    {
                        int screenWidth = SystemInformation.PrimaryMonitorSize.Width;
                        int screenHeight = SystemInformation.PrimaryMonitorSize.Height;
                        frmPoint frm = new frmPoint();
                        Point point = ConvertStringToPoint(sc.Command.Split(',')[0], sc.Command.Split(',')[1]);
                        frm.point = point;
                        frm.StartPosition = FormStartPosition.Manual;

                        if (point.X <= frm.Width && point.Y <= frm.Height)
                        {
                            frm.Location = new Point(point.X, point.Y);
                            frm.SetPoint("左上");
                        }
                        else if (point.X <= frm.Width && point.Y + frm.Height >= screenHeight)
                        {
                            frm.Location = new Point(point.X, point.Y - frm.Height);
                            frm.SetPoint("左下");
                        }
                        else if (point.X + frm.Width >= screenHeight && point.Y + frm.Height >= screenHeight)
                        {
                            frm.Location = new Point(point.X - frm.Width, point.Y - frm.Height);
                            frm.SetPoint("右下");
                        }
                        else
                        {
                            frm.Location = new Point(point.X - frm.Width, point.Y);
                            frm.SetPoint("右上");
                        }

                        frm.Show();
                        listFrmPoint.Add(frm);
                    }
                }
            }
            else
            {
                foreach (frmPoint frm in listFrmPoint)
                {
                    frm.Close();
                }
            }
        }
    }

    public class ScriptCommand
    {        
        public string Type { get; set; }
        public string Function { get; set; }
        public string Command { get; set; }
        public string Info { get; set; } = "";
        public bool IsLockPoint { get; set; } = false;
        public string Note { get; set; } = "";
        public string RowColor { get; set; } = "";

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;

            ScriptCommand other = (ScriptCommand)obj;
            return Type == other.Type && Function == other.Function && Command == other.Command &&
                Info == other.Info && IsLockPoint == other.IsLockPoint && RowColor == other.RowColor && Note == other.Note;
        }

        public override int GetHashCode()
        {
            return new { Type, Function, Command, Info, IsLockPoint, Note, RowColor }.GetHashCode();
        }
    }

    public class oldScriptCommand
    {
        public string Name { get; set; }
        public string Command { get; set; }
        public Point Point { get; set; }
        public bool LockWindow { get; set; }
    }

    public class HotKey
    {
        public string Key { get; set; }
        public string Command { get; set; }
    }

    public class RowColorARGB
    {
        public int A { get; set; }
        public int R { get; set; }
        public int G { get; set; }
        public int B { get; set; }
    }
}