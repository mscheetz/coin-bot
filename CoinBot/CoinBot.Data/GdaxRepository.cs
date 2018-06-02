using CoinBot.Business.Entities;
using CoinBot.Core;
using CoinBot.Data.Interface;
using GDAXSharp.Network.Authentication;
using GDAXSharp.Services.Accounts.Models;
using GDAXSharp.Services.Orders.Models.Responses;
using GDAXSharp.Services.Orders.Types;
using GDAXSharp.Services.Products.Models;
using GDAXSharp.Services.Products.Models.Responses;
using GDAXSharp.Services.Products.Types;
using GDAXSharp.Shared.Types;
using GDAXSharp.Shared.Utilities.Extensions;
using GDAXSharp.WebSocket.Models.Response;
using GDAXSharp.WebSocket.Types;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CoinBot.Data
{
    public class GdaxRepository : IGdaxRepository
    {
        private IRESTRepository _restRepo;
        private GDAXSharp.GDAXClient gdaxClient;
        private DateTimeHelper _dtHelper;
        private Helper _helper;
        private ApiInformation _apiInfo;
        private string baseUrl = "https://api.gdax.com/";

        /// <summary>
        /// Constructor
        /// </summary>
        public GdaxRepository()
        {
            _restRepo = new RESTRepository();
            _dtHelper = new DateTimeHelper();
            _helper = new Helper();
            _apiInfo = new ApiInformation();
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="apiInformation">Api Information</param>
        public GdaxRepository(ApiInformation apiInformation)
        {
            _restRepo = new RESTRepository();
            _dtHelper = new DateTimeHelper();
            SetExchangeApi(apiInformation);
        }

        /// <summary>
        /// Check if the Exchange Repository is ready for trading
        /// </summary>
        /// <returns>Boolean of validation</returns>
        public bool ValidateExchangeConfigured()
        {
            var ready = string.IsNullOrEmpty(_apiInfo.apiKey) ? false : true;
            if (!ready)
                return false;

            return string.IsNullOrEmpty(_apiInfo.apiSecret) ? false : true;
        }

        /// <summary>
        /// Set ApiInformation for repository
        /// </summary>
        /// <param name="apiInfo">ApiInformation object</param>
        /// <returns>Boolean when complete</returns>
        public bool SetExchangeApi(ApiInformation apiInfo)
        {
            _apiInfo = apiInfo;
            BuildClient();
            return true;
        }

        /// <summary>
        /// Build GDAX Client
        /// </summary>
        public void BuildClient()
        {
            var authenticator = new Authenticator(_apiInfo.apiKey, _apiInfo.apiSecret, _apiInfo.extraValue);
            gdaxClient = new GDAXSharp.GDAXClient(authenticator);
        }

        /// <summary>
        /// Get Candlesticks for a trading pair
        /// </summary>
        /// <param name="pair">Trading pair</param>
        /// <param name="stickCount">Number of sticks to return</param>
        /// <param name="candleGranularity">CandleGranularity enum</param>
        /// <returns>Candle array</returns>
        public async Task<Candle[]> GetCandleSticks(string pair, int stickCount, CandleGranularity candleGranularity)
        {
            //stickCount++;
            var start = _dtHelper.SubtractFromUTCNow(0, stickCount, 0);
            var end = DateTime.UtcNow;
            ProductType productType;
            Enum.TryParse(pair, out productType);

            var candlesticks = await gdaxClient.ProductsService.GetHistoricRatesAsync(productType, start, end, candleGranularity);

            return candlesticks.ToArray();
        }

        /// <summary>
        /// Get Current Order book
        /// </summary>
        /// <param name="pair">Trading pair</param>
        /// <returns>ProductsOrderBookResponse object</returns>
        public async Task<ProductsOrderBookResponse> GetOrderBook(string pair)
        {
            ProductType productType;
            Enum.TryParse(pair, out productType);

            var response = await gdaxClient.ProductsService.GetProductOrderBookAsync(productType, ProductLevel.One);

            return response;
        }

        /// <summary>
        /// Get recent trades
        /// </summary>
        /// <param name="pair">Trading pair</param>
        /// <returns>GdaxTrade array</returns>
        public async Task<GdaxTrade[]> GetTrades(string pair)
        {
            var gdaxPair = _helper.CreateGdaxPair(pair);
            var url = baseUrl + $"products/{gdaxPair}/trades";

            var response = await _restRepo.GetApiStream<GdaxTrade[]>(url, GetRequestHeaders());

            return response;
        }

        public async Task<ProductTicker> GetTicker(string pair)
        {
            var end = DateTime.UtcNow;
            ProductType productType;
            Enum.TryParse(pair, out productType);

            var response = await gdaxClient.ProductsService.GetProductTickerAsync(productType);

            return response;
        }

        public async Task<ProductStats> GetStats(string pair)
        {
            var end = DateTime.UtcNow;
            ProductType productType;
            Enum.TryParse(pair, out productType);

            var response = await gdaxClient.ProductsService.GetProductStatsAsync(productType);

            return response;
        }

        /// <summary>
        /// Get Balances for GDAX account
        /// </summary>
        /// <returns>Accout object</returns>
        public async Task<IEnumerable<GDAXSharp.Services.Accounts.Models.Account>> GetBalance()
        {
            var accountList = await gdaxClient.AccountsService.GetAllAccountsAsync();

            return accountList;
        }

        /// <summary>
        /// Place a limit order trade
        /// </summary>
        /// <param name="tradeParams">TradeParams for setting the trade</param>
        /// <returns>OrderResponse object</returns>
        public async Task<GDAXSharp.Services.Orders.Models.Responses.OrderResponse> PlaceTrade(TradeParams tradeParams)
        {
            OrderSide orderSide;
            ProductType productType;
            Enum.TryParse(tradeParams.side, out orderSide);
            Enum.TryParse(tradeParams.symbol, out productType);

            var response = await gdaxClient.OrdersService.PlaceLimitOrderAsync(orderSide, productType, tradeParams.quantity, tradeParams.price);

            return response;
        }

        /// <summary>
        /// Place a stop limit trade
        /// </summary>
        /// <param name="tradeParams">TradeParams for setting the SL</param>
        /// <returns>OrderResponse object</returns>
        public async Task<GDAXSharp.Services.Orders.Models.Responses.OrderResponse> PlaceStopLimit(TradeParams tradeParams)
        {
            OrderSide orderSide;
            ProductType productType;
            Enum.TryParse(tradeParams.side, out orderSide);
            Enum.TryParse(tradeParams.symbol, out productType);

            var response = await gdaxClient.OrdersService.PlaceStopLimitOrderAsync(orderSide, productType, tradeParams.quantity, tradeParams.price, tradeParams.price);

            return response;
        }

        /// <summary>
        /// Get details of an order
        /// </summary>
        /// <param name="id">Order Id</param>
        /// <returns>OrderResponse object</returns>
        public async Task<GDAXSharp.Services.Orders.Models.Responses.OrderResponse> GetOrder(string id)
        {
            var response = await gdaxClient.OrdersService.GetOrderByIdAsync(id);

            return response;
        }

        /// <summary>
        /// Cancel a placed trade
        /// </summary>
        /// <param name="id">Id of trade to cancel</param>
        /// <returns>CancelOrderResponse object</returns>
        public async Task<CancelOrderResponse> CancelTrade(string id)
        {
            var response = await gdaxClient.OrdersService.CancelOrderByIdAsync(id);

            return response;
        }

        /// <summary>
        /// Add request headers to api call
        /// </summary>
        /// <returns>Dictionary of request headers</returns>
        private Dictionary<string, string> GetRequestHeaders()
        {
            var headers = new Dictionary<string, string>();
            headers.Add("User-Agent", ".NET Framework Test Client");

            return headers;
        }
    }
}
