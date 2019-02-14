// -----------------------------------------------------------------------------
// <copyright file="Settings" company="Matt Scheetz">
//     Copyright (c) Matt Scheetz All Rights Reserved
// </copyright>
// <author name="Matt Scheetz" date="1/24/2019 7:55:04 PM" />
// -----------------------------------------------------------------------------

namespace range_bot.Models
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
        /// Pair to trade against
        /// </summary>
        public string Pair { get; set; }

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

        #endregion Properties
    }
}