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
        /// Trading strategy
        /// </summary>
        public Strategy tradingStrategy { get; set; }
        /// <summary>
        /// Trading strategy
        /// </summary>
        public TradeStatus tradingStatus { get; set; }
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
        /// <summary>
        /// Start bot when service starts
        /// </summary>
        public bool? startBotAutomatically { get; set; }
        /// <summary>
        /// Paper trading starting BTC quantity
        /// </summary>
        public decimal startingAmount { get; set; }
        /// <summary>
        /// Mooning and Tanking timer for checking next candlestick
        /// </summary>
        public int mooningTankingTime { get; set; }
        /// <summary>
        /// Mooning and Tanking percent for checking against next candlestick
        /// </summary>
        public double mooningTankingPercent { get; set; }
        /// <summary>
        /// Selected exchange to trade on
        /// </summary>
        public Exchange exchange { get; set; }
        /// <summary>
        /// Last buy value
        /// </summary>
        public decimal lastBuy { get; set; }
        /// <summary>
        /// Trading fee for exchange (GDAX)
        /// </summary>
        public decimal tradingFee { get; set; }
    }
}
