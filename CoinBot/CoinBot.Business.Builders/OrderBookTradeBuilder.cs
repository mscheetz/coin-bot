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
        private DateTimeHelper _dtHelper = new DateTimeHelper();
        private ITradeBuilder _trader;
        private IFileRepository _fileRepo;
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
        private decimal _lastPrice = 0.00000000M;
        private int _samePriceCheck = 0;
        private bool _supportGotten = false;
        private bool _resistanceGotten = false;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="repo">Repository interface</param>
        public OrderBookTradeBuilder()
        {
            _trader = new TradeBuilder();
            _fileRepo = new FileRepository();
            SetupBuilder();
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="repo">Repository interface</param>
        public OrderBookTradeBuilder(ITradeBuilder trader)
        {
            _trader = trader;
            _fileRepo = new FileRepository();
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
            _fileRepo = new FileRepository();
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
        /// <param name="cycle">Int of cycles to run (default -1, run infinitely)</param>
        /// <param name="tradingStatus">Bool of trading status (default null, use setting)</param>
        /// <returns>Boolean when complete</returns>
        public bool RunBot(Interval interval, int cycle = -1, bool? tradingStatus = null)
        {
            _trader.SetupRepository();
            _tradeType = TradeType.BUY;
            bool currentlyTrading = tradingStatus != null ? (bool)tradingStatus : _currentlyTrading;
            currentlyTrading = GetCompetitionStatus(currentlyTrading);

            _trader.SetBalances();
            _tradeType = _trader.GetInitialTradeType();

            while (currentlyTrading)
            {
                Task.WaitAll(Task.Delay(_botSettings.priceCheck));
                if (cycle % _botSettings.traderResetInterval == 0)
                { // Every N cycles, reset balances and check bot settings file
                    _tradeType = _trader.GetTradingType();
                    _trader.UpdateBotSettings(_lastBuy, _lastSell);
                    SetBotSettings(_trader.GetBotSettings());
                    currentlyTrading = _tradeType == TradeType.NONE 
                        ? false 
                        :_botSettings.runBot;
                }
                currentlyTrading = GetCompetitionStatus(currentlyTrading);
                if (currentlyTrading)
                {
                    _supportGotten = false;
                    _resistanceGotten = false;

                    var tradeOpen = _trader.OpenOrdersCheck();

                    if (tradeOpen != null)
                    {
                        if (_botSettings.tradingCompetition)
                        {
                            tradeOpen = TradingCompetitionCheck(tradeOpen);
                        }
                        else
                        {
                            tradeOpen = CeilingFloorCheck(tradeOpen);
                        }
                    }

                    var stopLoss = !_botSettings.stopLossCheck ? false : StopLossCheck();

                    if (stopLoss)
                    {
                        tradeOpen = _trader.OpenOrdersCheck();
                    }

                    if (tradeOpen == null)
                    {
                        UpdateLastPrices();

                        _tradeType = _trader.GetTradingType();
                        CheckLastPrice();

                        if (_tradeType == TradeType.BUY)
                        {
                            BuyCryptoCheck();
                        }
                        else if (_tradeType == TradeType.SELL)
                        {
                            SellCryptoCheck();
                        }
                    }
                    cycle++;
                }
            }
            return true;
        }

        /// <summary>
        /// Get trading status based on competition end time
        /// </summary>
        /// <param name="currentlyTrading">Boolean of currently trading status</param>
        /// <returns>Boolean of currently trading status</returns>
        private bool GetCompetitionStatus(bool currentlyTrading)
        {
            if (currentlyTrading
                    && _botSettings.tradingCompetition
                    && _botSettings.tradingCompetitionEndTimeStamp > 0
                    && _botSettings.tradingCompetitionEndTimeStamp <= _dtHelper.UTCtoUnixTimeMilliseconds())
            {
                currentlyTrading = false;
            }

            return currentlyTrading;
        }

        /// <summary>
        /// Check if stop loss percent has been reached
        /// Sell if so
        /// </summary>
        /// <returns>Boolean true if hit, false otherwise</returns>
        private bool StopLossCheck()
        {
            if (_tradeType == TradeType.SELL)
            {
                var candleStick = _trader.GetCandlesticks(_botSettings.tradingPair, Interval.OneM, 1);

                var stopPrice = _lastBuy - (_lastBuy * (decimal)(_botSettings.stopLoss/100));

                if (stopPrice >= candleStick[0].close && _lastBuy > 0.00000000M)
                {
                    _trader.CancelOpenOrders();

                    candleStick = _trader.GetCandlesticks(_botSettings.tradingPair, Interval.OneM, 1);
                    _trader.SellCrypto(candleStick[0].close, TradeType.STOPLOSS);
                    var signal = new TradeSignal
                    {
                        lastBuy = _lastBuy,
                        lastSell = _lastSell,
                        pair = _symbol,
                        price = candleStick[0].close,
                        signal = SignalType.OrderBook,
                        tradeType = TradeType.STOPLOSS,
                        transactionDate = DateTime.UtcNow
                    };
                    _fileRepo.LogSignal(signal);
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// If the set orders have moved out of range, cancel open orders
        /// </summary>
        /// <returns>OpenOrderDetail when complete</returns>
        private OpenOrderDetail CeilingFloorCheck(OpenOrderDetail ooDetail)
        {
            var price = 0.00000000M;
            if (_tradeType == TradeType.SELL)
            {
                price = OrderBookSellPrice();
                _resistanceGotten = true;
            }
            else
            {
                price = OrderBookBuyPrice();
                _supportGotten = true;
            }

            if (price == 0.00000000M)
            {
                _trader.CancelOpenOrders();
                return null;
            }

            return ooDetail;
        }

        /// <summary>
        /// Check last price vs current price
        /// </summary>
        /// <returns>Boolean when complete</returns>
        private bool CheckLastPrice()
        {
            var currentPrice = 0.00000000M;
            if (_tradeType == TradeType.BUY)
            {
                currentPrice = OrderBookBuyPrice();
            }
            else
            {
                currentPrice = OrderBookSellPrice();
            }
            if (currentPrice == _lastPrice)
            {
                _samePriceCheck++;
            }
            else
            {
                _samePriceCheck = 0;
            }
            _lastPrice = currentPrice;

            return true;
        }

        /// <summary>
        /// If a trading competition, cancel open trade if outside 1st position
        /// </summary>
        /// <returns>OpenOrderDetail when complete</returns>
        private OpenOrderDetail TradingCompetitionCheck(OpenOrderDetail ooDetail)
        {
            var lastPrice = _tradeType == TradeType.BUY ? _lastSell : _lastBuy;
            var currentPrice = 0.00000000M;
            if (_tradeType == TradeType.SELL)
            {
                currentPrice = OrderBookSellPrice();
            }
            else
            {
                currentPrice = OrderBookBuyPrice();
            }
            _resistanceGotten = true;
            if(currentPrice == _lastPrice)
            {
                _samePriceCheck++;
            }
            else
            {
                _samePriceCheck = 0;
            }
            int? position = _trader.GetPricePostion(ooDetail.price);
            long unixTime = 0;
            long timeDiff = 0;
            if (_botSettings.openOrderTimeMS > 0)
            {
                unixTime = _dtHelper.UTCtoUnixTimeMilliseconds();
                timeDiff = ooDetail.timestamp + _botSettings.openOrderTimeMS;
            }
            if((position >= 3 && _samePriceCheck < 19) 
                || (unixTime > 0 && unixTime >= timeDiff && currentPrice != _lastPrice))
            {
                _lastPrice = currentPrice;
                _trader.CancelOpenOrders();
                return null;
            }
            _lastPrice = currentPrice;

            return ooDetail;
        }

        /// <summary>
        /// Get price and place a sell order
        /// </summary>
        /// <returns>Boolean when complete</returns>
        private bool SellCryptoCheck()
        {
            var price = OrderBookSellPrice();

            price = _trader.OrderBookSellCheck(price);

            if ((price != 0.00000000M //&& _lastBuy > 0.00000000M 
                && price >= _lastBuy) || _samePriceCheck >= _botSettings.samePriceLimit)
            {
                _trader.SellCrypto(price, TradeType.SELL, false);
                _tradeType = TradeType.BUY;
                _lastSell = price;
                var signal = new TradeSignal
                {
                    lastBuy = _lastBuy,
                    lastSell = _lastSell,
                    pair = _symbol,
                    price = price,
                    signal = SignalType.OrderBook,
                    tradeType = TradeType.ORDERBOOKSELL,
                    transactionDate = DateTime.UtcNow
                };
                _fileRepo.LogSignal(signal);
            }  

            return true;
        }

        /// <summary>
        /// Get price and place a buy order
        /// </summary>
        /// <returns>Boolean when complete</returns>
        private bool BuyCryptoCheck()
        {
            var price = OrderBookBuyPrice();

            price = _trader.OrderBookBuyCheck(price);

            if (price != 0.00000000M 
                && ((!_botSettings.tradingCompetition && price <= _lastSell) || _botSettings.tradingCompetition))
            {
                _trader.BuyCrypto(price, TradeType.BUY, false, false);
                _tradeType = TradeType.SELL;
                _lastBuy = price;
                var signal = new TradeSignal
                {
                    lastBuy = _lastBuy,
                    lastSell = _lastSell,
                    pair = _symbol,
                    price = price,
                    signal = SignalType.OrderBook,
                    tradeType = TradeType.ORDERBOOKBUY,
                    transactionDate = DateTime.UtcNow
                };
                _fileRepo.LogSignal(signal);
            }
            return true;
        }

        /// <summary>
        /// Update the latest buy sell prices
        /// </summary>
        /// <returns>Boolean when complete</returns>
        private bool UpdateLastPrices()
        {
            var prices = _trader.GetLastBuySellPrice();

            _lastBuy = prices[0];
            _lastSell = prices[1];

            return true;
        }

        /// <summary>
        /// Get buy price from order book
        /// </summary>
        /// <returns>Decimal of price</returns>
        private decimal OrderBookBuyPrice()
        {
            var getNew = _supportGotten ? false : true;
            _supportGotten = true;

            return _trader.GetSupport(getNew);
        }

        /// <summary>
        /// Get sell price from order book
        /// </summary>
        /// <returns>Decimal of price</returns>
        private decimal OrderBookSellPrice()
        {
            var getNew = _resistanceGotten ? false : true;
            _resistanceGotten = true;

            return _trader.GetResistance(getNew);
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
