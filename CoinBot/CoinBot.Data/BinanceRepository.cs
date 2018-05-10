using CoinBot.Business.Entities;
using CoinBot.Core;
using CoinBot.Data.Interface;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CoinBot.Data
{
    public class BinanceRepository : IBinanceRepository
    {
        private Security security;
        private IRESTRepository _restRepo;
        private string baseUrl;
        private ApiInformation _apiInfo;

        public BinanceRepository()
        {
            security = new Security();
            _restRepo = new RESTRepository();
            baseUrl = "https://api.binance.com";
            _apiInfo = new ApiInformation();
        }

        public bool SetExchangeApi(ApiInformation apiInfo)
        {
            _apiInfo = apiInfo;
            return true;
        }

        public async Task<IEnumerable<Transaction>> GetTransactions()
        {
            string url = CreateUrl("/api/v3/allOrders");

            var response = await _restRepo.GetApiStream<IEnumerable<Transaction>>(url, GetRequestHeaders());

            return response;
        }

        public async Task<Account> GetBalance()
        {
            string url = CreateUrl("/api/v3/account");

            var response = await _restRepo.GetApiStream<Account>(url, GetRequestHeaders());

            return response;
        }

        public async Task<IEnumerable<BinanceTick>> GetCrytpos()
        {
            string url = "v1/open/tick";

            var response = await _restRepo.GetApi<List<BinanceTick>>(url);

            return response;
        }

        public async Task<IEnumerable<Candlestick>> GetCandlestick(string symbol, Interval interval, int limit = 500)
        {
            var intervalDescription = Core.Helper.GetEnumDescription((Interval)interval);

            var queryString = new List<string>
            {
                $"symbol={symbol}",
                $"interval={intervalDescription}",
                $"limit={limit.ToString()}"
            };

            string url = CreateUrl($"/api/v1/klines", false, queryString);

            var response = await _restRepo.GetApiStream<Candlestick[]>(url);

            return response;
        }

        public long GetBinanceTime()
        {
            string url = CreateUrl("/api/v1/time", false);

            var response = _restRepo.GetApi<ServerTime>(url);

            response.Wait();

            return response.Result.serverTime;
        }

        private Dictionary<string, string> GetRequestHeaders()
        {
            var headers = new Dictionary<string, string>();
            headers.Add("X-MBX-APIKEY", _apiInfo.apiKey);

            return headers;
        }

        private string CreateUrl(string apiPath, bool secure = true, List<string> queryString = null)
        {
            var qsValues = string.Empty;
            var url = string.Empty;
            if(queryString != null)
            {
                foreach(var qs in queryString)
                {
                    qsValues += qsValues != string.Empty ? "&" : "";
                    qsValues += qs;
                }
            }
            if (!secure)
            {
                url = baseUrl + $"{apiPath}";
                if (qsValues != string.Empty)
                    url += "?" + qsValues;

                return url;
            }
            var timestamp = GetBinanceTime().ToString();
            var timeStampQS = $"timestamp={timestamp}";
            var hmac = security.GetHMACSignature(_apiInfo.apiSecret, timeStampQS);

            url = baseUrl + $"{apiPath}?{timeStampQS}&signature={hmac}";
            if (qsValues != string.Empty)
                url += "&" + qsValues;

            return url;
        }
    }
}