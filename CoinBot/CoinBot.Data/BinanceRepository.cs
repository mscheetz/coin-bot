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
        private DateTimeHelper _dtHelper;

        /// <summary>
        /// Constructor
        /// </summary>
        public BinanceRepository()
        {
            security = new Security();
            _restRepo = new RESTRepository();
            baseUrl = "https://api.binance.com";
            _apiInfo = new ApiInformation();
            _dtHelper = new DateTimeHelper();
        }

        /// <summary>
        /// Set ApiInformation for repository
        /// </summary>
        /// <param name="apiInfo">ApiInformation object</param>
        /// <returns>Boolean when complete</returns>
        public bool SetExchangeApi(ApiInformation apiInfo)
        {
            _apiInfo = apiInfo;
            return true;
        }

        /// <summary>
        /// Get Transactions for account
        /// </summary>
        /// <returns>Collection of Transactions</returns>
        public async Task<IEnumerable<Transaction>> GetTransactions()
        {
            string url = CreateUrl("/api/v3/allOrders");

            var response = await _restRepo.GetApiStream<IEnumerable<Transaction>>(url, GetRequestHeaders());

            return response;
        }

        /// <summary>
        /// Get account balance
        /// </summary>
        /// <returns>Account object</returns>
        public async Task<Account> GetBalance()
        {
            string url = CreateUrl("/api/v3/account");

            var response = await _restRepo.GetApiStream<Account>(url, GetRequestHeaders());

            return response;
        }

        /// <summary>
        /// Get order information
        /// </summary>
        /// <param name="symbol">string of symbol</param>
        /// <param name="orderId">long of orderId</param>
        /// <returns>OrderResponse object</returns>
        public async Task<OrderResponse> GetOrder(string symbol, long orderId)
        {
            var queryString = new List<string>
            {
                $"symbol={symbol}",
                $"orderId={orderId}",
                $"timestamp={_dtHelper.UTCtoUnixTime()}"
            };

            string url = CreateUrl($"/api/v3/order", true, queryString.ToArray());

            var response = await _restRepo.GetApiStream<OrderResponse>(url, GetRequestHeaders());

            return response;
        }

        /// <summary>
        /// Post/Place a trade
        /// </summary>
        /// <param name="tradeParams">Trade to place</param>
        /// <returns>TradeResponse object</returns>
        public async Task<TradeResponse> PostTrade(TradeParams tradeParams)
        {
            string url = CreateUrl("/api/v3/order");

            tradeParams.timestamp = _dtHelper.UTCtoUnixTime();

            var response = await _restRepo.PostApi<TradeResponse, TradeParams>(url, tradeParams, GetRequestHeaders());

            return response;
        }

        /// <summary>
        /// Delete/Cancel a trade
        /// </summary>
        /// <param name="tradeParams">Trade to delete</param>
        /// <returns>TradeResponse object</returns>
        public async Task<TradeResponse> DeleteTrade(CancelTradeParams tradeParams)
        {
            string url = CreateUrl("/api/v3/order");

            var headers = GetRequestHeaders();

            headers.Add("symbol", tradeParams.symbol);
            if (tradeParams.orderId != 0)
            {
                headers.Add("orderId", tradeParams.orderId.ToString());
            }
            else if (!string.IsNullOrEmpty(tradeParams.origClientOrderId))
            {
                headers.Add("origClientOrderId", tradeParams.origClientOrderId);
            }
            headers.Add("timestamp", _dtHelper.UTCtoUnixTime().ToString());
            
            var response = await _restRepo.DeleteApi<TradeResponse>(url, headers);

            return response;
        }

        /// <summary>
        /// Get Ticker for all pairs
        /// </summary>
        /// <returns>Collection of BinanceTick objects</returns>
        public async Task<IEnumerable<BinanceTick>> GetCrytpos()
        {
            string url = "v1/open/tick";

            var response = await _restRepo.GetApiStream<List<BinanceTick>>(url);

            return response;
        }

        /// <summary>
        /// Get Candlesticks for a symbol
        /// </summary>
        /// <param name="symbol">Trading symbol</param>
        /// <param name="interval">Time interval</param>
        /// <param name="limit">Time limit</param>
        /// <returns>Array of Candlestick objects</returns>
        public async Task<Candlestick[]> GetCandlestick(string symbol, Interval interval, int limit = 500)
        {
            var intervalDescription = Core.Helper.GetEnumDescription((Interval)interval);

            var queryString = new List<string>
            {
                $"symbol={symbol}",
                $"interval={intervalDescription}",
                $"limit={limit.ToString()}"
            };

            string url = CreateUrl($"/api/v1/klines", false, queryString.ToArray());

            var response = await _restRepo.GetApiStream<Candlestick[]>(url);

            return response;
        }

        /// <summary>
        /// Get BinanceTime
        /// </summary>
        /// <returns>long of timestamp</returns>
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

        private string CreateUrl(string apiPath, bool secure = true, string[] queryString = null)
        {
            var qsValues = string.Empty;
            var url = string.Empty;
            if(queryString != null)
            {
                for(int i = 0; i < queryString.Length; i++)
                {
                    qsValues += qsValues != string.Empty ? "&" : "";
                    qsValues += queryString[i];
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