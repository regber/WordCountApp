using System;
using System.Threading.Tasks;
using System.Linq;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Net;
using System.Text.Json;
using System.Net.Http;
using System.Net.Http.Headers;

namespace WordCountApp
{
    class Program
    {
        static Stopwatch swMultThrd = new Stopwatch();


        static void Main(string[] filePaths)
        {
            Console.WriteLine("Start!");

            //В несколько потоков
            swMultThrd.Start();

            CountingWordsInFilesMultThrd(filePaths);

            swMultThrd.Stop();


            Console.WriteLine($"Time spent: {swMultThrd.ElapsedMilliseconds} milliseconds");


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
            if(!TestConnectToAPI())
            {
                return;
            }

            foreach (var filePath in filePaths)
            {

                var text = File.ReadAllText(filePath);

                var wordDictionaries = GetCountingWordsInFileFromAPI(text);

                SaveWordDictionariesToFiles(wordDictionaries, filePath);
            }
        }

        /// <summary>
        /// Подсчитать слова в тексте
        /// </summary>
        /// <param name="text">Текст</param>
        /// <returns></returns>
        private static Dictionary<string,int> GetCountingWordsInFileFromAPI(string text)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:44360");

                var textbase64 = Base64.Encode(text);

                var stringContent = @"""" + textbase64 + @"""";

                var content = new StringContent(stringContent, Encoding.UTF8, "application/json");

                content.Headers.ContentLength = stringContent.Length;

                var result = client.PostAsync("/api/WordCounter/GetWordDictionary", content);

                string resultContent = result.Result.Content.ReadAsStringAsync().Result;

                var wordDictionaries = JsonSerializer.Deserialize<Dictionary<string, int>>(Base64.Decode(resultContent));

                return wordDictionaries;
            }
        }

        /// <summary>
        /// Проверить наличие соединения с API
        /// </summary>
        /// <returns></returns>
        private static bool TestConnectToAPI()
        {
            try 
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("https://localhost:44360");

                    var result = client.GetAsync("/api/WordCounter/TestConnect");

                    string resultContent = result.Result.Content.ReadAsStringAsync().Result;

                    if(resultContent!= "ConnectionSuccessful")
                    {
                        throw new Exception("API не отвечает");
                    }
                }

                return true;
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}") ;
                return false;
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

    public static class Base64
    {
        public static string Encode(string text)
        {
            var plainTextBytes = Encoding.UTF8.GetBytes(text);

            return Convert.ToBase64String(plainTextBytes);
        }

        public static string Decode(string base64EncodedData)
        {
            var base64EncodedBytes = Convert.FromBase64String(base64EncodedData);

            return Encoding.UTF8.GetString(base64EncodedBytes);
        }
    }


}
