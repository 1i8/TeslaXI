using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

using TheLeftExit.Memory;

namespace TheLeftExit.Growtopia.Structures
{
    public static class StructureExtensionMethods
    {
        public static IEnumerable<Int64> EnumerateDoubleLinkedList(this IntPtr handle, Int64 headerAddress)
        {
            Int64 buffer = handle.ReadInt64(headerAddress);
            while(buffer != headerAddress)
            {
                yield return buffer + 0x10;
                buffer = handle.ReadInt64(buffer);
            }
        }
    }

    public struct ItemSlot
    {
        public Int16 Count;
        public Int16 ItemID;
    }

    public struct DroppedItem
    {
        public Single X;
        public Single Y;
        public ItemSlot Item;
    }

    public struct BlockData
    {
        public Int16 Background;
        public Int16 Foreground;
    }
}
