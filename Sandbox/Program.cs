using System;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.IO;

using TheLeftExit.Memory;
using TheLeftExit.Memory.Queries;
using TheLeftExit.Memory.RTTI;

using TheLeftExit.Growtopia;
using TheLeftExit.Growtopia.Structures;
using System.Collections.Generic;

namespace Sandbox
{
    class Program
    {
        static String itempath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Growtopia", "game", "tiles_page1.rttex");

        static String pickup = Path.Combine("C:", "pickup", "tiles_page1.bmp");

        public static void Main(string[] args)
        {

            var b = RTPACK.Decode(itempath);

            b.Save(pickup);
            
            ;

            /*using(var s = File.OpenRead(itempath))
            {
                var d = new ItemDecoder(s);
                String query = "Wizard's Staff";
                var res = d.Decode().Where(x => x.Name == query).First().ItemID.ToString("X4");
                ;
            }*/
        }
    }
}
