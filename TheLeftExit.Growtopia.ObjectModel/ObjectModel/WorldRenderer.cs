using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using TheLeftExit.Memory.Queries;
using TheLeftExit.Memory.Sources;

using static TheLeftExit.Memory.Queries.Conditions;

namespace TheLeftExit.Growtopia.ObjectModel {
    public struct WorldRenderer {
        public readonly ulong Address;
        public readonly MemorySource Source;

        public WorldRenderer(MemorySource memorySource, ulong targetAddress) {
            Address = targetAddress;
            Source = memorySource;
        }

        public WorldCamera WorldCamera => new WorldCamera(Source, WorldCameraQuery.GetResultByVal(Source, Address));
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static readonly PointerQuery WorldCameraQuery = new PointerQuery(RTTIByVal("WorldCamera"), 0x2000, 0x8);
    }
}
