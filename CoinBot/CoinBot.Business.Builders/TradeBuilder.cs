﻿using CoinBot.Business.Builders.Interface;
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
        private List<TradeInformation> _tradeInformation;
        private TradeInformation _lastTrade;
        private List<OpenOrder> _openOrderList;
        private List<OpenStopLoss> _openStopLossList;
        private decimal _lastBuy = 0.00000000M;
        private decimal _lastSell = 0.00000000M;
        private decimal _lastPrice = 0.00000000M;
        private decimal _lastQty = 0.00000000M;
        private TradeType _lastTradeType;

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
            GetAssetAndPair();
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
                mooningTankingTime = _botSettings.mooningTankingTime,
                priceCheck = _botSettings.priceCheck,
                sellPercent = _botSettings.sellPercent,
                startBotAutomatically = _botSettings.startBotAutomatically,
                startingAmount = _botSettings.startingAmount,
                stopLoss = _botSettings.stopLoss,
                tradePercent = _botSettings.tradePercent,
                tradingPair = _botSettings.tradingPair,
                tradingStatus = _botSettings.tradingStatus,
                tradingStrategy = _botSettings.tradingStrategy
            };

            if (settings.buyPercent > 0)
                updatedSettings.buyPercent = settings.buyPercent;
            if (settings.chartInterval != Interval.None)
                updatedSettings.chartInterval = settings.chartInterval;
            if (settings.mooningTankingTime > 0)
                updatedSettings.mooningTankingTime = settings.mooningTankingTime;
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
            if (!string.IsNullOrEmpty(settings.tradingPair))
                updatedSettings.tradingPair = settings.tradingPair;
            if (settings.tradingStatus != TradeStatus.None)
                updatedSettings.tradingStatus = settings.tradingStatus;
            if (settings.tradingStrategy != Strategy.None)
                updatedSettings.tradingStrategy = settings.tradingStrategy;

            _fileRepo.UpdateBotSettings(updatedSettings);
            _botSettings = updatedSettings;
            _symbol = _botSettings.tradingPair;

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
            //return _fileRepo.GetTransactions().OrderByDescending(t => t.timestamp);
        }

        #endregion Trade History

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
            //var lastBalance = balanceList.Last();

            //return balanceList.Where(b => b.timestamp == lastBalance).FirstOrDefault();
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
        /// Set paper balances on load
        /// </summary>
        public void SetBalances()
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

            LogBalances();
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

        #region Buy/Sell Crypto

        /// <summary>
        /// Buy crypto
        /// </summary>
        /// <param name="orderPrice">Buy price</param>
        /// <param name="tradeType">Trade Type</param>
        /// <returns>Boolean when complete</returns>
        public bool BuyCrypto(decimal orderPrice, TradeType tradeType)
        {
            var trade = MakeTrade(TradeType.BUY, orderPrice);

            var tradeComplete = ValidateTradeComplete(trade);

            if(!tradeComplete)
            {
                return false;
            }

            _lastBuy = orderPrice;

            _lastTrade = new TradeInformation
            {
                pair = _symbol,
                price = orderPrice,
                quantity = trade.origQty,
                timestamp = _dtHelper.UnixTimeToUTC(trade.transactTime),
                tradeType = EnumHelper.GetEnumDescription((TradeType)tradeType)
            };
            _tradeInformation.Add(_lastTrade);

            LogTransaction(_lastTrade);

            var stopLoss = PlaceStopLoss(orderPrice, trade.origQty);

            UpdateBalances();

            return true;
        }

        /// <summary>
        /// Sell crypto
        /// </summary>
        /// <param name="orderPrice">Current price</param>
        /// <param name="tradeType">Trade Type</param>
        /// <returns>Boolean when complete</returns>
        public bool SellCrypto(decimal orderPrice, TradeType tradeType)
        {
            CancelStopLoss();

            var trade = MakeTrade(TradeType.SELL, orderPrice);

            var tradeComplete = ValidateTradeComplete(trade);

            if (!tradeComplete)
            {
                return false;
            }

            _lastSell = orderPrice;

            _lastTrade = new TradeInformation
            {
                pair = _symbol,
                price = orderPrice,
                quantity = trade.origQty,
                timestamp = _dtHelper.UnixTimeToUTC(trade.transactTime),
                tradeType = EnumHelper.GetEnumDescription((TradeType)tradeType)
            };

            _tradeInformation.Add(_lastTrade);

            LogTransaction(_lastTrade);

            UpdateBalances();

            return true;
        }

        #endregion Buy/Sell Crypto

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
            while (!tradeComplete || i > 5)
            {
                i++;
                tradeComplete = CheckTradeStatus(trade);

                if (!tradeComplete && i < 5)
                {
                    Task.WaitAll(Task.Delay(1500));
                }
                else if (!tradeComplete && i == 5)
                {
                    var cancelTradeParams = new CancelTradeParams
                    {
                        orderId = trade.orderId,
                        origClientOrderId = trade.clientOrderId,
                        symbol = trade.symbol,
                        timestamp = trade.transactTime
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
                    quantity = _helper.RoundDown(quantity, 4);
                }
            }
            
            var roundedDown = _helper.RoundDown(quantity, 6);

            return roundedDown;
        }

        #endregion Pair and Quantity

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
                origClientOrderId = _openStopLossList[0].clientOrderId
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
        /// </summary>
        /// <param name="tradeType">TradeType object</param>
        /// <param name="orderPrice">Trade price</param>
        /// <returns>TradeResponse object</returns>
        public TradeResponse MakeTrade(TradeType tradeType, decimal orderPrice)
        {
            orderPrice = GetPricePadding(tradeType, orderPrice);

            var quantity = GetTradeQuantity(tradeType, orderPrice);

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

            var response = PlaceTrade(trade);

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
                    pricePadding = -0.03M;
                }
                else if(tradeType == TradeType.SELL || tradeType == TradeType.VOLUMESELL || tradeType == TradeType.VOLUMEBUYSELLOFF)
                {
                    pricePadding = 0.03M;
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
        public void CancelTrade(long orderId, string origClientOrderId)
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
