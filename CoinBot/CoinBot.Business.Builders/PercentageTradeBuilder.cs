using CoinBot.Business.Builders.Interface;
using CoinBot.Business.Entities;
using CoinBot.Core;
using CoinBot.Data;
using CoinBot.Data.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoinBot.Business.Builders
{
    public class PercentageTradeBuilder : IPercentageTradeBuilder
    {
        private IBinanceRepository _repo;
        private IFileRepository _fileRepo;
        private Helper _helper = new Helper();
        private ITradeBuilder _trader;
        private BotSettings _botSettings;
        private string _symbol;
        private bool _currentlyTrading;
        private TradeType _lastTrade;
        private TradeType _tradeType;
        private decimal _lastBuy = 0.00000000M;
        private decimal _lastSell = 0.00000000M;
        private int _tradeNumber = 0;
        private decimal _lastVolume = 0.00000000M;
        private string _asset = string.Empty;
        private string _pair = string.Empty;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="repo">Repository interface</param>
        public PercentageTradeBuilder()
        {
            _repo = new BinanceRepository();
            _fileRepo = new FileRepository();
            _trader = new TradeBuilder();
            SetupBuilder();
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="repo">Repository interface</param>
        public PercentageTradeBuilder(IBinanceRepository repo, IFileRepository fileRepo, ITradeBuilder trader)
        {
            _repo = repo;
            _fileRepo = fileRepo;
            _trader = trader;
            SetupBuilder();
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="repo">Repository interface</param>
        public PercentageTradeBuilder(IBinanceRepository repo, IFileRepository fileRepo, ITradeBuilder trader
            , BotSettings settings, decimal lastBuy = 0, decimal lastSell = 0, TradeType tradeType = TradeType.BUY)
        {
            _repo = repo;
            _fileRepo = fileRepo;
            _trader = trader;
            SetBotSettings(settings);
            if (lastBuy != 0)
            {
                _lastBuy = lastBuy;
            }
            if (lastSell != 0)
            {
                _lastSell = lastSell;
            }
            _tradeType = tradeType;
        }

        private void SetupBuilder()
        {
            _trader = new TradeBuilder();
            _lastTrade = TradeType.NONE;
            SetBotSettings(_trader.GetBotSettings());
        }

        /// <summary>
        /// Set BotSettings
        /// </summary>
        /// <param name="settings">Updated Bot Settings</param>
        /// <returns>Boolean when complete</returns>
        public bool SetBotSettings(BotSettings settings)
        {
            _botSettings = settings;
            _symbol = _botSettings.tradingPair;
            _currentlyTrading = _botSettings.startBotAutomatically == null
                                ? false
                                : (bool)_botSettings.startBotAutomatically;
            if(_botSettings.lastBuy !=0)
            {
                _lastBuy = _botSettings.lastBuy;
            }
            return true;
        }

        /// <summary>
        /// Start trading
        /// </summary>
        /// <param name="interval">Candlestick interval</param>
        public void StartTrading(Interval interval)
        {
            _currentlyTrading = true;

            RunBot(interval);
        }

        /// <summary>
        /// Stop Trading
        /// </summary>
        public void StopTrading()
        {
            _currentlyTrading = false;
        }

        /// <summary>
        /// Run Trading Bot
        /// </summary>
        /// <param name="interval">Candlestick Interval</param>
        /// <param name="cycles">Int of cycles to run (default -1, run infinitely)</param>
        /// <param name="tradingStatus">Bool of trading status (default null, use setting)</param>
        /// <returns>Boolean when complete</returns>
        public bool RunBot(Interval interval, int cycles = -1, bool? tradingStatus = null)
        {
            _trader.SetupRepository();
            _tradeType = TradeType.BUY;
            var currentStick = new BotStick();
            var previousStick = new BotStick();
            bool currentlyTrading = tradingStatus != null ? (bool)tradingStatus : _currentlyTrading;

            _trader.SetBalances();
            _tradeType = CheckInitialTradingType();

            while (currentlyTrading)
            {
                Task.WaitAll(Task.Delay(_botSettings.priceCheck));
                var candleStickArray = _trader.GetCandlesticks(_symbol, interval, 5);
                int i = candleStickArray.Length - 1;
                currentStick = candleStickArray[i];

                if (previousStick.close == 0)
                {
                    previousStick = candleStickArray[i];
                }

                StoppedOutCheck(currentStick.close);

                if (_tradeType == TradeType.BUY)
                {
                    BuyCryptoCheck(currentStick);
                }
                else if (_tradeType == TradeType.SELL)
                {
                    SellCryptoCheck(currentStick);
                }
                if (_tradeNumber >= cycles && cycles > 0)
                {
                    currentlyTrading = false;
                }
                previousStick = currentStick;
            }
            return true;
        }

        /// <summary>
        /// Check staring balances to determine the first trade type of the bot
        /// </summary>
        /// <returns>TradeType value</returns>
        public TradeType CheckInitialTradingType()
        {
            _asset = _trader.GetAsset();
            _pair = _trader.GetPair();

            var balances = _trader.GetBotBalance();
            var assetQty = balances.Where(b => b.symbol.Equals(_asset)).Select(b => b.quantity).FirstOrDefault();
            var pairQty = balances.Where(b => b.symbol.Equals(_pair)).Select(b => b.quantity).FirstOrDefault();

            if ((_pair == "USD" || _pair == "USDT")
                && pairQty < 10)
            {
                return TradeType.SELL;
            }
            else
            {
                return TradeType.BUY;
            }
        }

        /// <summary>
        /// Check if sellying coins or not
        /// </summary>
        /// <param name="cand0leStick">Candlestick object</param>
        /// <returns>Boolean if trade made</returns>
        private bool SellCryptoCheck(BotStick candleStick)
        {
            var tradeType = MooningAndTankingCheck(candleStick, TradeType.SELL);
            if (tradeType != TradeType.NONE)
            {
                var sellStatus = _trader.SellCrypto(candleStick.close, tradeType);

                if (sellStatus)
                {
                    _lastSell = candleStick.close;
                    _tradeType = TradeType.BUY;
                    _lastTrade = tradeType;
                    _tradeNumber++;
                }
                return sellStatus;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Check if buying coins or not
        /// </summary>
        /// <param name="candleStick">Candlestick object</param>
        /// <returns>Boolean if trade made</returns>
        private bool BuyCryptoCheck(BotStick candleStick)
        {
            var tradeType = MooningAndTankingCheck(candleStick, TradeType.BUY);
            if (tradeType != TradeType.NONE || _tradeNumber == 0)
            {
                if (_tradeNumber == 0)
                    tradeType = TradeType.BUY;

                var buyStatus = _trader.BuyCrypto(candleStick.close, tradeType);

                if (buyStatus)
                {
                    _lastBuy = candleStick.close;
                    _tradeType = TradeType.SELL;
                    _lastTrade = tradeType;
                    _tradeNumber++;
                }
                return buyStatus;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Check if Stop Loss hit
        /// </summary>
        /// <param name="currentPrice">Decimal of current price</param>
        private void StoppedOutCheck(decimal currentPrice)
        {
            var stoppedOut = _trader.StoppedOutCheck(currentPrice);
            if (stoppedOut != null)
            {
                _lastSell = (decimal)stoppedOut;
                _tradeType = TradeType.BUY;
                _lastTrade = TradeType.STOPLOSS;
                _tradeNumber++;
            }
        }

        /// <summary>
        /// Check if buy percent has been reached
        /// </summary>
        /// <param name="currentPrice">Current price</param>
        /// <returns>Boolean of result</returns>
        public bool BuyPercentReached(decimal currentPrice)
        {
            var percent = _helper.GetBuyPercent(currentPrice, _lastSell) * 100;

            return percent >= _botSettings.buyPercent;
        }

        /// <summary>
        /// Check if sell percent has been reached
        /// </summary>
        /// <param name="currentPrice">Current price</param>
        /// <returns>Boolean of result</returns>
        public bool SellPercentReached(decimal currentPrice)
        {
            var percent = _helper.GetSellPercent(currentPrice, _lastBuy) * 100;

            return percent >= _botSettings.sellPercent;
        }

        /// <summary>
        /// Check if mooning or tanking
        /// </summary>
        /// <param name="candleStick">Current trading stick</param>
        /// <param name="tradeType">Trade Type</param>
        /// <param name="startingPrice">decimal of starting price</param>
        /// <param name="iteration">Int of iteration</param>
        /// <returns>TradeType of result</returns>
        public TradeType MooningAndTankingCheck(BotStick candleStick, 
                                                TradeType tradeType,
                                                decimal startingPrice = 0.00000000M,
                                                int iteration = 0)
        {
            if(_botSettings.mooningTankingTime == 0)
            {
                return tradeType;
            }

            if(iteration == 0)
            {
                startingPrice = candleStick.close;
            }

            _lastVolume = candleStick.volume;

            var latestStick = GetNextCandlestick();

            var volumePercentChange = _helper.GetPercent(_lastVolume, latestStick.volume) * 100;

            if(_tradeType == TradeType.BUY)
            {
                var buyPercentReached = BuyPercentReached(latestStick.close);

                if (candleStick.close > latestStick.close
                    && buyPercentReached)
                {
                    // If current price is less than the previous check 
                    //  (price is dropping)
                    // and buy percent reached
                    // keep checking if dropping more
                    iteration++;
                    MooningAndTankingCheck(latestStick, tradeType, startingPrice, iteration);
                }
                else
                {
                    if (_lastTrade == TradeType.VOLUMESELL
                        && candleStick.close < latestStick.close
                        && _lastSell > latestStick.close)
                    {
                        // If last trade was volume sell and current stick is greater than last,
                        // but less than last sell, probably reached ATL: BUY volume sell
                        return TradeType.VOLUMESELLBUYOFF;
                    }
                    else if (volumePercentChange > _botSettings.mooningTankingPercent 
                        && candleStick.close < latestStick.close)
                    {
                        // If volume increased more than N% and Latest close is greater than previous close
                        // Probably a mini-moon: BUY
                        return TradeType.VOLUMEBUY;
                    }
                    else
                    {
                        // Else if not dropping in price from previous check
                        // or buy percent not reached
                        // return buy percent reached
                        return buyPercentReached ? TradeType.BUY : TradeType.NONE;
                    }
                }
            }
            else
            {
                var sellPercentReached = SellPercentReached(latestStick.close);

                if (candleStick.close < latestStick.close
                    && sellPercentReached)
                {
                    // If current price is greater than the previous check 
                    //  (price is increasing)
                    // and sell percent reached
                    // keep checking if increasing more
                    iteration++;
                    MooningAndTankingCheck(latestStick, tradeType, startingPrice, iteration);
                }
                else
                {
                    if(_lastTrade == TradeType.VOLUMEBUY 
                        && candleStick.close > latestStick.close 
                        && _lastBuy < latestStick.close)
                    {
                        // If last trade was volume buy and current stick is less than last,
                        // but greater than last buy, probably reached ATH: SELL volume buy
                        return TradeType.VOLUMEBUYSELLOFF;
                    }
                    else if (volumePercentChange > _botSettings.mooningTankingPercent 
                        && candleStick.close > latestStick.close)
                    {
                        // If volume increased more than N% and Latest close is less than previous close
                        // Probably a sell off: Sell
                        return TradeType.VOLUMESELL;
                    }
                    else
                    {
                        // Else if not increasing in price from previous check
                        // or sell percent not reached
                        // return sell percent reached
                        return sellPercentReached ? TradeType.SELL : TradeType.NONE;
                    }
                }
            }

            return tradeType;
        }

        /// <summary>
        /// Get next candlestick
        /// </summary>
        /// <returns>Candlestick object</returns>
        private BotStick GetNextCandlestick()
        {
            Task.WaitAll(Task.Delay(_botSettings.mooningTankingTime));

            var candlesticks = _trader.GetCandlesticks(_symbol, Interval.OneM, 1);

            return candlesticks[0];
        }
    }
}
