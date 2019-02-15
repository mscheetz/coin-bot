// -----------------------------------------------------------------------------
// <copyright file="IBollingerBandTraderBuilder" company="Matt Scheetz">
//     Copyright (c) Matt Scheetz All Rights Reserved
// </copyright>
// <author name="Matt Scheetz" date="2/15/2019 5:17:35 PM" />
// -----------------------------------------------------------------------------

namespace coinbot.strategies.zzz_Old_Bot.Interfaces
{
    #region Usings

    using System;
    using System.Collections.Generic;
    using System.Text;

    #endregion Usings

    public interface IBollingerBandTraderBuilder
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

        /// <summary>
        /// Get Bollinger Bands for a symbol
        /// </summary>
        /// <param name="interval">Candlestick interval</param>
        /// <returns>Array of Candlesticks</returns>
        BotStick[] GetBollingerBands(Interval interval);
    }
}