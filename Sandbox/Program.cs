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

namespace Sandbox
{
    class Program
    {
        static void Main(string[] args)
        {

            var p = Process.GetProcessesByName("Growtopia").First();
            GrowtopiaGame g = new GrowtopiaGame(p.Id, 0xA04130);
            GameWindow w = new GameWindow(p.Id);

            IntPtr handle = p.Handle;
            Int64 baseAddress = (Int64)p.MainModule.BaseAddress;
            Int32 range = p.MainModule.ModuleMemorySize;

            byte?[] ACBsig = {
                0x0F, 0x29, null, null, null, 0xF3, 0x0F, null,
                null, null, null, null, null, 0xF3, 0x0F, null,
                null, 0xE8, null, null, null, null, 0x48, 0x8B,
                null, 0xE8, null, null, null, null, 0x8B, 0xC8 };

            PointerQuery ACBQuery = new PointerQuery
            {
                Condition = AOB(ACBsig),
                Range = range,
                Step = 0x1,
                Kind = ScanType.ScanByValue
            };

            PointerQuery FirstJneBeforeACBQuery = new PointerQuery
            {
                Condition = AOB(0x76, 0x08),
                Range = 0x1000,
                Step = -0x1,
                Kind = ScanType.ScanByValue
            };

            PointerQueryResult ACB = ACBQuery.Run(handle, baseAddress);
            PointerQueryResult Patchable = FirstJneBeforeACBQuery.Run(handle, ACB.Target);
            String ACBOffset = (Patchable.Target - baseAddress).ToString("X");

            PointerQuery IMEQuery = new PointerQuery
            {
                Condition = AOB(0x75, 0x08, 0x85, 0xC9, 0x0F, 0x85),
                Range = range,
                Step = 0x01,
                Kind = ScanType.ScanByValue
            };

            PointerQueryResult IME = IMEQuery.Run(handle, baseAddress);
            String IMEOffset = IME.Offset.ToString("X");

            // It's slow, but it gets the job done.

            ;
        }

    }
}
