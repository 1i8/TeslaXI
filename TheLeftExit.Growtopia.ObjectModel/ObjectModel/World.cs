using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using TheLeftExit.Memory.Queries;
using TheLeftExit.Memory.Sources;

using static TheLeftExit.Memory.Queries.Conditions;

namespace TheLeftExit.Growtopia.ObjectModel {
    public struct World {
        public readonly ulong Address;
        public readonly MemorySource Source;

        public World(MemorySource memorySource, ulong targetAddress) {
            Address = targetAddress;
            Source = memorySource;
        }

        public WorldTileMap WorldTileMap => new WorldTileMap(Source, WorldTileMapQuery.GetResultByVal(Source, Address));
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static PointerQuery WorldTileMapQuery = new PointerQuery(RTTIByVal("WorldTileMap"), 0x300, 0x8);

        public WorldObjectMap WorldObjectMap => new WorldObjectMap(Source, WorldObjectMapQuery.GetResultByVal(Source, Address));
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static PointerQuery WorldObjectMapQuery = new PointerQuery(RTTIByVal("WorldObjectMap"), 0x300, 0x8);
    }
}