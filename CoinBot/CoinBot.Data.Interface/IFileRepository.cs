using CoinBot.Business.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace CoinBot.Data.Interface
{
    public interface IFileRepository
    {
        /// <summary>
        /// Check if config file exists
        /// </summary>
        /// <returns>Boolean of validation</returns>
        bool ConfigExists();

        /// <summary>
        /// Check if settings file exists
        /// </summary>
        /// <returns>Boolean of validation</returns>
        bool BotSettingsExists();

        /// <summary>
        /// Get App configuration data from file
        /// </summary>
        /// <returns>BotConfig object</returns>
        ApiInformation GetConfig();

        /// <summary>
        /// Get BotSettings
        /// </summary>
        /// <returns>BotSettings object</returns>
        BotSettings GetSettings();

        /// <summary>
        /// Update BotSettings file
        /// </summary>
        /// <param name="botSettings">Updated BotSettings</param>
        /// <returns>Boolean when complete</returns>
        bool UpdateBotSettings(BotSettings botSettings);

        /// <summary>
        /// Get Transactions
        /// </summary>
        /// <returns>Collection of TradeInformation</returns>
        List<TradeInformation> GetTransactions();

        /// <summary>
        /// Write transaction to log
        /// </summary>
        /// <param name="tradeInformation">TradeInformation to write</param>
        /// <returns>Boolean when complete</returns>
        bool LogTransaction(TradeInformation tradeInformation);

        /// <summary>
        /// Get Transactions
        /// </summary>
        /// <returns>Collection of BotBalance</returns>
        List<List<BotBalance>> GetBalances();

        /// <summary>
        /// Write balances to file
        /// </summary>
        /// <param name="botBalance">BotBalances to write</param>
        /// <returns>Boolean when complete</returns>
        bool LogBalances(List<BotBalance> botBalance);
    }
}
