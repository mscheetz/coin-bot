using CoinBot.Business.Builders.Interface;
using CoinBot.Business.Entities;
using CoinBot.Business.Entities.KuCoinEntities;
using CoinBot.Core;
using CoinBot.Data;
using CoinBot.Data.Interface;
using GDAXSharp.Services.Products.Models;
using GDAXSharp.Services.Products.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoinBot.Business.Builders
{
    public class ExchangeBuilder : IExchangeBuilder
    {
        private Exchange _thisExchange;
        private IBinanceRepository _bianceRepo;
        private IGdaxRepository _gdaxRepo;
        private IKuCoinRepository _kuRepo;
        private Helper _helper;
        private DateTimeHelper _dtHelper;
        private bool _competition;
        private string _symbol;
        private string _pair = string.Empty;
        private string _asset = string.Empty;

        public ExchangeBuilder()
        {
            _bianceRepo = new BinanceRepository();
            _gdaxRepo = new GdaxRepository();
            _kuRepo = new KuCoinRepository();
            _helper = new Helper();
            _dtHelper = new DateTimeHelper();
        }

        public ExchangeBuilder(IBinanceRepository binanceRepo)
        {
            _bianceRepo = binanceRepo;
            _gdaxRepo = new GdaxRepository();
            _kuRepo = new KuCoinRepository();
            _helper = new Helper();
            _dtHelper = new DateTimeHelper();
        }

        public ExchangeBuilder(IGdaxRepository gdaxRepo)
        {
            _bianceRepo = new BinanceRepository();
            _kuRepo = new KuCoinRepository();
            _gdaxRepo = gdaxRepo;
            _helper = new Helper();
            _dtHelper = new DateTimeHelper();
        }

        public ExchangeBuilder(IKuCoinRepository kuRepo)
        {
            _bianceRepo = new BinanceRepository();
            _gdaxRepo = new GdaxRepository();
            _kuRepo = kuRepo;
            _helper = new Helper();
            _dtHelper = new DateTimeHelper();
        }

        public ExchangeBuilder(IBinanceRepository binanceRepo, IGdaxRepository gdaxRepo, IKuCoinRepository kuRepo)
        {
            _bianceRepo = binanceRepo;
            _gdaxRepo = gdaxRepo;
            _kuRepo = kuRepo;
            _helper = new Helper();
            _dtHelper = new DateTimeHelper();
        }

        /// <summary>
        /// Set BotSettings
        /// </summary>
        /// <param name="settings">BotSettings Object</param>
        public void SetExchange(BotSettings settings)
        {
            _symbol = settings.tradingPair;
            _thisExchange = settings.exchange;
            _competition = settings.tradingCompetition;
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
            else if (_thisExchange == Exchange.KUCOIN)
            {
                return _kuRepo.ValidateExchangeConfigured();
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
            else if (_thisExchange == Exchange.KUCOIN)
            {
                return _kuRepo.SetExchangeApi(apiInfo);
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
            int i = 0;
            if (_thisExchange == Exchange.BINANCE)
            {
                Candlestick[] sticks = null;

                while (sticks == null && i < 3)
                {
                    i++;
                    sticks = _bianceRepo.GetCandlestick(symbol, interval, range).Result;
                }

                return BinanceStickToBotStick(sticks);
            }
            else if (_thisExchange == Exchange.GDAX)
            {
                var trades = _gdaxRepo.GetTrades(symbol).Result;

                while (trades == null)
                {
                    trades = _gdaxRepo.GetTrades(symbol).Result;
                }

                return GetSticksFromGdaxTrades(trades, range);
            }
            else if (_thisExchange == Exchange.KUCOIN)
            {
                range = 10;
                var values = _kuRepo.GetCandlesticks(symbol, interval, range).Result;
                while ((values == null || values.close.Length == 0) && i < 3)
                {
                    i++;
                    values = _kuRepo.GetCandlesticks(symbol, interval, range).Result;
                }
                return values.close.Length == 0 ? new BotStick[0] : KuCoinChartValueArrayToBotStickArray(values);
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
        public List<Entities.Balance> GetBalance(string asset, string pair)
        {
            int i = 0;
            if (_thisExchange == Exchange.BINANCE)
            {
                var account = _bianceRepo.GetBalance().Result;

                if (account == null || account.balances == null)
                {
                    return new List<Entities.Balance>();
                }

                return account.balances.Where(b => b.asset.Equals(asset) || b.asset.Equals(pair)).ToList();
            }
            else if (_thisExchange == Exchange.GDAX)
            {
                var accountList = _gdaxRepo.GetBalance().Result.ToArray();

                var balances = new List<Entities.Balance>();

                for (i = 0; i < accountList.Count(); i++)
                {
                    if (accountList[i].Currency.ToString().Equals(asset)
                        || accountList[i].Currency.ToString().Equals(pair))
                    {
                        var balance = new Entities.Balance
                        {
                            asset = accountList[i].Currency.ToString(),
                            free = accountList[i].Available,
                            locked = accountList[i].Hold
                        };
                        balances.Add(balance);
                    }
                }

                return balances;
            }
            else if (_thisExchange == Exchange.KUCOIN)
            {
                var accountList = _kuRepo.GetBalance().Result;

                while(accountList == null && i < 3)
                {
                    accountList = _kuRepo.GetBalance().Result;
                    i++;
                }

                var balances = new List<Entities.Balance>();

                if(accountList == null)
                {
                    return balances;
                }

                accountList = accountList.ToArray();

                for (i = 0; i < accountList.Count(); i++)
                {
                    if (accountList[i].coinType.ToString().Equals(asset)
                        || accountList[i].coinType.ToString().Equals(pair))
                    {
                        var balance = new Entities.Balance
                        {
                            asset = accountList[i].coinType.ToString(),
                            free = accountList[i].balance,
                            locked = accountList[i].freezeBalance
                        };
                        balances.Add(balance);
                    }
                }

                return balances;
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
            if (_thisExchange == Exchange.BINANCE)
            {
                var response = _bianceRepo.PostTrade(tradeParams).Result;

                return response;
            }
            else if (_thisExchange == Exchange.GDAX)
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
            else if (_thisExchange == Exchange.KUCOIN)
            {
                var response = _kuRepo.PostTrade(tradeParams).Result;

                if (response.msg.Equals("Operation succeeded.") || response.msg.Equals("OK"))
                {
                    var tradeResponse = new TradeResponse
                    {
                        clientOrderId = response.data["orderOid"],
                        origQty = tradeParams.quantity,
                        price = tradeParams.price,
                        side = (TradeType)Enum.Parse(typeof(TradeType), tradeParams.side),
                        symbol = tradeParams.symbol,
                        type = (OrderType)Enum.Parse(typeof(OrderType), tradeParams.type)
                    };
                    return tradeResponse;
                }
                return null;
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
            else if (_thisExchange == Exchange.KUCOIN)
            {
                var response = _kuRepo.GetOrder(trade.symbol, trade.side, trade.orderId).Result;

                return response == null ? null : KuCoinOrderListDetailToOrderResponse(response, trade.symbol);
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
            else if (_thisExchange == Exchange.KUCOIN)
            {
                var response = _kuRepo.DeleteTrade(tradeParams.symbol, tradeParams.origClientOrderId, tradeParams.type).Result;

                var tradeResponse = new TradeResponse();

                if(response.code.Equals("OK"))
                {
                    tradeResponse.clientOrderId = tradeParams.origClientOrderId;
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
            var response = GetOrders(symbol);
            int i = 0;
            while (response == null && i < 3)
            {
                response = GetOrders(symbol);
                i++;
            }

            if (response == null)
            {
                    return null;
            }

            var orderReverse = response.Where(o => o.status == OrderStatus.FILLED)
                                        .OrderByDescending(o => o.time).ToArray();

            var orderList = new List<OrderResponse>();

            var buyFound = false;
            var sellFound = false;
            for (i = 0; i < orderReverse.Length; i++)
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

        /// <summary>
        /// Get orders for a pair
        /// </summary>
        /// <param name="symbol">String of symbol</param>
        /// <returns>OrderResponse array</returns>
        public OrderResponse[] GetOrders(string symbol)
        {
            int i = 0;
            if (_thisExchange == Exchange.BINANCE)
            {
                var binanceOrders = _bianceRepo.GetOrders(symbol).Result;

                while (binanceOrders == null && i < 3)
                {
                    binanceOrders = _bianceRepo.GetOrders(symbol).Result;
                    i++;
                }

                return binanceOrders;
            }
            else if (_thisExchange == Exchange.GDAX)
            {
                return null;
            }
            else if (_thisExchange == Exchange.KUCOIN)
            {
                var kuOrders = _kuRepo.GetOrders(symbol).Result;

                while (kuOrders == null && i < 3)
                {
                    kuOrders = _kuRepo.GetOrders(symbol).Result;
                    i++;
                }

                return KuCoinOrderListDetailToOrderResponseArray(kuOrders, symbol);
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
        /// <returns>Nullable decimal of open price</returns>
        public decimal? OpenOrdersExist(string symbol)
        {
            OrderResponse[] response = null;
            int i = 0;
            if (_thisExchange == Exchange.BINANCE)
            {
                while(response == null)
                {
                    response = _bianceRepo.GetOpenOrders(symbol).Result;
                }
            }
            else if (_thisExchange == Exchange.GDAX)
            {
                return null;
            }
            else if (_thisExchange == Exchange.KUCOIN)
            {
                while (response == null)
                {
                    var kuOrders = _kuRepo.GetOpenOrders(symbol).Result;

                    while (kuOrders == null && i < 3)
                    {
                        kuOrders = _kuRepo.GetOpenOrders(symbol).Result;
                        i++;
                    }

                    if(kuOrders != null)
                        response = KuCoinOrderResponseToOrderResponse(kuOrders);
                }
            }
            else
            {
                return null;
            }

            return response != null && response.Length > 0 ? (decimal?)response[0].price : null;
        }

        /// <summary>
        /// Get open orders
        /// </summary>
        /// <param name="symbol">Symbol to check</param>
        /// <returns>Array of OrderResponse objects</returns>
        public OrderResponse[] GetOpenOrders(string symbol)
        {
            OrderResponse[] response = null;
            int i = 0;
            if (_thisExchange == Exchange.BINANCE)
            {
                while (response == null)
                {
                    response = _bianceRepo.GetOpenOrders(symbol).Result;
                }
            }
            else if (_thisExchange == Exchange.GDAX)
            {
            }
            else if (_thisExchange == Exchange.KUCOIN)
            {
                while(response == null)
                {
                    var kuOrders = _kuRepo.GetOpenOrders(symbol).Result;

                    while (kuOrders == null && i < 3)
                    {
                        kuOrders = _kuRepo.GetOpenOrders(symbol).Result;
                        i++;
                    }

                    if(kuOrders != null)
                        response = KuCoinOrderResponseToOrderResponse(kuOrders);
                }
            }
            else
            {
            }

            return response;
        }

        /// <summary>
        /// Get order book position of a price
        /// </summary>
        /// <param name="symbol">String of trading symbol</param>
        /// <param name="price">Decimal of price to find</param>
        /// <returns>Int of position</returns>
        public int? GetPricePosition(string symbol, decimal price)
        {
            int i = 0;
            var orderBook = GetOrderBook(symbol);

            while (orderBook == null && i < 3)
            {
                Task.WaitAll(Task.Delay(1000));
                orderBook = GetOrderBook(symbol);
                i++;
            }

            if (price >= orderBook.asks[0].price)
            {
                for (i = 0; i < orderBook.asks.Length; i++)
                {
                    if(price <= orderBook.asks[i].price)
                    {
                        return i;
                    }
                }
            }
            else if (price <= orderBook.bids[0].price)
            {
                for (i = 0; i < orderBook.bids.Length; i++)
                {
                    if (price >= orderBook.bids[i].price)
                    {
                        return i;
                    }

                }
            }
            return null;
        }

        /// <summary>
        /// Get 1st price with the most support at or above specified volume
        /// </summary>
        /// <param name="symbol">String of trading pair</param>
        /// <param name="volume">Volume to set buy price</param>
        /// <returns>Decimal of price</returns>
        public OrderBookDetail GetSupport(string symbol, decimal volume)
        {
            decimal support = 0.00000000M;
            int i = 0;
            int precision = 0;
            OrderBookDetail response;
            //var pair = symbol.Substring(symbol.Length - 4) == "USDT"
            //                ? symbol.Substring(symbol.Length - 4)
            //                : symbol.Substring(symbol.Length - 3);
            var orderBook = GetOrderBook(symbol);

            while (orderBook == null && i < 3)
            {
                Task.WaitAll(Task.Delay(1000));
                orderBook = GetOrderBook(symbol);
                i++;
            }

            if (_competition)
            {
                var obPrice = orderBook.bids[i].price;
                var trimedPrice = obPrice.ToString().TrimEnd('0');
                var price = Convert.ToDecimal(trimedPrice);
                var thisPrecision = BitConverter.GetBytes(decimal.GetBits(price)[3])[2];
                response = new OrderBookDetail
                {
                    price = price,
                    precision = thisPrecision,
                    position = 0
                };

                return response;
            }
            var staleMate = StaleMateCheck(orderBook.bids.Take(2).ToArray(), orderBook.asks.Take(2).ToArray(), volume);

            if (!staleMate)
            {
                for (i = 0; i < orderBook.bids.Length; i++)
                {
                    var obPrice = orderBook.bids[i].price;
                    var trimedPrice = obPrice.ToString().TrimEnd('0');
                    var price = Convert.ToDecimal(trimedPrice);
                    var thisPrecision = BitConverter.GetBytes(decimal.GetBits(price)[3])[2];
                    precision = thisPrecision > precision ? thisPrecision : precision;
                    var vol = orderBook.bids[i].price * orderBook.bids[i].quantity;

                    if (vol >= volume)
                    {
                        support = price;
                        break;
                    }
                }
            }
            response = new OrderBookDetail
            {
                price = support,
                precision = precision,
                position = i
            };

            return response;
    }

        /// <summary>
        /// Get 1st price with the most resistance at or above specified volume
        /// </summary>
        /// <param name="symbol">String of trading pair</param>
        /// <param name="volume">Volume to set buy price</param>
        /// <returns>Decimal of price</returns>
        public OrderBookDetail GetResistance(string symbol, decimal volume)
        {
            decimal resistance = 0.00000000M;
            OrderBookDetail response;
            int i = 0;
            int precision = 0;
            var orderBook = GetOrderBook(symbol);
            while (orderBook == null && i < 3)
            {
                Task.WaitAll(Task.Delay(1000));
                orderBook = GetOrderBook(symbol);
                i++;
            }

            if(_competition)
            {
                var obPrice = orderBook.asks[i].price;
                var trimedPrice = obPrice.ToString().TrimEnd('0');
                var price = Convert.ToDecimal(trimedPrice);
                var thisPrecision = BitConverter.GetBytes(decimal.GetBits(price)[3])[2];
                response = new OrderBookDetail
                {
                    price = price,
                    precision = thisPrecision,
                    position = 0
                };

                return response;
            }

            var staleMate = StaleMateCheck(orderBook.bids.Take(2).ToArray(), orderBook.asks.Take(2).ToArray(), volume);

            if (!staleMate)
            {
                for (i = 0; i < orderBook.asks.Length; i++)
                {
                    var obPrice = orderBook.asks[i].price;
                    var trimedPrice = obPrice.ToString().TrimEnd('0');
                    var price = Convert.ToDecimal(trimedPrice);
                    var thisPrecision = BitConverter.GetBytes(decimal.GetBits(price)[3])[2];
                    precision = thisPrecision > precision ? thisPrecision : precision;
                    var vol = orderBook.asks[i].price * orderBook.asks[i].quantity;

                    if (vol >= volume)
                    {
                        resistance = orderBook.asks[i].price;
                        break;
                    }
                }
            }
            response = new OrderBookDetail
            {
                price = resistance,
                precision = precision,
                position = i
            };

            return response;
        }

        /// <summary>
        /// Compare 1st 2 BinanceOrder objects, if one or both at volume, stale mate is reached
        /// </summary>
        /// <param name="buys">Top Buy orders</param>
        /// <param name="sells">Bottom Sell orders</param>
        /// <param name="volume">Volume to trigger</param>
        /// <returns>Boolean if stale mate reached</returns>
        public bool StaleMateCheck(BinanceOrders[] buys, BinanceOrders[] sells, decimal volume)
        {
            if(_competition)
            {
                return false;
            }
            else if ((buys[0].price * buys[0].quantity >= volume 
                || buys[1].price * buys[1].quantity >= volume)
                && (sells[0].price * sells[0].quantity >= volume
                || sells[1].price * sells[1].quantity >= volume))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Get OrderBook
        /// </summary>
        /// <param name="symbol">Symbol of orderbook</param>
        /// <returns>OrderBook object</returns>
        public Entities.OrderBook GetOrderBook(string symbol)
        {
            Entities.OrderBook orderBook = null;
            int i = 0;
            if (_thisExchange == Exchange.BINANCE)
            {
                orderBook = _bianceRepo.GetOrderBook(symbol).Result;

                while (orderBook == null && i < 3)
                {
                    orderBook = _bianceRepo.GetOrderBook(symbol).Result;
                    i++;
                }                    
            }
            else if(_thisExchange == Exchange.GDAX)
            {
                var gdaxBook = _gdaxRepo.GetOrderBook(symbol).Result;
            }
            else if(_thisExchange == Exchange.KUCOIN)
            {
                var kuBook = _kuRepo.GetOrderBook(symbol).Result;

                while (kuBook == null && i < 3)
                {
                    kuBook = _kuRepo.GetOrderBook(symbol).Result;
                    i++;
                }

                if(kuBook !=null)
                    orderBook = KuCoinOrderBookToOrderBook(kuBook);
            }

            return orderBook;

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

        private OrderResponse[] KuCoinOrderResponseToOrderResponse(OpenOrderResponse kuOrders)
        {
            var buys = KuCoinOpenOrderDetailToOrderReponse(kuOrders.openBuys, TradeType.BUY);
            var sells = KuCoinOpenOrderDetailToOrderReponse(kuOrders.openSells, TradeType.SELL);

            var orderResponses = new OrderResponse[buys.Length + sells.Length];
            buys.CopyTo(orderResponses, 0);
            sells.CopyTo(orderResponses, buys.Length);

            return orderResponses;
        }

        private OrderResponse[] KuCoinOpenOrderDetailToOrderReponse(OpenOrderDetail[] openOrderDetails, TradeType tradeType)
        {
            var orderResponseList = new List<OrderResponse>();

            for (int i = 0; i < openOrderDetails.Length; i++)
            {
                var orderResponse = new OrderResponse
                {
                    clientOrderId = openOrderDetails[i].orderId,
                    executedQty = openOrderDetails[i].filledQuantity,
                    origQty = openOrderDetails[i].quantity,
                    price = openOrderDetails[i].price,
                    side = tradeType,
                    symbol = _symbol,
                    time = openOrderDetails[i].timestamp
                };
                orderResponseList.Add(orderResponse);
            }

            return orderResponseList.ToArray();
        }

        private Entities.OrderBook KuCoinOrderBookToOrderBook(OrderBookResponse orderBookResponse)
        {
            var orderBook = new Entities.OrderBook
            {
                lastUpdateId = orderBookResponse.timestamp,
                asks = KuCoinOrdersToBinanceOrders(orderBookResponse.sells),
                bids = KuCoinOrdersToBinanceOrders(orderBookResponse.buys)
            };

            return orderBook;
        }

        private BotStick[] KuCoinChartValueArrayToBotStickArray(ChartValue values)
        {
            var botStickList = new List<BotStick>();

            int i = values.close.Length;

            for (int j = 0; j < i; j++)
            {
                var iVal = j;
                var volume = values.volume[iVal];
                var close = values.close[j];
                if(close == 0)
                {
                    var k = j;
                    while (k < (i - 1))
                    {
                        k++;
                        close = values.close[k];
                        if (k < (i - 1) && close != 0)
                        {
                            iVal = k;
                        }
                    }
                    if (close == 0 && j > 0)
                    {
                        k = j - 1;
                        close = values.close[k];
                        if (close != 0)
                        {
                            iVal = k;
                        }
                    }
                    volume = 0M;
                }
                if (values.close != null)
                {
                    var botStick = new BotStick
                    {
                        close = values.close[iVal],
                        closeTime = values.timestamp[iVal],
                        high = values.high[iVal],
                        low = values.low[iVal],
                        open = values.open[iVal],
                        volume = volume
                    };

                    botStickList.Add(botStick);
                }
            }

            return botStickList.ToArray();
        }

        private BinanceOrders[] KuCoinOrdersToBinanceOrders(Entities.KuCoinEntities.OrderBook[] kuOrders)
        {
            var orderList = new List<BinanceOrders>();

            for (int i = 0; i < kuOrders.Length; i++)
            {
                var order = new BinanceOrders
                {
                    //ignore = kuOrders[i].pairTotal,
                    price = kuOrders[i].price,
                    quantity = kuOrders[i].quantity
                };

                orderList.Add(order);
            }

            return orderList.ToArray();
        }

        private OrderResponse KuCoinOrderListDetailToOrderResponse(OrderListDetail order, string symbol)
        {
            var response = new OrderResponse
            {
                clientOrderId = order.oid,
                executedQty = order.amount,
                orderId = order.id,
                price = order.dealPrice,
                side = order.direction.Equals("BUY") ? TradeType.BUY : TradeType.SELL,
                status = OrderStatus.FILLED,
                symbol = symbol,
                time = order.createdAt,
                type = OrderType.LIMIT
            };

            return response;
        }

        private OrderResponse[] KuCoinOrderListDetailToOrderResponseArray(OrderListDetail[] orderListDetails, string symbol)
        {
            var responses = new List<OrderResponse>();
            for (int i = 0; i < orderListDetails.Length; i++)
            {
                var order = orderListDetails[i];

                var response = KuCoinOrderListDetailToOrderResponse(order, symbol);

                responses.Add(response);
            }
            
            return responses.ToArray();
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
        private IEnumerable<Entities.Balance> GdaxAccountCollectionToBalanceCollection(IEnumerable<GDAXSharp.Services.Accounts.Models.Account> accountList)
        {
            return _helper.MapEntity<IEnumerable<GDAXSharp.Services.Accounts.Models.Account>, IEnumerable<Entities.Balance>>(accountList);
        }

        /// <summary>
        /// Convert GDAX Account collection to Balance collection
        /// </summary>
        /// <param name="response">GDAX Account collection</param>
        /// <returns>Balance collection</returns>
        private IEnumerable<Entities.Balance> GdaxAccountArrayToBalanceCollection(GDAXAccount[] accountList)
        {
            return _helper.MapEntity<IEnumerable<GDAXAccount>, IEnumerable<Entities.Balance>>(accountList.ToList());
        }
    }
}
