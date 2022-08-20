using System;
using System.IO;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace WordCountApp
{
    class Program
    {
        static async Task Main(string[] filePaths)
        {


                Console.WriteLine("Start!");

            foreach (var path in filePaths)
            {
                //Console.WriteLine(path); 
                using (StreamReader sr = new StreamReader(path))
                {
                    
                    while(!sr.EndOfStream)
                    {
                        var line = await sr.ReadLineAsync();
                        var wordsInLine = GetSeparatedWords(line, @"[a-я]*[a-я][a-я]*");

                        foreach (var word in wordsInLine)
                        {
                            Console.WriteLine(word);
                        }
                        Console.ReadKey();
                    }
                }
            }

            Console.WriteLine("Completed!");

            Console.ReadLine();
        }

        private static IEnumerable<string> GetSeparatedWords(string text,string regexPattern)
        {
            Regex separator = new Regex(regexPattern, RegexOptions.Compiled | RegexOptions.IgnoreCase);

            return separator.Matches(text).Select(m => m.Value);
        }
    }
}
