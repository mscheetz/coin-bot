// -----------------------------------------------------------------------------
// <copyright file="StrategyBase" company="Matt Scheetz">
//     Copyright (c) Matt Scheetz All Rights Reserved
// </copyright>
// <author name="Matt Scheetz" date="2/15/2019 4:30:54 PM" />
// -----------------------------------------------------------------------------

namespace coinbot.strategies.Strategies
{
    #region Usings

    using coinbot.strategies.Contracts.Models;
    using ExchangeHub;
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Threading.Tasks;

    #endregion Usings

    public class StrategyBase
    {
        #region Properties

        private Settings _settings;
        private ExchangeHub _exchange = null;
        private decimal _tradeBalance = 0.0M;
        private decimal _baseBalance = 0.0M;

        #endregion Properties

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="settings">Bot settings</param>
        /// <param name="exchange">Exchange to trade on</param>
        public StrategyBase(Settings settings, ExchangeHub exchange)
        {
            this._settings = settings;
            this._exchange = exchange;
        }

        /// <summary>
        /// Run bot
        /// </summary>
        public virtual async Task RunBot()
        {
        }
    }
}