using System.Collections.Generic;
using WordCountLibrary;


namespace WordCountService
{
    // ПРИМЕЧАНИЕ. Команду "Переименовать" в меню "Рефакторинг" можно использовать для одновременного изменения имени класса "Service1" в коде, SVC-файле и файле конфигурации.
    // ПРИМЕЧАНИЕ. Чтобы запустить клиент проверки WCF для тестирования службы, выберите элементы Service1.svc или Service1.svc.cs в обозревателе решений и начните отладку.
    public class WordCountService : IWordCountService
    {
        #region WordCalcul

        /// <summary>
        /// Посчитать слова в тексте
        /// </summary>
        /// <param name="fileByteArr">Текст конвертированный в массив байтов</param>
        /// <returns></returns>
        public Dictionary<string, int> CountingWordsInFileMultThrd(byte[] fileByteArr)
        {
            return WordCounter.CountingWordsInFileMultThrd(fileByteArr);
        }

        #endregion

        #region Common Methods

        /// <summary>
        /// проверка соединения
        /// </summary>
        /// <returns> OK </returns>
        public string TestConnection()
        {
            return "ConnectionSuccessful";
        }

        #endregion

    }
}
