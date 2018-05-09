using System;
using System.Collections.Generic;
using System.Text;

namespace CoinBot.Business.Entities
{
    public class BotSettings
    {
        /// <summary>
        /// Trading Pair
        /// </summary>
        public string tradingPair { get; set; }
        /// <summary>
        /// Buy Percent change from previous sell
        /// </summary>
        public double buyPercent { get; set; }
        /// <summary>
        /// Sell Percent change from previous buy
        /// </summary>
        public double sellPercent { get; set; }
        /// <summary>
        /// Stop loss percent
        /// </summary>
        public double stopLoss { get; set; }
        /// <summary>
        /// Percent of holdings to trade
        /// </summary>
        public double tradePercent { get; set; }
        /// <summary>
        /// Price check interval in milliseconds
        /// </summary>
        public int priceCheck { get; set; }
        /// <summary>
        /// Chart interval to check
        /// </summary>
        public Interval chartInterval { get; set; }
    }
}
