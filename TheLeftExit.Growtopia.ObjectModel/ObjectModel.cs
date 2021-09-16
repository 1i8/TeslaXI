using System;
using System.Runtime.InteropServices;
using TheLeftExit.Memory.Queries;
using TheLeftExit.Memory.Sources;

using static TheLeftExit.Growtopia.ObjectModel.ObjectModelHelper;

namespace TheLeftExit.Growtopia.ObjectModel {
    /* Required structure
    
    public struct ClassName {
        private ulong address;
        private MemorySource source;
        
        public ClassName(MemorySource memorySource, ulong targetAddress) {
            address = targetAddress;
            source = memorySource;
        }
        
        // For each members:
        public ChildName ChildName => new ChildName(source, ...);
        private static PointerQuery childNameQuery = new PointerQuery(...));
    }

     */
    public struct GrowtopiaGame {
        private ulong address;
        private MemorySource source;

        public GrowtopiaGame(MemorySource memorySource, ulong targetAddress) {
            address = targetAddress;
            source = memorySource;
        }

        [PointerQuery("appQuery")]
        public App App => new App(source, appQuery.GetResultByRef(source, address));
        private static PointerQuery appQuery = new PointerQuery(RTTIByRef("App"), uint.MaxValue, 0x08);
    }

    public struct App {
        private ulong address;
        private MemorySource source;

        public App(MemorySource memorySource, ulong targetAddress) {
            address = targetAddress;
            source = memorySource;
        }

        [PointerQuery("gameLogicComponentQuery")]
        public GameLogicComponent GameLogicComponent => new GameLogicComponent(source, gameLogicComponentQuery.GetResultByRef(source, address));
        private static PointerQuery gameLogicComponentQuery = new PointerQuery(RTTIByRef("GameLogicComponent"), 0x2000, 0x8);
    }

    public struct GameLogicComponent {
        private ulong address;
        private MemorySource source;

        public GameLogicComponent(MemorySource memorySource, ulong targetAddress) {
            address = targetAddress;
            source = memorySource;
        }

        [PointerQuery("worldQuery")]
        public World World => new World(source, worldQuery.GetResultByRef(source, address));
        private static PointerQuery worldQuery = new PointerQuery(RTTIByRef("World"), 0x2000, 0x8);

        [PointerQuery("worldRendererQuery")]
        public WorldRenderer WorldRenderer => new WorldRenderer(source, worldRendererQuery.GetResultByRef(source, address));
        private static PointerQuery worldRendererQuery = new PointerQuery(RTTIByRef("WorldRenderer"), 0x2000, 0x8);

        [PointerQuery("netAvatarQuery")]
        public NetAvatar NetAvatar => new NetAvatar(source, netAvatarQuery.GetResultByRef(source, address));
        private static PointerQuery netAvatarQuery = new PointerQuery(RTTIByRef("NetAvatar"), 0x2000, 0x8);
    }

    public struct World {
        private ulong address;
        private MemorySource source;

        public World(MemorySource memorySource, ulong targetAddress) {
            address = targetAddress;
            source = memorySource;
        }
    }

    public struct WorldRenderer {
        private ulong address;
        private MemorySource source;

        public WorldRenderer(MemorySource memorySource, ulong targetAddress) {
            address = targetAddress;
            source = memorySource;
        }
    }

    public struct NetAvatar {
        private ulong address;
        private MemorySource source;

        public NetAvatar(MemorySource memorySource, ulong targetAddress) {
            address = targetAddress;
            source = memorySource;
        }

        public Point Position => source.ForceRead<Point>(address + 0x08);
    }

    [StructLayout(LayoutKind.Explicit)]
    public struct Point {
        [FieldOffset(0)]
        public float X;
        [FieldOffset(4)]
        public float Y;
        public override string ToString() => $"({X}, {Y})";
    }
}
