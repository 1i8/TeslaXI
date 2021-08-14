using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TheLeftExit.Growtopia.Native
{
    public static class Input
    {
        private const uint WM_KEYDOWN = 0x0100;
        private const uint WM_KEYUP = 0x0101;

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, uint wParam, uint lParam);

        public static void SendKey(this IntPtr handle, VK key, bool down)
        {
            if (handle != IntPtr.Zero)
                SendMessage(handle, down ? WM_KEYDOWN : WM_KEYUP, (uint)key, 0);
        }

        [DllImport("USER32.DLL")]
        public static extern bool SetWindowText(this IntPtr hWnd, string lpString);

        [DllImport("user32.dll")]
        private static extern short GetAsyncKeyState(int vKey);

        public static bool IsKeyDown(this VK key) =>
            (GetAsyncKeyState((Int32)key) & 0x8000) != 0;
    }
}