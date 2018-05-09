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

        public async Task<IEnumerable<Candlestick>> GetCandlestick(string symbol, Interval interval)
        {
            string url = $"/api/v1/klines?symbol={symbol}&interval={interval.ToString()}";

            var response = await _restRepo.GetApi<List<Candlestick>>(url);

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

        private string CreateUrl(string apiPath, bool secure = true)
        {
            if (!secure)
            {
                return baseUrl + $"{apiPath}";
            }
            var timestamp = GetBinanceTime().ToString();
            var queryString = $"timestamp={timestamp}";
            var hmac = security.GetHMACSignature(_apiInfo.apiSecret, queryString);

            return baseUrl + $"{apiPath}?{queryString}&signature={hmac}";
        }
    }
}