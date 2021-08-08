using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheLeftExit.Growtopia.Native;

namespace TheLeftExit.Growtopia.ObjectModel
{
    public static class QueryConditions
    {
        public static bool IsInventory(IntPtr h, Int64 a)
        {
            Int64 header = h.ReadInt64(a);
            Int64 item1 = h.ReadInt64(header);
            Int64 item2 = h.ReadInt64(item1);
            Int32 itemInfo1 = h.ReadInt32(item1 + 0x10);
            Int32 itemInfo2 = h.ReadInt32(item2 + 0x10);
            return itemInfo1 == 0x00010012 && itemInfo2 == 0x00010020;
        }

        public static bool IsDoubleLinkedList(IntPtr h, Int64 a)
        {
            Int64 header = h.ReadInt64(a);
            Int64 item1 = h.ReadInt64(header);
            Int64 loopback = h.ReadInt64(item1 + 0x08);
            return loopback != 0 && header == loopback;
        }

        public static PointerQueryCondition RTTI(String name) =>
            (IntPtr h, Int64 a) => h.GetRTTIClassName64(a) == name;
    }
}
