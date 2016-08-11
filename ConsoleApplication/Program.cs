using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace TestApplication
{
    class Program
    {
        private static List<int> one = new List<int>
        {
            1,2,3
        };
        private static List<int> two = new List<int>
        {
            2,3,4,4,3,4,2,1
        };

        static string s = "32984yfidfch8i#fiuvnwsf9rh9f#fivew9f8349f#";
        static void Main(string[] args)
        {
            var one = File.ReadAllLines("C:\\Users\\Roman\\Downloads\\afterHangids").Select(x => x.Trim()).Select(x => Convert.ToInt32(x));
            var two = File.ReadAllLines("C:\\Users\\Roman\\Downloads\\beforeHangids").Select(x => x.Trim()).Select(x => Convert.ToInt32(x));
            Console.WriteLine("Before:");
            two.Except(one).Select(x => string.Format("{0}{1}", x, Environment.NewLine)).All(x =>
            {
                Console.WriteLine(x);
                return true;
            });
            Console.WriteLine("After:");
            one.Except(two).Select(x => string.Format("{0}{1}", x, Environment.NewLine)).All(x =>
            {
                Console.WriteLine(x);
                return true;
            });
            Console.WriteLine("Done");
            Console.ReadKey();
        }
    }
}
