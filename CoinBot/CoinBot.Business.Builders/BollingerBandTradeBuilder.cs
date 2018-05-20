using CoinBot.Business.Builders.Interface;
using CoinBot.Business.Entities;
using CoinBot.Core;
using CoinBot.Data;
using CoinBot.Data.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace CoinBot.Business.Builders
{
    public class BollingerBandTradeBuilder : IBollingerBandTradeBuilder
    {
        private IBinanceRepository _repo;
        private IFileRepository _fileRepo;
        private DateTimeHelper _dtHelper = new DateTimeHelper();
        private Helper _helper = new Helper();
        private const int candlestickCount = 21;
        private BotSettings botSettings;
        private string _symbol;
        private bool currentlyTrading;
        private List<Bag> bags;
        private List<TradeInformation> tradeInformation;
        private TradeInformation lastTrade;
        private List<OpenOrder> openOrderList;
        private List<OpenStopLoss> openStopLossList;
        private List<BotBalance> botBalances;
        private decimal lastBuy = 0.00000000M;
        private decimal lastSell = 0.00000000M;
        private int tradeNumber = 0;
        private TradeType tradeType;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="repo">Repository interface</param>
        public BollingerBandTradeBuilder()
        {
            _repo = new BinanceRepository();
            _fileRepo = new FileRepository();
            SetupBuilder();
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="repo">Repository interface</param>
        public BollingerBandTradeBuilder(IBinanceRepository repo, IFileRepository fileRepo)
        {
            _repo = repo;
            _fileRepo = fileRepo;
            SetupBuilder();
        }

        private void SetupBuilder()
        {
            botSettings = _fileRepo.GetSettings();
            _symbol = botSettings.tradingPair;
            bags = new List<Bag>();
            tradeInformation = new List<TradeInformation>();
            openOrderList = new List<OpenOrder>();
            openStopLossList = new List<OpenStopLoss>();
            botBalances = new List<BotBalance>();
            currentlyTrading = botSettings.startBotAutomatically == null 
                                ? false 
                                : (bool)botSettings.startBotAutomatically;

            //if(botSettings.tradingStrategy == Strategy.BollingerBands)
            //    RunBot(Interval.FiveM);
        }

        /// <summary>
        /// Set BotSettings
        /// </summary>
        /// <param name="settings">Updated Bot Settings</param>
        /// <returns>Boolean when complete</returns>
        public bool SetBotSettings(BotSettings settings)
        {
            botSettings = settings;
            _symbol = botSettings.tradingPair;

            return true;
        }

        /// <summary>
        /// Start trading
        /// </summary>
        /// <param name="interval">Candlestick interval</param>
        public void StartTrading(Interval interval)
        {
            currentlyTrading = true;

            RunBot(interval);
        }

        /// <summary>
        /// Stop Trading
        /// </summary>
        public void StopTrading()
        {
            currentlyTrading = false;
        }

        /// <summary>
        /// Get 21 day Simple Moving Average
        /// </summary>
        /// <returns>decimal of SMA</returns>
        public decimal Get21DaySMA()
        {
            decimal sma = 0;

            var candlesticks = GetCandlesticks(Interval.OneD, 21);

            sma = candlesticks.Average(c => c.close);

            return sma;
        }

        /// <summary>
        /// Set up respository
        /// </summary>
        /// <returns>Boolean when complete</returns>
        private bool SetupRepository()
        {
            var repoReady = _repo.ValidateExchangeConfigured();

            if (repoReady)
                return true;

            var apiInfo = GetApiInformation();

            _repo.SetExchangeApi(apiInfo);

            return true;
        }

        /// <summary>
        /// Get ApiInformation from config
        /// </summary>
        /// <returns>ApiInformation for repository</returns>
        private ApiInformation GetApiInformation()
        {
            var config = _fileRepo.GetConfig();

            var apiInfo = new ApiInformation
            {
                apiKey = config.apiKey,
                apiSecret = config.apiSecret
            };

            return apiInfo;
        }

        /// <summary>
        /// Run Trading Bot
        /// </summary>
        /// <param name="interval">Candlestick Interval</param>
        private void RunBot(Interval interval)
        {
            SetupRepository();
            tradeType = TradeType.BUY;
            var currentStick = new Candlestick();
            var previousStick = new Candlestick();

            if (botSettings.tradingStatus == TradeStatus.PaperTrading)
                SetPaperBalance();

            while (currentlyTrading)
            {
                Task.WaitAll(Task.Delay(botSettings.priceCheck));
                var bbs = GetBollingerBands(interval);
                int i = bbs.Length - 1;
                currentStick = bbs[i];

                if (previousStick.close == 0)
                    previousStick = bbs[i];
                var tradeMade = false;

                if(StoppedOutCheck(currentStick.close))
                {
                    tradeType = TradeType.BUY;
                }

                if (tradeType == TradeType.BUY)
                {
                    if (currentStick.bollingerBand.bottomBand > currentStick.close)
                    {
                        BuyCrypto(currentStick.close);
                        tradeMade = true;
                    }
                }
                else if (tradeType == TradeType.SELL)
                {
                    if(currentStick.bollingerBand.topBand < currentStick.close 
                        && lastSell < currentStick.close 
                        && GetPercent(currentStick.close) >= botSettings.sellPercent)
                    {
                        SellCrypto(currentStick.close);
                        tradeMade = true;
                    }
                }
                if (tradeMade)
                {
                    UpdateBalances();
                }
                previousStick = currentStick;
            }
        }

        /// <summary>
        /// Calculate percent difference from current price and last buy
        /// </summary>
        /// <param name="currentPrice">Current price</param>
        /// <returns>double of percent difference</returns>
        private double GetPercent(decimal currentPrice)
        {
            return (double)(currentPrice / lastBuy) - 1;
        }

        /// <summary>
        /// Update balances from exchange
        /// </summary>
        private void UpdateBalances()
        {
            botBalances = new List<BotBalance>();

            var balances = GetBalances();

            for (var i = 0; i < balances.Count; i++)
            {
                var botBalance = new BotBalance
                {
                    symbol = balances[i].asset,
                    quantity = balances[i].free + balances[i].locked,
                    timestamp = DateTime.UtcNow
                };

                botBalances.Add(botBalance);
            }

            LogBalances();
        }

        /// <summary>
        /// Set paper balances on load
        /// </summary>
        private void SetPaperBalance()
        {
            botBalances = new List<BotBalance>();

            var balances = GetPaperBalances(botSettings.startingAmount);

            for (var i = 0; i < balances.Count; i++)
            {
                var botBalance = new BotBalance
                {
                    symbol = balances[i].asset,
                    quantity = balances[i].free + balances[i].locked,
                    timestamp = DateTime.UtcNow
                };

                botBalances.Add(botBalance);
            }

            LogBalances();
        }

        /// <summary>
        /// Check if Stop Loss Hit
        /// </summary>
        /// <param name="currentPrice">Current price of coin</param>
        /// <returns>Boolean value</returns>
        private bool StoppedOutCheck(decimal currentPrice)
        {
            if (openStopLossList.Count == 0 || currentPrice >= openStopLossList[0].price)
                return false;

            var stoppedOut = CheckTradeStatus(openStopLossList[0].orderId);

            if(stoppedOut)
            {
                lastSell = openStopLossList[0].price;
                openStopLossList.RemoveAt(0);
            }

            return stoppedOut;
        }

        /// <summary>
        /// Buy crypto
        /// </summary>
        /// <param name="orderPrice">Buy price</param>
        private void BuyCrypto(decimal orderPrice)
        {
            var trade = MakeTrade(orderPrice);

            bool tradeComplete = false;
            while(!tradeComplete)
            {
                tradeComplete = CheckTradeStatus(trade.orderId);
            }
            lastBuy = orderPrice;

            lastTrade = new TradeInformation
            {
                pair = _symbol,
                price = orderPrice,
                quantity = trade.origQty,
                timestamp = _dtHelper.UnixTimeToUTC(trade.transactTime),
                tradeType = EnumHelper.GetEnumDescription((TradeType)tradeType)
            };
            tradeInformation.Add(lastTrade);

            LogTransaction(lastTrade);

            var stopLoss = PlaceStopLoss(orderPrice, trade.origQty);

            openStopLossList.Add(
                new OpenStopLoss
                {
                    symbol = _symbol,
                    clientOrderId = stopLoss.clientOrderId,
                    orderId = stopLoss.orderId,
                    price = stopLoss.price
                });

            tradeType = TradeType.SELL;
        }

        /// <summary>
        /// Check status of placed trade
        /// </summary>
        /// <param name="orderId">OrderId of trade</param>
        /// <returns>Boolean value of filled status</returns>
        private bool CheckTradeStatus(long orderId)
        {
            var orderStatus = GetOrderStatus(orderId);

            return orderStatus.status == OrderStatus.FILLED ? true : false;
        }

        /// <summary>
        /// Get quantity to trade based
        /// </summary>
        /// <param name="orderPrice">Requested trade price</param>
        /// <returns>decimal of quantity to purchase</returns>
        private decimal GetTradeQuantity(decimal orderPrice)
        {
            decimal quantity = 0;
            if (tradeType == TradeType.BUY)
            {
                var pairBalance = botBalances.Where(b => b.symbol.Equals("BTC")).FirstOrDefault();

                quantity = pairBalance.quantity / orderPrice;
            }
            else if(tradeType == TradeType.SELL)
            {
                var symbolBalance = botBalances.Where(b => b.symbol.Equals(_symbol)).FirstOrDefault();

                quantity = symbolBalance.quantity / orderPrice;
            }

            var roundedDown = _helper.RoundDown(quantity, 2);

            return roundedDown;
        }

        /// <summary>
        /// Sell crypto
        /// </summary>
        /// <param name="orderPrice">Current price</param>
        private void SellCrypto(decimal orderPrice)
        {
            CancelStopLoss();

            var trade = MakeTrade(orderPrice);

            bool tradeComplete = false;
            while (!tradeComplete)
            {
                tradeComplete = CheckTradeStatus(trade.orderId);
            }
            lastSell = orderPrice;

            lastTrade = new TradeInformation
            {
                pair = _symbol,
                price = orderPrice,
                quantity = trade.origQty,
                timestamp = _dtHelper.UnixTimeToUTC(trade.transactTime),
                tradeType = EnumHelper.GetEnumDescription((TradeType)tradeType)
            };

            tradeInformation.Add(lastTrade);

            LogTransaction(lastTrade);

            tradeType = TradeType.BUY;
        }

        /// <summary>
        /// Cancel a stop loss
        /// </summary>
        /// <returns>Boolean value when complete</returns>
        private bool CancelStopLoss()
        {
            var tradeParams = new CancelTradeParams
            {
                symbol = _symbol,
                orderId = openStopLossList[0].orderId,
                origClientOrderId = openStopLossList[0].clientOrderId
            };

            var result = CancelTrade(tradeParams);

            bool stopLossCanceled = false;
            while(!stopLossCanceled)
            {
                stopLossCanceled = CheckTradeStatus(openStopLossList[0].orderId);
            }

            openStopLossList.RemoveAt(0);

            return stopLossCanceled;
        }

        /// <summary>
        /// Make a trade
        /// </summary>
        /// <param name="orderPrice">Trade price</param>
        /// <returns>TradeResponse object</returns>
        private TradeResponse MakeTrade(decimal orderPrice)
        {
            var quantity = GetTradeQuantity(orderPrice);

            var trade = new TradeParams
            {
                price = orderPrice,
                symbol = _symbol,
                side = tradeType.ToString(),
                type = OrderType.LIMIT.ToString(),
                quantity = quantity,
                timeInForce = "GTC"
            };

            var response = PlaceTrade(trade);

            return response;
        }

        /// <summary>
        /// Place a stop loss
        /// </summary>
        /// <param name="orderPrice">Trade price</param>
        /// <param name="quantity">Quantity to trade</param>
        /// <returns>TradeResponse object</returns>
        private TradeResponse PlaceStopLoss(decimal orderPrice, decimal quantity)
        {
            decimal stopLossPercent = (decimal)Math.Abs(botSettings.stopLoss)/100;
            decimal stopLossPrice = orderPrice - (orderPrice * stopLossPercent);

            var trade = new TradeParams
            {
                symbol = _symbol,
                side = TradeType.SELL.ToString(),
                type = OrderType.STOP_LOSS.ToString(),
                quantity = quantity,
                stopPrice = stopLossPrice,
                price = stopLossPrice
            };

            var response = PlaceTrade(trade);

            return response;
        }

        /// <summary>
        /// Cancel trade
        /// </summary>
        /// <param name="orderId">OrderId to cancel</param>
        /// <param name="origClientOrderId">ClientOrderId to cancel</param>
        private void CancelTrade(long orderId, string origClientOrderId)
        {
            var tradeParams = new CancelTradeParams
            {
                symbol = _symbol,
                orderId = orderId,
                origClientOrderId = origClientOrderId
            };

            var response = CancelTrade(tradeParams);
        }

        /// <summary>
        /// Get Bollinger Bands for a symbol
        /// </summary>
        /// <param name="interval">Candlestick interval</param>
        /// <returns>Array of Candlesticks</returns>
        public Candlestick[] GetBollingerBands(Interval interval)
        {
            var candlesticks = GetCandlesticks(interval, candlestickCount)
                                    //.OrderBy(c => c.closeTime)
                                    .ToArray();
            
            AddBollingerBands(ref candlesticks);

            return candlesticks.Where(c => c.bollingerBand != null).ToArray();
        }

        /// <summary>
        /// Add Bollinger Bands and Volume data to list
        /// </summary>
        /// <param name="candlesticks">Array of Candlesticks</param>
        private void AddBollingerBands(ref Candlestick[] candlesticks)
        {
            int period = candlestickCount;
            int factor = 2;
            decimal total_average = 0;
            decimal total_squares = 0;
            decimal prev_vol = 0;

            for (int i = 0; i < candlesticks.Length; i++)
            {
                total_average += candlesticks[i].close;
                total_squares += (decimal)Math.Pow((double)candlesticks[i].close, 2);
                prev_vol = prev_vol == 0 ? candlesticks[i].volume : prev_vol;

                var volData = CalculateVolumeChanges(candlesticks[i].volume, prev_vol);

                if(volData != null)
                {
                    if(volData.ContainsKey("volumeDifference"))
                        candlesticks[i].volumeChange = volData["volumeDifference"];

                    if (volData.ContainsKey("volumePercentChange"))
                        candlesticks[i].volumePercentChange = volData["volumePercentChange"];
                }

                if (i >= period - 1)
                {
                    var bollingerBand = new BollingerBand();
                    decimal average = total_average / period;

                    decimal stdev = (decimal)Math.Sqrt((double)(total_squares - (decimal)Math.Pow((double)total_average, 2) / period) / period);
                    bollingerBand.movingAvg = average;
                    bollingerBand.topBand = average + factor * stdev;
                    bollingerBand.bottomBand = average - factor * stdev;

                    candlesticks[i].bollingerBand = bollingerBand;
                    total_average -= candlesticks[i - period + 1].close;
                    total_squares -= (decimal)Math.Pow((double)candlesticks[i - period + 1].close, 2);
                }

                prev_vol = candlesticks[i].volume;
            }
        }

        /// <summary>
        /// Calculate volume data
        /// </summary>
        /// <param name="curr_vol">Current volume</param>
        /// <param name="prev_vol">Previous volume</param>
        /// <returns>Dictionary of values</returns>
        private Dictionary<string, decimal> CalculateVolumeChanges(decimal curr_vol, decimal prev_vol)
        {
            decimal vol_diff = prev_vol == 0 ? 0 : curr_vol - prev_vol;
            decimal vol_change = prev_vol == 0 ? 0 : (curr_vol / prev_vol) - 1;

            return new Dictionary<string, decimal>
            {
                {"volumeDifference", vol_diff },
                {"volumePercentChange", vol_change }
            };            
        }

        /// <summary>
        /// Get Candlesticks from exchange API
        /// </summary>
        /// <param name="interval">Candlestick Interval</param>
        /// <param name="range">Number of candlesticks to get</param>
        /// <returns>Array of Candlestick objects</returns>
        private Candlestick[] GetCandlesticks(Interval interval, int range)
        {
            return _repo.GetCandlestick(_symbol, interval, range).Result;
        }

        /// <summary>
        /// Place a trade
        /// </summary>
        /// <param name="tradeParams">Trade parameters</param>
        /// <returns>TradeResponse object</returns>
        private TradeResponse PlaceTrade(TradeParams tradeParams)
        {
            tradeNumber++;
            if (botSettings.tradingStatus == TradeStatus.LiveTrading)
                return _repo.PostTrade(tradeParams).Result;
            else if (botSettings.tradingStatus == TradeStatus.PaperTrading)
                return PlacePaperTrade(tradeParams);
            else
                return null;
        }

        /// <summary>
        /// Place a paper trade for testing purposes
        /// </summary>
        /// <param name="tradeParams">Trade parameters</param>
        /// <returns>TradeResponse object</returns>
        private TradeResponse PlacePaperTrade(TradeParams tradeParams)
        {
            OrderType orderType;
            TimeInForce TIF;
            TradeType tradeType;
            Enum.TryParse(tradeParams.type, out orderType);
            Enum.TryParse(tradeParams.timeInForce, out TIF);
            Enum.TryParse(tradeParams.side, out tradeType);
            var response = new TradeResponse
            {
                clientOrderId = $"PaperTrade_{tradeNumber}",
                executedQty = tradeParams.quantity,
                orderId = tradeNumber,
                origQty = tradeParams.quantity,
                price = tradeParams.price,
                side = tradeType,
                status = OrderStatus.FILLED,
                symbol = tradeParams.symbol,
                timeInForce = TIF,
                transactTime = _dtHelper.UTCtoUnixTime(),
                type = orderType
            };

            return response;
        }

        /// <summary>
        /// Cancel a trade
        /// </summary>
        /// <param name="tradeParams">CancelTrade parameters</param>
        /// <returns>TradeResponse object</returns>
        private TradeResponse CancelTrade(CancelTradeParams tradeParams)
        {
            if (botSettings.tradingStatus == TradeStatus.LiveTrading)
                return _repo.DeleteTrade(tradeParams).Result;
            else if (botSettings.tradingStatus == TradeStatus.PaperTrading)
                return CancelPaperTrade(tradeParams);
            else
                return null;
        }

        /// <summary>
        /// Cancel a paper trade for testing purposes
        /// </summary>
        /// <param name="tradeParams">Trade parameters</param>
        /// <returns>TradeResponse object</returns>
        private TradeResponse CancelPaperTrade(CancelTradeParams tradeParams)
        {
            var response = new TradeResponse
            {
                clientOrderId = $"PaperTrade_{tradeNumber}",
                orderId = tradeNumber,
                status = OrderStatus.FILLED,
                symbol = tradeParams.symbol,
                transactTime = _dtHelper.UTCtoUnixTime()
            };

            return response;
        }

        /// <summary>
        /// Get status of a trade
        /// </summary>
        /// <param name="orderId">OrderId of trade</param>
        /// <returns>OrderResponse</returns>
        private OrderResponse GetOrderStatus(long orderId)
        {
            if (botSettings.tradingStatus == TradeStatus.LiveTrading)
                return _repo.GetOrder(_symbol, orderId).Result;
            else if (botSettings.tradingStatus == TradeStatus.PaperTrading)
                return GetPaperOrderStatus(orderId);
            else
                return null;
        }

        /// <summary>
        /// Get status of a paper trade
        /// </summary>
        /// <param name="orderId">OrderId of trade</param>
        /// <returns>OrderResponse</returns>
        private OrderResponse GetPaperOrderStatus(long orderId)
        {
            var response = new OrderResponse
            {
                orderId = orderId,
                status = OrderStatus.FILLED
            };

            return response;
        }

        /// <summary>
        /// Get balances available
        /// </summary>
        /// <returns>Collection of balance objects</returns>
        private List<Balance> GetBalances()
        {
            var balances = new List<Balance>();

            if (botSettings.tradingStatus == TradeStatus.LiveTrading)
            {
                var result = _repo.GetBalance().Result;

                balances = result.balances;
            }
            else if (botSettings.tradingStatus == TradeStatus.PaperTrading)
            {
                balances = GetPaperBalances();
            }
            return balances;
        }

        /// <summary>
        /// Get paper balances available
        /// </summary>
        /// <returns>Collection of balance objects</returns>
        private List<Balance> GetPaperBalances(decimal btcStartingValue = 0)
        {
            var balances = new List<Balance>();
            var asset = _symbol.Replace("BTC", "");
            decimal btcQuantity = 0;
            decimal assetQuantity = 0;

            if (btcStartingValue > 0)
            {
                btcQuantity = btcStartingValue;
            }
            else
            {
                Enum.TryParse(lastTrade.tradeType, out TradeType lastTradeType);
                if (lastTradeType == TradeType.BUY)
                {
                    btcQuantity = 0;
                    assetQuantity = lastTrade.quantity;
                }
                else if (lastTradeType == TradeType.SELL)
                {
                    btcQuantity = lastTrade.quantity * lastTrade.price;
                    assetQuantity = 0;
                }
            }

            balances.Add(
                new Balance
                {
                    asset = "BTC",
                    free = btcQuantity,
                    locked = 0
                });

            balances.Add(
                new Balance
                {
                    asset = asset,
                    free = assetQuantity,
                    locked = 0
                });

            return balances;
        }

        /// <summary>
        /// Log balances to file
        /// </summary>
        /// <returns>Boolean when complete</returns>
        private bool LogBalances()
        {
            return _fileRepo.LogBalances(botBalances);
        }

        /// <summary>
        /// Log Trades
        /// </summary>
        /// <param name="tradeInformation">New TradeInformation object</param>
        /// <returns></returns>
        private bool LogTransaction(TradeInformation tradeInformation)
        {
            return _fileRepo.LogTransaction(tradeInformation);
        }
    }
}
