using System;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;


using TheLeftExit.Growtopia.ObjectModel;
using TheLeftExit.TeslaX;
using System.Collections.Generic;
using System.Drawing;

using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;

using TheLeftExit.Memory.Queries;
using TheLeftExit.Memory.RTTI;
using TheLeftExit.Memory.Sources;

namespace Sandbox {
    class Program {
        static void Main(string[] args) {
            Process gt = Process.GetProcessesByName("Growtopia").First();

            ProcessMemory memory = new ProcessMemory((uint)gt.Id);

            PointerQuery appQuery = new PointerQuery {
                Condition = RTTI("App"),
                Direction = PointerQueryScanDirection.Forward,
                Kind = PointerQueryScanType.ScamByRefReturnRef,
                Range = (UInt32)gt.MainModule.ModuleMemorySize,
                Step = 0x04
            };

            Stopwatch sw = Stopwatch.StartNew();
            PointerQueryResult pqr = appQuery.Run(memory, (UInt64)gt.MainModule.BaseAddress);
            sw.Stop();

            Console.WriteLine(pqr.Offset.ToString("X"));
            Console.WriteLine(sw.Elapsed);
            Console.ReadKey();
        }

        public static PointerQueryCondition RTTI(string name) => (MemorySource memorySource, UInt64 addr) =>
            memorySource.GetRTTIClassNames64(addr)?.Contains(name) ?? false;

        static unsafe T Cast<T>(Span<byte> source) where T : unmanaged {
            fixed (byte* bytePtr = &source.GetPinnableReference())
                return *(T*)bytePtr;
        }
    }
}