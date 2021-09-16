using System.ComponentModel;
using TheLeftExit.Memory.Queries;
using TheLeftExit.Memory.Sources;

using static TheLeftExit.Memory.Queries.Conditions;
using static TheLeftExit.Growtopia.ObjectModel.ObjectModelHelper;

namespace TheLeftExit.Growtopia.ObjectModel {
    public struct GameLogicComponent {
        public readonly ulong Address;
        public readonly MemorySource Source;

        public GameLogicComponent(MemorySource memorySource, ulong targetAddress) {
            Address = targetAddress;
            Source = memorySource;
        }

        public World World => new World(Source, WorldQuery.GetResultByRef(Source, Address));
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static readonly PointerQuery WorldQuery = new PointerQuery(RTTIByRef("World"), 0x2000, 0x8);

        public WorldRenderer WorldRenderer => new WorldRenderer(Source, WorldRendererQuery.GetResultByRef(Source, Address));
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static readonly PointerQuery WorldRendererQuery = new PointerQuery(RTTIByRef("WorldRenderer"), 0x2000, 0x8);

        public NetAvatar NetAvatar => new NetAvatar(Source, NetAvatarQuery.GetResultByRef(Source, Address));
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static readonly PointerQuery NetAvatarQuery = new PointerQuery(RTTIByRef("NetAvatar"), 0x2000, 0x8);

        public GameList<ItemSlot> Inventory => new GameList<ItemSlot>(Source, InventoryQuery.GetResultByVal(Source, Address));
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static readonly PointerQuery InventoryQuery = new PointerQuery(IsInventory, 0x300, 0x8);
    }
}
