using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TheLeftExit.Memory.Sources;
using TheLeftExit.Memory.Queries;
using TheLeftExit.Memory.RTTI;
using TheLeftExit.Growtopia.ObjectModel;

namespace TheLeftExit.LibraryDemo {
    partial class Program {
        [LibraryDemo("Get the App/GLC/NetAvatar addresses and offsets (TheLeftExit.Growtopia.ObjectModel)")]
        public static void GettingClassInfo() {
            Console.WriteLine("Retrieving Growtopia process and getting its information...");
            Process p = Process.GetProcessesByName("Growtopia").Single();

            uint processId = (uint)p.Id;
            ulong baseAddress = (ulong)p.MainModule.BaseAddress;

            Console.WriteLine("Creating a ProcessMemory instance...");
            ProcessMemory processMemory = new ProcessMemory(processId);

            Console.WriteLine("Retrieving class info...");
            GrowtopiaGame game = new GrowtopiaGame(processMemory, baseAddress);
            App app = game.App;
            GameLogicComponent glc = app.GameLogicComponent;
            NetAvatar netAvatar = glc.NetAvatar;

            Console.WriteLine();
            Console.WriteLine($"App: {app.Address:X} | +{GrowtopiaGame.AppQuery.Offset:X}");
            Console.WriteLine($"GLC: {glc.Address:X} | +{App.GameLogicComponentQuery.Offset:X}");
            Console.WriteLine($"NetAvatar: {netAvatar.Address:X} | +{GameLogicComponent.NetAvatarQuery.Offset:X}");
        }
    }
}
