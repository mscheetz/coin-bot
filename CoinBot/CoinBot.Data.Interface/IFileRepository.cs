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
        /// Set App configuration data
        /// </summary>
        /// <param name="apiInformation">Updated settings</param>
        /// <returns>Boolean when completee</returns>
        bool SetConfig(ApiInformation apiInformation, bool noWrite = true);

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
        bool UpdateBotSettings(BotSettings botSettings, bool noWrite = true);

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
        bool LogTransaction(TradeInformation tradeInformation, bool noWrite = true);

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
        bool LogBalances(List<BotBalance> botBalance, bool noWrite = true);
        
        /// <summary>
        /// Get TradeSignals
        /// </summary>
        /// <returns>Collection of TradeSignals</returns>
        List<TradeSignal> GetSignals();

        /// <summary>
        /// Write trade signal to file
        /// </summary>
        /// <param name="signal">TradeSignal to write</param>
        /// <returns>Boolean when complete</returns>
        bool LogSignal(TradeSignal signal, bool noWrite = true);

        /// <summary>
        /// Log an error with an object
        /// </summary>
        /// <typeparam name="T">Object type to log</typeparam>
        /// <param name="message">Message to log</param>
        /// <param name="obj">Object to log</param>
        /// <returns>Boolean when complete</returns>
        bool LogError<T>(string message, T obj, bool noWrite = true);

        /// <summary>
        /// Log an error
        /// </summary>
        /// <param name="message">Message to log</param>
        /// <returns>Boolean when complete</returns>
        bool LogError(string message, bool noWrite = true);
    }
}
