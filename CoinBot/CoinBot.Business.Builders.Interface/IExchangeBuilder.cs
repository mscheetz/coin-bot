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
        /// Check if open orders exist
        /// </summary>
        /// <param name="symbol">Trading pair to check</param>
        /// <returns>Boolean of result</returns>
        bool OpenOrdersExist(string symbol);

        /// <summary>
        /// Get open orders
        /// </summary>
        /// <param name="symbol">Symbol to check</param>
        /// <returns>Array of OrderResponse objects</returns>
        OrderResponse[] GetOpenOrders(string symbol);

        /// <summary>
        /// Get order book position of a price
        /// </summary>
        /// <param name="symbol">String of trading symbol</param>
        /// <param name="price">Decimal of price to find</param>
        /// <returns>Int of position</returns>
        int? GetPricePosition(string symbol, decimal price);

        /// <summary>
        /// Get 1st price with the most support at or above specified volume
        /// </summary>
        /// <param name="symbol">String of trading pair</param>
        /// <param name="volume">Volume to set buy price</param>
        /// <returns>Decimal of price</returns>
        OrderBookDetail GetSupport(string symbol, decimal volume);

        /// <summary>
        /// Get 1st price with the most resistance at or above specified volume
        /// </summary>
        /// <param name="symbol">String of trading pair</param>
        /// <param name="volume">Volume to set buy price</param>
        /// <returns>Decimal of price</returns>
        OrderBookDetail GetResistance(string symbol, decimal volume);
    }
}
