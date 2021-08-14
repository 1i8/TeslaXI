using System;
using System.IO;
using System.Linq;
using TheLeftExit.Growtopia;
using TheLeftExit.Growtopia.Decoding;
using TheLeftExit.Growtopia.ObjectModel;

namespace TheLeftExit.TeslaX
{
    public partial class TeslaBot
    {
        private GrowtopiaGame game;
        private GameWindow window;

        private ItemDefinition[] items;

        public TeslaBot(Int32 processId)
        {
            game = new GrowtopiaGame(processId);
            window = new GameWindow(processId);

            String pathToItems = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Growtopia", "cache", "items.dat");
            items = ItemsDAT.Decode(pathToItems).ToArray();
        }

        public Int32 Range { get; set; } = 128;
        public Int32 TargetDistance { get; set; } = 32;
    }
}
