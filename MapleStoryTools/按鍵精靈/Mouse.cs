using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MapleStoryTools
{
    internal class Mouse
    {
        [DllImport("user32.dll", SetLastError = true)]
        public static extern void mouse_event(uint dwFlags, int dx, int dy, uint dwData, int dwExtraInfo);

        [DllImport("user32.dll")]
        public static extern bool SetCursorPos(int x, int y);

        private const uint MOUSEEVENTF_LEFTDOWN = 0x0002;
        private const uint MOUSEEVENTF_LEFTUP = 0x0004;
        private const uint MOUSEEVENTF_RIGHTDOWN = 0x0008;
        private const uint MOUSEEVENTF_RIGHTUP = 0x0010;
        private const uint MOUSEEVENTF_MIDDLEDOWN = 0x0020;
        private const uint MOUSEEVENTF_MIDDLEUP = 0x0040;
        private const uint MOUSEEVENTF_ABSOLUTE = 0x8000;
        private const uint MOUSEEVENTF_MOVE = 0x0001;

        [StructLayout(LayoutKind.Sequential)]
        struct INPUT
        {
            public uint type;
            public MOUSEINPUT mi;
        }

        [StructLayout(LayoutKind.Sequential)]
        struct MOUSEINPUT
        {
            public int dx;
            public int dy;
            public uint mouseData;
            public uint dwFlags;
            public uint time;
            public IntPtr dwExtraInfo;
        }

        public void LeftClick(Point pPoint)
        {
            int x = pPoint.X;
            int y = pPoint.Y;

            MoveMouse(pPoint);

            Thread.Sleep(100);

            mouse_event(MOUSEEVENTF_LEFTDOWN, 0, 0, 0, 0);

            Thread.Sleep(100);

            mouse_event(MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);

            Thread.Sleep(100);
        }

        public void RightClick(Point pPoint)
        {
            int x = pPoint.X;
            int y = pPoint.Y;

            MoveMouse(pPoint);

            Thread.Sleep(100);

            mouse_event(MOUSEEVENTF_RIGHTDOWN, 0, 0, 0, 0);

            Thread.Sleep(100);

            mouse_event(MOUSEEVENTF_RIGHTUP, 0, 0, 0, 0);

            Thread.Sleep(100);
        }

        public Point CurrentPosition()
        {
            // 獲取目前滑鼠的座標
            Point mousePosition = Cursor.Position;
            return mousePosition;
        }

        public void MoveMouse(Point pPoint)
        {
            SetCursorPos(pPoint.X, pPoint.Y);
            //Cursor.Position = point;
            Cursor.Show();
        }

    }
}
