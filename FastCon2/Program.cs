using System;
using FastCon2;

namespace MyApp // Note: actual namespace depends on the project name.
{
    internal class Program
    {
        private MyConsole mc = new MyConsole();

        private void go()
        {
            if (mc.SetupCon())
            {
                mc.WriteAt('A', 10, 10);
                mc.WriteAt("Frank Rosbak", 10, 12);
                for (int x = 0; x < 110; x++)
                    mc.WriteAt('*', x, 14);
            }
        }
        
        static void Main(string[] args)
        {
            Program p = new Program();
            p.go();
            Console.ReadKey();
        }
    }
}