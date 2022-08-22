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

        /// <summary>
        /// Извлекает список отдельных слов из переданного <paramref name="text"></paramref>
        /// </summary>
        /// <param name="text">Текст из которого будут извлечены отдельные слова</param>
        /// <returns>Возвращает список содержащий отдельные слова из переданного <paramref name="text"></paramref></returns>
        private static IEnumerable<string> GetWords(string text)
        {
            Regex separator = new Regex(@"(?<tag>(<(\S*?)>)|(<+?.*?>+?))|(?<word>([a-я']{1,}))", RegexOptions.Compiled|RegexOptions.IgnoreCase);

            return separator.Matches(text).Where(m=>m.Groups["word"].Value !=string.Empty).Select(m => m.Value.ToLower());
        }

        /// <summary>
        /// Сохраняет словарь слов <paramref name="pairsCollection"/> в файл по указаному <paramref name="path"/> в виде списка слов с указанным напротив кол-вом этих слов
        /// </summary>
        /// <typeparam name="TKey">Ключ хранящий слово</typeparam>
        /// <typeparam name="TValue">Значение хранящие количество слов</typeparam>
        /// <param name="pairsCollection">Словарь хранящий слова в качестве ключей и их количество в качестве значениий</param>
        /// <param name="path">Место сохранения файла</param>
        private static async void SaveWordDictionaryToFile<TKey, TValue>(IEnumerable<KeyValuePair<TKey, TValue>> pairsCollection,string path)
        {
            using(StreamWriter writer = new StreamWriter(path))
            {
                foreach(var pair in pairsCollection)
                {
                    await writer.WriteLineAsync($"{pair.Key}-{pair.Value}");
                }
            }
        }

        /// <summary>
        /// Подсчитывает количество уникальных слов в <paramref name="text"/> и сохраняет их в <paramref name="countedWords"></paramref>
        /// </summary>
        /// <param name="text">Текст в котором необходимо посчитать слова</param>
        /// <param name="countedWords">Словарь в который сохраняется перечень слов, где в качестве ключа импользуется слово, а в качестве значения кол-во слов в переданном <paramref name="text"/></param>
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

        /// <summary>
        /// Подсчитывает количество слов в текстовых файлах
        /// </summary>
        /// <param name="filePaths">Массив путей к текстовым файлам</param>
        /// <returns></returns>
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

                    SaveWordDictionaryToFile(orderCountedWords, $"Counted words in {Path.GetFileNameWithoutExtension(path)}.txt");
                }
            }
        }
    }
}
