﻿using CoinBot.Business.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace CoinBot.Business.Builders.Interface
{
    public interface IExchangeBuilder
    {
        /// <summary>
        /// Set BotSettings
        /// </summary>
        /// <param name="settings">BotSettings Object</param>
        void SetExchange(BotSettings settings);

        /// <summary>
        /// Validate exhange api is configured
        /// </summary>
        /// <param name="exchange">Current exchange to use</param>
        /// <returns>Boolean if configured correctly</returns>
        bool ValidateExchangeConfigured(Exchange exchange);

        /// <summary>
        /// Set Exchange Api Info
        /// </summary>
        /// <param name="apiInfo">ApiInformation for exhange</param>
        /// <returns>Boolean when complete</returns>
        bool SetExchangeApi(ApiInformation apiInfo);

        /// <summary>
        /// Get Candlesticks
        /// </summary>
        /// <param name="symbol">Trading Symbol</param>
        /// <param name="interval">Candlestick Interval</param>
        /// <param name="range">Number of sticks to return</param>
        /// <returns>BotStick Array</returns>
        BotStick[] GetCandlesticks(string symbol, Interval interval, int range);

        /// <summary>
        /// Get Balances for account
        /// </summary>
        /// <returns>Collection of Balance objects</returns>
        List<Balance> GetBalance();

        /// <summary>
        /// Place a Trade
        /// </summary>
        /// <param name="tradeParams">TradeParams object</param>
        /// <returns>TradeResponse object</returns>
        TradeResponse PlaceTrade(TradeParams tradeParams);

        /// <summary>
        /// Get Order Details
        /// </summary>
        /// <param name="trade">TradeResponse object</param>
        /// <param name="symbol">Trading symbol</param>
        /// <returns>OrderResponse object</returns>
        OrderResponse GetOrderDetail(TradeResponse trade, string symbol = "");

        /// <summary>
        /// Delete a trade
        /// </summary>
        /// <param name="tradeParams">CancelTradeParams object</param>
        /// <returns>TradeResponse object</returns>
        TradeResponse DeleteTrade(CancelTradeParams tradeParams);

        /// <summary>
        /// Convert GdaxTrade array to BotStick array
        /// </summary>
        /// <param name="trades">GdaxTrade array</param>
        /// <returns>BotStick array</returns>
        BotStick[] GetSticksFromGdaxTrades(GdaxTrade[] trades);
    }
}
