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
    public class OrderBookTradeBuilder : IOrderBookTradeBuilder
    {
        private Helper _helper = new Helper();
        private ITradeBuilder _trader;
        private BotSettings _botSettings;
        private string _symbol;
        private bool _currentlyTrading;
        private TradeType _lastTrade;
        private TradeType _tradeType;
        private decimal _lastBuy = 0.00000000M;
        private decimal _lastSell = 0.00000000M;
        private decimal _localHigh = 0.00000000M;
        private decimal _localLow = 0.00000000M;
        private int _tradeNumber = 0;
        private decimal _lastVolume = 0.00000000M;
        private string _asset = string.Empty;
        private string _pair = string.Empty;
        private decimal _moonTankPrice = 0.00000000M;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="repo">Repository interface</param>
        public OrderBookTradeBuilder()
        {
            _trader = new TradeBuilder();
            SetupBuilder();
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="repo">Repository interface</param>
        public OrderBookTradeBuilder(ITradeBuilder trader)
        {
            _trader = trader;
            SetupBuilder();
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="repo">Repository interface</param>
        public OrderBookTradeBuilder(ITradeBuilder trader
            , BotSettings settings, decimal lastBuy = 0, decimal lastSell = 0, TradeType tradeType = TradeType.BUY)
        {
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
            if(_currentlyTrading && !settings.runBot)
            {
                _currentlyTrading = false;
            }
            if(_botSettings.lastBuy !=0 && _lastBuy == 0.00000000M)
            {
                _lastBuy = _botSettings.lastBuy;
            }
            if (_botSettings.lastSell != 0 && _lastSell == 0.00000000M)
            {
                _lastSell = _botSettings.lastSell;
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
            _tradeType = _trader.GetInitialTradeType();

            while (currentlyTrading)
            {
                Task.WaitAll(Task.Delay(_botSettings.priceCheck));
                if (cycles % 20 == 0)
                { // Every 10 cycles, reset balances and check bot settings file
                    _tradeType = _trader.GetTradingType();
                    _trader.UpdateBotSettings(_lastBuy, _lastSell);
                    SetBotSettings(_trader.GetBotSettings());
                }
                var candleStickArray = _trader.GetCandlesticks(_symbol, interval, 5);
                int i = candleStickArray.Length - 1;
                currentStick = candleStickArray[i];

                if (previousStick.close == 0 && i > 2)
                {
                    previousStick = candleStickArray[i-1];
                }

                StoppedOutCheck(currentStick.close);

                if (previousStick.closeTime != currentStick.closeTime)
                {
                    if (_tradeType == TradeType.BUY)
                    {
                        BuyCryptoCheck(currentStick, previousStick);
                    }
                    else if (_tradeType == TradeType.SELL)
                    {
                        SellCryptoCheck(currentStick, previousStick);
                    }
                    cycles++;
                }
            }
            return true;
        }

        /// <summary>
        /// Check if sellying coins or not
        /// </summary>
        /// <param name="candleStick">current Candlestick object</param>
        /// <param name="previousStick">previous Candlestick object</param>
        /// <returns>Boolean if trade made</returns>
        private bool SellCryptoCheck(BotStick candleStick, BotStick previousStick)
        {
            var tradeType = TradeType.SELL;//MooningCheck(candleStick, TradeType.SELL);
            if (tradeType != TradeType.NONE)
            {
                var sellStatus = _trader.SellCrypto(_moonTankPrice, tradeType);

                _lastSell = _moonTankPrice;
                if (sellStatus)
                {
                    _lastSell = _moonTankPrice;
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
        /// <param name="candleStick">current Candlestick object</param>
        /// <param name="previousStick">previous Candlestick object</param>
        /// <returns>Boolean if trade made</returns>
        private bool BuyCryptoCheck(BotStick candleStick, BotStick previousStick)
        {
            var tradeType = TradeType.BUY;// TankingCheck(candleStick, TradeType.BUY);
            if (tradeType != TradeType.NONE || _tradeNumber == 0)
            {

                var buyStatus = _trader.BuyCrypto(_moonTankPrice, tradeType, true);

                _lastBuy = _moonTankPrice;
                if (buyStatus)
                {
                    _lastBuy = _moonTankPrice;
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
        /// Reset the local high and low values
        /// </summary>
        /// <param name="price">Current price</param>
        /// <param name="iteration">Current iteration</param>
        private void SetLocals(decimal price, int iteration)
        {
            if (iteration == 0)
            {
                this._localHigh = price;
                this._localLow = price;
            }
            else
            {
                this._localHigh = price > this._localHigh ? price : this._localHigh;
                this._localLow = price < this._localLow ? price : this._localLow;
            }
        }

        /// <summary>
        /// Get next candlestick
        /// </summary>
        /// <returns>Candlestick object</returns>
        private BotStick[] GetNextCandlestick()
        {
            Task.WaitAll(Task.Delay(_botSettings.mooningTankingTime));

            var candlesticks = _trader.GetCandlesticks(_symbol, Interval.OneM, 2);

            return candlesticks;
        }
    }
}
