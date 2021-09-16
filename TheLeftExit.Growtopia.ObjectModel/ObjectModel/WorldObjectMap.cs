using System.ComponentModel;
using TheLeftExit.Memory.Queries;
using TheLeftExit.Memory.Sources;

using static TheLeftExit.Growtopia.ObjectModel.ObjectModelHelper;

namespace TheLeftExit.Growtopia.ObjectModel {
    public struct WorldObjectMap {
        public readonly ulong Address;
        public readonly MemorySource Source;

        public WorldObjectMap(MemorySource memorySource, ulong targetAddress) {
            Address = targetAddress;
            Source = memorySource;
        }

        public GameList<WorldObject> DroppedItems => new GameList<WorldObject>(Source, DroppedItemsQuery.GetResultByVal(Source, Address));
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static readonly PointerQuery DroppedItemsQuery = new PointerQuery(IsDoubleLinkedList, 0x100, 0x8);
    }
}
