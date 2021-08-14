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
        String itempath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Growtopia", "game", "tiles_page1.rttex");

        String pickup = Path.Combine("C:", "pickup", "tiles_page1.bmp");

        public static void Main(string[] args)
        {
            Console.WriteLine("TeslaXI console demo. Initializing...");
            var p = Process.GetProcessesByName("Growtopia").First();
            IntPtr wh = p.MainWindowHandle;
            GrowtopiaGame g = new GrowtopiaGame(p.Id, 0xA04130);
            TeslaBot bot = new TeslaBot(p.Id);
            Console.WriteLine("TeslaBot instantiated. Please check Growtopia's window title for instructions.");
            Console.WriteLine("To properly close the app, press Ctrl+C while the bot isn't running.");

            wh.SetWindowText("TeslaXI is running. Press Ctrl+X to start breaking the block in front of you.");


            while (true)
            {
                while (true)
                {
                    if (VK.Control.IsKeyDown() && VK.X.IsKeyDown())
                        break;
                    if(VK.Control.IsKeyDown() && VK.C.IsKeyDown())
                    {
                        wh.SetWindowText("Growtopia");
                        Process.GetCurrentProcess().Kill();
                    }
                    Thread.Sleep(10);
                }

                WorldTile tile = bot.GetTileAhead();

                wh.SetWindowText($"Breaking: {bot.WorldTileToString(tile)}");
                bot.Break(x => wh.SetWindowText(x),
                    x => tile.Foreground != 0 && tile.Foreground == x.Foreground || tile.Background != 0 && tile.Background == x.Background,
                    CancellationToken.None); // 5990
                
            }
        }

        public void HarvestAVerticalRowOfATMsWithRayman()
        {
            var p = Process.GetProcessesByName("Growtopia").First();
            GrowtopiaGame g = new GrowtopiaGame(p.Id, 0xA04130);

            var w = p.MainWindowHandle;

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
                    //w.HoldKeyAsync(VK.A, 1).Wait();
                    Thread.Sleep(200);
                    //w.HoldKeyAsync(VK.Space, 1).Wait();
                    Thread.Sleep(400);
                }
                if (toPunchRight)
                {
                    //w.HoldKeyAsync(VK.D, 1).Wait();
                    Thread.Sleep(200);
                    //w.HoldKeyAsync(VK.Space, 1).Wait();
                    Thread.Sleep(400);
                }
                //w.HoldKeyAsync(VK.W, 50).Wait();
                Thread.Sleep(660);
                //break;
            }

            Console.ReadKey();
            HarvestAVerticalRowOfATMsWithRayman();
        }
    }
}
