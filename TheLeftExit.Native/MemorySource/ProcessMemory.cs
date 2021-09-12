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

namespace TheLeftExit.Memory {
    public class ProcessMemory : MemorySource, IDisposable {
        private HANDLE handle;

        // TODO: add checks on initialization, implement IsOpen property

        public ProcessMemory(uint processId) {
            handle = OpenProcess(processId);
        }

        public override unsafe bool ReadBytes(ulong address, nuint count, Span<byte> buffer) {
            fixed(byte* p = &buffer.GetPinnableReference())
                return Kernel32.ReadProcessMemory(handle, (void*)address, p, count);
        }

        private HANDLE OpenProcess(uint id) =>
            Kernel32.OpenProcess(PROCESS_ACCESS_RIGHTS.PROCESS_VM_READ, false, id);

        private BOOL CloseProcess(HANDLE pHandle) =>
            Kernel32.CloseHandle(pHandle);

        public void Dispose() {
            CloseProcess(handle);
        }
    }
}
