using System;

using TheLeftExit.Memory.Queries;
using TheLeftExit.Memory.Sources;

using static TheLeftExit.Growtopia.ObjectModel.ObjectModelHelper;

namespace TheLeftExit.Growtopia.ObjectModel {
    public struct App {
        private ulong address;
        private MemorySource source;

        public App(MemorySource memorySource, ulong targetAddress) {
            address = targetAddress;
            source = memorySource;
        }

        private static PointerQuery GameLogicComponentQuery = new PointerQuery(RTTIByRef("GameLogicComponent"), 0x2000, 0x8);
        public GameLogicComponent GameLogicComponent => new GameLogicComponent(source, GameLogicComponentQuery.GetNestedClassAddress(source, address));
    }

    public struct GameLogicComponent {
        private ulong address;
        private MemorySource source;

        public GameLogicComponent(MemorySource memorySource, ulong targetAddress) {
            address = targetAddress;
            source = memorySource;
        }
    }
}
