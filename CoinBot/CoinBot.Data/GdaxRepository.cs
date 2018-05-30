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
        private ApiInformation _apiInfo;
        private ICollection<WSSTicker> _wssTicker;

        /// <summary>
        /// Constructor
        /// </summary>
        public GdaxRepository()
        {
            _restRepo = new RESTRepository();
            _dtHelper = new DateTimeHelper();
            _apiInfo = new ApiInformation();
            _wssTicker = new List<WSSTicker>();
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

        public async Task<IList<IList<ProductTrade>>> GetTrades(string pair)
        {
            ProductType productType;
            Enum.TryParse(pair, out productType);

            var response = await gdaxClient.ProductsService.GetTradesAsync(productType);

            return response;

            //var baseUrl = "https://api.gdax.com";
            //var url = baseUrl + $"/products/{productType.GetEnumMemberValue()}/trades";

            //var response = await _restRepo.GetApi<IList<IList<ProductTrade>>>(url);

            //return response;
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

        public WSSTicker[] GetWSSTicker(string pair)
        {
            return _wssTicker.OrderBy(w => w.time).ToArray();
        }

        public async Task StartWebsocket(string pair)
        {
            //gdaxClient.WebSocket.OnTickerReceived();
            //ProductType productType;
            //Enum.TryParse(pair, out productType);
            //ClientWebSocket socket = new ClientWebSocket();
            //Task task = socket.ConnectAsync(new Uri("wss://ws-feed.gdax.com"), CancellationToken.None);
            //task.Wait();
            //Thread readThread = new Thread(
            //    delegate (object obj)
            //    {
            //        byte[] recBytes = new byte[1024];
            //        while (true)
            //        {
            //            ArraySegment<byte> t = new ArraySegment<byte>(recBytes);
            //            Task<WebSocketReceiveResult> receiveAsync = socket.ReceiveAsync(t, CancellationToken.None);
            //            receiveAsync.Wait();
            //            string jsonString = Encoding.UTF8.GetString(recBytes);
            //            WSSTicker tick = null;
            //            try
            //            {
            //                tick = JsonConvert.DeserializeObject<WSSTicker>(jsonString);
            //            }
            //            catch
            //            {
            //                tick = null;
            //            }
            //            if(tick != null && tick.type.Equals("ticker"))
            //            {
            //                _wssTicker.Add(tick);
            //                var nowMinus5 = _dtHelper.SubtractFromUTCNow(0, 2, 0);
            //                _wssTicker = _wssTicker.Where(w => w.time >= nowMinus5).ToList();
            //            }
            //            //Console.Out.WriteLine("jsonString = {0}", jsonString);
            //            recBytes = new byte[1024];
            //        }

            //    });
            //readThread.Start();
            //string json = $"{{\"product_ids\":[\"{productType.GetEnumMemberValue()}\"],\"type\":\"subscribe\",\"channels\":[\"ticker\"]}}";
            //byte[] bytes = Encoding.UTF8.GetBytes(json);
            //ArraySegment<byte> subscriptionMessageBuffer = new ArraySegment<byte>(bytes);
            //await socket.SendAsync(subscriptionMessageBuffer, WebSocketMessageType.Text, true, CancellationToken.None);
            ////Console.ReadLine();
        }

        public void LaunchWSS(string pair)
        {
            ProductType productType;
            Enum.TryParse(pair, out productType);

            //subscribe to the ticker channel type
            var productTypes = new List<ProductType> { productType };
            var channels = new List<ChannelType> { ChannelType.Ticker };

            var webSocket = gdaxClient.WebSocket;

            webSocket.Start(productTypes, channels);

            webSocket.OnTickerReceived += (sender, e) =>
            {
                ProcessTicker(e);
            };

            Console.ReadKey();
        }

        private void ProcessTicker(WebfeedEventArgs<Ticker> webfeedEventArgs)
        {
            var tick = new WSSTicker
            {
                best_ask = webfeedEventArgs.LastOrder.BestAsk,
                best_bid = webfeedEventArgs.LastOrder.BestBid,
                high_24h = webfeedEventArgs.LastOrder.High24H,
                last_size = webfeedEventArgs.LastOrder.LastSize,
                low_24h = webfeedEventArgs.LastOrder.Low24H,
                open_24h = webfeedEventArgs.LastOrder.Open24H,
                price = webfeedEventArgs.LastOrder.Price,
                product_id = webfeedEventArgs.LastOrder.ProductId,
                sequence = webfeedEventArgs.LastOrder.Sequence,
                side = webfeedEventArgs.LastOrder.Side.GetEnumMemberValue(),
                time = webfeedEventArgs.LastOrder.Time.DateTime,
                trade_id = webfeedEventArgs.LastOrder.TradeId,
                type = webfeedEventArgs.LastOrder.Type.GetEnumMemberValue(),
                volume_24h = webfeedEventArgs.LastOrder.Volume24H,
                volume_30d = webfeedEventArgs.LastOrder.Volume30D

            };
            _wssTicker.Add(tick);
            var nowMinus5 = _dtHelper.SubtractFromUTCNow(0, 2, 0);
            _wssTicker = _wssTicker.Where(w => w.time >= nowMinus5).ToList();
        }
    }
}
