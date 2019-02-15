// -----------------------------------------------------------------------------
// <copyright file="ITraderBuilder" company="Matt Scheetz">
//     Copyright (c) Matt Scheetz All Rights Reserved
// </copyright>
// <author name="Matt Scheetz" date="2/15/2019 4:41:38 PM" />
// -----------------------------------------------------------------------------

namespace coinbot.strategies.zzz_Old_Bot
{
    using Binance.NetCore.Entities;
    #region Usings

    using System;
    using System.Collections.Generic;
    using System.Text;

    #endregion Usings

    public interface ITradeBuilder
    {
        /// <summary>
        /// Check if config file exists
        /// </summary>
        /// <returns>Boolean of result</returns>
        bool ConfigFileExits();

        /// <summary>
        /// Check if settings file exists
        /// </summary>
        /// <returns>Boolean of result</returns>
        bool SettingsFileExists();

        /// <summary>
        /// Get password
        /// </summary>
        /// <returns>String of password</returns>
        string GetPassword();

        /// <summary>
        /// Update bot password
        /// </summary>
        /// <param name="password">String of new password</param>
        /// <returns>Bool when complete</returns>
        bool UpdatePassword(string password);

        /// <summary>
        /// Get BotSettings
        /// </summary>
        /// <returns>BotSettings object</returns>
        BotSettings GetBotSettings();

        /// <summary>
        /// Get BotConfig
        /// </summary>
        /// <returns>BotConfig object</returns>
        BotConfig GetBotConfig();

        /// <summary>
        /// Get current balances
        /// </summary>
        /// <returns>Collection of BotBallance objects</returns>
        List<BotBalance> GetBotBalance();

        /// <summary>
        /// Load bot settings from disk
        /// </summary>
        void LoadBotSettingsFile();

        /// <summary>
        /// Update bot settings from file
        /// </summary>
        /// <param name="_lastBuy">Last buy value</param>
        /// <param name="_lastSell">Last sell value</param>
        /// <returns>Boolean when complete</returns>
        bool UpdateBotSettings(decimal _lastBuy, decimal _lastSell);

        /// <summary>
        /// Set BotSettings in builder
        /// </summary>
        /// <param name="botConfig">Updated BotConfig values</param>
        /// <returns>Boolean when complete</returns>
        bool SetBotSettings(BotConfig botConfig);

        /// <summary>
        /// Set BotSettings in builder
        /// </summary>
        /// <param name="settings">Updated Settings</param>
        /// <returns>Boolean when complete</returns>
        bool SetBotSettings(BotSettings settings);

        /// <summary>
        /// Set api information
        /// </summary>
        /// <param name="apiInformation">Updated ApiInformation</param>
        /// <returns>Boolean when complete</returns>
        bool SetApiInformation(ApiInformation apiInformation);

        /// <summary>
        /// Get Api Key from disc
        /// </summary>
        /// <returns>String of api key</returns>
        string GetApiKey();

        /// <summary>
        /// Update balances and get initial trade type
        /// </summary>
        /// <returns>TradeType value</returns>
        TradeType GetInitialTradeType();

        /// <summary>
        /// Update balances and get current trade type
        /// </summary>
        /// <param name="logBalances">Write balances to log?</param>
        /// <returns>TradeType value</returns>
        TradeType GetTradingType(bool logBalances = false);

        /// <summary>
        /// Get all open stop losses
        /// </summary>
        /// <returns>Collection of OpenStopLoss</returns>
        IEnumerable<OpenStopLoss> GetStopLosses();

        /// <summary>
        /// Get all transactions
        /// </summary>
        /// <param name="transactionCount">Count of transations to return (default 10)</param>
        /// <returns>Collection of TradeInformation</returns>
        IEnumerable<TradeInformation> GetTradeHistory(int transactionCount);

        /// <summary>
        /// Get all trade signals
        /// </summary>
        /// <param name="signalCount">Count of trade signals to return</param>
        /// <returns>Collection of TradeSignal objects</returns>
        IEnumerable<TradeSignal> GetSignalHistory(int signalCount);

        /// <summary>
        /// Get current balance
        /// </summary>
        /// <param name="recordsToReturn">Number of records to return (default 1)</param>
        /// <returns>BotBalance object</returns>
        IEnumerable<List<BotBalance>> GetBalance(int recordsToReturn = 1);

        /// <summary>
        /// Get current asset
        /// </summary>
        /// <returns>String of asset</returns>
        string GetAsset();

        /// <summary>
        /// Get current trading pair
        /// </summary>
        /// <returns>String of pair</returns>
        string GetPair();

        /// <summary>
        /// Update balances from exchange
        /// </summary>
        void UpdateBalances();

        /// <summary>
        /// Get Balance history
        /// </summary>
        /// <returns>Collection of BotBalance objects</returns>
        IEnumerable<IEnumerable<BotBalance>> GetBalanceHistory();

        /// <summary>
        /// Set balances
        /// </summary>
        /// <param name="logBalance">Log the balance bool</param>
        void SetBalances(bool logBalance = true);

        /// <summary>
        /// Get paper balances available
        /// </summary>
        /// <param name="btcStartingValue">Starting btc value for paper trading (default = 0)</param>
        /// <returns>Collection of balance objects</returns>
        List<Balance> GetPaperBalances(decimal btcStartingValue = 0);

        /// <summary>
        /// Log balances to file
        /// </summary>
        /// <returns>Boolean when complete</returns>
        bool LogBalances();

        /// <summary>
        /// Log Trades
        /// </summary>
        /// <param name="tradeInformation">New TradeInformation object</param>
        /// <returns></returns>
        bool LogTransaction(TradeInformation tradeInformation);

        /// <summary>
        /// Write a signal to file
        /// </summary>
        /// <param name="signal">Signal to log</param>
        /// <returns>Boolean when complete</returns>
        bool LogTradeSignal(SignalType signalType, TradeType tradeType, decimal price, decimal volume = 0M);

        /// <summary>
        /// Capture the current transaction and log it
        /// </summary>
        /// <param name="price">Transaction price</param>
        /// <param name="quantity">Transaction quantity</param>
        /// <param name="timeStamp">Transaction time</param>
        /// <param name="tradeType">Transaction TradeType</param>
        /// <returns>Boolean when complete</returns>
        bool CaptureTransaction(decimal price, decimal quantity, long timeStamp, TradeType tradeType);

        /// <summary>
        /// Get balances available
        /// </summary>
        /// <param name="startingQuantity">Starting quantity (for paper trading, default 0)</param>
        /// <returns>Collection of balance objects</returns>
        List<Balance> GetBalances(decimal startingQuantity = 0M);

        /// <summary>
        /// Get Candlesticks from exchange API
        /// </summary>
        /// <param name="symbol">Trading symbol</param>
        /// <param name="interval">Candlestick Interval</param>
        /// <param name="range">Number of candlesticks to get</param>
        /// <returns>Array of BotStick objects</returns>
        BotStick[] GetCandlesticks(string symbol, Interval interval, int range);

        /// <summary>
        /// Get order book position of a price
        /// </summary>
        /// <param name="price">Decimal of price to find</param>
        /// <returns>Int of position in order book</returns>
        int? GetPricePostion(decimal price);

        /// <summary>
        /// Get next resistance level
        /// </summary>
        /// <param name="getNew">Boolean to get a new value</param>
        /// <returns>Decimal of next resistance</returns>
        decimal GetResistance(bool getNew = false);

        /// <summary>
        /// Get next support level
        /// </summary>
        /// <param name="getNew">Boolean to get a new value</param>
        /// <returns>Decimal of next support</returns>
        decimal GetSupport(bool getNew = false);

        /// <summary>
        /// Place a trade
        /// </summary>
        /// <param name="tradeParams">Trade parameters</param>
        /// <returns>TradeResponse object</returns>
        TradeResponse PlaceTrade(TradeParams tradeParams);

        /// <summary>
        /// Set up respository
        /// </summary>
        /// <returns>Boolean when complete</returns>
        bool SetupRepository();

        /// <summary>
        /// Check if Stop Loss Hit
        /// </summary>
        /// <param name="currentPrice">Current price of coin</param>
        /// <returns>Nullable decimal value of stop loss</returns>
        decimal? StoppedOutCheck(decimal currentPrice);

        /// <summary>
        /// Buy crypto
        /// </summary>
        /// <param name="orderPrice">Buy price</param>
        /// <param name="tradeType">Trade Type</param>
        /// <param name="stopLoss">Place stoploss? default false</param>
        /// <param name="validateTrade">Validated trade complete, default = true</param>
        /// <returns>Boolean when complete</returns>
        bool BuyCrypto(decimal orderPrice, TradeType tradeType, bool stopLoss = false, bool validateTrade = true);

        /// <summary>
        /// Sell crypto
        /// </summary>
        /// <param name="orderPrice">Current price</param>
        /// <param name="tradeType">Trade Type</param>
        /// <param name="validateTrade">Validate trade is complete, default = true</param>
        /// <returns>Boolean when complete</returns>
        bool SellCrypto(decimal orderPrice, TradeType tradeType, bool validateTrade = true);

        /// <summary>
        /// Get price padding to avoid GDAX transaction fees
        /// </summary>
        /// <param name="tradeType">Current trade type</param>
        /// <param name="orderPrice">Order price</param>
        /// <returns>Update order price</returns>
        decimal GetPricePadding(TradeType tradeType, decimal orderPrice);

        /// <summary>
        /// Check status of placed trade
        /// </summary>
        /// <param name="trade">TradeResponse of trade</param>
        /// <returns>Boolean value of filled status</returns>
        bool CheckTradeStatus(TradeResponse trade);

        /// <summary>
        /// Get quantity to trade based
        /// </summary>
        /// <param name="tradeType">TradeType object</param>
        /// <param name="orderPrice">Requested trade price</param>
        /// <returns>decimal of quantity to purchase</returns>
        decimal GetTradeQuantity(TradeType tradeType, decimal orderPrice);

        /// <summary>
        /// Cancel a stop loss
        /// </summary>
        /// <returns>Boolean value when complete</returns>
        bool CancelStopLoss();

        /// <summary>
        /// Make a trade
        /// </summary>
        /// <param name="tradeType">TradeType object</param>
        /// <param name="orderPrice">Trade price</param>
        /// <returns>TradeResponse object</returns>
        TradeResponse MakeTrade(TradeType tradeType, decimal orderPrice);

        /// <summary>
        /// Place a stop loss
        /// </summary>
        /// <param name="orderPrice">Trade price</param>
        /// <param name="quantity">Quantity to trade</param>
        /// <returns>TradeResponse object</returns>
        TradeResponse PlaceStopLoss(decimal orderPrice, decimal quantity);

        /// <summary>
        /// Cancel trade
        /// </summary>
        /// <param name="orderId">OrderId to cancel</param>
        /// <param name="origClientOrderId">ClientOrderId to cancel</param>
        /// <param name="tradeType">Trade type</param>
        void CancelTrade(long orderId, string origClientOrderId, string tradeType = "");

        /// <summary>
        /// Cancel all open orders for the current trading pair
        /// </summary>
        /// <returns>Boolen when complete</returns>
        bool CancelOpenOrders();

        /// <summary>
        /// Cancel a trade
        /// </summary>
        /// <param name="tradeParams">CancelTrade parameters</param>
        /// <returns>TradeResponse object</returns>
        TradeResponse CancelTrade(CancelTradeParams tradeParams);

        /// <summary>
        /// Cancel a paper trade for testing purposes
        /// </summary>
        /// <param name="tradeParams">Trade parameters</param>
        /// <returns>TradeResponse object</returns>
        TradeResponse CancelPaperTrade(CancelTradeParams tradeParams);

        /// <summary>
        /// Get status of a trade
        /// </summary>
        /// <param name="trade">TradeResponse of trade</param>
        /// <returns>OrderResponse</returns>
        OrderResponse GetOrderStatus(TradeResponse trade);

        /// <summary>
        /// Open orders check
        /// </summary>
        /// <returns>OpenOrderDetail of open order</returns>
        OpenOrderDetail OpenOrdersCheck();

        /// <summary>
        /// Gets latest buy and sell prices for the current pair
        /// </summary>
        /// <returns>Array of decimals</returns>
        decimal[] GetLastBuySellPrice();

        /// <summary>
        /// Get status of a paper trade
        /// </summary>
        /// <param name="orderId">OrderId of trade</param>
        /// <returns>OrderResponse</returns>
        OrderResponse GetPaperOrderStatus(long orderId);

        #region Moon and Tank Check

        /// <summary>
        /// Check if mooning
        /// </summary>
        /// <param name="startingPrice">decimal of starting price</param>
        /// <param name="prevStick">Previous BotStick (default null)</param>
        /// <param name="iteration">Int of iteration</param>
        /// <returns>Decimal of sell price</returns>
        decimal OrderBookSellCheck(decimal startingPrice = 0.00000000M
                                    , BotStick prevStick = null
                                    , int iteration = 0);

        /// <summary>
        /// Check if tanking
        /// </summary>
        /// <param name="startingPrice">decimal of starting price</param>
        /// <param name="prevStick">Previous BotStick (default null)</param>
        /// <param name="iteration">Int of iteration</param>
        /// <returns>Decimal of buy price</returns>
        decimal OrderBookBuyCheck(decimal startingPrice = 0.00000000M
                                    , BotStick prevStick = null
                                    , int iteration = 0);

        /// <summary>
        /// Get next candlestick
        /// </summary>
        /// <param name="interval">Trade interval, default 1 minute</param>
        /// <param name="stickCount">Int of sticks to return, default 2</param>
        /// <returns>Candlestick object</returns>
        BotStick[] GetNextCandlestick();

        #endregion Moon and Tank Check
    }
}