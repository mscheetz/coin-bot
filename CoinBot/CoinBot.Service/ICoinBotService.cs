using CoinBot.Business.Entities;
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
        /// <param name="transactionCount">Count of transations to return (default 10)</param>
        /// <returns>Collection of TradeInformation</returns>
        IEnumerable<TradeInformation> GetTransactionHistory(int transactionCount = 10);

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
