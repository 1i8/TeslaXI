using System;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;
using System.Collections.Generic;
using System.Drawing;

using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;

using TheLeftExit.Memory.Queries;
using TheLeftExit.Memory.RTTI;
using TheLeftExit.Memory.Sources;

using TheLeftExit.Growtopia.ObjectModel;

namespace Sandbox {
    class Program {
        static void Main(string[] args) {
            //new GrowtopiaGame();
            Int64? testOffset = ObjectModelHelper.GetOffset(typeof(GrowtopiaGame), "App");

            Process gt = Process.GetProcessesByName("Growtopia").First();

            ProcessMemory memory = new ProcessMemory((uint)gt.Id);

            GrowtopiaGame game = new GrowtopiaGame(memory, (ulong)gt.MainModule.BaseAddress);

            NetAvatar netAvatar = game.App.GameLogicComponent.NetAvatar;

            while (true) {
                Console.WriteLine(netAvatar.Position);
                Thread.Sleep(10);
            }
        }
    }
}