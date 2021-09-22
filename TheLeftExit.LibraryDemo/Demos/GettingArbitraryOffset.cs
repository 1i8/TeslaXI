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
        [LibraryDemo("Demo: Get an offset for a value located in GameLogicComponent")]
        public static void GettingArbitraryOffset() {
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

            Console.Write("Enter an integer value to search: ");
            int value = int.Parse(Console.ReadLine());

            PointerQueryCondition SeekValue = (source, address) => {
                if (source.Read<int>(address) == value)
                    return PointerQueryConditionResult.Return;
                return PointerQueryConditionResult.Continue;
            };

            PointerQuery query = new PointerQuery(SeekValue, 0x800, 0x8);

            var result = query.GetResult(processMemory, glc.Address);

            if (result.HasValue) {
                Console.WriteLine($"Found offset: 0x{result.Value.Offset:X}");
            } else
                Console.WriteLine("Nothing found.");
        }
    }
}
