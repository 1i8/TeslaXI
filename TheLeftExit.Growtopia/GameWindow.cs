using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public GameWindow(Int32 processId) => windowHandle = Process.GetProcessById(processId).MainWindowHandle;
    }
}
