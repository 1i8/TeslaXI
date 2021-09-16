using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using TheLeftExit.Memory.Queries;
using TheLeftExit.Memory.Sources;

using static TheLeftExit.Memory.Queries.Conditions;

namespace TheLeftExit.Growtopia.ObjectModel {
    public struct WorldCamera {
        public readonly ulong Address;
        public readonly MemorySource Source;

        public WorldCamera(MemorySource memorySource, ulong targetAddress) {
            Address = targetAddress;
            Source = memorySource;
        }

        public (float X, float Y) CameraPosition => Source.ForceRead<(float, float)>(Address + 0x10);
        public float ZoomFactor => Source.ForceRead<float>(Address + 0x2C);
        public (float Width, float Height) ScreenSize => Source.ForceRead<(float, float)>(Address + 0x38);
    }
}
