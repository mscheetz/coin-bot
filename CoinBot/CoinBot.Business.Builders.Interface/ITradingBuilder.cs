using CoinBot.Business.Entities;
using System;
using System.Collections.Generic;

namespace CoinBot.Business.Builders.Interface
{
    public interface ITradingBuilder
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
        /// Set BotSettings in builder
        /// </summary>
        /// <param name="settings">Updated Settings</param>
        /// <returns>Boolean when complete</returns>
        bool SetBotSettings(BotSettings settings);

        /// <summary>
        /// Start trading
        /// </summary>
        /// <param name="interval">Candlestick interval</param>
        void StartTrading(Interval interval);

        /// <summary>
        /// Stop Trading
        /// </summary>
        void StopTrading();

        /// <summary>
        /// Get 21 day Simple Moving Average
        /// </summary>
        /// <returns>decimal of SMA</returns>
        decimal Get21DaySMA();
        
        /// <summary>
        /// Get Bollinger Bands for a symbol
        /// </summary>
        /// <param name="interval">Candlestick interval</param>
        /// <returns>Array of Candlesticks</returns>
        Candlestick[] GetBollingerBands(Interval interval);
        
        /// <summary>
        /// Get all transactions
        /// </summary>
        /// <returns>Collection of TradeInformation</returns>
        IEnumerable<TradeInformation> GetTradeHistory();

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
    }
}
