using System;
using System.Threading.Tasks;
using System.Linq;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;
using WordCountServiceReference;
using System.Text;

namespace WordCountApp
{
    class Program
    {
        static Stopwatch swMultThrd = new Stopwatch();
        static WordCountServiceClient client = new WordCountServiceClient();

        static void Main(string[] filePaths)
        {
            Console.WriteLine("Start!");

            //В несколько потоков
            swMultThrd.Start();

            CountingWordsInFilesMultThrd(filePaths);

            swMultThrd.Stop();

            Console.WriteLine($"Time spent for multi thread: {swMultThrd.ElapsedMilliseconds} milliseconds");

            //SaveWordDictionariesToFiles(wordDictionaries, filePaths);

            Console.WriteLine("Completed!");

            Console.ReadLine();
        }


        /// <summary>
        /// Подсчитывает количество слов в текстовых файлах  в несколько потоков
        /// </summary>
        /// <param name="filePaths">Массив путей к текстовым файлам</param>
        /// <returns></returns>
        private static void CountingWordsInFilesMultThrd(string[] filePaths)
        {
            try
            {
                // проверка соединения

                if (!string.Equals(client.TestConnection(), "ConnectionSuccessful", StringComparison.InvariantCultureIgnoreCase))
                {
                    throw new Exception("Проверка соединения не удалась");
                }

                foreach (var filePath in filePaths)
                {
                    var textByteArray = Encoding.Unicode.GetBytes(File.ReadAllText(filePath));

                    var wordDictionary = client.CountingWordsInFileMultThrd(textByteArray);//метод из сервиса

                    SaveWordDictionariesToFiles(wordDictionary, filePath);
                }
            }
            catch(Exception ex)
            {
                // в случае ошибки необходимо принудительно закрыть клиент методом Abort()
                client.Abort();

                // выводим информацию об ошибке
                Console.WriteLine();
                Console.WriteLine("Ошибка: {0}", ex.Message);
            }
        }


        /// <summary>
        /// Сохраняет словарь слов <paramref name="pairsCollection"/> в файл по указаному <paramref name="path"/> в виде списка слов с указанным напротив кол-вом этих слов
        /// </summary>
        /// <typeparam name="TKey">Ключ хранящий слово</typeparam>
        /// <typeparam name="TValue">Значение хранящие количество слов</typeparam>
        /// <param name="pairsCollection">Словарь хранящий слова в качестве ключей и их количество в качестве значениий</param>
        /// <param name="path">Место сохранения файла</param>
        private static void SaveWordDictionariesToFiles<TKey, TValue>(Dictionary<TKey, TValue> pairsCollection, string path)
        {
            using (StreamWriter writer = new StreamWriter($"Counted words in {Path.GetFileNameWithoutExtension(path)}.txt"))
            {
                foreach (var pair in pairsCollection)
                {
                    writer.WriteLine($"{pair.Key}-{pair.Value}");
                }
            }
        }
    }
}
