using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TheLeftExit.Growtopia.Native;

namespace TheLeftExit.Growtopia.ObjectModel
{
    // C# does not allow inheriting/interfacing constructors or static fields.
    // I meant to constain GameList<T> with T being a class with .ctor(IntPtr,Int64).
    // Instead each GameList<T> contains this delegate linking to a constructor of T.
    public delegate T GameStructConstructor<T>(IntPtr handle, Int64 address);

    public readonly struct ItemSlot
    {
        public readonly Int16 ItemID;
        public readonly Int16 ItemCount;

        public ItemSlot(IntPtr handle, Int64 address)
        {
            ItemID = handle.ReadInt16(address);
            ItemCount = handle.ReadInt16(address + 0x02);
        }
        public static ItemSlot Constructor(IntPtr handle, Int64 address) => new ItemSlot(handle, address);
    }

    public readonly struct WorldObject
    {
        public readonly PointF Position;
        public readonly ItemSlot Item;
        public WorldObject(IntPtr handle, Int64 address)
        {
            Position = new PointF(handle.ReadSingle(address + 0x08), handle.ReadSingle(address + 0x0C));
            Item = new ItemSlot(handle, address + 0x10);
        }
        public static WorldObject Constructor(IntPtr handle, Int64 address) => new WorldObject(handle, address);
    }

    public readonly struct WorldTile
    {
        public readonly Int16 Foreground;
        public readonly Int16 Background;
        public readonly Int32 TicksPassed;
        public WorldTile(IntPtr handle, Int64 address)
        {
            Foreground = handle.ReadInt16(address + 0x04);
            Background = handle.ReadInt16(address + 0x06);
            TicksPassed = handle.ReadInt32(handle.ReadInt64(address + 0x28) + 0x74);
        }
        public static WorldTile Constructor(IntPtr handle, Int64 address) => new WorldTile(handle, address);
    }
}
