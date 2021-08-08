using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheLeftExit.Growtopia.Classes;
using TheLeftExit.Memory.Queries;
using TheLeftExit.Memory;

using static TheLeftExit.Growtopia.GameConditions;

namespace TheLeftExit.Growtopia.Classes
{
    public class Growtopia : GameClass // It's not exactly one, but the inherited class has some nice features
    {
        private static bool queried = false;
        public Growtopia(Int32 processId, Int32 appOffset = -1)
        {
            if (appOffset != -1)
                AppQuery.Default = appOffset;

            Address = (Int64)Process.GetProcessById(processId).MainModule.BaseAddress;
            Handle = MemoryReader.OpenProcess(processId);

            if (!queried)
            {
                FindMemberOffsets();
                queried = true;
            }
        }

        [QueriedNestedClass]
        public App App => Offset<App>(AppOffset, ref AppInitialized);
        private static PointerQuery AppQuery = new PointerQuery { Condition = RTTI("App"), Range = 0x1200000 };
        private static Int32 AppOffset;
        private static bool AppInitialized;
    }
}
