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

            PointerQuery appQuery = new PointerQuery(RTTI("App"), 0x1200000, 0x08);
            PointerQuery glcQuery = new PointerQuery(RTTI("GameLogicComponent"), 0x2000, 0x08);

            PointerQueryResult? app = appQuery.Run(memory, (UInt64)gt.MainModule.BaseAddress); // Scanning at Growtopia.exe
            if (!app.HasValue) throw null;
            if (!memory.TryRead<UInt64>(app.Value.Target, out UInt64 appAddress)) throw null; // Getting value address of App at the heap from its reference at Growtopia.exe

            PointerQueryResult? glc = glcQuery.Run(memory, appAddress); // Scanning at App
            if (!glc.HasValue) throw null;

            Console.WriteLine(app.Value.Offset.ToString("X"));
            Console.WriteLine(glc.Value.Offset.ToString("X"));
            Console.ReadKey();
        }

        public static PointerQueryCondition RTTI(string name) => (MemorySource memorySource, UInt64 addr) => {
            if (!memorySource.TryRead<UInt64>(addr, out UInt64 target)) return PointerQueryConditionResult.Break;
            if (memorySource.GetRTTIClassNames64(target)?.Contains(name) ?? false) return PointerQueryConditionResult.Return;
            return PointerQueryConditionResult.Continue;
        };
    }
}