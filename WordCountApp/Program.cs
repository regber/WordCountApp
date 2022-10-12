using System;
using WordCountLibrary;
using System.Threading.Tasks;
using System.Linq;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;

namespace WordCountApp
{
    class Program
    {
        static Stopwatch sw = new Stopwatch();
        static Stopwatch swMultThrd = new Stopwatch();

        static List<Dictionary<string, int>> wordDictionaries = new List<Dictionary<string, int>>();

        static void Main(string[] filePaths)
        {
            Console.WriteLine("Start!");

            //В один поток
            sw.Start();

            CountingWordsInFiles(filePaths);

            sw.Stop();

            //В несколько потоков
            swMultThrd.Start();

            CountingWordsInFilesMultThrd(filePaths);

            swMultThrd.Stop();

            Console.WriteLine($"Time spent for one thread: {sw.ElapsedMilliseconds} milliseconds");
            Console.WriteLine($"Time spent for multi thread: {swMultThrd.ElapsedMilliseconds} milliseconds");
            Console.WriteLine($"Time spent delta: {Math.Abs(sw.ElapsedMilliseconds- swMultThrd.ElapsedMilliseconds)} milliseconds");

            SaveWordDictionariesToFiles(wordDictionaries, filePaths);

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

            var countingWordsInFileMethod = methods[2];

            wordDictionaries.Clear();

            foreach (var filePath in filePaths)
            {
                wordDictionaries.Add((Dictionary<string, int>)countingWordsInFileMethod.Invoke(null, new object[] { filePath }));
            }
        }

        /// <summary>
        /// Подсчитывает количество слов в текстовых файлах  в несколько потоков
        /// </summary>
        /// <param name="filePaths">Массив путей к текстовым файлам</param>
        /// <returns></returns>
        private static void CountingWordsInFilesMultThrd(string[] filePaths)
        {
            var type = typeof(WordCounter);

            var methods = type.GetMethods(System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);

            var countingWordsInFileMethod = methods[2];

            wordDictionaries.Clear();

            foreach (var filePath in filePaths)
            {
                wordDictionaries.Add(WordCounter.CountingWordsInFileMultThrd(filePath));
            }
        }


        /// <summary>
        /// Сохраняет словарь слов <paramref name="pairsCollection"/> в файл по указаному <paramref name="path"/> в виде списка слов с указанным напротив кол-вом этих слов
        /// </summary>
        /// <typeparam name="TKey">Ключ хранящий слово</typeparam>
        /// <typeparam name="TValue">Значение хранящие количество слов</typeparam>
        /// <param name="pairsCollection">Словарь хранящий слова в качестве ключей и их количество в качестве значениий</param>
        /// <param name="path">Место сохранения файла</param>
        private static void SaveWordDictionariesToFiles<TKey, TValue>(List<Dictionary<TKey, TValue>> pairsCollections, string[] paths)
        {
            for(int idx=0; idx < pairsCollections.Count; idx++)
            {
                using (StreamWriter writer = new StreamWriter($"Counted words in {Path.GetFileNameWithoutExtension(paths[idx])}.txt"))
                {
                    foreach (var pair in pairsCollections[idx])
                    {
                        writer.WriteLine($"{pair.Key}-{pair.Value}");
                    }
                }
            }

        }
    }
}
