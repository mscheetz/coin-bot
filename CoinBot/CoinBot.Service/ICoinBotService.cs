﻿using CoinBot.Business.Entities;
using System;
using System.Collections.Generic;

namespace CoinBot.Service
{
    public interface ICoinBotService
    {
        /// <summary>
        /// Validate passwords match
        /// </summary>
        /// <param name="attemptPassword">Attempted password</param>
        /// <returns>Boolean of match attempt</returns>
        bool ValidatePassword(string attemptPassword);

#if DEBUG
        /// <summary>
        /// Update bot password
        /// </summary>
        /// <param name="password">String of new password</param>
        /// <returns>Bool when complete</returns>
        bool UpdatePassword(string password);

        /// <summary>
        /// Update bot settings
        /// </summary>
        /// <param name="botConfig">New bot settings</param>
        /// <returns>Boolean value when complete</returns>
        bool UpdateBotConfig(BotConfig botConfig);

        /// <summary>
        /// Update bot settings
        /// </summary>
        /// <param name="botConfig">New bot settings</param>
        /// <returns>Boolean value when complete</returns>
        bool UpdateApiAccess(ApiInformation apiInformation);
#endif

        /// <summary>
        /// Get current BotConfig
        /// </summary>
        /// <returns>BotConfig object</returns>
        BotConfig GetBotConfig();

        /// <summary>
        /// Get current Exchange API Key
        /// </summary>
        /// <returns>String of api key</returns>
        string GetApiKey();

        /// <summary>
        /// Start trading
        /// </summary>
        /// <param name="interval">Candlestick interval</param>
        bool StartBot(Interval interval);

        /// <summary>
        /// Stop Trading
        /// </summary>
        bool StopBot();

        /// <summary>
        /// Get all transactions
        /// </summary>
        /// <param name="transactionCount">Count of transations to return (default 10)</param>
        /// <returns>Collection of TradeInformation</returns>
        IEnumerable<TradeInformation> GetTransactionHistory(int transactionCount = 10);

        /// <summary>
        /// Get last N trade signals
        /// </summary>
        /// <param name="signalCount">Count of trade signals to return (default 10)</param>
        /// <returns>Collection of TradeSignal objects</returns>
        IEnumerable<TradeSignal> GetTradeSignalHistory(int signalCount = 10);

        /// <summary>
        /// Get current balance
        /// </summary>
        /// <returns>Collection of BotBalance objects</returns>
        IEnumerable<BotBalance> GetBalance();

        /// <summary>
        /// Get current balance
        /// </summary>
        /// <param name="count">Count of balances to return</param>
        /// <returns>BotBalance object</returns>
        IEnumerable<IEnumerable<BotBalance>> GetBalances(int count);

        /// <summary>
        /// Get Balance history
        /// </summary>
        /// <returns>Collection of BotBalance objects</returns>
        IEnumerable<IEnumerable<BotBalance>> GetBalanceHistory();
        
        /// <summary>
        /// Get all open stop losses
        /// </summary>
        /// <returns>Collection of OpenStopLoss</returns>
        IEnumerable<OpenStopLoss> GetStopLosses();

        /// <summary>
        /// Cancel all GDAX trades
        /// </summary>
        /// <returns>Boolean when complete</returns>
        bool CancelAllGdaxTrades();
    }
}
