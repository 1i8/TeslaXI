#pragma warning disable CA1416

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.System.Memory;

namespace TheLeftExit.Memory.Sources {
    public class CachedMemory : ProcessMemory {
        public CachedMemory(uint processId) : base(processId) {
            regions = new List<MemoryRegion>();
        }

        public unsafe override bool ReadBytes(ulong address, nuint count, Span<byte> buffer) {
            if (address > 1ul << 48)
                return false;
            foreach(MemoryRegion r in regions) 
                switch(r.Disposition(address, count)) {
                    case AddressDisposition.Invalid:
                        return false;
                    case AddressDisposition.Cached:
                        r[address, count].CopyTo(buffer);
                        return true;
                }
            MemoryRegion newRegion = new MemoryRegion(handle, (void*)address);
            regions.Add(newRegion);
            if (newRegion.Disposition(address, count) == AddressDisposition.Cached) {
                newRegion[address, count].CopyTo(buffer);
                return true;
            } else
                return false;
        }

        protected List<MemoryRegion> regions;
        public void Flush() => regions.Clear();

        protected struct MemoryRegion {
            public ulong BaseAddress { get; set; }
            public ulong Size { get; set; }
            public ulong Tail => checked(BaseAddress + Size);
            public Memory<byte>? Memory { get; set; }

            public unsafe MemoryRegion(HANDLE handle, void* address) {
                MEMORY_BASIC_INFORMATION mbi;

                Kernel32.VirtualQueryEx(handle, address, &mbi, (nuint)sizeof(MEMORY_BASIC_INFORMATION));
                // If this throws, we're fucked anyway, might as well not handle it.
                BaseAddress = (ulong)mbi.BaseAddress;
                Size = (ulong)mbi.RegionSize;
                if ((mbi.State & VIRTUAL_ALLOCATION_TYPE.MEM_COMMIT) == 0) {
                    Memory = null;
                    return;
                }

                Memory = new byte[Size];
                bool readResult;
                fixed (void* ptr = Memory.Value.Span)
                    readResult = Kernel32.ReadProcessMemory(handle, mbi.BaseAddress, ptr, mbi.RegionSize);

                if (!readResult)
                    Memory = null;
            }

            public AddressDisposition Disposition(ulong address, ulong size) {
                if (size > ulong.MaxValue - address) // Range overflows
                    return AddressDisposition.Invalid;
                ulong tail = address + size;
                if (address < BaseAddress && tail >= BaseAddress || address <= Tail && tail > Tail) // Range contains/intersects region
                    return AddressDisposition.Invalid;
                if (tail < BaseAddress || address > Tail) // Range is outside of region
                    return AddressDisposition.Unknown;
                return Memory.HasValue ? AddressDisposition.Cached : AddressDisposition.Invalid;
            }
            public Span<byte> this[ulong address, ulong size] => Memory.Value.Slice((int)(address - BaseAddress), (int)size).Span;
        }

        protected enum AddressDisposition {
            Unknown,
            Cached,
            Invalid
        }
    }
}
