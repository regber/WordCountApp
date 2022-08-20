﻿using System;
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
            Dictionary<string, int> countedWords = new Dictionary<string, int>();

            Console.WriteLine("Start!");

            foreach (var path in filePaths)
            {
                using (StreamReader sr = new StreamReader(path))
                {
                
                    while (!sr.EndOfStream)
                    {
                        var text = await sr.ReadToEndAsync();
                        ProcessTextToWords(countedWords, text);
                    }

                    var orderCountedWords = countedWords.OrderByDescending(pair => pair.Value);

                    SavePairsCollectionToFile(orderCountedWords, "CountedWords.txt");
                }
            }

            Console.WriteLine("Completed!");

            Console.ReadLine();
        }

        private static IEnumerable<string> GetSeparatedWords(string text,string regexPattern)
        {
            Regex separator = new Regex(regexPattern, RegexOptions.Compiled | RegexOptions.IgnoreCase);

            return separator.Matches(text).Select(m => m.Value.ToLower());
        }

        private static async void SavePairsCollectionToFile<TKey, TValue>(IEnumerable<KeyValuePair<TKey, TValue>> pairsCollection,string path)
        {
            using(StreamWriter sw = new StreamWriter(path))
            {
                foreach(var pair in pairsCollection)
                {
                    await sw.WriteLineAsync($"{pair.Key}-{pair.Value}");
                }
            }
        }

        private static void ProcessTextToWords(IDictionary<string, int> countedWords, string line)
        {
            var wordsInText = GetSeparatedWords(line, @"[a-я]*[a-я][a-я]*");

            foreach (var word in wordsInText)
            {
                if (countedWords.ContainsKey(word))
                    countedWords[word]++;
                else
                    countedWords.Add(word, 1);
            }
        }
    }
}
