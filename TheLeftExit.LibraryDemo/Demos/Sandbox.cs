#pragma warning disable CA1416

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

using Windows.Win32.Foundation;
using Windows.Win32.System.Threading;
using Windows.Win32.System.Memory;
using Windows.Win32;

using static TheLeftExit.Memory.Queries.Conditions;

namespace TheLeftExit.LibraryDemo {
    partial class Program {
        [LibraryDemo("Sandbox")]
        public unsafe static void Sandbox() {
            Process p = Process.GetProcessesByName("Growtopia").Single();

            uint processId = (uint)p.Id;
            ulong baseAddress = (ulong)p.MainModule.BaseAddress;

            CachedMemory source = new CachedMemory(processId);

            GrowtopiaGame game = new GrowtopiaGame(source, baseAddress);

            var sw = Stopwatch.StartNew();

            PointerQuery query = new PointerQuery(RTTIByRef("App").Forgive(), 0x2000000, 0x08);
            PointerQueryResult? res = query.GetResult(source, baseAddress);

            sw.Stop();
            Console.WriteLine(sw.ElapsedMilliseconds);

            Console.ReadKey();
        }
    }
}
