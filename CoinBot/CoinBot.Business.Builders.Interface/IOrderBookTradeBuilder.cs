using CoinBot.Business.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace CoinBot.Business.Builders.Interface
{
    public interface IOrderBookTradeBuilder
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
        /// <param name="tradingStatus">Bool of trading status (default null, use setting)</param>
        /// <returns>Boolean when complete</returns>
        bool RunBot(Interval interval, int cycles = -1, bool? tradingStatus = null);

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
        /// <param name="previousStick">Previous trading stick</param>
        /// <param name="tradeType">Trade Type</param>
        /// <param name="startingPrice">decimal of starting price</param>
        /// <param name="iteration">Int of iteration</param>
        /// <returns>TradeType of result</returns>
        TradeType MooningAndTankingCheck(BotStick candleStick, BotStick previousStick,
                                                TradeType tradeType,
                                                decimal startingPrice = 0.00000000M,
                                                int iteration = 0);
    }
}
