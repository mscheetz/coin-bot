using System;
using System.Collections.Generic;
using System.Text;

namespace CoinBot.Business.Entities
{
    public class BotSettings
    {
        /// <summary>
        /// Password for bot
        /// </summary>
        public string botPassword { get; set; }
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
        /// Check for stop loss?
        /// </summary>
        public bool stopLossCheck { get; set; }
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
        /// Last sell value
        /// </summary>
        public decimal lastSell { get; set; }
        /// <summary>
        /// Trading fee for exchange (GDAX)
        /// </summary>
        public decimal tradingFee { get; set; }
        /// <summary>
        /// Trade validation timer
        /// </summary>
        public int tradeValidationCheck { get; set; }
        /// <summary>
        /// When true, bot runs
        /// When false, bot stops
        /// </summary>
        public bool runBot { get; set; }
        /// <summary>
        /// Total quantity (in trading pair) to check before setting buy/sell price
        /// </summary>
        public decimal orderBookQuantity { get; set; }
        /// <summary>
        /// Interval, in cycles to reset balances and bot settings
        /// </summary>
        public int traderResetInterval { get; set; }
        /// <summary>
        /// Trade in trading competition?
        /// </summary>
        public bool tradingCompetition { get; set; }
        /// <summary>
        /// Unix timestamp of end of trading competition
        /// </summary>
        public long tradingCompetitionEndTimeStamp { get; set; }
        /// <summary>
        /// Milliseconds before canceling an open order
        /// </summary>
        public long openOrderTimeMS { get; set; }
        /// <summary>
        /// If price is the same for this many cycles, use current price as new price
        /// ie if you bought at a higer price and it is sitting lower, after this many cycles
        /// sell at new, lower price
        /// used for trading competitions
        /// </summary>
        public long samePriceLimit { get; set; }

        public BotSettings()
        {
            runBot = true;
        }
    }
}
