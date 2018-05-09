using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CoinBot.Data.Interface
{
    public interface IRESTRepository
    {
        /// <summary>
        /// Get call to api
        /// </summary>
        /// <typeparam name="T">Type to return</typeparam>
        /// <param name="url">Url to access</param>
        /// <param name="headers">Http Request headers (optional)</param>
        /// <returns>Type requested</returns>
        Task<T> GetApi<T>(string url, Dictionary<string, string> headers = null);

        /// <summary>
        /// Get call to api stream
        /// For large json responses
        /// </summary>
        /// <typeparam name="T">Type to return</typeparam>
        /// <param name="url">Url to access</param>
        /// <param name="headers">Http Request headers (optional)</param>
        /// <returns>Type requested</returns>
        Task<T> GetApiStream<T>(string url, Dictionary<string, string> headers = null);
    }
}
