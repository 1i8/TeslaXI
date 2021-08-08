using System;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.IO;

using TheLeftExit.Memory;
using TheLeftExit.Memory.Queries;
using TheLeftExit.Memory.RTTI;

using TheLeftExit.Growtopia;
using TheLeftExit.Growtopia.Classes;
using System.Collections.Generic;
using System.Drawing;

namespace Sandbox
{
    class Program
    {
        static String itempath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Growtopia", "game", "tiles_page1.rttex");

        static String pickup = Path.Combine("C:", "pickup", "tiles_page1.bmp");

        public static void Main(string[] args)
        {
            var p = Process.GetProcessesByName("Growtopia").First();

            //var s = p.Handle.GetRTTIClassNames64(0x1a4540f61c0);

            Growtopia g = new Growtopia(p.Id, 0xA04130);
            var pos = g.App.GameLogicComponent.NetAvatar.Position;
            var tile = g.App.GameLogicComponent.World.WorldTileMap[pos.X / 32, pos.Y / 32];

            ;

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
