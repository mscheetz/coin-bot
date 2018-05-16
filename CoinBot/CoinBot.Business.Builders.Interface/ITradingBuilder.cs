﻿using CoinBot.Business.Entities;
using System;
using System.Collections.Generic;

namespace CoinBot.Business.Builders.Interface
{
    public interface ITradingBuilder
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
        /// Get 21 day Simple Moving Average
        /// </summary>
        /// <returns>decimal of SMA</returns>
        decimal Get21DaySMA();
    }
}
