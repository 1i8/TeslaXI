using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace TheLeftExit.Memory.Sources
{
    public abstract class MemorySource {
        public abstract bool ReadBytes(ulong address, nuint count, Span<byte> buffer);

        public unsafe T? Read<T>(ulong address) where T : unmanaged {
            Span<byte> buffer = stackalloc byte[sizeof(T)];
            return ReadBytes(address, (nuint)sizeof(T), buffer) ? Unsafe.As<byte,T>(ref buffer.GetPinnableReference()) : null;
        }

        public unsafe bool TryRead<T>(ulong address, out T result) where T : unmanaged {
            Span<byte> buffer = stackalloc byte[sizeof(T)];
            if (ReadBytes(address, (nuint)sizeof(T), buffer)) {
                result = Unsafe.As<byte, T>(ref buffer.GetPinnableReference());
                return true;
            } else {
                result = default(T);
                return false;
            }
        }
    }
}
