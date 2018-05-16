using CoinBot.Business.Builders.Interface;
using CoinBot.Business.Entities;
using CoinBot.Core;
using CoinBot.Data;
using CoinBot.Data.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoinBot.Business.Builders
{
    public class TradingBuilder : ITradingBuilder
    {
        private IBinanceRepository _repo;
        private IFileRepository _fileRepo;
        private const int candlestickCount = 21;
        private BotSettings botSettings;
        private List<Bag> bags;
        private List<TradeInformation> tradeInformation;
        private decimal availableToTrade;
        private List<OpenOrder> openOrderList;
        private string _symbol;
        private List<OpenStopLoss> openStopLossList;
        private List<BotBalance> botBalances;
        private bool currentlyTrading;
        private decimal lastBuy = 0.00000000M;
        private decimal lastSell = 0.00000000M;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="repo">Repository interface</param>
        public TradingBuilder(IBinanceRepository repo)
        {
            _repo = repo;
            _fileRepo = new FileRepository();
            botSettings = _fileRepo.GetConfig();
            _symbol = botSettings.tradingPair;
            bags = new List<Bag>();
            tradeInformation = new List<TradeInformation>();
            availableToTrade = 0;
            openOrderList = new List<OpenOrder>();
            openStopLossList = new List<OpenStopLoss>();
            botBalances = new List<BotBalance>();
            currentlyTrading = true;
            RunBot(Interval.OneM);
        }

        /// <summary>
        /// Set BotSettings in builder
        /// </summary>
        /// <param name="settings">Updated Settings</param>
        /// <returns>Boolean when complete</returns>
        public bool SetBotSettings(BotSettings settings)
        {
            _fileRepo.UpdateBotSettings(settings);
            botSettings = settings;
            _symbol = settings.tradingPair;

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
        /// Run Trading Bot
        /// </summary>
        /// <param name="interval">Candlestick Interval</param>
        private void RunBot(Interval interval)
        {
            TradeType tradeType = TradeType.BUY;
            var currentStick = new Candlestick();
            var previousStick = new Candlestick();
            
            while(currentlyTrading)
            {
                var bbs = GetBollingerBands(interval);
                int i = bbs.Length;
                currentStick = bbs[i];

                if (previousStick.close == 0)
                    previousStick = bbs[i];

                Task.Run(async () =>
                {
                    await Task.Delay(botSettings.priceCheck);

                    bbs = GetBollingerBands(interval);
                    currentStick = bbs[i];
                    var tradeMade = false;
                    
                    if(StoppedOutCheck(currentStick.close))
                    {
                        tradeType = TradeType.BUY;
                    }

                    if (tradeType == TradeType.BUY)
                    {
                        if (currentStick.bollingerBand.bottomBand > currentStick.close)
                        {
                            BuyCrypto(ref tradeType, currentStick.close);
                            tradeMade = true;
                        }
                    }
                    else if (tradeType == TradeType.SELL)
                    {
                        if(currentStick.bollingerBand.topBand < currentStick.close 
                            && lastSell < currentStick.close 
                            && GetPercent(currentStick.close) >= botSettings.sellPercent)
                        {
                            SellCrypto(ref tradeType, currentStick.close);
                            tradeMade = true;
                        }
                    }
                    if (tradeMade)
                    {
                        UpdateBalances();
                    }
                    previousStick = currentStick;
                });
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
                    quantity = balances[i].free + balances[i].locked
                };

                botBalances.Add(botBalance);
            }
        }

        /// <summary>
        /// Check if Stop Loss Hit
        /// </summary>
        /// <param name="currentPrice">Current price of coin</param>
        /// <returns>Boolean value</returns>
        private bool StoppedOutCheck(decimal currentPrice)
        {
            if (openStopLossList.Count == 0 || currentPrice <= openStopLossList[0].price)
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
        /// <param name="tradeType">TradeType enum</param>
        /// <param name="orderPrice">Buy price</param>
        private void BuyCrypto(ref TradeType tradeType, decimal orderPrice)
        {
            var trade = MakeTrade(orderPrice, tradeType);

            bool tradeComplete = false;
            while(!tradeComplete)
            {
                tradeComplete = CheckTradeStatus(trade.orderId);
            }
            lastBuy = orderPrice;

            tradeInformation.Add(
                new TradeInformation
                {
                    pair = _symbol,
                    price = orderPrice,
                    quantity = trade.origQty,
                    tradeTime = trade.transactTime,
                    tradeType = tradeType
                });

            var stopLoss = PlaceStopLoss(orderPrice);

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
        /// Sell crypto
        /// </summary>
        /// <param name="tradeType">Trade type</param>
        /// <param name="orderPrice">Current price</param>
        private void SellCrypto(ref TradeType tradeType, decimal orderPrice)
        {
            CancelStopLoss();

            var trade = MakeTrade(orderPrice, tradeType);

            bool tradeComplete = false;
            while (!tradeComplete)
            {
                tradeComplete = CheckTradeStatus(trade.orderId);
            }
            lastSell = orderPrice;

            tradeInformation.Add(
                new TradeInformation
                {
                    pair = _symbol,
                    price = orderPrice,
                    quantity = trade.origQty,
                    tradeTime = trade.transactTime,
                    tradeType = tradeType
                });

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
        /// <param name="tradeType">Trade type</param>
        /// <returns>TradeResponse object</returns>
        private TradeResponse MakeTrade(decimal orderPrice, TradeType tradeType)
        {
            var trade = new TradeParams
            {
                price = orderPrice,
                symbol = _symbol,
                side = tradeType.ToString(),
                type = OrderType.LIMIT.ToString(),
                quantity = 0,
                timeInForce = "GTC"
            };

            var response = PlaceTrade(trade);
        }

        /// <summary>
        /// Place a stop loss
        /// </summary>
        /// <param name="orderPrice">Trade price</param>
        /// <returns>TradeResponse object</returns>
        private TradeResponse PlaceStopLoss(decimal orderPrice)
        {
            decimal stopLossPercent = (decimal)Math.Abs(botSettings.stopLoss);
            decimal stopLossPrice = orderPrice - (orderPrice * stopLossPercent);

            var trade = new TradeParams
            {
                symbol = _symbol,
                side = TradeType.SELL.ToString(),
                type = OrderType.STOP_LOSS.ToString(),
                quantity = 0,
                stopPrice = stopLossPrice
            };

            var response = PlaceTrade(trade);
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
                                    .OrderByDescending(c => c.closeTime)
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
            decimal vol_diff = curr_vol - prev_vol;
            decimal vol_change = (curr_vol / prev_vol) - 1;

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
            return _repo.PostTrade(tradeParams).Result;
        }

        /// <summary>
        /// Cancel a trade
        /// </summary>
        /// <param name="tradeParams">CancelTrade parameters</param>
        /// <returns>TradeResponse object</returns>
        private TradeResponse CancelTrade(CancelTradeParams tradeParams)
        {
            return _repo.DeleteTrade(tradeParams).Result;
        }

        /// <summary>
        /// Get status of a trade
        /// </summary>
        /// <param name="orderId">OrderId of trade</param>
        /// <returns>OrderResponse</returns>
        private OrderResponse GetOrderStatus(long orderId)
        {
            return _repo.GetOrder(_symbol, orderId).Result;
        }
        
        /// <summary>
        /// Get balances on exchange
        /// </summary>
        /// <returns>Collection of balance objects</returns>
        private List<Balance> GetBalances()
        {
            var result = _repo.GetBalance().Result;

            return result.balances;
        }
    }
}
