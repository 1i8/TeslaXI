using System;
using System.IO;
using System.Linq;
using TheLeftExit.Growtopia.Decoding;
using TheLeftExit.Growtopia.ObjectModel;

namespace TheLeftExit.TeslaX
{
    public partial class TeslaBot
    {
        private GrowtopiaGame game;

        private ItemDefinition[] items;

        public TeslaBot(Int32 processId)
        {
            game = new GrowtopiaGame(processId);

            String pathToItems = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Growtopia", "cache", "items.dat");
            items = ItemsDAT.Decode(pathToItems).ToArray();
        }
    }
}
