using CoinBot.Business.Builders.Interface;
using CoinBot.Business.Entities;
using CoinBot.Core;
using CoinBot.Data;
using CoinBot.Data.Interface;
using GDAXSharp.Services.Products.Models;
using GDAXSharp.Services.Products.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CoinBot.Business.Builders
{
    public class ExchangeBuilder : IExchangeBuilder
    {
        private Exchange _thisExchange;
        private IBinanceRepository _bianceRepo;
        private IGdaxRepository _gdaxRepo;
        private Helper _helper;
        private DateTimeHelper _dtHelper;
        private string _pair = string.Empty;
        private string _asset = string.Empty;

        public ExchangeBuilder()
        {
            _bianceRepo = new BinanceRepository();
            _gdaxRepo = new GdaxRepository();
            _helper = new Helper();
            _dtHelper = new DateTimeHelper();
        }

        public ExchangeBuilder(IBinanceRepository binanceRepo)
        {
            _bianceRepo = binanceRepo;
            _gdaxRepo = new GdaxRepository();
            _helper = new Helper();
            _dtHelper = new DateTimeHelper();
        }

        public ExchangeBuilder(IGdaxRepository gdaxRepo)
        {
            _bianceRepo = new BinanceRepository();
            _gdaxRepo = gdaxRepo;
            _helper = new Helper();
            _dtHelper = new DateTimeHelper();
        }

        public ExchangeBuilder(IBinanceRepository binanceRepo, IGdaxRepository gdaxRepo)
        {
            _bianceRepo = binanceRepo;
            _gdaxRepo = gdaxRepo;
            _helper = new Helper();
            _dtHelper = new DateTimeHelper();
        }

        /// <summary>
        /// Set BotSettings
        /// </summary>
        /// <param name="settings">BotSettings Object</param>
        public void SetExchange(BotSettings settings)
        {
            _thisExchange = settings.exchange;
        }

        /// <summary>
        /// Validate exhange api is configured
        /// </summary>
        /// <param name="exchange">Current exchange to use</param>
        /// <returns>Boolean if configured correctly</returns>
        public bool ValidateExchangeConfigured(Exchange exchange)
        {
            _thisExchange = exchange;

            if (_thisExchange == Exchange.BINANCE)
            {
                return _bianceRepo.ValidateExchangeConfigured();
            }
            else if (_thisExchange == Exchange.GDAX)
            {
                return _gdaxRepo.ValidateExchangeConfigured();
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Set Exchange Api Info
        /// </summary>
        /// <param name="apiInfo">ApiInformation for exhange</param>
        /// <returns>Boolean when complete</returns>
        public bool SetExchangeApi(ApiInformation apiInfo)
        {
            if(_thisExchange == Exchange.BINANCE)
            {
                return _bianceRepo.SetExchangeApi(apiInfo);
            }
            else if(_thisExchange == Exchange.GDAX)
            {
                return _gdaxRepo.SetExchangeApi(apiInfo, false);
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Get Candlesticks
        /// </summary>
        /// <param name="symbol">Trading Symbol</param>
        /// <param name="interval">Candlestick Interval</param>
        /// <param name="range">Number of sticks to return</param>
        /// <returns>BotStick Array</returns>
        public BotStick[] GetCandlesticks(string symbol, Interval interval, int range)
        {
            if(_thisExchange == Exchange.BINANCE)
            {
                Candlestick[] sticks = null;

                while(sticks == null)
                {
                    sticks = _bianceRepo.GetCandlestick(symbol, interval, range).Result;
                }

                return BinanceStickToBotStick(sticks);
            }
            else if(_thisExchange == Exchange.GDAX)
            {
                var trades = _gdaxRepo.GetTrades(symbol).Result;

                while (trades == null)
                {
                    trades = _gdaxRepo.GetTrades(symbol).Result;
                }

                return GetSticksFromGdaxTrades(trades, range);
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Get Balances for account
        /// </summary>
        /// <param name="asset">String of asset</param>
        /// <param name="pair">String of trading pair</param>
        /// <returns>Collection of Balance objects</returns>
        public List<Balance> GetBalance(string asset, string pair)
        {
            if (_thisExchange == Exchange.BINANCE)
            {
                var account = _bianceRepo.GetBalance().Result;

                if (account == null || account.balances == null)
                {
                    return new List<Balance>();
                }

                return account.balances.Where(b => b.asset.Equals(asset) || b.asset.Equals(pair)).ToList();
            }
            else if (_thisExchange == Exchange.GDAX)
            {
                var accountList = _gdaxRepo.GetBalance().Result.ToArray();

                var balances = new List<Balance>();

                for (int i = 0; i < accountList.Count(); i++)
                {
                    if (accountList[i].Currency.ToString().Equals(asset)
                        || accountList[i].Currency.ToString().Equals(pair))
                    {
                        var balance = new Balance
                        {
                            asset = accountList[i].Currency.ToString(),
                            free = accountList[i].Available,
                            locked = accountList[i].Hold
                        };
                        balances.Add(balance);
                    }
                }

                return balances;// GdaxAccountCollectionToBalanceCollection(accountList).ToList();
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Place a Trade
        /// </summary>
        /// <param name="tradeParams">TradeParams object</param>
        /// <returns>TradeResponse object</returns>
        public TradeResponse PlaceTrade(TradeParams tradeParams)
        {
            if(_thisExchange == Exchange.BINANCE)
            {
                var response = _bianceRepo.PostTrade(tradeParams).Result;

                return response;
            }
            else if(_thisExchange == Exchange.GDAX)
            {
                // TODO use new trade api
                if (tradeParams.type == "STOPLOSS")
                {
                    GDAXSharp.Services.Orders.Models.Responses.OrderResponse response = _gdaxRepo.PlaceStopLimit(tradeParams).Result;

                    return GdaxOrderResponseToTradeResponse(response);
                }
                else
                {
                    var gdaxParams = new GDAXTradeParams
                    {
                        price = tradeParams.price,
                        product_id = tradeParams.symbol,
                        side = tradeParams.side.ToLower(),
                        size = tradeParams.quantity,
                        type = "limit"
                    };
                    GDAXOrderResponse response = _gdaxRepo.PlaceRestTrade(gdaxParams).Result;

                    return GdaxRestOrderResponseToTradeResponse(response);
                }

            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Get Order Details
        /// </summary>
        /// <param name="trade">TradeResponse object</param>
        /// <param name="symbol">Trading symbol</param>
        /// <returns>OrderResponse object</returns>
        public OrderResponse GetOrderDetail(TradeResponse trade, string symbol = "")
        {
            if (_thisExchange == Exchange.BINANCE)
            {
                var response = _bianceRepo.GetOrder(symbol, trade.orderId).Result;

                return response;
            }
            else if (_thisExchange == Exchange.GDAX)
            {
                var response = _gdaxRepo.GetRestOrder(trade.clientOrderId).Result;

                return response == null ? null : GdaxOrderResponseToOrderResponse(response);
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Delete a trade
        /// </summary>
        /// <param name="tradeParams">CancelTradeParams object</param>
        /// <returns>TradeResponse object</returns>
        public TradeResponse DeleteTrade(CancelTradeParams tradeParams)
        {
            if (_thisExchange == Exchange.BINANCE)
            {
                var response = _bianceRepo.DeleteTrade(tradeParams).Result;

                return response;
            }
            else if (_thisExchange == Exchange.GDAX)
            {
                var response = _gdaxRepo.CancelAllTradesRest().Result;

                var tradeResponse = new TradeResponse();

                if (response != null)
                {
                    tradeResponse.clientOrderId = response.OrderIds.ToList().FirstOrDefault().ToString();
                }

                return tradeResponse;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Get Latest Buy and Sell orders that were filled
        /// </summary>
        /// <param name="symbol">String of trading symbol</param>
        /// <returns>Array of OrderReponses</returns>
        public OrderResponse[] GetLatestOrders(string symbol)
        {
            if (_thisExchange == Exchange.BINANCE)
            {
                var response = _bianceRepo.GetOrders(symbol).Result;

                var orderReverse = response.Where(o => o.status == OrderStatus.FILLED)
                                           .OrderByDescending(o => o.time).ToArray();

                var orderList = new List<OrderResponse>();

                var buyFound = false;
                var sellFound = false;
                for (int i = 0; i < orderReverse.Length; i++)
                {
                    if(orderReverse[i].side == TradeType.BUY && !buyFound)
                    {
                        orderList.Add(orderReverse[i]);
                        buyFound = true;
                    }
                    if (orderReverse[i].side == TradeType.SELL && !sellFound)
                    {
                        orderList.Add(orderReverse[i]);
                        sellFound = true;
                    }

                    if (buyFound && sellFound)
                    {
                        break;
                    }
                }

                return orderList.ToArray();
            }
            else if(_thisExchange == Exchange.GDAX)
            {
                return null;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Check if open orders exist
        /// </summary>
        /// <param name="symbol">Trading pair to check</param>
        /// <returns>Boolean of result</returns>
        public bool OpenOrdersExist(string symbol)
        {
            if (_thisExchange == Exchange.BINANCE)
            {

                OrderResponse[] response = null;

                while(response == null)
                {
                    response = _bianceRepo.GetOpenOrders(symbol).Result;
                }

                return response.Length > 0 ? true : false;
            }
            else if (_thisExchange == Exchange.GDAX)
            {
                return false;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Get 1st price with the most support
        /// </summary>
        /// <param name="symbol">String of trading pair</param>
        /// <returns>Decimal of price</returns>
        public decimal GetSupport(string symbol)
        {
            if (_thisExchange == Exchange.BINANCE)
            {
                var pair = symbol.Substring(symbol.Length - 4) == "USDT"
                                ? symbol.Substring(symbol.Length - 4)
                                : symbol.Substring(symbol.Length - 3);
                var orderBook = _bianceRepo.GetOrderBook(symbol).Result;

                decimal support = 0.00000000M;
                decimal prevQty = 0.0M;

                for (int i = 0; i < orderBook.bids.Length; i++)
                {
                    support = i == 0 ? orderBook.bids[i].price : support;

                    if (orderBook.bids[i].quantity > prevQty)
                    {
                        support = orderBook.bids[i].price;
                    }

                    if ((pair != "USDT" || pair != "USD")
                        && orderBook.asks[i].price * orderBook.asks[i].quantity >= 0.1M)
                    {
                        break;
                    }
                    else if ((pair == "USDT" || pair == "USD")
                        && orderBook.bids[i].quantity >= 1)
                    {
                        break;
                    }

                    prevQty = orderBook.bids[i].quantity;
                }

                return support;
            }
            else if (_thisExchange == Exchange.GDAX)
            {
                return 0.00000000M;
            }
            else
            {
                return 0.00000000M;
            }
        }

        /// <summary>
        /// Get 1st price with the most resistance
        /// </summary>
        /// <param name="symbol">String of trading pair</param>
        /// <returns>Decimal of price</returns>
        public decimal GetResistance(string symbol)
        {
            if (_thisExchange == Exchange.BINANCE)
            {
                var pair = symbol.Substring(symbol.Length - 4) == "USDT"
                                ? symbol.Substring(symbol.Length - 4)
                                : symbol.Substring(symbol.Length - 3);
                var orderBook = _bianceRepo.GetOrderBook(symbol).Result;

                decimal resistance = 0.00000000M;
                decimal prevQty = 0.0M;

                for (int i = 0; i < orderBook.asks.Length; i++)
                {
                    resistance = i == 0 ? orderBook.asks[i].price : resistance;

                    if (orderBook.asks[i].quantity > prevQty)
                    {
                        resistance = orderBook.asks[i].price;
                    }

                    if ((pair != "USDT" || pair != "USD")
                        && orderBook.asks[i].price * orderBook.asks[i].quantity >= 0.1M)
                    {
                        break;
                    }
                    else if ((pair == "USDT" || pair == "USD")
                        && orderBook.asks[i].quantity >= 1)
                    {
                        break;
                    }

                    prevQty = orderBook.asks[i].quantity;
                }

                return resistance;
            }
            else if (_thisExchange == Exchange.GDAX)
            {
                return 0.00000000M;
            }
            else
            {
                return 0.00000000M;
            }
        }

        /// <summary>
        /// Convert GdaxTrade array to BotStick array
        /// </summary>
        /// <param name="trades">ProductTrade array</param>
        /// <param name="range">Size of array to return</param>
        /// <returns>BotStick array</returns>
        public BotStick[] GetSticksFromGdaxTrades(GdaxTrade[] trades, int range)
        {
            var close = trades[0].Price;
            var maxDate = trades[0].Time;
            maxDate = maxDate.AddSeconds(-maxDate.Second);
            var tradesPrev = trades.Where(t => t.Time < maxDate).OrderByDescending(t => t.Time).ToArray();
            var prevClose = 0.0M;
            if (tradesPrev.Length > 0)
            {
                prevClose = tradesPrev[0].Price;
            }
            var grouped = trades.GroupBy(
                t => new
                {
                    Date = _dtHelper.LocalToUnixTime(t.Time.AddSeconds(-t.Time.Second).AddMilliseconds(-t.Time.Millisecond))
                })
                .Select(
                t => new BotStick
                {
                    closeTime = t.Max(m => _dtHelper.LocalToUnixTime(m.Time.AddSeconds(-m.Time.Second).AddMilliseconds(-m.Time.Millisecond))),
                    high = t.Max(m => m.Price),
                    low = t.Min(m => m.Price),
                    volume = t.Sum(m => m.Size)
                }).ToList();

            grouped[0].close = close;
            if (grouped.Count > 1)
            {
                grouped[1].close = prevClose;
            }

            int size = grouped.Count() < range ? grouped.Count() : range;

            var groupedArray = grouped.Take(size).ToArray();

            Array.Reverse(groupedArray);

            return groupedArray;
        }

        /// <summary>
        /// Convert Binance Candlestick array to BotStick array
        /// </summary>
        /// <param name="binanceArray">Binance Candlestick array</param>
        /// <returns>BotStick array</returns>
        private BotStick[] BinanceStickToBotStick(Candlestick[] binanceArray)
        {
            return this._helper.MapEntity<Candlestick[], BotStick[]>(binanceArray);
        }

        /// <summary>
        /// Convert GDAX Candle array to BotStick array
        /// </summary>
        /// <param name="gdaxArray">GDAX Candle array</param>
        /// <returns>BotStick array</returns>
        private BotStick[] GdaxStickToBotStick(Candle[] gdaxArray)
        {
            return this._helper.MapEntity<Candle[], BotStick[]>(gdaxArray);
        }

        /// <summary>
        /// Convert GDAX OrderResponse to TradeResponse
        /// </summary>
        /// <param name="response">GDAX OrderReponse object</param>
        /// <returns>TradeReponse object</returns>
        private TradeResponse GdaxOrderResponseToTradeResponse(GDAXSharp.Services.Orders.Models.Responses.OrderResponse response)
        {
            if(response == null)
            {
                TradeResponse nullResponse = null;

                return nullResponse;
            }

            TradeType tradeType;
            Enum.TryParse(response.Side.ToString(), out tradeType);
            OrderStatus orderStatus;
            Enum.TryParse(response.Status.ToString(), out orderStatus);
            TimeInForce tif;
            Enum.TryParse(response.TimeInForce.ToString(), out tif);
            OrderType orderType;
            Enum.TryParse(response.OrderType.ToString(), out orderType);

            var tradeResponse = new TradeResponse
            {
                clientOrderId = response.Id.ToString(),
                executedQty = response.ExecutedValue,
                orderId = 0,
                origQty = response.Size,
                price = response.Price,
                side = tradeType,
                status = orderStatus,
                symbol = response.ProductId.ToString(),
                timeInForce = tif,
                transactTime = _dtHelper.LocalToUnixTime(response.CreatedAt),
                type = orderType
            };

            return tradeResponse;
        }

        /// <summary>
        /// Convert GDAX OrderResponse to TradeResponse
        /// </summary>
        /// <param name="response">GDAX OrderReponse object</param>
        /// <returns>TradeReponse object</returns>
        private TradeResponse GdaxRestOrderResponseToTradeResponse(GDAXOrderResponse response)
        {
            if (response == null)
            {
                TradeResponse nullResponse = null;

                return nullResponse;
            }

            TradeType tradeType;
            Enum.TryParse(response.side, out tradeType);
            OrderStatus orderStatus;
            Enum.TryParse(response.status, out orderStatus);
            TimeInForce tif;
            Enum.TryParse(response.time_in_force.ToString(), out tif);
            OrderType orderType;
            Enum.TryParse(response.type, out orderType);

            var tradeResponse = new TradeResponse
            {
                clientOrderId = response.id,
                executedQty = response.filled_size,
                orderId = 0,
                origQty = response.size,
                price = response.price,
                side = tradeType,
                status = orderStatus,
                symbol = response.product_id,
                timeInForce = tif,
                transactTime = _dtHelper.LocalToUnixTime(response.created_at.UtcDateTime),
                type = orderType
            };

            return tradeResponse;
        }

        /// <summary>
        /// Convert GDAXOrder object to OrderResponse
        /// </summary>
        /// <param name="response">GDAXOrder object</param>
        /// <returns>OrderReponse object</returns>
        private OrderResponse GdaxOrderResponseToOrderResponse(GDAXOrder response)
        {
            if (response.id == null)
            {
                return null;
            }
            TradeType tradeType;
            Enum.TryParse(response.side.ToString().ToUpper(), out tradeType);
            OrderStatus orderStatus;
            Enum.TryParse(response.status.ToString().ToUpper(), out orderStatus);
            OrderType orderType;
            Enum.TryParse(response.type.ToString().ToUpper(), out orderType);

            var orderReponse = new OrderResponse
            {
                clientOrderId = response.id,
                executedQty = response.fill_size,
                origQty = response.size,
                price = response.executed_value,
                side = tradeType,
                status = orderStatus,
                symbol = response.product_id,
                time = _dtHelper.LocalToUnixTime(response.crated_at.UtcDateTime),
                type = orderType
            };

            return orderReponse;
        }

        /// <summary>
        /// Convert GDAX CancelOrderResponse to TradeResponse
        /// </summary>
        /// <param name="response">GDAX CancelOrderResponse object</param>
        /// <returns>TradeReponse object</returns>
        private TradeResponse GdaxCancelOrderResponseToTradeResponse(GDAXSharp.Services.Orders.Models.Responses.CancelOrderResponse response)
        {
            var tradeResponse = new TradeResponse
            {
                clientOrderId = response.OrderIds.First().ToString(),
                transactTime = _dtHelper.UTCtoUnixTime()
            };

            return tradeResponse;
        }

        /// <summary>
        /// Convert GDAX Account collection to Balance collection
        /// </summary>
        /// <param name="response">GDAX Account collection</param>
        /// <returns>Balance collection</returns>
        private IEnumerable<Balance> GdaxAccountCollectionToBalanceCollection(IEnumerable<GDAXSharp.Services.Accounts.Models.Account> accountList)
        {
            return _helper.MapEntity<IEnumerable<GDAXSharp.Services.Accounts.Models.Account>, IEnumerable<Balance>>(accountList);
        }

        /// <summary>
        /// Convert GDAX Account collection to Balance collection
        /// </summary>
        /// <param name="response">GDAX Account collection</param>
        /// <returns>Balance collection</returns>
        private IEnumerable<Balance> GdaxAccountArrayToBalanceCollection(GDAXAccount[] accountList)
        {
            return _helper.MapEntity<IEnumerable<GDAXAccount>, IEnumerable<Balance>>(accountList.ToList());
        }
    }
}
