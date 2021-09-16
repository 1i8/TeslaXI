using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using TheLeftExit.Growtopia;
using TheLeftExit.Growtopia.Decoding;
using TheLeftExit.Growtopia.ObjectModel;
using TheLeftExit.Memory.Sources;

namespace TheLeftExit.TeslaX
{
    public partial class TeslaBot
    {
        private GrowtopiaGame game;
        private GameWindow window;

        private ItemDefinition[] items;

        public TeslaBot(Int32 processId)
        {
            using(Process p = Process.GetProcessById(processId))
                game = new GrowtopiaGame(new ProcessMemory((uint)processId), (ulong)p.MainModule.BaseAddress);
            window = new GameWindow(processId);

            String pathToItems = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Growtopia", "cache", "items.dat");
            items = ItemsDAT.Decode(pathToItems).ToArray();
        }

        public Int32 Range { get; set; } = 128;
        public Int32 TargetDistance { get; set; } = 32;
    }
}
