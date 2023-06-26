using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Automation;
using System.Windows.Forms;

namespace MapleStoryTools
{
    public partial class frmLockWindows : Form
    {
        #region Win32
        // 導入取得視窗控制項的 Process 函式
        [DllImport("user32.dll", SetLastError = true)]
        public static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        // 導入取得目前活躍視窗函式
        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        // 導入取得視窗控制項標題函式
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);
        #endregion

        public frmLockWindows()
        {
            // 滑鼠點擊事件處理
            MouseHook.MouseClick += MouseHook_MouseClick;

            InitializeComponent();
        }

        private void MouseHook_MouseClick(object sender, MouseEventArgs e)
        {
            timer1.Enabled = true;
        }

        private void frmLockWindows_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            Process process = Process.GetProcessesByName("LINE")[0];

            // 取得應用程式主視窗的 AutomationElement
            var mainWindow = AutomationElement.RootElement.FindFirst(
                TreeScope.Children,
                new PropertyCondition(AutomationElement.ProcessIdProperty, process.Id));

            // 獲取主視窗的標題
            string title = mainWindow.Current.Name;
            Console.WriteLine("Title: " + title);

            // 獲取主視窗的大小和位置
            System.Windows.Rect bounds = mainWindow.Current.BoundingRectangle;
            Console.WriteLine("Left: " + bounds.Left);
            Console.WriteLine("Top: " + bounds.Top);
            Console.WriteLine("Width: " + bounds.Width);
            Console.WriteLine("Height: " + bounds.Height);
        }

        private void frmLockWindows_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.OpenForms["frmMain"].Show();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            // 訂閱滑鼠事件
            MouseHook.Start();
        }

        // 取得目前活躍視窗的控制項
        private static IntPtr GetActiveWindow()
        {
            const int nChars = 256;
            StringBuilder sb = new StringBuilder(nChars);

            IntPtr handle = GetForegroundWindow();
            if (GetWindowText(handle, sb, nChars) > 0)
            {
                return handle;
            }

            return IntPtr.Zero;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            // 取得目前活躍視窗的控制項
            IntPtr activeWindowHandle = GetActiveWindow();

            // 取得該視窗的 Process
            uint processId;
            GetWindowThreadProcessId(activeWindowHandle, out processId);
            Process process = Process.GetProcessById((int)processId);

            if (process.ProcessName != "MapleStoryTools")
            {
                // 顯示 Process 資訊
                Console.WriteLine("Process Name: " + process.ProcessName);
                Console.WriteLine("Process ID: " + process.Id);


                textBox1.Text = "Process Name: " + process.ProcessName;
                textBox2.Text = "Process ID: " + process.Id;

                MouseHook.Stop();
                timer1.Enabled = false;
            }
        }
    }
}
