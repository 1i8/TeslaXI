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
        [LibraryDemo("Tool: RTTI mapper (from given address until end of memory region)")]
        public static void GettingRTTIMap() {
            Console.WriteLine("Retrieving Growtopia process and getting its information...");
            Process p = Process.GetProcessesByName("Growtopia").Single();

            uint processId = (uint)p.Id;
            ulong baseAddress = (ulong)p.MainModule.BaseAddress;
            uint size = (uint)p.MainModule.ModuleMemorySize;

            Console.WriteLine("Creating a ProcessMemory instance...");
            ProcessMemory processMemory = new ProcessMemory(processId);

            Console.WriteLine("Leave this blank to scan the main module. Add ! at the end if you're using a pointer.");
            Console.Write("Address: 0x");
            string input = Console.ReadLine();
            ulong address;
            if (input == "")
                address = baseAddress;
            else {
                address = ulong.Parse(input.Trim('!'), System.Globalization.NumberStyles.HexNumber);
                if (input.EndsWith('!'))
                    address = processMemory.Read<ulong>(address).Value;
            }

            Console.WriteLine("Scanning...");
            Console.WriteLine();
            for(ulong a = address; true; a += 8) {
                if (!processMemory.TryRead(a, out ulong nestedAddress)) break;

                // Reading both by value and by reference
                string[] names = processMemory.GetRTTIClassNames64(a);
                string[] nestedNames = processMemory.GetRTTIClassNames64(nestedAddress);

                if(IsInteresting(names)) {
                    Console.Write($"{a:X} | {a - address:X} =>");
                    PrintNames(names);
                }

                if (IsInteresting(nestedNames)) {
                    Console.Write($"{a:X} | {a - address:X} ->");
                    PrintNames(nestedNames);
                }
            }
        }

        private static bool IsInteresting(string[] names) =>
            names != null && names.Length > 0 && !names.Contains("type_info") && !names.All(x => x.Contains("::"));

        private static void PrintNames(string[] names) {
            foreach (string n in names)
                Console.Write($" {n}");
            Console.WriteLine();
        }
    }
}
