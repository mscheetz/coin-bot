using CoinBot.Business.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace CoinBot.Business.Builders.Interface
{
    public interface IPercentageTradeBuilder
    {
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
        /// Run Trading Bot
        /// </summary>
        /// <param name="interval">Candlestick Interval</param>
        /// <param name="cycles">Int of cycles to run (default -1, run infinitely)</param>
        /// <returns>Boolean when complete</returns>
        bool RunBot(Interval interval, int cycles = -1, bool currentlyTrading = false);

        /// <summary>
        /// Check if buy percent has been reached
        /// </summary>
        /// <param name="currentPrice">Current price</param>
        /// <returns>Boolean of result</returns>
        bool BuyPercentReached(decimal currentPrice);

        /// <summary>
        /// Check if sell percent has been reached
        /// </summary>
        /// <param name="currentPrice">Current price</param>
        /// <returns>Boolean of result</returns>
        bool SellPercentReached(decimal currentPrice);

        /// <summary>
        /// Check if mooning or tanking
        /// </summary>
        /// <param name="candleStick">Current trading stick</param>
        /// <returns>Boolean of result</returns>
        bool MooningAndTankingCheck(Candlestick candleStick);
    }
}
