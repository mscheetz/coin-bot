using CoinBot.Business.Entities;
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
        /// <param name="asset">String of asset</param>
        /// <param name="pair">String of trading pair</param>
        /// <returns>Collection of Balance objects</returns>
        List<Balance> GetBalance(string asset, string pair);

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
        /// Get Last Buy and Sell orders that were filled
        /// </summary>
        /// <param name="symbol">String of trading symbol</param>
        /// <returns>Array of OrderReponses</returns>
        OrderResponse[] GetLatestOrders(string symbol);

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
        /// <param name="range">Size of array to return</param>
        /// <returns>BotStick array</returns>
        BotStick[] GetSticksFromGdaxTrades(GdaxTrade[] trades, int range);

        /// <summary>
        /// Get 1st price with the most support
        /// </summary>
        /// <param name="symbol">String of trading pair</param>
        /// <returns>Decimal of price</returns>
        decimal GetSupport(string symbol);

        /// <summary>
        /// Get 1st price with the most resistance
        /// </summary>
        /// <param name="symbol">String of trading pair</param>
        /// <returns>Decimal of price</returns>
        decimal GetResistance(string symbol);
    }
}
