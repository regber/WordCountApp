using System.Collections.Generic;
using System.ServiceModel;

namespace WordCountService
{

    [ServiceContract]
    public interface IWordCountService
    {
        #region WordCalcul

        [OperationContract]
        Dictionary<string, int> CountingWordsInFileMultThrd(byte[] fileByteArr);

        #endregion

        #region Common Methods

        /// <summary>
        /// проверка соединения
        /// </summary>
        /// <returns> OK </returns>
        [OperationContract]
        string TestConnection();

        #endregion


    }
}
