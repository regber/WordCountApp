using System;
using WordCountLibrary;
using System.Threading.Tasks;
using System.Linq;
using System.IO;
using System.Collections.Generic;

namespace WordCountApp
{
    class Program
    {
        static void Main(string[] filePaths)
        {
            Console.WriteLine("Start!");

            CountingWordsInFiles(filePaths);

            Console.WriteLine("Completed!");

            Console.ReadLine();
        }

        /// <summary>
        /// Подсчитывает количество слов в текстовых файлах
        /// </summary>
        /// <param name="filePaths">Массив путей к текстовым файлам</param>
        /// <returns></returns>
        private static void CountingWordsInFiles(string[] filePaths)
        {
            var type = typeof(WordCounter);

            var methods = type.GetMethods(System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);

            var desiredMethod = methods[2];

            foreach (var filePath in filePaths)
            {
                var wordDictionary = (Dictionary<string, int>)desiredMethod.Invoke(null, new object[] { filePath });

                SaveWordDictionaryToFile<string, int>(wordDictionary, $"Counted words in {Path.GetFileNameWithoutExtension(filePath)}.txt");
            }
        }


        /// <summary>
        /// Сохраняет словарь слов <paramref name="pairsCollection"/> в файл по указаному <paramref name="path"/> в виде списка слов с указанным напротив кол-вом этих слов
        /// </summary>
        /// <typeparam name="TKey">Ключ хранящий слово</typeparam>
        /// <typeparam name="TValue">Значение хранящие количество слов</typeparam>
        /// <param name="pairsCollection">Словарь хранящий слова в качестве ключей и их количество в качестве значениий</param>
        /// <param name="path">Место сохранения файла</param>
        private static void SaveWordDictionaryToFile<TKey, TValue>(IEnumerable<KeyValuePair<TKey, TValue>> pairsCollection, string path)
        {
            using (StreamWriter writer = new StreamWriter(path))
            {
                foreach (var pair in pairsCollection)
                {
                    writer.WriteLine($"{pair.Key}-{pair.Value}");
                }
            }
        }
    }
}
