#pragma warning disable CA1416

using System;
using System.Collections.Generic;
using System.Linq;

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.System.Threading;

namespace TheLeftExit.Memory.Sources {
    public class ProcessMemory : MemorySource, IDisposable {
        protected HANDLE handle;

        // TODO: add checks on initialization, implement IsOpen property

        public ProcessMemory(uint processId) {
            handle = OpenProcess(processId);
        }

        public override unsafe bool ReadBytes(ulong address, nuint count, Span<byte> buffer) {
            fixed(byte* p = buffer)
                return Kernel32.ReadProcessMemory(handle, (void*)address, p, count);
        }

        protected HANDLE OpenProcess(uint id) =>
            Kernel32.OpenProcess(PROCESS_ACCESS_RIGHTS.PROCESS_ALL_ACCESS, false, id);

        protected BOOL CloseProcess(HANDLE pHandle) =>
            Kernel32.CloseHandle(pHandle);

        public void Dispose() {
            CloseProcess(handle);
        }
    }
}
