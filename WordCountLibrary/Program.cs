using System;
using System.IO;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Collections.Concurrent;
using System.Threading;

namespace WordCountLibrary
{
    public static class WordCounter
    {
        

        /// <summary>
        /// Извлекает список отдельных слов из переданного <paramref name="text"></paramref>
        /// </summary>
        /// <param name="text">Текст из которого будут извлечены отдельные слова</param>
        /// <returns>Возвращает список содержащий отдельные слова из переданного <paramref name="text"></paramref></returns>
        private static IEnumerable<string> GetWords(string text)
        {
            Regex separator = new Regex(@"(?<tag>(<(\S*?)>)|(<+?.*?>+?))|(?<word>([a-я']{1,}))", RegexOptions.Compiled | RegexOptions.IgnoreCase);

            return separator.Matches(text).Where(m => m.Groups["word"].Value != string.Empty).Select(m => m.Value.ToLower());
        }



        /// <summary>
        /// Подсчитывает количество уникальных слов в <paramref name="text"/> и сохраняет их в <paramref name="countedWords"></paramref>
        /// </summary>
        /// <param name="text">Текст в котором необходимо посчитать слова</param>
        /// <param name="countedWords">Словарь в который сохраняется перечень слов, где в качестве ключа импользуется слово, а в качестве значения кол-во слов в переданном <paramref name="text"/></param>
        private static void CountingUniqueWords(string text, ConcurrentDictionary<string, int> countedWords)
        {
            var words = GetWords(text);

            foreach (var word in words)
            {
                countedWords.AddOrUpdate(word, 1, (key, oldVal) => oldVal + 1);
            }
        }

        /// <summary>
        /// Подсчитывает количество слов в текстовом файле
        /// </summary>
        /// <param name="filePath">путь к текстовому файлу</param>
        /// <returns></returns>
        private static Dictionary<string, int> CountingWordsInFile(string filePath)
        {
            var countedWords = new Dictionary<string, int>();
            var concurDir = new ConcurrentDictionary<string, int>();

            var lines = File.ReadAllLines(filePath);

            var textBlockSize = 1000;
            var textBlockCount = (int)Math.Ceiling((double)(lines.Length / textBlockSize))+1;

            Parallel.For(0, textBlockCount, i => 
            {
                var textBlock = string.Concat(lines.Skip(i* textBlockSize).Take(textBlockSize));
                CountingUniqueWords(textBlock, concurDir);
            });

            countedWords = concurDir.OrderByDescending(pair => pair.Value).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

            return countedWords;
        }
    }
}
