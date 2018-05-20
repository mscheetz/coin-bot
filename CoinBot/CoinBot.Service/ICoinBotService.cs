﻿using CoinBot.Business.Entities;
using System;
using System.Collections.Generic;

namespace CoinBot.Service
{
    public interface ICoinBotService
    {
        /// <summary>
        /// Update bot settings
        /// </summary>
        /// <param name="botSettings">New bot settings</param>
        /// <returns>Boolean value when complete</returns>
        bool UpdateBotSettings(BotSettings botSettings);

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
        /// <returns>Collection of TradeInformation</returns>
        IEnumerable<TradeInformation> GetTransactionHistory();

        /// <summary>
        /// Get current balance
        /// </summary>
        /// <returns>BotBalance object</returns>
        BotBalance GetBalance();

        /// <summary>
        /// Get Balance history
        /// </summary>
        /// <returns>Collection of BotBalance objects</returns>
        IEnumerable<BotBalance> GetBalanceHistory();
        
        /// <summary>
        /// Get all open stop losses
        /// </summary>
        /// <returns>Collection of OpenStopLoss</returns>
        IEnumerable<OpenStopLoss> GetStopLosses();
    }
}
