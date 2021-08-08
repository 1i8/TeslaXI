using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace TheLeftExit.Memory
{
    public class ProcessMemoryException : Exception
    {
        public ProcessMemoryException() { }

        public ProcessMemoryException(String message) : base(message) { }

        public ProcessMemoryException(Int64 address) : base($"Could not read at {address:X}.") { }
    }

    public enum ProcessAccessFlags : int
    {
        All = 0x001F0FFF,
        Terminate = 0x00000001,
        CreateThread = 0x00000002,
        VMOperation = 0x00000008,
        VMRead = 0x00000010,
        VMWrite = 0x00000020,
        DupHandle = 0x00000040,
        SetInformation = 0x00000200,
        QueryInformation = 0x00000400,
        Synchronize = 0x00100000
    }

    public static class MemoryReader
    {
        #region Win32 API
        [DllImport("kernel32.dll")]
        private static extern IntPtr OpenProcess(Int32 dwDesiredAccess, bool bInheritHandle, Int32 dwProcessId);

        [DllImport("kernel32.dll")]
        private static extern bool CloseHandle(IntPtr hObject);

        [DllImport("kernel32.dll")]
        private static extern bool GetHandleInformation(IntPtr hObject, out Int32 lpdwFlags);

        [DllImport("kernel32.dll")]
        private static extern bool ReadProcessMemory(Int32 hProcess, Int64 lpBaseAddress, Byte[] lpBuffer, Int32 dwSize, Int32 lpNumberOfBytesRead);
        #endregion

        public static bool IsValidHandle(this IntPtr handle) => GetHandleInformation(handle, out _);
        public static IntPtr OpenProcess(Int32 id) => OpenProcess((int)ProcessAccessFlags.All, false, id);
        public static void CloseProcess(this IntPtr handle) => CloseHandle(handle);

        // Most basic & most efficient memory reader
        // For best performance, user code should call this and work with byte[] directly
        public static bool ReadBytes(this IntPtr handle, Int64 address, Int32 bytecount, out Byte[] result)
        {
            result = new Byte[bytecount];
            return ReadProcessMemory((int)handle, address, result, bytecount, 0);
        }

        #region Error-code based reading functions
        public static bool ReadByte(this IntPtr handle, Int64 address, out Byte res)
        {
            Byte[] buffer;
            bool ret = handle.ReadBytes(address, sizeof(Byte), out buffer);
            res = buffer[0];
            return ret;
        }
        public static bool ReadSByte(this IntPtr handle, Int64 address, out SByte res)
        {
            Byte[] buffer;
            bool ret = handle.ReadBytes(address, sizeof(SByte), out buffer);
            res = (sbyte)buffer[0];
            return ret;
        }
        public static bool ReadInt16(this IntPtr handle, Int64 address, out Int16 res)
        {
            Byte[] buffer;
            bool ret = handle.ReadBytes(address, sizeof(Int16), out buffer);
            res = BitConverter.ToInt16(buffer);
            return ret;
        }
        public static bool ReadUInt16(this IntPtr handle, Int64 address, out UInt16 res)
        {
            Byte[] buffer;
            bool ret = handle.ReadBytes(address, sizeof(UInt16), out buffer);
            res = BitConverter.ToUInt16(buffer);
            return ret;
        }
        public static bool ReadInt32(this IntPtr handle, Int64 address, out Int32 res)
        {
            Byte[] buffer;
            bool ret = handle.ReadBytes(address, sizeof(Int32), out buffer);
            res = BitConverter.ToInt32(buffer);
            return ret;
        }
        public static bool ReadUInt32(this IntPtr handle, Int64 address, out UInt32 res)
        {
            Byte[] buffer;
            bool ret = handle.ReadBytes(address, sizeof(UInt32), out buffer);
            res = BitConverter.ToUInt32(buffer);
            return ret;
        }
        public static bool ReadInt64(this IntPtr handle, Int64 address, out Int64 res)
        {
            Byte[] buffer;
            bool ret = handle.ReadBytes(address, sizeof(Int64), out buffer);
            res = BitConverter.ToInt64(buffer);
            return ret;
        }
        public static bool ReadUInt64(this IntPtr handle, Int64 address, out UInt64 res)
        {
            Byte[] buffer;
            bool ret = handle.ReadBytes(address, sizeof(UInt64), out buffer);
            res = BitConverter.ToUInt64(buffer);
            return ret;
        }
        public static bool ReadBoolean(this IntPtr handle, Int64 address, out bool res)
        {
            Byte[] buffer;
            bool ret = handle.ReadBytes(address, sizeof(bool), out buffer);
            res = BitConverter.ToBoolean(buffer);
            return ret;
        }
        public static bool ReadSingle(this IntPtr handle, Int64 address, out Single res)
        {
            Byte[] buffer;
            bool ret = handle.ReadBytes(address, sizeof(Single), out buffer);
            res = BitConverter.ToSingle(buffer);
            return ret;
        }
        public static bool ReadDouble(this IntPtr handle, Int64 address, out Double res)
        {
            Byte[] buffer;
            bool ret = handle.ReadBytes(address, sizeof(Double), out buffer);
            res = BitConverter.ToDouble(buffer);
            return ret;
        }
        public static bool ReadString(this IntPtr handle, Int64 address, Int32 length, Encoding encoding, out String res)
        {
            Byte[] buffer;
            bool ret = handle.ReadBytes(address, length, out buffer);
            res = encoding.GetString(buffer);
            return ret;
        }
        #endregion

        #region Exception based reading functions
        public static Byte ReadByte(this IntPtr handle, Int64 address, bool throwOnFail = false)
        {
            if (!handle.ReadByte(address, out Byte res) && throwOnFail)
                throw new ProcessMemoryException(address);
            return res;
        }
        public static SByte ReadSByte(this IntPtr handle, Int64 address, bool throwOnFail = false)
        {
            if (!handle.ReadSByte(address, out SByte res) && throwOnFail)
                throw new ProcessMemoryException(address);
            return res;
        }
        public static Int16 ReadInt16(this IntPtr handle, Int64 address, bool throwOnFail = false)
        {
            if (!handle.ReadInt16(address, out Int16 res) && throwOnFail)
                throw new ProcessMemoryException(address);
            return res;
        }
        public static UInt16 ReadUInt16(this IntPtr handle, Int64 address, bool throwOnFail = false)
        {
            if (!handle.ReadUInt16(address, out UInt16 res) && throwOnFail)
                throw new ProcessMemoryException(address);
            return res;
        }
        public static Int32 ReadInt32(this IntPtr handle, Int64 address, bool throwOnFail = false)
        {
            if (!handle.ReadInt32(address, out Int32 res) && throwOnFail)
                throw new ProcessMemoryException(address);
            return res;
        }
        public static UInt32 ReadUInt32(this IntPtr handle, Int64 address, bool throwOnFail = false)
        {
            if (!handle.ReadUInt32(address, out UInt32 res) && throwOnFail)
                throw new ProcessMemoryException(address);
            return res;
        }
        public static Int64 ReadInt64(this IntPtr handle, Int64 address, bool throwOnFail = false)
        {
            if (!handle.ReadInt64(address, out Int64 res) && throwOnFail)
                throw new ProcessMemoryException(address);
            return res;
        }
        public static UInt64 ReadUInt64(this IntPtr handle, Int64 address, bool throwOnFail = false)
        {
            if (!handle.ReadUInt64(address, out UInt64 res) && throwOnFail)
                throw new ProcessMemoryException(address);
            return res;
        }
        public static bool ReadBoolean(this IntPtr handle, Int64 address, bool throwOnFail = false)
        {
            if (!handle.ReadBoolean(address, out bool res) && throwOnFail)
                throw new ProcessMemoryException(address);
            return res;
        }
        public static Single ReadSingle(this IntPtr handle, Int64 address, bool throwOnFail = false)
        {
            if (!handle.ReadSingle(address, out Single res) && throwOnFail)
                throw new ProcessMemoryException(address);
            return res;
        }
        public static Double ReadDouble(this IntPtr handle, Int64 address, bool throwOnFail = false)
        {
            if (!handle.ReadDouble(address, out Double res) && throwOnFail)
                throw new ProcessMemoryException(address);
            return res;
        }
        public static String ReadString(this IntPtr handle, Int64 address, Int32 length, Encoding encoding, bool throwOnFail = false)
        {
            if (!handle.ReadString(address, length, encoding, out String res))
                throw new ProcessMemoryException(address);
            return res;
        }
        #endregion

        public static Int64 ReadOffsets(this IntPtr handle, Int64 baseAddress, params Int32[] offsets)
        {
            Int64 buffer = handle.ReadInt64(baseAddress);
            foreach(Int32 o in offsets)
            {
                buffer = handle.ReadInt64(buffer + o);
            }
            return buffer;
        }
    }
}
