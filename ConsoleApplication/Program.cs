using System;

namespace TestApplication
{
    public class TestCL
    {
        public string onr { get; set; }

        public string two { get; set; }
    }

    class Program
    {
        private static TestCL _prop;

        private static TestCL prop
        {
            get
            {
                Console.WriteLine("Getting");
                return _prop;
            }
            set
            {
                Console.WriteLine("Setting");
                _prop = value;
            }
        }

        static void Main(string[] args)
        {
            prop = new TestCL();
            prop.onr = "fuck";
            Console.WriteLine("Done");
            Console.ReadKey();
        }
    }
}
