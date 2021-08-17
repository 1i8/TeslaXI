using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;

using TheLeftExit.Growtopia.Native;

namespace TheLeftExit.Growtopia
{
    public class GameWindow
    {
        private IntPtr windowHandle;

        public const VK
            PunchKey = VK.Space,
            JumpKey = VK.W,
            LeftKey = VK.A,
            RightKey = VK.D,
            EscapeKey = VK.Escape;

        public void KeyDown(VK key) => windowHandle.SendKey(key, true);
        public void KeyUp(VK key) => windowHandle.SendKey(key, false);
        public void SendKey(VK key, bool down) => windowHandle.SendKey(key, down);
        public void KeyPress(VK key, Int32 duration)
        {
            KeyDown(key); Thread.Sleep(duration); KeyUp(key);
        }

        public void MouseDown(Int32 x, Int32 y) => windowHandle.SendMouse(WM.LBUTTONDOWN, x, y);
        public void MouseUp(Int32 x, Int32 y) => windowHandle.SendMouse(WM.LBUTTONUP, x, y);
        public void MouseMove(Int32 x, Int32 y) => windowHandle.SendMouse(WM.MOUSEMOVE, x, y);
        public void MouseClick(Int32 x, Int32 y, Int32 duration)
        {
            MouseDown(x, y); Thread.Sleep(duration); MouseUp(x, y);
        }

        public GameWindow(Int32 processId) => windowHandle = Process.GetProcessById(processId).MainWindowHandle;
    }
}
