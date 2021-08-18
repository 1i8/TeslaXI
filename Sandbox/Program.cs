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

namespace Sandbox
{
    class Program
    {
        static void Main(string[] args)
        {

            var p = Process.GetProcessesByName("Growtopia").First();
            GrowtopiaGame g = new GrowtopiaGame(p.Id, 0xA04130);
            GameWindow w = new GameWindow(p.Id);

            HarvestAVerticalRowOfATMsWithRayman(g, w);
        }

        // My personal ATM harvester, don't mind it here.
        static void HarvestAVerticalRowOfATMsWithRayman(GrowtopiaGame g, GameWindow w)
        {
            while (g.App.GameLogicComponent.NetAvatar.Position.Y > 32)
            {
                bool toPunchLeft = false, toPunchRight = false;
                WorldCamera cam = g.App.GameLogicComponent.WorldRenderer.WorldCamera;
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
                    Point lbp = cam.MapBlockToScreen(pos.X - 1, pos.Y).Location;
                    lbp.Offset((Int32)(cam.ZoomFactor * 16), (Int32)(cam.ZoomFactor * 16));
                    w.MouseClick(lbp.X, lbp.Y, 100);
                    Thread.Sleep(250);
                }
                if (toPunchRight)
                {
                    Point lbp = cam.MapBlockToScreen(pos.X + 1, pos.Y).Location;
                    lbp.Offset((Int32)(cam.ZoomFactor * 16), (Int32)(cam.ZoomFactor * 16));
                    w.MouseClick(lbp.X, lbp.Y, 100);
                    Thread.Sleep(250);
                }
                w.KeyPress(VK.W, 100);
                Thread.Sleep(400);
            }
        }
    }
}
