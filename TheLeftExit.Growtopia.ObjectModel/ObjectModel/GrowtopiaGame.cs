using System.ComponentModel;
using TheLeftExit.Memory.Queries;
using TheLeftExit.Memory.Sources;

using static TheLeftExit.Memory.Queries.Conditions;

namespace TheLeftExit.Growtopia.ObjectModel {
    public struct GrowtopiaGame {
        public readonly ulong Address;
        public readonly MemorySource Source;

        public GrowtopiaGame(MemorySource memorySource, ulong targetAddress) {
            Address = targetAddress;
            Source = memorySource;
        }

        public App App => new App(Source, AppQuery.GetResultByRef(Source, Address));
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static readonly PointerQuery AppQuery = new PointerQuery(RTTIByRef("App"), uint.MaxValue, 0x08);
    }
}
