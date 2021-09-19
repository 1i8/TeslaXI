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
        [LibraryDemo("Get RTTI class names for all structures in the main module")]
        public static void GettingRTTIMap() {
            Console.WriteLine("Retrieving Growtopia process and getting its information...");
            Process p = Process.GetProcessesByName("Growtopia").Single();

            uint processId = (uint)p.Id;
            ulong baseAddress = (ulong)p.MainModule.BaseAddress;
            uint size = (uint)p.MainModule.ModuleMemorySize;

            Console.WriteLine("Creating a ProcessMemory instance...");
            ProcessMemory processMemory = new ProcessMemory(processId);

            Console.WriteLine("Scanning...");
            Console.WriteLine();
            for(ulong address = baseAddress; address < baseAddress + size; address += 8) {
                string[] names = processMemory.GetRTTIClassNames64(address);
                if(names != null && !names.Contains("type_info") && !names.Any(x => x.Contains("::"))) {
                    Console.Write((address - baseAddress).ToString("X"));
                    foreach (string s in names)
                        Console.Write(" " + s);
                    Console.WriteLine();
                }
            }
        }
    }
}
