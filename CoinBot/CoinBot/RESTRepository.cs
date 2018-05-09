using CoinBot.Data.Interface;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace CoinBot.Data
{
    public class RESTRepository : IRESTRepository
    {
        public RESTRepository()
        {
        }

        /// <summary>
        /// Get call to api
        /// </summary>
        /// <typeparam name="T">Type to return</typeparam>
        /// <param name="url">Url to access</param>
        /// <param name="headers">Http Request headers (optional)</param>
        /// <returns>Type requested</returns>
        public async Task<T> GetApi<T>(string url, Dictionary<string, string> headers = null)
        {
            using (var client = new HttpClient())
            {
                if (headers != null)
                {
                    foreach (var header in headers)
                    {
                        client.DefaultRequestHeaders.Add(header.Key, header.Value);
                    }
                }

                var response = await client.GetAsync(url);

                string responseMessage = await response.Content.ReadAsStringAsync();

                return JsonConvert.DeserializeObject<T>(responseMessage);
            }
        }

        /// <summary>
        /// Get call to api stream 
        /// For large json responses
        /// </summary>
        /// <typeparam name="T">Type to return</typeparam>
        /// <param name="url">Url to access</param>
        /// <param name="headers">Http Request headers (optional)</param>
        /// <returns>Type requested</returns>
        public async Task<T> GetApiStream<T>(string url, Dictionary<string, string> headers = null)
        {
            using (var client = new HttpClient())
            {
                if (headers != null)
                {
                    foreach (var header in headers)
                    {
                        client.DefaultRequestHeaders.Add(header.Key, header.Value);
                    }
                }

                var responseMessage = String.Empty;
                var response = await client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);
                var sb = new StringBuilder();
                using (var stream = await response.Content.ReadAsStreamAsync())
                {
                    using (var sr = new StreamReader(stream, Encoding.UTF8))
                    {
                        sb.Append(sr.ReadToEnd());
                    }

                    responseMessage = sb.ToString();
                }

                return JsonConvert.DeserializeObject<T>(responseMessage);
            }
        }
    }
}
