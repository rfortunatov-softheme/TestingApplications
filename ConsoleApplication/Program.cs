using System;

namespace TestApplication
{
    class Program
    {
        static void Main(string[] args)
        {
            var test = new TapesDatabaseClientTests.TapesDatabaseClientTests();
            test.TestClient();
            Console.WriteLine("Done");
            Console.ReadKey();
        }
    }
}
