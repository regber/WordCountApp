using System;

namespace WordCountApp
{
    class Program
    {
        static void Main(string[] filePaths)
        {
            Console.WriteLine("Start!");

            foreach (var path in filePaths)
            {
                Console.WriteLine(path);
            }

            Console.WriteLine("Completed!");

            Console.ReadLine();
        }
    }
}
