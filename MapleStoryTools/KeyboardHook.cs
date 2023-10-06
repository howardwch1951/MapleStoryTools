using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Runtime.CompilerServices.RuntimeHelpers;

namespace MapleStoryTools
{
    internal class KeyboardHook
    {
        private const int WH_KEYBOARD_LL = 13;
        private const int WM_KEYDOWN = 0x0100;
        private const int WM_KEYUP = 0x0101;
        private const int VK_LALT = 0xA4; // Alt 鍵的虛擬鍵碼
        private const int VK_RALT = 0xA5; // Alt 鍵的虛擬鍵碼
        private const int WM_ALTDOWN = 0x0104;

        private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        private LowLevelKeyboardProc _proc;
        private IntPtr _hookId = IntPtr.Zero;

        public event Action<string> KeyDown;
        public event Action<string> KeyUp;

        public void Start()
        {
            _proc = HookCallback;
            _hookId = SetHook(_proc);
        }

        public void Stop()
        {
            UnhookWindowsHookEx(_hookId);
        }

        private IntPtr SetHook(LowLevelKeyboardProc proc)
        {
            using (Process curProcess = Process.GetCurrentProcess())
            using (ProcessModule curModule = curProcess.MainModule)
            {
                return SetWindowsHookEx(WH_KEYBOARD_LL, proc, GetModuleHandle(curModule.ModuleName), 0);
            }
        }

        private IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0)
            {
                int keyCode = Marshal.ReadInt32(lParam);
                Keys key = (Keys)keyCode;
                if (wParam == (IntPtr)WM_KEYDOWN)
                {
                    KeyDown?.Invoke(FormatKeyValue(key));
                }
                else if (wParam == (IntPtr)WM_KEYUP)
                {
                    KeyUp?.Invoke(FormatKeyValue(key));
                }
                else if (wParam == (IntPtr)WM_ALTDOWN)
                {
                    KeyDown?.Invoke(FormatKeyValue(key));
                }
            }

            return CallNextHookEx(_hookId, nCode, wParam, lParam);
        }

        public string FormatKeyValue(Keys key)
        {
            switch (key)
            {
                case Keys.Escape:
                    return "Esc";
                case Keys.Oemtilde:
                    return "``";
                case Keys.Tab:
                    return "Tab";
                case Keys.Capital:
                    return "CapsLock";
                case Keys.LShiftKey:
                    return "LShift";
                case Keys.RShiftKey:
                    return "RShift";
                case Keys.LControlKey:
                case Keys.RControlKey:
                    return "Ctrl";
                case Keys.LWin:
                    return "Win";
                case Keys.LMenu:
                case Keys.RMenu:
                    return "Alt";
                case Keys.Apps:
                    return "Menu";
                case Keys.Return:
                    return "Enter";
                case Keys.PageDown:
                    return "PgDn";
                case Keys.PageUp:
                    return "PgUp";
                case Keys.Oem5:
                    return "\\";
                case Keys.Back:
                    return "BackSpace";
                case Keys.PrintScreen:
                    return "PrtSc";
                case Keys.Decimal:
                    return ".";
                case Keys.Add:
                    return "+";
                case Keys.Subtract:
                    return "-";
                case Keys.Multiply:
                    return "*";
                case Keys.Divide:
                    return "/";
                case Keys.D0:
                case Keys.D1:
                case Keys.D2:
                case Keys.D3:
                case Keys.D4:
                case Keys.D5:
                case Keys.D6:
                case Keys.D7:
                case Keys.D8:
                case Keys.D9:
                    return key.ToString().Replace("D", "");
                case Keys.OemPeriod:
                    return ".";
                case Keys.Oemcomma:
                    return ",";
                default:
                    return key.ToString();
            }
        }
    }
}
