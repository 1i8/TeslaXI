using System.ComponentModel;
using TheLeftExit.Memory.Queries;
using TheLeftExit.Memory.Sources;

using static TheLeftExit.Memory.Queries.Conditions;

namespace TheLeftExit.Growtopia.ObjectModel {
    public struct App {
        public readonly ulong Address;
        public readonly MemorySource Source;

        public App(MemorySource memorySource, ulong targetAddress) {
            Address = targetAddress;
            Source = memorySource;
        }

        public GameLogicComponent GameLogicComponent => new GameLogicComponent(Source, GameLogicComponentQuery.GetResultByRef(Source, Address));
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static readonly PointerQuery GameLogicComponentQuery = new PointerQuery(RTTIByRef("GameLogicComponent"), 0x2000, 0x8);
    }
}
