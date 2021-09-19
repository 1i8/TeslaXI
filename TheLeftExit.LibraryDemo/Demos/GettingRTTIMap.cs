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
        [LibraryDemo("Get RTTI class names for an address")]
        public static void GettingRTTINames() {
            Console.WriteLine("Retrieving Growtopia process and getting its information...");
            Process p = Process.GetProcessesByName("Growtopia").Single();

            uint processId = (uint)p.Id;
            ulong baseAddress = (ulong)p.MainModule.BaseAddress;

            Console.WriteLine("Creating a ProcessMemory instance...");
            ProcessMemory processMemory = new ProcessMemory(processId);

            Console.Write("Address: 0x");
            ulong address = ulong.Parse(Console.ReadLine(), System.Globalization.NumberStyles.HexNumber);

            Console.WriteLine("Retrieving class name info...");
            string[] names = processMemory.GetRTTIClassNames64(address);

            Console.WriteLine();
            foreach (string n in names ?? Array.Empty<string>())
                Console.WriteLine(n);
        }
    }
}
