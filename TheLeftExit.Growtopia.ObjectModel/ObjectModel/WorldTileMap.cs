using System.ComponentModel;
using TheLeftExit.Memory.Queries;
using TheLeftExit.Memory.Sources;

using static TheLeftExit.Growtopia.ObjectModel.ObjectModelHelper;

namespace TheLeftExit.Growtopia.ObjectModel {
    public struct WorldTileMap {
        public readonly ulong Address;
        public readonly MemorySource Source;

        public WorldTileMap(MemorySource memorySource, ulong targetAddress) {
            Address = targetAddress;
            Source = memorySource;
        }

        public (uint Width, uint Height) Size => Source.ForceRead<(uint, uint)>(Address + 0x08);

        [EditorBrowsable(EditorBrowsableState.Never)]
        public ulong MapBase => Source.ForceRead<ulong>(Address + 0x18);

        public WorldTile this[uint x, uint y] => Source.ForceRead<WorldTile>(MapBase + BlockOffset * (y * Size.Width + x));

        private const byte BlockOffset = 0x90;
    }
}
