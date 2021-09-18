using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TheLeftExit.Memory.Sources;
using TheLeftExit.Memory.Queries;
using TheLeftExit.Memory.RTTI;

namespace TheLeftExit.LibraryDemo {
    partial class Program {
        [LibraryDemo]
        public static void GettingAppOffset() {
            Console.WriteLine("Retrieving Growtopia process and getting its information...");
            Process p = Process.GetProcessesByName("Growtopia").Single();

            uint processId = (uint)p.Id;
            ulong baseAddress = (ulong)p.MainModule.BaseAddress;
            // This line requires admin rights, otherwise it may throw an Access denied exception.

            Console.WriteLine("Creating a ProcessMemory instance...");
            ProcessMemory processMemory = new ProcessMemory(processId);

            PointerQuery appQuery = new PointerQuery(AppCondition, 0x1200000, 0x08);
            // An optional fourth parameter, [long? offset], allows you to specify the cached offset.

            Console.WriteLine("Running a query...");
            PointerQueryResult? appQueryResult = appQuery.GetResult(processMemory, baseAddress);
            // An optional third parameter, [PointerQueryOptions options], allows you to specify how the query will treat the cached offset.

            if (!appQueryResult.HasValue) {
                Console.WriteLine("Uh oh, nothing was found.");
                // If a query fails, it returns null. Under the normal conditions, this query should not fail.
            } else {
                PointerQueryResult appInfo = appQueryResult.Value;
                Console.WriteLine("App structure found.");
                Console.WriteLine($"Address: {appInfo.Target:X}");
                Console.WriteLine($"Offset: {appInfo.Offset:X}");
            }
        }

        private static PointerQueryConditionResult AppCondition(MemorySource source, ulong address) {
            if (!source.TryRead(address, out ulong targetAddress))
                return PointerQueryConditionResult.Break;
            if (source.GetRTTIClassNames64(targetAddress)?.Contains("App") ?? false)
                return PointerQueryConditionResult.Return;
            return PointerQueryConditionResult.Continue;
        }
    }
}
