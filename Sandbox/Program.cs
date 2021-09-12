using System;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;

using TheLeftExit.Growtopia;
using TheLeftExit.Growtopia.ObjectModel;
using TheLeftExit.Growtopia.Native;
using TheLeftExit.TeslaX;
using System.Collections.Generic;
using System.Drawing;

using static TheLeftExit.Growtopia.GameConditions;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;

using TheLeftExit.Memory;

namespace Sandbox {
    class Program {
        static void Main(string[] args) {
            Process gt = Process.GetProcessesByName("Growtopia").First();

            ProcessMemory memory = new ProcessMemory((uint)gt.Id);

            ulong appAddress = (ulong)new GrowtopiaGame(gt.Id).App.Address;

            string[] names = TheLeftExit.Memory.RTTIMethods.GetRTTIClassNames64(memory, appAddress);

            ;
        }

        static unsafe T Cast<T>(Span<byte> source) where T : unmanaged {
            fixed (byte* bytePtr = &source.GetPinnableReference())
                return *(T*)bytePtr;
        }
    }
}