using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection.Metadata;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace TheLeftExit.Memory.RTTI
{
    public static class RTTIMethods
    {
        [DllImport("dbghelp.dll", CharSet = CharSet.Unicode)]
        private static extern Int32 UnDecorateSymbolName(String DecoratedName, StringBuilder UnDecoratedName, Int32 UndecoratedLength, Int32 Flags);

        private static String undecorateString(String source)
        {
            source = source.Split('\0')[0];
            if (source.Contains("@"))
            {
                var sb = new StringBuilder(255);
                UnDecorateSymbolName("?" + source, sb, sb.Capacity, 0x1000);
                source = sb.ToString();
            }
            return source;
        }

        private struct BaseClassInfo
        {
            public Int64 ArrayPointer;
            public Int32 Count;
            public Int64 BaseAddress64;
        }
        
        /// <summary>
        /// Retrieves base class names for structure at <paramref name="address"/> (or null if no such information exists).
        /// </summary>
        public static String[] GetRTTIClassNames64(this IntPtr handle, Int64 address)
        {
            BaseClassInfo info = handle.getBaseClassInfo64(address);
            if (info.Count == 0)
                return null;
            String[] res = new String[info.Count];
            for (Int32 i = 0; i < info.Count; i++)
            {
                res[i] = handle.getBaseClassName64(info.ArrayPointer + 4 * i, info.BaseAddress64);
                res[i] = undecorateString(res[i]);
            }
            return res;
        }

        /// <summary>
        /// Retrieves first found class name for structure at <paramref name="address"/> (or null if no such information exists).
        /// </summary>
        public static String GetRTTIClassName64(this IntPtr handle, Int64 address)
        {
            BaseClassInfo info = handle.getBaseClassInfo64(address);
            if (info.Count == 0)
                return null;
            String res = handle.getBaseClassName64(info.ArrayPointer, info.BaseAddress64);
            res = undecorateString(res);
            return res;
        }

        #region RTTI scraping for 64-bit processes
        private static BaseClassInfo getBaseClassInfo64(this IntPtr handle, Int64 address)
        {
            if (!handle.ReadInt64(address, out Int64 struct_addr)) return new BaseClassInfo();
            if (!handle.ReadInt64(struct_addr - 8, out Int64 object_locator_ptr)) return new BaseClassInfo();
            if (!handle.ReadInt64(object_locator_ptr + 0x14, out Int64 base_offset)) return new BaseClassInfo();
            Int64 base_address = object_locator_ptr - base_offset;
            if (!handle.ReadInt32(object_locator_ptr + 0x10, out Int32 class_hierarchy_descriptor_offset)) return new BaseClassInfo();
            Int64 class_hierarchy_descriptor_ptr = base_address + class_hierarchy_descriptor_offset;
            if (!handle.ReadInt32(class_hierarchy_descriptor_ptr + 0x08, out Int32 base_class_count)) return new BaseClassInfo();
            if (base_class_count == 0 || base_class_count > 24) return new BaseClassInfo();
            if (!handle.ReadInt32(class_hierarchy_descriptor_ptr + 0x0C, out Int32 base_class_array_offset)) return new BaseClassInfo();
            Int64 base_class_array_ptr = base_address + base_class_array_offset;
            return new BaseClassInfo { ArrayPointer = base_class_array_ptr, Count = base_class_count, BaseAddress64 = base_address };
        }

        private static String getBaseClassName64(this IntPtr handle, Int64 baseClassPointer, Int64 baseAddress)
        {
            String res;
            if (!handle.ReadInt32(baseClassPointer, out Int32 base_class_descriptor_offset)) return null;
            Int64 base_class_descriptor_ptr = baseAddress + base_class_descriptor_offset;
            if (!handle.ReadInt32(base_class_descriptor_ptr, out Int32 type_descriptor_offset)) return null;
            Int64 type_descriptor_ptr = baseAddress + type_descriptor_offset;
            if (!handle.ReadString(type_descriptor_ptr + 0x14, 60, Encoding.UTF8, out res)) return null;
            return res;
        }
        #endregion

        /// <summary>
        /// Retrieves base class names for structure at <paramref name="address"/> (or null if no such information exists).
        /// </summary>
        public static String[] GetRTTIClassNames32(this IntPtr handle, Int64 address)
        {
            BaseClassInfo info = handle.getBaseClassInfo32(address);
            if (info.Count == 0)
                return null;
            String[] res = new String[info.Count];
            for (Int32 i = 0; i < info.Count; i++)
            {
                res[i] = handle.getBaseClassName32(info.ArrayPointer + 4 * i);
                res[i] = undecorateString(res[i]);
            }
            return res;
        }

        /// <summary>
        /// Retrieves first found class name for structure at <paramref name="address"/> (or null if no such information exists).
        /// </summary>
        public static String GetRTTIClassName32(this IntPtr handle, Int64 address)
        {
            BaseClassInfo info = handle.getBaseClassInfo32(address);
            if (info.Count == 0)
                return null;
            String res = handle.getBaseClassName32(info.ArrayPointer);
            res = undecorateString(res);
            return res;
        }

        #region RTTI scraping for 32-bit processes
        private static BaseClassInfo getBaseClassInfo32(this IntPtr handle, Int64 address)
        {
            if (!handle.ReadInt32(address, out Int32 struct_addr)) return new BaseClassInfo();
            if (!handle.ReadInt32(struct_addr - 4, out Int32 object_locator_ptr)) return new BaseClassInfo();
            if (!handle.ReadInt32(object_locator_ptr + 0x10, out Int32 class_hierarchy_descriptor_ptr)) return new BaseClassInfo();
            if (!handle.ReadInt32(class_hierarchy_descriptor_ptr + 0x08, out Int32 base_class_count)) return new BaseClassInfo();
            if (base_class_count == 0 || base_class_count > 24) return new BaseClassInfo();
            if (!handle.ReadInt32(class_hierarchy_descriptor_ptr + 0x0C, out Int32 base_class_array_ptr)) return new BaseClassInfo();
            return new BaseClassInfo { ArrayPointer = base_class_array_ptr, Count = base_class_count };
        }

        private static String getBaseClassName32(this IntPtr handle, Int64 baseClassPointer)
        {
            String res;
            if (!handle.ReadInt32(baseClassPointer, out Int32 base_class_descriptor_ptr)) return null;
            if (!handle.ReadInt32(base_class_descriptor_ptr, out Int32 type_descriptor_ptr)) return null;
            if (!handle.ReadString(type_descriptor_ptr + 0x0C, 60, Encoding.UTF8, out res)) return null;
            return res;
        }
        #endregion

        public static Dictionary<Int64, String[]> ScanClasses(this IntPtr handle, long baseAddress, int maxOffset, byte step = 0x08)
        {
            Dictionary<Int64, String[]> res = new();
            for(long i = baseAddress; i < baseAddress + maxOffset; i += step)
            {
                if (!handle.ReadInt64(i, out long addr))
                    continue;
                string[] classes = handle.GetRTTIClassNames64(addr);
                if (classes != null)
                    res.Add(i - baseAddress, classes);
            }
            return res;
        }
    }
}
