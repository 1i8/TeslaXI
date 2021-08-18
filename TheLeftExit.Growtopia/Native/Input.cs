using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using System.Drawing;

namespace TheLeftExit.Growtopia.Native
{
    public static class Input
    {
        private const uint WM_KEYDOWN = 0x0100;
        private const uint WM_KEYUP = 0x0101;

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr SendMessage(IntPtr hWnd, UInt32 Msg, UInt32 wParam, UInt32 lParam);

        private static UInt32 MAKELPARAM(Int32 x, Int32 y) => (UInt32)(y << 16 | x & 0xFFFF);

        /// <summary>
        /// Sends a key event for <paramref name="key"/> to a window handle <paramref name="handle"/>.
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="key"></param>
        /// <param name="down"></param>
        public static void SendKey(this IntPtr handle, VK key, bool down) =>
            SendMessage(handle, down ? WM_KEYDOWN : WM_KEYUP, (UInt32)key, 0);

        /// <summary>
        /// Sends a left mouse button mouse event at (<paramref name="x"/>, <paramref name="y"/>) to a window handle <paramref name="handle"/>.
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="action"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public static void SendMouse(this IntPtr handle, WM action, Int32 x, Int32 y) =>
            SendMessage(handle, (UInt32)action, (UInt32)MK.LBUTTON, MAKELPARAM(x, y));

        [DllImport("USER32.DLL")]
        public static extern bool SetWindowText(this IntPtr hWnd, String lpString);

        [DllImport("user32.dll")]
        private static extern short GetAsyncKeyState(int vKey);

        /// <summary>
        /// Check whether <paramref name="key"/> is down.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static bool IsKeyDown(this VK key) =>
            (GetAsyncKeyState((Int32)key) & 0x8000) != 0;
    }
}