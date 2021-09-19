using System;
using System.Linq;
using System.Reflection;

namespace TheLeftExit.LibraryDemo {
    public static partial class Program {
        public static void Main() {
            MethodInfo[] demoList = typeof(Program).GetMethods().Where(x => x.GetCustomAttribute<LibraryDemoAttribute>() != null).ToArray();

            bool enterKeyPressed = false;
            int selectedDemo = 0;
            while(!enterKeyPressed) {
                Console.Clear();
                Console.WriteLine("Select a demo with LEFT/RIGHT arrow keys, then press ENTER to start:");
                Console.WriteLine($"{selectedDemo + 1}: {demoList[selectedDemo].GetCustomAttribute<LibraryDemoAttribute>().Description}");
                ConsoleKeyInfo key = Console.ReadKey();
                switch (key.Key) {
                    case ConsoleKey.LeftArrow:
                        selectedDemo -= 1;
                        if (selectedDemo == -1)
                            selectedDemo = demoList.Length - 1;
                        break;
                    case ConsoleKey.RightArrow:
                        selectedDemo += 1;
                        if (selectedDemo == demoList.Length)
                            selectedDemo = 0;
                        break;
                    case ConsoleKey.Enter:
                        enterKeyPressed = true;
                        break;
                }
            }

            Console.WriteLine();
            demoList[selectedDemo].Invoke(null, null);
            Console.WriteLine();
            Console.WriteLine("Demo over. Press any key to exit...");
            Console.ReadKey();
        }

        private class LibraryDemoAttribute : Attribute {
            public string Description { get; set; }
            public LibraryDemoAttribute(string s) : base() { Description = s; }
        }
    }
}
