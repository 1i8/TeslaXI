using System;
using System.Linq;
using System.Diagnostics;
using System.Collections.Generic;
using System.Collections.Concurrent;

using TheLeftExit.Growtopia.Native;
using System.Threading.Tasks;
using System.IO;
using System.Threading;

namespace TheLeftExit
{
    public class ClassInfo
    {
        public Int64 Address;
        public String ClassName;
        public String FullPath;
        public bool Scanned;
    }

    public static class StructureCrawler
    {
        static bool RangeContains(this List<(Int64, Int64)> list, Int64 value) => list.Any(x => value >= x.Item1 && value <= x.Item2);

        static bool IsClassName(this String s) => s != null && s != "type_info" && s != "SurfaceAnim" && s != "Entity" /*&& s != "Scroll"*/ && !s.Contains("::");

        static List<ClassInfo> classesGlobal = new();

        static List<ClassInfo> ScanRange(this IntPtr handle, Int64 Start, String Path, Int64 maxRange = 0x8000)
        {
            List<ClassInfo> classes = new();

            Console.WriteLine($"Scanning {Path}");

            const Byte step = 8;

            Int64 i = Start;

            // Scanning until cannot read or enter previously scanned space.
            while(i - Start < maxRange && handle.ReadInt64(i, out Int64 iRef))
            {
                if (!classes.Concat(classesGlobal).Any(x => x.Address == i || x.Address == iRef))
                {
                    // Attempt to get class name by reference.
                    String res = handle.GetRTTIClassName64(iRef);
                    if (res.IsClassName())
                    {
                        // Add the class and scan its process space.
                        classes.Add(new ClassInfo
                        {
                            Address = iRef,
                            ClassName = res,
                            FullPath = $"{Path}-[{(i - Start):X}]->{res}",
                            Scanned = false
                        });
                        //Console.WriteLine($"{Path}-[{(i - Start):X}]->{res}");
                    }
                    else
                    {
                        // Attempt to get class name by value.
                        res = handle.GetRTTIClassName64(i);
                        if (res.IsClassName())
                        {
                            // Add the class.
                            classes.Add(new ClassInfo
                            {
                                Address = i,
                                ClassName = res,
                                FullPath = $"{Path}=[{(i - Start):X}]=>{res}",
                                Scanned = true
                            });
                            Console.WriteLine($"{Path}=[{(i - Start):X}]=>{res}");
                        }
                    }
                }

                i += step;
            }

            return classes;
        }

        static void Main(string[] args)
        {
            var p = Process.GetProcessesByName("Growtopia").First();

            Int64 glcAddr = new Growtopia.ObjectModel.GrowtopiaGame(p.Id).App.GameLogicComponent.Address;

            IntPtr handle = Memory.OpenProcess(p.Id);

            classesGlobal = handle.ScanRange(/*(Int64)p.MainModule.BaseAddress*/ glcAddr, "GameLogicComponent");
            while(true)
            {
                var toScan = classesGlobal.Where(x => x.Scanned == false).ToArray();
                if (toScan.Length == 0)
                    break;
                foreach(ClassInfo c in toScan)
                {
                    c.Scanned = true;
                    classesGlobal.AddRange(handle.ScanRange(c.Address, c.FullPath));
                }
            }

            File.WriteAllText(@"C:\map.txt", classesGlobal.Select(x => x.FullPath).Aggregate((x1, x2) => x1 + Environment.NewLine + x2));

            //var res = classes.Select(x => x.ClassName).Distinct().Aggregate((x1, x2) => x1 + Environment.NewLine + x2);

            ;
        }
    }
}
