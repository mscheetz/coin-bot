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
    public class VolumeTradeBuilder : IVolumeTradeBuilder
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
        public VolumeTradeBuilder()
        {
            _trader = new TradeBuilder();
            SetupBuilder();
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="repo">Repository interface</param>
        public VolumeTradeBuilder(ITradeBuilder trader)
        {
            _trader = trader;
            SetupBuilder();
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="repo">Repository interface</param>
        public VolumeTradeBuilder(ITradeBuilder trader
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
                    currentlyTrading = _tradeType == TradeType.NONE
                        ? false
                        : _botSettings.runBot;
                }
                var candleStickArray = _trader.GetCandlesticks(_symbol, interval, 5);
                int i = candleStickArray.Length - 1;
                currentStick = candleStickArray[i];

                if (previousStick.close == 0 && i > 2)
                {
                    previousStick = candleStickArray[i-1];
                }

                //StoppedOutCheck(currentStick.close);

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
            var tradeType = MooningCheck(candleStick, TradeType.SELL);
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
            var tradeType = TankingCheck(candleStick, TradeType.BUY);
            if (tradeType != TradeType.NONE || _tradeNumber == 0)
            {
                //if (_tradeNumber == 0)
                //    tradeType = TradeType.BUY;

                var buyStatus = _trader.BuyCrypto(_moonTankPrice, tradeType);

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
        /// Check if mooning or tanking
        /// </summary>
        /// <param name="candleStick">Current trading stick</param>
        /// <param name="previousStick">Previous trading stick</param>
        /// <param name="tradeType">Trade Type</param>
        /// <param name="startingPrice">decimal of starting price</param>
        /// <param name="iteration">Int of iteration</param>
        /// <returns>TradeType of result</returns>
        public TradeType MooningAndTankingCheck(BotStick candleStick, BotStick previousStick,
                                                TradeType tradeType,
                                                decimal startingPrice = 0.00000000M,
                                                int iteration = 0)
        {
            if (_botSettings.mooningTankingTime == 0)
            {
                //return tradeType;
            }

            if (iteration == 0)
            {
                startingPrice = candleStick.close;
            }

            _lastVolume = candleStick.volume;

            var sticks = GetNextCandlestick();

            var latestStick = sticks[0];
            candleStick = sticks[1];
            previousStick = sticks[1];

            if (latestStick.closeTime == candleStick.closeTime)
            { // If latest stick has the same close time as current stick, make previous stick the current stick
                candleStick = previousStick;
            }

            _moonTankPrice = latestStick.close;

            var volumePercentChange = _helper.GetPercent(_lastVolume, latestStick.volume) * 100;

            if (_tradeType == TradeType.BUY)
            {
                var buyPercentReached = BuyPercentReached(latestStick.close);

                if (candleStick.close > latestStick.close
                    && buyPercentReached && iteration < 10)
                {
                    // If current price is less than the previous check 
                    //  (price is dropping)
                    // and buy percent reached
                    // keep checking if dropping more
                    iteration++;
                    MooningAndTankingCheck(latestStick, candleStick, tradeType, startingPrice, iteration);
                }
                else
                {
                    if (_lastTrade == TradeType.VOLUMESELL
                        && candleStick.close < latestStick.close
                        && _lastSell > latestStick.close
                        && _botSettings.mooningTankingTime > 0)
                    {
                        // If last trade was volume sell and current stick is greater than last,
                        // but less than last sell, probably reached ATL: BUY volume sell
                        _trader.LogTradeSignal(SignalType.Volume, TradeType.VOLUMESELLBUYOFF, latestStick.close, latestStick.volume);

                        return TradeType.VOLUMESELLBUYOFF;
                    }
                    else if (volumePercentChange > _botSettings.mooningTankingPercent
                        && candleStick.close < latestStick.close
                        && _botSettings.mooningTankingTime > 0)
                    {
                        // If volume increased more than N% and Latest close is greater than previous close
                        // Probably a mini-moon: BUY
                        _trader.LogTradeSignal(SignalType.Volume, TradeType.VOLUMEBUY, latestStick.close, latestStick.volume);

                        return TradeType.VOLUMEBUY;
                    }
                    else
                    {
                        // Else if not dropping in price from previous check
                        // or buy percent not reached
                        // return buy percent reached
                        if (buyPercentReached)
                        {
                            _trader.LogTradeSignal(SignalType.Percent, TradeType.BUY, latestStick.close, latestStick.volume);

                            return TradeType.BUY;
                        }
                        else
                        {
                            return TradeType.NONE;
                        }
                    }
                }
            }
            else
            {
                var sellPercentReached = SellPercentReached(latestStick.close);

                if (candleStick.close < latestStick.close
                    && sellPercentReached && iteration < 10)
                {
                    // If current price is greater than the previous check 
                    //  (price is increasing)
                    // and sell percent reached
                    // keep checking if increasing more
                    iteration++;
                    MooningAndTankingCheck(latestStick, candleStick, tradeType, startingPrice, iteration);
                }
                else if (sellPercentReached)
                {
                    // Else if not increasing in price from previous check
                    // or sell percent not reached
                    // return sell percent reached
                    _trader.LogTradeSignal(SignalType.Percent, TradeType.SELL, latestStick.close, latestStick.volume);

                    return TradeType.SELL;
                }
                else
                {
                    if (_lastTrade == TradeType.VOLUMEBUY
                        && candleStick.close > latestStick.close
                        && _lastBuy < latestStick.close
                        && _botSettings.mooningTankingTime > 0)
                    {
                        // If last trade was volume buy and current stick is less than last,
                        // but greater than last buy, probably reached ATH: SELL volume buy
                        _trader.LogTradeSignal(SignalType.Volume, TradeType.VOLUMEBUYSELLOFF, latestStick.close, latestStick.volume);

                        return TradeType.VOLUMEBUYSELLOFF;
                    }
                    else if (volumePercentChange > _botSettings.mooningTankingPercent
                        && candleStick.close > latestStick.close
                        && _botSettings.mooningTankingTime > 0)
                    {
                        // If volume increased more than N% and Latest close is less than previous close
                        // Probably a sell off: Sell
                        _trader.LogTradeSignal(SignalType.Volume, TradeType.VOLUMESELL, latestStick.close, latestStick.volume);

                        return TradeType.VOLUMESELL;
                    }
                    else
                    {
                        return TradeType.NONE;
                    }
                }
            }

            return tradeType;
        }

        /// <summary>
        /// Check if mooning
        /// </summary>
        /// <param name="startingStick">Starting BotStick</param>
        /// <param name="tradeType">Trade Type</param>
        /// <param name="prevStick">Previous BotStick (default null)</param>
        /// <param name="startingPrice">decimal of starting price</param>
        /// <param name="iteration">Int of iteration</param>
        /// <returns>TradeType of result</returns>
        public TradeType MooningCheck(BotStick startingStick
                                    , TradeType tradeType
                                    , BotStick prevStick = null
                                    , decimal startingPrice = 0.00000000M
                                    , int iteration = 0)
        {
            if(_botSettings.mooningTankingTime == 0)
            {
                //return tradeType;
            }

            var sticks = GetNextCandlestick();

            var currentStick = sticks[0];
            var lastStick = sticks[1];

            if(prevStick == null)
            {
                prevStick = lastStick;
            }

            if(iteration == 0)
            {
                startingPrice = lastStick.close;
            }

            _lastVolume = lastStick.volume;

            SetLocals(currentStick.close, iteration);

            _moonTankPrice = currentStick.close;

            var volumePercentChange = _helper.GetPercent(_lastVolume, currentStick.volume) * 100;

            var sellPercentReached = SellPercentReached(currentStick.close);

            if (currentStick.open < currentStick.close
                && startingStick.close < currentStick.close
                && prevStick.close < currentStick.close)
            //&& sellPercentReached && iteration < 10)
            {
                // TODO: set the latest price as the starting price to see if it is increasing during current candle
                // TODO: do same on tanking
                // If current price is greater than the previous check 
                //  (price is increasing)
                // and sell percent reached
                // keep checking if increasing more
                iteration++;
                MooningCheck(startingStick, tradeType, lastStick, startingPrice, iteration);
            }
            else if (sellPercentReached)
            {
                // Else if not increasing in price from previous check
                // or sell percent not reached
                // return sell percent reached
                _trader.LogTradeSignal(SignalType.Percent, TradeType.SELL, currentStick.close, currentStick.volume);

                return TradeType.SELL;
            }
            else if (_botSettings.mooningTankingTime > 0)
            {
                if (_lastTrade == TradeType.VOLUMEBUY
                    && currentStick.open < currentStick.close
                    && lastStick.close < currentStick.close 
                    && _lastBuy < currentStick.close
                    && _localHigh > currentStick.close)
                {
                    // If last trade was volume buy and current stick is less than last,
                    // but greater than last buy, probably reached ATH: SELL volume buy
                    _trader.LogTradeSignal(SignalType.Volume, TradeType.VOLUMEBUYSELLOFF, currentStick.close, currentStick.volume);

                    return TradeType.VOLUMEBUYSELLOFF;
                }
                else if (volumePercentChange > _botSettings.mooningTankingPercent
                    && currentStick.open > currentStick.close
                    && lastStick.close > currentStick.close
                    && _lastBuy < currentStick.close)
                {
                    // If volume increased more than N% and Latest close is less than previous close
                    // Probably a sell off: Sell
                    _trader.LogTradeSignal(SignalType.Volume, TradeType.VOLUMESELL, currentStick.close, currentStick.volume);

                    return TradeType.VOLUMESELL;
                }
                else
                {
                    return TradeType.NONE;
                }
            }
            else
            {
                return TradeType.NONE;
            }

            return tradeType;
        }
        /// <summary>
        /// Check if tanking
        /// </summary>
        /// <param name="startingStick">Starting BotStick</param>
        /// <param name="tradeType">Trade Type</param>
        /// <param name="prevStick">Previous BotStick (default null)</param>
        /// <param name="startingPrice">decimal of starting price</param>
        /// <param name="iteration">Int of iteration</param>
        /// <returns>TradeType of result</returns>
        public TradeType TankingCheck(BotStick startingStick
                                    , TradeType tradeType
                                    , BotStick prevStick = null
                                    , decimal startingPrice = 0.00000000M
                                    , int iteration = 0)
        {
            if (_botSettings.mooningTankingTime == 0)
            {
                //return tradeType;
            }

            var sticks = GetNextCandlestick();

            var currentStick = sticks[0];
            var lastStick = sticks[1];

            if (prevStick == null)
            {
                prevStick = lastStick;
            }

            if (iteration == 0)
            {
                startingPrice = lastStick.close;
            }

            SetLocals(currentStick.close, iteration);

            _lastVolume = lastStick.volume;

            _moonTankPrice = currentStick.close;

            var volumePercentChange = _helper.GetPercent(_lastVolume, currentStick.volume) * 100;

            var buyPercentReached = BuyPercentReached(currentStick.close);

            if (currentStick.open > currentStick.close
                && lastStick.close > currentStick.close
                && prevStick.close > currentStick.close)
                //&& buyPercentReached && iteration < 10)
            {
                // If current price is less than the previous check 
                //  (price is dropping)
                // and buy percent reached
                // keep checking if dropping more
                iteration++;
                TankingCheck(startingStick, tradeType, lastStick, startingPrice, iteration);
            }
            else if (buyPercentReached)
            {
                // Else if not dropping in price from previous check
                // or buy percent not reached
                // return buy percent reached
                _trader.LogTradeSignal(SignalType.Percent, TradeType.BUY, currentStick.close, currentStick.volume);

                return TradeType.BUY;
            }
            else if (_botSettings.mooningTankingTime > 0)
            {
                if (_lastTrade == TradeType.VOLUMESELL
                    && currentStick.open > currentStick.close
                    && lastStick.close > currentStick.close
                    && _lastSell > currentStick.close
                    && _localLow < currentStick.close)
                {
                    // If last trade was volume sell and current stick is greater than last,
                    // but less than last sell, probably reached ATL: BUY volume sell
                    _trader.LogTradeSignal(SignalType.Volume, TradeType.VOLUMESELLBUYOFF, currentStick.close, currentStick.volume);

                    return TradeType.VOLUMESELLBUYOFF;
                }
                else if (volumePercentChange > _botSettings.mooningTankingPercent
                    && currentStick.open < currentStick.close
                    && lastStick.close < currentStick.close
                    && _lastSell > currentStick.close)
                {
                    // If volume increased more than N% and Latest close is greater than previous close
                    // Probably a mini-moon: BUY
                    _trader.LogTradeSignal(SignalType.Volume, TradeType.VOLUMEBUY, currentStick.close, currentStick.volume);

                    return TradeType.VOLUMEBUY;
                }
                else
                {
                        return TradeType.NONE;
                }
            }
            else
            {
                return TradeType.NONE;
            }

            return tradeType;
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
