using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;

using TheLeftExit.Growtopia.Native;

namespace TheLeftExit.Growtopia
{
    /// <summary>
    /// Wrapper for basic input functionality.
    /// </summary>
    public class GameWindow
    {
        private IntPtr windowHandle;

        /// <summary>
        /// Default key bindings (and Space for punch).
        /// </summary>
        public const VK
            PunchKey = VK.Space,
            JumpKey = VK.W,
            LeftKey = VK.A,
            RightKey = VK.D,
            EscapeKey = VK.Escape;

        /// <summary>
        /// Sends a KEY DOWN event. Don't forget to send the KEY UP afterward!
        /// </summary>
        /// <param name="key"></param>
        public void KeyDown(VK key) => windowHandle.SendKey(key, true);
        /// <summary>
        /// Sends a KEY UP event.
        /// </summary>
        /// <param name="key"></param>
        public void KeyUp(VK key) => windowHandle.SendKey(key, false);
        /// <summary>
        /// Sends a KEY UP/DOWN event.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="down"></param>
        public void SendKey(VK key, bool down) => windowHandle.SendKey(key, down);
        /// <summary>
        /// Sends a KEY DOWN event, waits for <paramref name="duration"/> milliseconds, then sends a KEY UP event.<br/>
        /// Code execution is paused for the duration.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="duration"></param>
        public void KeyPress(VK key, Int32 duration)
        {
            KeyDown(key); Thread.Sleep(duration); KeyUp(key);
        }

        /// <summary>
        /// Sends a left button MOUSE DOWN event at specified location. Don't forget to send the MOUSE UP afterward!
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void MouseDown(Int32 x, Int32 y) => windowHandle.SendMouse(WM.LBUTTONDOWN, x, y);
        /// <summary>
        /// Sends a left button MOUSE UP event.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void MouseUp(Int32 x, Int32 y) => windowHandle.SendMouse(WM.LBUTTONUP, x, y);
        /// <summary>
        /// Sends a MOUSE MOVE event.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void MouseMove(Int32 x, Int32 y) => windowHandle.SendMouse(WM.MOUSEMOVE, x, y);
        /// <summary>
        /// Sends a left button MOUSE DOWN event, waits for <paramref name="duration"/> milliseconds, then sends a MOUSE UP event.<br/>
        /// Code execution is paused for the duration.
        /// </summary>
        public void MouseClick(Int32 x, Int32 y, Int32 duration)
        {
            MouseDown(x, y); Thread.Sleep(duration); MouseUp(x, y);
        }

        /// <summary>
        /// Creates a new <see cref="GameWindow"/> for process with ID of <paramref name="processId"/>.
        /// </summary>
        /// <param name="processId"></param>
        public GameWindow(Int32 processId) => windowHandle = Process.GetProcessById(processId).MainWindowHandle;
    }
}
