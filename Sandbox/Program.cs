using System;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;

using TheLeftExit.Growtopia;
using TheLeftExit.Growtopia.ObjectModel;
using TheLeftExit.Growtopia.Native;
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

            Growtopia g = new Growtopia(p.Id, 0xA04130);

            uint A = 65, D = 68, Space = 32, W = 87;

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
                    w.HoldKeyAsync(A, 1).Wait();
                    Thread.Sleep(200);
                    w.HoldKeyAsync(Space, 1).Wait();
                    Thread.Sleep(400);
                }
                if (toPunchRight)
                {
                    w.HoldKeyAsync(D, 1).Wait();
                    Thread.Sleep(200);
                    w.HoldKeyAsync(Space, 1).Wait();
                    Thread.Sleep(400);
                }
                w.HoldKeyAsync(W, 50).Wait();
                Thread.Sleep(660);
                //break;
            }

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
