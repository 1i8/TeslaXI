using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace TheLeftExit.Growtopia.ObjectModel {
    [StructLayout(LayoutKind.Explicit)]
    public struct ItemSlot {
        [FieldOffset(0)]
        public ushort ID;
        [FieldOffset(2)]
        public ushort Count;
    }

    [StructLayout(LayoutKind.Explicit)]
    public struct WorldObject {
        [FieldOffset(8)]
        public (float X, float Y) Position;
        [FieldOffset(16)]
        public ItemSlot Item;
    }

    [StructLayout(LayoutKind.Explicit)]
    public struct WorldTile {
        [FieldOffset(0)]
        private int none;
        [FieldOffset(4)]
        public ushort Foreground;
        [FieldOffset(6)]
        public ushort Background;

        public bool IsEmpty => (Foreground | Background) == 0;
    }
}
