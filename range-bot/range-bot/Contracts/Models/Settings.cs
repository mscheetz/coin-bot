// -----------------------------------------------------------------------------
// <copyright file="Settings" company="Matt Scheetz">
//     Copyright (c) Matt Scheetz All Rights Reserved
// </copyright>
// <author name="Matt Scheetz" date="1/24/2019 7:55:04 PM" />
// -----------------------------------------------------------------------------

namespace coinbot.strategies.Contracts.Models
{
    #region Usings

    using ExchangeHub.Contracts;
    using System;
    using System.Collections.Generic;
    using System.Text;

    #endregion Usings

    public class Settings
    {
        #region Properties

        /// <summary>
        /// Exchange api credentials
        /// </summary>
        public ApiCredentials ApiCredentials { get; set; }

        /// <summary>
        /// Exchange to trade on
        /// </summary>
        public Exchange Exchange { get; set; }

        /// <summary>
        /// Time interval to check exchange prices
        /// </summary>
        public TimeInterval TimeInterval { get; set; }

        /// <summary>
        /// Trading Strategy
        /// </summary>
        public Strategy TradingStrategy { get; set; }

        /// <summary>
        /// Pair to trade against
        /// </summary>
        public string Pair { get; set; }

        public TimeInterval ChartInterval { get; set; }

        #region Range Bot Settings

        /// <summary>
        /// Lower limit of buy range
        /// </summary>
        public decimal BuyLow { get; set; }

        /// <summary>
        /// Upper limit of buy range
        /// </summary>
        public decimal BuyHigh { get; set; }

        /// <summary>
        /// Lower limit of sell range
        /// </summary>
        public decimal SellLow { get; set; }

        /// <summary>
        /// Upper limit of sell rage
        /// </summary>
        public decimal SellHigh { get; set; }

        /// <summary>
        /// Stop loss price
        /// </summary>
        public decimal StopPrice { get; set; }

        /// <summary>
        /// Max amount of base symbol to spend in a buy
        /// </summary>
        public decimal BuyLimit { get; set; }

        /// <summary>
        /// Max amount of trading symbol to spend in a sell
        /// </summary>
        public decimal SellLimit { get; set; }

        #endregion Range Bot Settings
                
        #region Volume Bot Settings

        public double BuyPercent { get; set; }

        public double SellPercent { get; set; }

        public double StopLoss { get; set; }

        /// <summary>
        /// Look at Buy/Sell Limit properties
        /// </summary>
        public double TradePercent { get; set; }

        /// <summary>
        /// Mooning and Tanking timer for checking next candlestick
        /// </summary>
        public int MooningTakingTimeCheck { get; set; }

        /// <summary>
        /// Mooning and Tanking percent for checking against next candlestick
        /// </summary>
        public double MooningTankingPercent { get; set; }

        /// <summary>
        /// Trade validation timer
        /// </summary>
        public int TradeValidationCheck { get; set; }

        /// <summary>
        /// Milliseconds before canceling an open order
        /// </summary>
        public long OpenOrderTimeMS { get; set; }

        /// <summary>
        /// If price is the same for this many cycles, use current price as new price
        /// ie if you bought at a higer price and it is sitting lower, after this many cycles
        /// sell at new, lower price
        /// used for trading competitions
        /// </summary>
        public long SamePriceLimit { get; set; }

        #endregion Volume Bot Settings

        #endregion Properties
    }
}