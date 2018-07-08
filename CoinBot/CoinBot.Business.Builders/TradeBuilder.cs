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
    public class TradeBuilder : ITradeBuilder
    {
        #region Private Members

        private IFileRepository _fileRepo;
        private IExchangeBuilder _exchBldr;
        private DateTimeHelper _dtHelper = new DateTimeHelper();
        private Helper _helper = new Helper();
        private BotSettings _botSettings;
        private string _symbol;
        private string _asset;
        private string _pair;
        private List<BotBalance> _botBalances;
        private int _tradeNumber;
        private SignalType _signalType;
        private List<TradeInformation> _tradeInformation;
        private TradeInformation _lastTrade;
        private List<OpenOrder> _openOrderList;
        private List<OpenStopLoss> _openStopLossList;
        private decimal _lastBuy = 0.00000000M;
        private decimal _lastSell = 0.00000000M;
        private decimal _lastPrice = 0.00000000M;
        private decimal _lastQty = 0.00000000M;
        private TradeType _lastTradeType;
        private decimal _resistance = 0.00000000M;
        private decimal _support = 0.00000000M;

        #endregion Private Members

        #region Constructors

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="repo">Repository interface</param>
        public TradeBuilder()
        {
            _fileRepo = new FileRepository();
            _exchBldr = new ExchangeBuilder();
            SetupBuilder();
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="repo">Repository interface</param>
        public TradeBuilder(IFileRepository fileRepo)
        {
            _fileRepo = fileRepo;
            _exchBldr = new ExchangeBuilder();
            SetupBuilder();
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="repo">Repository interface</param>
        public TradeBuilder(IExchangeBuilder exchBldr)
        {
            _fileRepo = new FileRepository();
            _exchBldr = exchBldr;
            SetupBuilder();
        }

        /// <summary>
        /// Constructor for unit tests
        /// </summary>
        /// <param name="repo">Repository interface</param>
        public TradeBuilder(IFileRepository fileRepo, IExchangeBuilder exchBldr)
        {
            _fileRepo = fileRepo;
            _exchBldr = exchBldr;
            SetupBuilder();
        }

        /// <summary>
        /// Constructor for unit tests
        /// </summary>
        /// <param name="repo">Repository interface</param>
        public TradeBuilder(IFileRepository fileRepo, IExchangeBuilder exchBldr, List<BotBalance> botBalanceList)
        {
            _fileRepo = fileRepo;
            _exchBldr = exchBldr;
            SetupBuilder(botBalanceList);
        }

        #endregion Constructors

        #region Builder Setup

        private void SetupBuilder()
        {
            _botSettings = GetBotSettings();
            _botBalances = new List<BotBalance>();
            _tradeInformation = new List<TradeInformation>();
            _openOrderList = new List<OpenOrder>();
            _openStopLossList = new List<OpenStopLoss>();
            _exchBldr.SetExchange(_botSettings);
            SetupRepository();
        }

        /// <summary>
        /// Setup Builder for unit tests
        /// </summary>
        /// <param name="botBalanceList"></param>
        private void SetupBuilder(List<BotBalance> botBalanceList)
        {
            SetupBuilder();
            _botBalances = botBalanceList;
        }

        #endregion Builder Setup

        #region Settings Management

        /// <summary>
        /// Check if config file exists
        /// </summary>
        /// <returns>Boolean of result</returns>
        public bool ConfigFileExits()
        {
            return _fileRepo.ConfigExists();
        }

        /// <summary>
        /// Check if settings file exists
        /// </summary>
        /// <returns>Boolean of result</returns>
        public bool SettingsFileExists()
        {
            return _fileRepo.BotSettingsExists();
        }

        /// <summary>
        /// Get BotSettings
        /// </summary>
        /// <returns>BotSettings object</returns>
        public BotSettings GetBotSettings()
        {
            if (_botSettings == null)
                LoadBotSettingsFile();

            return _botSettings;
        }

        /// <summary>
        /// Load bot settings from disk
        /// </summary>
        public void LoadBotSettingsFile()
        {
            _botSettings = _fileRepo.GetSettings();
            _symbol = _botSettings.tradingPair;
            switch(_botSettings.tradingStrategy)
            {
                case Strategy.OrderBook:
                    _signalType = SignalType.OrderBook;
                    break;
                case Strategy.Percentage:
                    _signalType = SignalType.Percent;
                    break;
                case Strategy.Volume:
                    _signalType = SignalType.Volume;
                    break;
                default:
                    _signalType = SignalType.None;
                    break;
            }
            GetAssetAndPair();
        }

        /// <summary>
        /// Update bot settings from file
        /// </summary>
        /// <param name="_lastBuy">Last buy value</param>
        /// <param name="_lastSell">Last sell value</param>
        /// <returns>Boolean when complete</returns>
        public bool UpdateBotSettings(decimal _lastBuy, decimal _lastSell)
        {
            var settings = _fileRepo.GetSettings();
            settings.lastBuy = _lastBuy;
            settings.lastSell = _lastSell;
            _botSettings = settings;
            _symbol = settings.tradingPair;
            GetAssetAndPair();

            return true;
        }

        /// <summary>
        /// Set BotSettings in builder
        /// </summary>
        /// <param name="settings">Updated Settings</param>
        /// <returns>Boolean when complete</returns>
        public bool SetBotSettings(BotSettings settings)
        {
            var updatedSettings = new BotSettings
            {
                buyPercent = _botSettings.buyPercent,
                chartInterval = _botSettings.chartInterval,
                exchange = _botSettings.exchange,
                lastBuy = _botSettings.lastBuy,
                lastSell = _botSettings.lastSell,
                mooningTankingPercent = _botSettings.mooningTankingPercent,
                mooningTankingTime = _botSettings.mooningTankingTime,
                orderBookQuantity = _botSettings.orderBookQuantity,
                priceCheck = _botSettings.priceCheck,
                sellPercent = _botSettings.sellPercent,
                startBotAutomatically = _botSettings.startBotAutomatically,
                startingAmount = _botSettings.startingAmount,
                stopLoss = _botSettings.stopLoss,
                stopLossCheck = _botSettings.stopLossCheck,
                tradePercent = _botSettings.tradePercent,
                tradeValidationCheck = _botSettings.tradeValidationCheck,
                traderResetInterval = _botSettings.traderResetInterval,
                tradingFee = _botSettings.tradingFee,
                tradingPair = _botSettings.tradingPair,
                tradingStatus = _botSettings.tradingStatus,
                tradingStrategy = _botSettings.tradingStrategy,
                runBot = _botSettings.runBot
            };

            if (settings.buyPercent > 0)
                updatedSettings.buyPercent = settings.buyPercent;
            if (settings.chartInterval != Interval.None)
                updatedSettings.chartInterval = settings.chartInterval;
            if (settings.exchange != Exchange.NONE)
                updatedSettings.exchange = settings.exchange;
            if (settings.lastBuy > 0)
                updatedSettings.lastBuy = settings.lastBuy;
            if (settings.lastSell > 0)
                updatedSettings.lastSell = settings.lastSell;
            if (settings.mooningTankingTime > 0)
                updatedSettings.mooningTankingTime = settings.mooningTankingTime;
            if (settings.orderBookQuantity > 0)
                updatedSettings.orderBookQuantity = settings.orderBookQuantity;
            if (settings.priceCheck > 0)
                updatedSettings.priceCheck = settings.priceCheck;
            if (settings.sellPercent > 0)
                updatedSettings.sellPercent = settings.sellPercent;
            if (settings.startBotAutomatically != null)
                updatedSettings.startBotAutomatically = settings.startBotAutomatically;
            if (settings.startingAmount > 0)
                updatedSettings.startingAmount = settings.startingAmount;
            if (settings.stopLoss > 0)
                updatedSettings.stopLoss = settings.stopLoss;
            if (settings.tradePercent > 0)
                updatedSettings.tradePercent = settings.tradePercent;
            if (settings.traderResetInterval > 0)
                updatedSettings.traderResetInterval = settings.traderResetInterval;
            if (settings.tradeValidationCheck > 0)
                updatedSettings.tradeValidationCheck = settings.tradeValidationCheck;
            if (!string.IsNullOrEmpty(settings.tradingPair))
                updatedSettings.tradingPair = settings.tradingPair;
            if (settings.tradingStatus != TradeStatus.None)
                updatedSettings.tradingStatus = settings.tradingStatus;
            if (settings.tradingStrategy != Strategy.None)
                updatedSettings.tradingStrategy = settings.tradingStrategy;


            updatedSettings.mooningTankingPercent = settings.mooningTankingPercent;
            updatedSettings.runBot = settings.runBot;
            updatedSettings.stopLossCheck = settings.stopLossCheck;
            updatedSettings.tradingCompetition = settings.tradingCompetition;
            updatedSettings.tradingFee = settings.tradingFee;

            _fileRepo.UpdateBotSettings(updatedSettings);
            _botSettings = updatedSettings;
            _symbol = _botSettings.tradingPair;
            GetAssetAndPair();

            return true;
        }

        /// <summary>
        /// Set up respository
        /// </summary>
        /// <returns>Boolean when complete</returns>
        public bool SetupRepository()
        {
            var repoReady = _exchBldr.ValidateExchangeConfigured(_botSettings.exchange);

            if (repoReady)
                return true;

            var apiInfo = GetApiInformation();

            _exchBldr.SetExchangeApi(apiInfo);

            return true;
        }

        /// <summary>
        /// Set api information
        /// </summary>
        /// <param name="apiInformation">Updated ApiInformation</param>
        /// <returns>Boolean when complete</returns>
        public bool SetApiInformation(ApiInformation apiInformation)
        {
            var result = _fileRepo.SetConfig(apiInformation);

            return result;
        }

        /// <summary>
        /// Get Api Key from disc
        /// </summary>
        /// <returns>String of api key</returns>
        public string GetApiKey()
        {
            var apiInfo = _fileRepo.GetConfig();

            return apiInfo != null ? apiInfo.apiKey : string.Empty;
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
                apiSecret = config.apiSecret,
                extraValue = config.extraValue
            };

            return apiInfo;
        }

        /// <summary>
        /// Update balances and get initial trade type
        /// </summary>
        /// <returns>TradeType value</returns>
        public TradeType GetInitialTradeType()
        {
            return GetTradingType(true);
        }

        /// <summary>
        /// Update balances and get current trade type
        /// </summary>
        /// <param name="logBalances">Write balances to log?</param>
        /// <returns>TradeType value</returns>
        public TradeType GetTradingType(bool logBalances = false)
        {
            SetBalances(logBalances);
            var assetQty = _botBalances.Where(b => b.symbol.Equals(_asset)).Select(b => b.quantity).FirstOrDefault();
            var pairQty = _botBalances.Where(b => b.symbol.Equals(_pair)).Select(b => b.quantity).FirstOrDefault();

            if ((_pair == "USD" || _pair == "USDT")
                && pairQty < 10.0M)
            {
                return TradeType.SELL;
            }
            else if ((_pair == "BTC" || _pair == "ETH")
                && pairQty < 0.0002M)
            {
                return TradeType.SELL;
            }
            else
            {
                return TradeType.BUY;
            }
        }

        #endregion Settings Management

        #region Trade History

        /// <summary>
        /// Get all transactions
        /// </summary>
        /// <param name="transactionCount">Count of transactions to return</param>
        /// <returns>Collection of TradeInformation</returns>
        public IEnumerable<TradeInformation> GetTradeHistory(int transactionCount)
        {
            var tradeList = _fileRepo.GetTransactions();

            return tradeList.Skip(Math.Max(0, tradeList.Count - transactionCount));
        }

        #endregion Trade History

        #region Signal History

        /// <summary>
        /// Get all trade signals
        /// </summary>
        /// <param name="signalCount">Count of trade signals to return</param>
        /// <returns>Collection of TradeSignal objects</returns>
        public IEnumerable<TradeSignal> GetSignalHistory(int signalCount)
        {
            var signalList = _fileRepo.GetSignals();

            return signalList.Skip(Math.Max(0, signalList.Count - signalCount));
        }

        #endregion Signal History

        #region Balance Managers
        /// <summary>
        /// Get current balance
        /// </summary>
        /// <param name="recordsToReturn">Number of records to return (default 1)</param>
        /// <returns>BotBalance object</returns>
        public IEnumerable<List<BotBalance>> GetBalance(int recordsToReturn = 1)
        {
            var balanceList = _fileRepo.GetBalances();
            
            if(recordsToReturn == 0)
            {
                return balanceList;
            }

            return balanceList.Skip(Math.Max(0, balanceList.Count - recordsToReturn));
        }

        /// <summary>
        /// Get Balance history
        /// </summary>
        /// <returns>Collection of BotBalance objects</returns>
        public IEnumerable<IEnumerable<BotBalance>> GetBalanceHistory()
        {
            return _fileRepo.GetBalances();
        }

        /// <summary>
        /// Get current balances
        /// </summary>
        /// <returns>Collection of BotBallance objects</returns>
        public List<BotBalance> GetBotBalance()
        {
            return _botBalances;
        }

        /// <summary>
        /// Set balances
        /// </summary>
        /// <param name="logBalance">Log the balance bool</param>
        public void SetBalances(bool logBalance = true)
        {
            _botBalances = new List<BotBalance>();

            var balances = GetBalances(_botSettings.startingAmount);

            for (var i = 0; i < balances.Count; i++)
            {
                var botBalance = new BotBalance
                {
                    symbol = balances[i].asset,
                    quantity = balances[i].free + balances[i].locked,
                    timestamp = DateTime.UtcNow
                };

                _botBalances.Add(botBalance);
            }

            if (logBalance)
            {
                LogBalances();
            }
        }
        
        /// <summary>
        /// Get paper balances available
        /// </summary>
        /// <param name="startingAmount">Starting amount for paper trading (default = 0)</param>
        /// <returns>Collection of balance objects</returns>
        public List<Balance> GetPaperBalances(decimal startingAmount = 0)
        {
            var symbol = _botSettings.tradingPair;

            var balances = new List<Balance>();
            decimal pairQuantity = 0;
            decimal assetQuantity = 0;

            if (startingAmount > 0)
            {
                pairQuantity = startingAmount;
            }
            else
            {
                if (_lastTradeType == TradeType.BUY)
                {
                    pairQuantity = 0;
                    assetQuantity = _lastQty;
                }
                else if (_lastTradeType == TradeType.SELL)
                {
                    pairQuantity = _lastQty;
                    assetQuantity = 0;
                }
            }

            balances.Add(
                new Balance
                {
                    asset = _pair,
                    free = pairQuantity,
                    locked = 0
                });

            balances.Add(
                new Balance
                {
                    asset = _asset,
                    free = assetQuantity,
                    locked = 0
                });

            return balances;
        }

        /// <summary>
        /// Get balances available
        /// </summary>
        /// <param name="startingQuantity">Starting quantity (for paper trading, default 0)</param>
        /// <returns>Collection of balance objects</returns>
        public List<Balance> GetBalances(decimal startingQuantity = 0M)
        {
            var balances = new List<Balance>();

            if (_botSettings.tradingStatus == TradeStatus.LiveTrading)
            {
                balances = _exchBldr.GetBalance(_asset, _pair);
            }
            else if (_botSettings.tradingStatus == TradeStatus.PaperTrading)
            {
                balances = GetPaperBalances(startingQuantity);
            }
            return balances;
        }

        /// <summary>
        /// Update balances from exchange
        /// </summary>
        public void UpdateBalances()
        {
            _botBalances = new List<BotBalance>();

            var balances = GetBalances();

            for (var i = 0; i < balances.Count; i++)
            {
                var botBalance = new BotBalance
                {
                    symbol = balances[i].asset,
                    quantity = balances[i].free + balances[i].locked,
                    timestamp = DateTime.UtcNow
                };

                _botBalances.Add(botBalance);
            }

            LogBalances();
        }
        #endregion Balance Managers

        #region Logging
        /// <summary>
        /// Log balances to file
        /// </summary>
        /// <returns>Boolean when complete</returns>
        public bool LogBalances()
        {
            return _fileRepo.LogBalances(_botBalances);
        }

        /// <summary>
        /// Log Trades
        /// </summary>
        /// <param name="tradeInformation">New TradeInformation object</param>
        /// <returns></returns>
        public bool LogTransaction(TradeInformation tradeInformation)
        {
            return _fileRepo.LogTransaction(tradeInformation);
        }

        /// <summary>
        /// Write a signal to file
        /// </summary>
        /// <param name="signal">Signal to log</param>
        /// <returns>Boolean when complete</returns>
        public bool LogTradeSignal(SignalType signalType, TradeType tradeType, decimal price, decimal volume = 0M)
        {
            var signal = new TradeSignal
            {
                bandLower = 0M,
                bandUpper = 0M,
                currentVolume = volume,
                lastBuy = _lastBuy,
                lastSell = _lastSell,
                pair = _symbol,
                price = price,
                signal = signalType,
                tradeType = tradeType,
                transactionDate = DateTime.UtcNow
            };
            return _fileRepo.LogSignal(signal);
        }

        #endregion Logging

        #region Candlesticks

        /// <summary>
        /// Get Candlesticks from exchange API
        /// </summary>
        /// <param name="symbol">Trading symbol</param>
        /// <param name="interval">Candlestick Interval</param>
        /// <param name="range">Number of candlesticks to get</param>
        /// <returns>Array of BotStick objects</returns>
        public BotStick[] GetCandlesticks(string symbol, Interval interval, int range)
        {
            var candleSticks = _exchBldr.GetCandlesticks(symbol, interval, range);

            while(candleSticks == null || candleSticks.Count() == 0)
            {
                candleSticks = GetCandlesticks(symbol, interval, range);
            }

            return candleSticks;
        }

        #endregion Candlesticks

        #region OrderBook

        /// <summary>
        /// Get order book position of a price
        /// </summary>
        /// <param name="price">Decimal of price to find</param>
        /// <returns>Int of position in order book</returns>
        public int? GetPricePostion(decimal price)
        {
            return _exchBldr.GetPricePosition(_symbol, price);
        }

        /// <summary>
        /// Get next resistance level if within 3 spots of bottom resistance
        /// </summary>
        /// <param name="getNew">Boolean to get a new value</param>
        /// <returns>Decimal of next resistance</returns>
        public decimal GetResistance(bool getNew = false)
        {
            if (getNew)
            {
                var detail = _exchBldr.GetResistance(_symbol, _botSettings.orderBookQuantity);
                var resistance = detail.price;
                var places = detail.position;
                var limit = _botSettings.tradingCompetition ? 1 : 3;
                var sellPrice = places == 0
                    ? resistance
                    : resistance - _helper.DecimalValueAtPrecision(detail.precision);

                _resistance = places <= limit ? sellPrice : 0.00000000M; ;
            }

            return _resistance;
        }

        /// <summary>
        /// Get next support level if within 3 spots of top support level
        /// </summary>
        /// <param name="getNew">Boolean to get a new value</param>
        /// <returns>Decimal of next support</returns>
        public decimal GetSupport(bool getNew = false)
        {
            if (getNew)
            {
                var detail = _exchBldr.GetSupport(_symbol, _botSettings.orderBookQuantity);
                var support = detail.price;
                var places = detail.position;
                var limit = _botSettings.tradingCompetition ? 1 : 3;
                var buyPrice = places == 0
                    ? support
                    : support + _helper.DecimalValueAtPrecision(detail.precision);

                _support = places <= limit ? buyPrice : 0.00000000M;
            }

            return _support;
        }

        #endregion OrderBook

        #region Place Trade

        /// <summary>
        /// Place a trade
        /// </summary>
        /// <param name="tradeParams">Trade parameters</param>
        /// <returns>TradeResponse object</returns>
        public TradeResponse PlaceTrade(TradeParams tradeParams)
        {
            _tradeNumber++;
            if (_botSettings.tradingStatus == TradeStatus.LiveTrading)
                return _exchBldr.PlaceTrade(tradeParams);
            else if (_botSettings.tradingStatus == TradeStatus.PaperTrading)
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
                clientOrderId = $"PaperTrade_{_tradeNumber}",
                executedQty = tradeParams.quantity,
                orderId = _tradeNumber,
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

        #endregion Place Trade

        #region Stop Loss Management

        /// <summary>
        /// Check if Stop Loss Hit
        /// </summary>
        /// <param name="currentPrice">Current price of coin</param>
        /// <returns>Nullable decimal value of stop loss</returns>
        public decimal? StoppedOutCheck(decimal currentPrice)
        {
            if (_openStopLossList.Count == 0 || currentPrice >= _openStopLossList[0].price)
                return null;

            var trade = new TradeResponse
            {
                orderId = _openStopLossList[0].orderId,
                clientOrderId = _openStopLossList[0].clientOrderId
            };

            var stoppedOut = CheckTradeStatus(trade);

            if (stoppedOut)
            {
                ProcessStopLoss();
            }

            return _lastSell;
        }

        /// <summary>
        /// Process a stop loss
        /// </summary>
        /// <returns>Boolean when complete</returns>
        public bool ProcessStopLoss()
        {
            _lastSell = _openStopLossList[0].price;
            var slQty = _openStopLossList[0].quantity;
            _openStopLossList.RemoveAt(0);

            _lastTrade = new TradeInformation
            {
                pair = _symbol,
                price = _lastSell,
                quantity = slQty,
                timestamp = DateTime.UtcNow,
                tradeType = EnumHelper.GetEnumDescription((TradeType)TradeType.STOPLOSS)
            };
            _tradeInformation.Add(_lastTrade);

            LogTransaction(_lastTrade);

            _lastQty = GetTradeQuantity(TradeType.SELL, _lastSell);

            UpdateBalances();

            return true;
        }

        #endregion Stop Loss Management

        #region Buy/Sell

        /// <summary>
        /// Buy crypto
        /// </summary>
        /// <param name="orderPrice">Buy price</param>
        /// <param name="tradeType">Trade Type</param>
        /// <param name="stopLoss">Place stoploss? default false</param>
        /// <param name="validateTrade">Validated trade complete, default = true</param>
        /// <returns>Boolean when complete</returns>
        public bool BuyCrypto(decimal orderPrice, TradeType tradeType, bool stopLoss = false, bool validateTrade = true)
        {
            var tradeComplete = false;
            int i = 0;
            TradeResponse trade = null;
            while (!tradeComplete && i < 2)
            {
                trade = MakeTrade(TradeType.BUY, orderPrice);

                if (trade == null || trade.clientOrderId == null)
                {
                    return false;
                }

                if (validateTrade)
                {
                    tradeComplete = ValidateTradeComplete(trade);
                }
                else
                { 
                    tradeComplete = true;
                }

                if (i == 1 && !tradeComplete)
                {
                    return false;
                }

                i++;
                if(!tradeComplete) // If trade was not filled, try at a lower price
                {
                    orderPrice = orderPrice - 0.01M;
                }
            }

            if(!validateTrade)
            {
                return true;
            }

            UpdateBalances();

            CaptureTransaction(orderPrice, trade.origQty, trade.transactTime, tradeType);

            _lastBuy = orderPrice;

            if (stopLoss)
            {
                var stopLossResponse = PlaceStopLoss(orderPrice, trade.origQty);
            }

            return CheckTradeSuccess(TradeType.BUY);
        }

        /// <summary>
        /// Sell crypto
        /// </summary>
        /// <param name="orderPrice">Current price</param>
        /// <param name="tradeType">Trade Type</param>
        /// <param name="validateTrade">Validate trade is complete, default = true</param>
        /// <returns>Boolean when complete</returns>
        public bool SellCrypto(decimal orderPrice, TradeType tradeType, bool validateTrade = true)
        {
            var tradeComplete = false;
            int i = 0;
            TradeResponse trade = null;
            while (!tradeComplete && i < 2)
            {
                CancelStopLoss();

                trade = MakeTrade(TradeType.SELL, orderPrice);

                if (trade == null || trade.clientOrderId == null)
                {
                    return false;
                }

                if (validateTrade)
                {
                    tradeComplete = ValidateTradeComplete(trade);
                }
                else
                {
                    tradeComplete = true;
                }

                if(tradeComplete)
                {
                    break;
                }
                else if(i == 1 && !tradeComplete)
                {
                    return false;
                }

                i++;
                if (!tradeComplete) // If trade was not filled, try at a higher price
                {
                    orderPrice = orderPrice + 0.01M;
                }
            }
            if(!validateTrade)
            {
                return true;
            }

            UpdateBalances();

            CaptureTransaction(orderPrice, trade.origQty, trade.transactTime, tradeType);

            _lastSell = orderPrice;

            return CheckTradeSuccess(TradeType.SELL);
        }

        /// <summary>
        /// Check success of trade based on updated balances
        /// </summary>
        /// <param name="type">TradeType executed</param>
        /// <returns>Boolean if trade was successful</returns>
        public bool CheckTradeSuccess(TradeType type)
        {
            var pairBalance = _botBalances.Where(b => b.symbol.Equals(_pair)).FirstOrDefault();
            var assetBalance = _botBalances.Where(b => b.symbol.Equals(_asset)).FirstOrDefault();
            if (pairBalance == null)
            {
                return false;
            }
            if (type == TradeType.BUY)
            {
                return pairBalance.quantity < 10.0M ? true : false;
            }
            else
            {
                return pairBalance.quantity > 10.0M ? true : false;
            }
        }

        /// <summary>
        /// Capture the current transaction and log it
        /// </summary>
        /// <param name="price">Transaction price</param>
        /// <param name="quantity">Transaction quantity</param>
        /// <param name="timeStamp">Transaction time</param>
        /// <param name="tradeType">Transaction TradeType</param>
        /// <returns>Boolean when complete</returns>
        public bool CaptureTransaction(decimal price, decimal quantity, long timeStamp, TradeType tradeType)
        {
            _lastTrade = new TradeInformation
            {
                pair = _symbol,
                price = price,
                quantity = quantity,
                timestamp = DateTime.UtcNow,
                tradeType = EnumHelper.GetEnumDescription((TradeType)tradeType)
            };

            _tradeInformation.Add(_lastTrade);

            LogTransaction(_lastTrade);

            return true;
        }

        #endregion Buy/Sell

        #region Validate Trade

        /// <summary>
        /// Validate that a placed trade was complete
        /// </summary>
        /// <param name="trade">TradeReponse object</param>
        /// <returns>Boolean value on trade validation</returns>
        public bool ValidateTradeComplete(TradeResponse trade)
        {
            bool tradeComplete = false;
            int i = 0;
            while (!tradeComplete || i > 2)
            {
                i++;
                tradeComplete = CheckTradeStatus(trade);
                if (tradeComplete)
                {
                    break;
                }
                if (!tradeComplete && i < 2)
                {
                    Task.WaitAll(Task.Delay(_botSettings.tradeValidationCheck));
                }
                else if (!tradeComplete && i == 2)
                {
                    var cancelTradeParams = new CancelTradeParams
                    {
                        orderId = trade.orderId,
                        origClientOrderId = trade.clientOrderId,
                        symbol = trade.symbol,
                        timestamp = trade.transactTime,
                        type = trade.side.ToString()
                    };
                    CancelTrade(cancelTradeParams);

                    return false;
                }
            }

            return tradeComplete;
        }

        /// <summary>
        /// Check status of placed trade
        /// </summary>
        /// <param name="orderId">OrderId of trade</param>
        /// <returns>Boolean value of filled status</returns>
        public bool CheckTradeStatus(TradeResponse trade)
        {
            var orderStatus = GetOrderStatus(trade);

            if (orderStatus == null)
                return false;

            return orderStatus.status == OrderStatus.FILLED ? true : false;
        }

        #endregion Validate Trade

        #region Pair and Quantity

        /// <summary>
        /// Get current asset
        /// </summary>
        /// <returns>String of asset</returns>
        public string GetAsset()
        {
            return _asset;
        }

        /// <summary>
        /// Get current trading pair
        /// </summary>
        /// <returns>String of pair</returns>
        public string GetPair()
        {
            return _pair;
        }

        /// <summary>
        /// Get Asset and Pair from symbol
        /// </summary>
        public void GetAssetAndPair()
        {
            if (_symbol.Contains("USDT"))
            {
                _asset = _symbol.Replace("USDT", "");
                _pair = "USDT";
            }
            else if (_symbol.Contains("USD"))
            {
                _asset = _symbol.Replace("USD", "");
                _pair = "USD";
            }
            else if (_symbol.Contains("BTC"))
            {
                _asset = _symbol.Replace("BTC", "");
                _pair = "BTC";
            }
            else if (_symbol.Contains("ETH"))
            {
                _asset = _symbol.Replace("ETH", "");
                _pair = "ETH";
            }
            else if (_symbol.Contains("NEO"))
            {
                _asset = _symbol.Replace("NEO", "");
                _pair = "NEO";
            }
            else if (_symbol.Contains("BNB"))
            {
                _asset = _symbol.Replace("BNB", "");
                _pair = "BNB";
            }
        }

        /// <summary>
        /// Get quantity to trade based
        /// </summary>
        /// <param name="tradeType">TradeType object</param>
        /// <param name="orderPrice">Requested trade price</param>
        /// <returns>decimal of quantity to purchase</returns>
        public decimal GetTradeQuantity(TradeType tradeType, decimal orderPrice)
        {
            int roundTo = 1;
            decimal quantity = 0.00000000M;
            if (tradeType == TradeType.BUY)
            {
                var pairBalance = _botBalances.Where(b => b.symbol.Equals(_pair)).FirstOrDefault();

                quantity = pairBalance.quantity / orderPrice;
            }
            else if (tradeType == TradeType.SELL)
            {
                var symbolBalance = _botBalances.Where(b => b.symbol.Equals(_asset)).FirstOrDefault();

                quantity = symbolBalance.quantity;
                if (_asset.Equals("BTC"))
                {
                    quantity = _helper.RoundDown(quantity, 6);
                }
            }
            
            if(_asset.Equals("BTC"))
            {
                roundTo = 6;
            }

            if(_botSettings.exchange == Exchange.GDAX)
            {
                // Remove potential trade fee for GDAX trades
                quantity = quantity - (quantity * _botSettings.tradingFee);
            }

            decimal roundedDown = _helper.RoundDown(quantity, roundTo);

            return roundedDown;
        }

        #endregion Pair and Quantity

        #region Moon and Tank Check
        
        /// <summary>
        /// Check if mooning
        /// </summary>
        /// <param name="startingPrice">decimal of starting price</param>
        /// <param name="prevStick">Previous BotStick (default null)</param>
        /// <param name="iteration">Int of iteration</param>
        /// <returns>decimal of sell price</returns>
        public decimal OrderBookSellCheck(decimal startingPrice = 0.00000000M
                                    , BotStick prevStick = null
                                    , int iteration = 0)
        {
            var checkToSell = true;
            decimal sellPrice = 0.00000000M;
            while (checkToSell)
            {
                var sticks = GetNextCandlestick();
                var currentStick = sticks[0];
                var lastStick = sticks[1];

                if (prevStick == null)
                {
                    prevStick = lastStick;
                }

                if ((startingPrice > 0.00000000M && startingPrice < currentStick.close)
                    && currentStick.open < currentStick.close
                    && prevStick.close < currentStick.close)
                {
                    // TODO: set the latest price as the starting price to see if it is increasing during current candle
                    // TODO: do same on tanking
                    // If current price is greater than the previous check 
                    //  (price is increasing)
                    // and sell percent reached
                    // keep checking if increasing more
                    iteration++;
                    //sellPrice = OrderBookSellCheck(startingPrice, lastStick, iteration);
                    prevStick = lastStick;
                }
                else
                {
                    sellPrice = iteration == 0 ? startingPrice : currentStick.close;
                    checkToSell = false;
                }
            }

            return sellPrice;
        }

        /// <summary>
        /// Check if tanking
        /// </summary>
        /// <param name="startingPrice">decimal of starting price</param>
        /// <param name="prevStick">Previous BotStick (default null)</param>
        /// <param name="iteration">Int of iteration</param>
        /// <returns>decimal of buy price</returns>
        public decimal OrderBookBuyCheck(decimal startingPrice = 0.00000000M
                                    , BotStick prevStick = null
                                    , int iteration = 0)
        {
            var checkToBuy = true;
            decimal buyPrice = 0.00000000M;
            while (checkToBuy)
            {
                var sticks = GetNextCandlestick();

                var currentStick = sticks[0];
                var lastStick = sticks[1];

                if (prevStick == null)
                {
                    prevStick = lastStick;
                }

                if ((startingPrice > 0.00000000M && startingPrice > currentStick.close)
                    && currentStick.open > currentStick.close
                    && lastStick.close > currentStick.close
                    && prevStick.close > currentStick.close)
                {
                    // If current price is less than the previous check 
                    //  (price is dropping)
                    // and buy percent reached
                    // keep checking if dropping more
                    iteration++;
                    //buyPrice = OrderBookBuyCheck(startingPrice, lastStick, iteration);
                    prevStick = lastStick;
                }
                else
                {
                    buyPrice = iteration == 0 ? startingPrice : currentStick.close;
                    checkToBuy = false;
                }
            }
            return buyPrice;
        }

        /// <summary>
        /// Get next candlestick
        /// </summary>
        /// <param name="interval">Trade interval, default 1 minute</param>
        /// <param name="stickCount">Int of sticks to return, default 2</param>
        /// <returns>Candlestick object</returns>
        public BotStick[] GetNextCandlestick()
        {
            Task.WaitAll(Task.Delay(_botSettings.mooningTankingTime));

            var candlesticks = GetCandlesticks(_symbol, _botSettings.chartInterval, 2);

            return candlesticks;
        }
        
        #endregion Moon and Tank Check

        #region Stop Loss Management

        /// <summary>
        /// Get all open stop losses
        /// </summary>
        /// <returns>Collection of OpenStopLoss</returns>
        public IEnumerable<OpenStopLoss> GetStopLosses()
        {
            return _openStopLossList;
        }

        /// <summary>
        /// Cancel a stop loss
        /// </summary>
        /// <returns>Boolean value when complete</returns>
        public bool CancelStopLoss()
        {
            if (_openStopLossList == null || _openStopLossList.Count == 0)
                return true;

            var tradeParams = new CancelTradeParams
            {
                symbol = _symbol,
                orderId = _openStopLossList[0].orderId,
                origClientOrderId = _openStopLossList[0].clientOrderId,
                type = "SELL"
            };

            var result = CancelTrade(tradeParams);

            var trade = new TradeResponse
            {
                orderId = _openStopLossList[0].orderId,
                clientOrderId = _openStopLossList[0].clientOrderId
            };
            bool stopLossCanceled = false;
            while (!stopLossCanceled)
            {
                stopLossCanceled = CheckTradeStatus(trade);
            }

            _openStopLossList.RemoveAt(0);

            return stopLossCanceled;
        }

        /// <summary>
        /// Place a stop loss
        /// </summary>
        /// <param name="orderPrice">Trade price</param>
        /// <param name="quantity">Quantity to trade</param>
        /// <returns>TradeResponse object</returns>
        public TradeResponse PlaceStopLoss(decimal orderPrice, decimal quantity)
        {
            decimal stopLossPercent = (decimal)Math.Abs(_botSettings.stopLoss) / 100;
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
            
            _openStopLossList.Add(
                new OpenStopLoss
                {
                    symbol = _symbol,
                    clientOrderId = response.clientOrderId,
                    orderId = response.orderId,
                    price = response.price,
                    quantity = quantity
                });

            return response;
        }

        #endregion Stop Loss Management

        #region Trade Management

        /// <summary>
        /// Make a trade
        /// make 5 trade attempts if response comes back null
        /// </summary>
        /// <param name="tradeType">TradeType object</param>
        /// <param name="orderPrice">Trade price</param>
        /// <returns>TradeResponse object</returns>
        public TradeResponse MakeTrade(TradeType tradeType, decimal orderPrice)
        {
            TradeResponse response = null;
            int i = 0;
            while (response == null && i < 5)
            {
                //if (i > 0)
                //{
                //    orderPrice = GetPricePadding(tradeType, orderPrice);
                //}

                var quantity = GetTradeQuantity(tradeType, orderPrice);
                if(i > 0)
                {
                    quantity = quantity - i;
                }
                _lastPrice = orderPrice;
                _lastQty = quantity;
                _lastTradeType = tradeType;

                var trade = new TradeParams
                {
                    price = orderPrice,
                    symbol = _symbol,
                    side = tradeType.ToString(),
                    type = OrderType.LIMIT.ToString(),
                    quantity = quantity,
                    timeInForce = "GTC"
                };

                response = PlaceTrade(trade);
                i++;
            }

            return response;
        }

        /// <summary>
        /// Get price padding to avoid GDAX transaction fees
        /// </summary>
        /// <param name="tradeType">Current trade type</param>
        /// <param name="orderPrice">Order price</param>
        /// <returns>Update order price</returns>
        public decimal GetPricePadding(TradeType tradeType, decimal orderPrice)
        {
            if (_botSettings.exchange == Exchange.GDAX)
            {
                var pricePadding = 0.00M;

                if (tradeType == TradeType.BUY || tradeType == TradeType.VOLUMEBUY || tradeType == TradeType.VOLUMESELLBUYOFF)
                {
                    pricePadding = -0.01M;
                }
                else if(tradeType == TradeType.SELL || tradeType == TradeType.VOLUMESELL || tradeType == TradeType.VOLUMEBUYSELLOFF)
                {
                    pricePadding = 0.01M;
                }

                orderPrice += pricePadding;
            }

            return orderPrice;
        }

        /// <summary>
        /// Cancel trade
        /// </summary>
        /// <param name="orderId">OrderId to cancel</param>
        /// <param name="origClientOrderId">ClientOrderId to cancel</param>
        /// <param name="tradeType">Trade type</param>
        public void CancelTrade(long orderId, string origClientOrderId, string tradeType = "")
        {
            var tradeParams = new CancelTradeParams
            {
                symbol = _symbol,
                orderId = orderId,
                origClientOrderId = origClientOrderId,
                type = tradeType
            };

            var response = CancelTrade(tradeParams);
        }

        /// <summary>
        /// Cancel all open orders for the current trading pair
        /// </summary>
        /// <returns>Boolen when complete</returns>
        public bool CancelOpenOrders()
        {
            var openOrders = _exchBldr.GetOpenOrders(_symbol);

            while (openOrders != null && openOrders.Count() > 0)
            {
                for (var i = 0; i < openOrders.Length; i++)
                {
                    var signal = new TradeSignal
                    {
                        pair = _symbol,
                        price = openOrders[i].price,
                        signal = _signalType,
                        tradeType = TradeType.CANCELTRADE,
                        transactionDate = DateTime.UtcNow,
                    };
                    _fileRepo.LogSignal(signal);
                    CancelTrade(openOrders[i].orderId, openOrders[i].clientOrderId, openOrders[i].side.ToString());
                }
                openOrders = _exchBldr.GetOpenOrders(_symbol);
            }

            return true;
        }

        /// <summary>
        /// Gets latest buy and sell prices for the current pair
        /// </summary>
        /// <returns>Array of decimals</returns>
        public decimal[] GetLastBuySellPrice()
        {
            var orders = _exchBldr.GetLatestOrders(_symbol);

            if(orders == null)
            {
                return new decimal[] { 0.00000000M, 0.00000000M };
            }

            var lastBuy = orders.Where(o => o.side == TradeType.BUY).Select(o => o.price).FirstOrDefault();
            var lastSell = orders.Where(o => o.side == TradeType.SELL).Select(o => o.price).FirstOrDefault();
            
            return new decimal[] 
            {
                lastBuy > 0 ? lastBuy : 0.00000000M,
                lastSell > 0 ? lastSell : 0.00000000M
            };
        }

        /// <summary>
        /// Open orders check
        /// </summary>
        /// <returns>Nullable decimal of open price</returns>
        public decimal? OpenOrdersCheck()
        {
            return _exchBldr.OpenOrdersExist(_symbol);
        }

        /// <summary>
        /// Cancel a trade
        /// </summary>
        /// <param name="tradeParams">CancelTrade parameters</param>
        /// <returns>TradeResponse object</returns>
        public TradeResponse CancelTrade(CancelTradeParams tradeParams)
        {
            if (_botSettings.tradingStatus == TradeStatus.LiveTrading)
                return _exchBldr.DeleteTrade(tradeParams);
            else if (_botSettings.tradingStatus == TradeStatus.PaperTrading)
                return CancelPaperTrade(tradeParams);
            else
                return null;
        }

        /// <summary>
        /// Cancel a paper trade for testing purposes
        /// </summary>
        /// <param name="tradeParams">Trade parameters</param>
        /// <returns>TradeResponse object</returns>
        public TradeResponse CancelPaperTrade(CancelTradeParams tradeParams)
        {
            var response = new TradeResponse
            {
                clientOrderId = $"PaperTrade_{_tradeNumber}",
                orderId = _tradeNumber,
                status = OrderStatus.FILLED,
                symbol = tradeParams.symbol,
                transactTime = _dtHelper.UTCtoUnixTime()
            };

            return response;
        }

        #endregion Trade Management

        #region Order Management

        /// <summary>
        /// Get status of a trade
        /// </summary>
        /// <param name="orderId">OrderId of trade</param>
        /// <returns>OrderResponse</returns>
        public OrderResponse GetOrderStatus(TradeResponse trade)
        {
            if (_botSettings.tradingStatus == TradeStatus.LiveTrading)
                return _exchBldr.GetOrderDetail(trade, _symbol);
            else if (_botSettings.tradingStatus == TradeStatus.PaperTrading)
                return GetPaperOrderStatus(trade.orderId);
            else
                return null;
        }

        /// <summary>
        /// Get status of a paper trade
        /// </summary>
        /// <param name="orderId">OrderId of trade</param>
        /// <returns>OrderResponse</returns>
        public OrderResponse GetPaperOrderStatus(long orderId)
        {
            var response = new OrderResponse
            {
                orderId = orderId,
                status = OrderStatus.FILLED
            };

            return response;
        }

        #endregion Order Management
    }
}
