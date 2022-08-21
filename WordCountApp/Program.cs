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

            await CountingWordsInFiles(filePaths);

            Console.WriteLine("Completed!");

            Console.ReadLine();
        }

        private static IEnumerable<string> GetWords(string text)
        {
            Regex separator = new Regex(@"(?<tag>(<(\S*?)>)|(<+?.*?>+?))|(?<word>([a-я']{1,}))", RegexOptions.Compiled|RegexOptions.IgnoreCase);

            return separator.Matches(text).Where(m=>m.Groups["word"].Value !=string.Empty).Select(m => m.Value.ToLower());
        }

        private static async void SavePairsCollectionToFile<TKey, TValue>(IEnumerable<KeyValuePair<TKey, TValue>> pairsCollection,string path)
        {
            using(StreamWriter writer = new StreamWriter(path))
            {
                foreach(var pair in pairsCollection)
                {
                    await writer.WriteLineAsync($"{pair.Key}-{pair.Value}");
                }
            }
        }

        private static void CountingUniqueWords(string text, IDictionary<string, int> countedWords)
        {
            var words = GetWords(text);

            foreach (var word in words)
            {
                if (countedWords.ContainsKey(word))
                    countedWords[word]++;
                else
                    countedWords.Add(word, 1);
            }
        }

        private static async Task CountingWordsInFiles(string[] filePaths)
        {
            Dictionary<string, int> countedWords = new Dictionary<string, int>();

            foreach (var path in filePaths)
            {
                using (StreamReader reader = new StreamReader(path))
                {

                    while (!reader.EndOfStream)
                    {
                        var text = await reader.ReadToEndAsync();
                        CountingUniqueWords(text, countedWords);
                    }

                    var orderCountedWords = countedWords.OrderByDescending(pair => pair.Value);

                    SavePairsCollectionToFile(orderCountedWords, $"Counted words in {Path.GetFileNameWithoutExtension(path)}.txt");
                }
            }
        }
    }
}
