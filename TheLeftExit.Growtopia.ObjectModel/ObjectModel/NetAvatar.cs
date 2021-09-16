using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using TheLeftExit.Memory.Queries;
using TheLeftExit.Memory.Sources;

using static TheLeftExit.Memory.Queries.Conditions;

namespace TheLeftExit.Growtopia.ObjectModel {
    public struct NetAvatar {
        public readonly ulong Address;
        public readonly MemorySource Source;

        public NetAvatar(MemorySource memorySource, ulong targetAddress) {
            Address = targetAddress;
            Source = memorySource;
        }

        public (float X, float Y) Position => Source.ForceRead<(float, float)>(Address + 0x08);
        public bool FacingLeft => Source.ForceRead<bool>(Address + 0x61);
    }
}
