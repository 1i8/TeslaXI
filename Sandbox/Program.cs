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

using System.Windows.Input;

namespace Sandbox
{
    class Program
    {
        static String itempath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Growtopia", "game", "tiles_page1.rttex");

        static String pickup = Path.Combine("C:", "pickup", "tiles_page1.bmp");

        public static void Main(string[] args)
        {
            var p = Process.GetProcessesByName("Growtopia").First();

            var w = p.MainWindowHandle;

            //var s = p.Handle.GetRTTIClassNames64(0x1a4540f61c0);

            GrowtopiaGame g = new GrowtopiaGame(p.Id, 0xA04130);

            TeslaBot bot = new TeslaBot(p.Id);

            // Second parameter takes WorldObject and returns bool. Use it to filter what blocks you want broken.
            // To fine-tune movement, change properties in TeslaBot.cs.
            bot.Break(x => Console.WriteLine(x), x => true, CancellationToken.None);

            /*
            while (g.App.GameLogicComponent.NetAvatar.Position.Y > 32)
            {
                bool toPunchLeft = false, toPunchRight = false;
                Point pos = g.App.GameLogicComponent.NetAvatar.Position;
                pos = new Point(pos.X / 32, pos.Y / 32);
                for (int i = 1; i <= 3; i++)
                {
                    if (g.App.GameLogicComponent.World.WorldTileMap[pos.X + i, pos.Y].TicksPassed >= 22 * 60 * 60)
                        toPunchRight = true;
                    if (g.App.GameLogicComponent.World.WorldTileMap[pos.X - i, pos.Y].TicksPassed >= 22 * 60 * 60)
                        toPunchLeft = true;
                }
                if (toPunchLeft)
                {
                    w.HoldKeyAsync(VK.A, 1).Wait();
                    Thread.Sleep(200);
                    w.HoldKeyAsync(VK.Space, 1).Wait();
                    Thread.Sleep(400);
                }
                if (toPunchRight)
                {
                    w.HoldKeyAsync(VK.D, 1).Wait();
                    Thread.Sleep(200);
                    w.HoldKeyAsync(VK.Space, 1).Wait();
                    Thread.Sleep(400);
                }
                w.HoldKeyAsync(VK.W, 50).Wait();
                Thread.Sleep(660);
                //break;
            }
            
            Console.ReadKey();
            Main(null);

            */

            /*using(var s = File.OpenRead(itempath))
            {
                var d = new ItemDecoder(s);
                String query = "Wizard's Staff"
                var res = d.Decode().Where(x => x.Name == query).First().ItemID.ToString("X4");
                ;
            }*/
        }
    }
}
