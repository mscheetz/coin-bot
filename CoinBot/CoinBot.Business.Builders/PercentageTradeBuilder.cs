using CoinBot.Business.Builders.Interface;
using CoinBot.Business.Entities;
using CoinBot.Core;
using CoinBot.Data;
using CoinBot.Data.Interface;
using System;
using System.Collections.Generic;
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
        private TradeType _tradeType;
        private decimal _lastBuy = 0.00000000M;
        private decimal _lastSell = 0.00000000M;
        private int _tradeNumber = 0;

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
            SetBotSettings(_trader.GetBotSettings());
            //if (botSettings.tradingStrategy == Strategy.Percentage)
            //    RunBot(Interval.FiveM);
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
        /// <returns>Boolean when complete</returns>
        public bool RunBot(Interval interval, int cycles = -1, bool currentlyTrading = false)
        {
            _trader.SetupRepository();
            _tradeType = TradeType.BUY;
            var currentStick = new Candlestick();
            var previousStick = new Candlestick();

            if (_botSettings.tradingStatus == TradeStatus.PaperTrading)
                _trader.SetPaperBalance();

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
        /// Check if sellying coins or not
        /// </summary>
        /// <param name="cand0leStick">Candlestick object</param>
        /// <returns>Boolean if trade made</returns>
        private bool SellCryptoCheck(Candlestick candleStick)
        {
            if (SellPercentReached(candleStick.close)
                && MooningAndTankingCheck(candleStick))
            {
                _trader.SellCrypto(candleStick.close);
                _lastSell = candleStick.close;
                _tradeType = TradeType.BUY;
                _tradeNumber++;
                return true;
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
        private bool BuyCryptoCheck(Candlestick candleStick)
        {
            if ((BuyPercentReached(candleStick.close)
                && MooningAndTankingCheck(candleStick))
                || _tradeNumber == 0)
            {
                _trader.BuyCrypto(candleStick.close);
                _lastBuy = candleStick.close;
                _tradeType = TradeType.SELL;
                _tradeNumber++;
                return true;
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
            var percent = _helper.GetBuyPercent(currentPrice, _lastSell);

            return percent >= _botSettings.buyPercent;
        }

        /// <summary>
        /// Check if sell percent has been reached
        /// </summary>
        /// <param name="currentPrice">Current price</param>
        /// <returns>Boolean of result</returns>
        public bool SellPercentReached(decimal currentPrice)
        {
            var percent = _helper.GetSellPercent(currentPrice, _lastBuy);

            return percent >= _botSettings.sellPercent;
        }

        /// <summary>
        /// Check if mooning or tanking
        /// </summary>
        /// <param name="candleStick">Current trading stick</param>
        /// <returns>Boolean of result</returns>
        public bool MooningAndTankingCheck(Candlestick candleStick)
        {
            if(_botSettings.mooningTankingTime == 0)
            {
                return true;
            }

            var nextStick = GetNextCandlestick();

            if(_tradeType == TradeType.BUY)
            {
                var buyPercentReached = BuyPercentReached(nextStick.close);
                if (candleStick.close > nextStick.close 
                    && buyPercentReached)
                {
                    // If current price is less than the previous check 
                    //  (price is dropping)
                    // and buy percent reached
                    // keep checking if dropping more
                    MooningAndTankingCheck(nextStick);
                }
                else
                {
                    // Else if not dropping in price from previous check
                    // or buy percent not reached
                    // return buy percent reached
                    return buyPercentReached;
                }
            }
            else
            {
                var sellPercentReached = SellPercentReached(nextStick.close);
                if (candleStick.close < nextStick.close
                    && sellPercentReached)
                {
                    // If current price is greater than the previous check 
                    //  (price is increasing)
                    // and sell percent reached
                    // keep checking if increasing more
                    MooningAndTankingCheck(nextStick);
                }
                else
                {
                    // Else if not increasing in price from previous check
                    // or sell percent not reached
                    // return sell percent reached
                    return sellPercentReached;
                }
            }

            return true;
        }

        /// <summary>
        /// Get next candlestick
        /// </summary>
        /// <returns>Candlestick object</returns>
        private Candlestick GetNextCandlestick()
        {
            Task.WaitAll(Task.Delay(_botSettings.mooningTankingTime));

            var candlesticks = _trader.GetCandlesticks(_symbol, Interval.OneM, 1);

            return candlesticks[0];
        }
    }
}
