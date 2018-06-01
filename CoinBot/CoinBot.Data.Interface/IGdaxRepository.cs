using CoinBot.Business.Entities;
using GDAXSharp.Services.Orders.Models.Responses;
using GDAXSharp.Services.Products.Models;
using GDAXSharp.Services.Products.Models.Responses;
using GDAXSharp.Services.Products.Types;
using GDAXSharp.WebSocket.Models.Response;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CoinBot.Data.Interface
{
    public interface IGdaxRepository
    {
        /// <summary>
        /// Check if the Exchange Repository is ready for trading
        /// </summary>
        /// <returns>Boolean of validation</returns>
        bool ValidateExchangeConfigured();

        /// <summary>
        /// Set ApiInformation for repository
        /// </summary>
        /// <param name="apiInfo">ApiInformation object</param>
        /// <returns>Boolean when complete</returns>
        bool SetExchangeApi(ApiInformation apiInfo);

        /// <summary>
        /// Build GDAX Client
        /// </summary>
        void BuildClient();

        /// <summary>
        /// Get Candlesticks for a trading pair
        /// </summary>
        /// <param name="pair">Trading pair</param>
        /// <param name="stickCount">Number of sticks to return</param>
        /// <param name="candleGranularity">CandleGranularity enum</param>
        /// <returns>Candle array</returns>
        Task<Candle[]> GetCandleSticks(string pair, int stickCount, CandleGranularity candleGranularity);

        /// <summary>
        /// Get Current Order book
        /// </summary>
        /// <param name="pair">Trading pair</param>
        /// <returns>ProductsOrderBookResponse object</returns>
        Task<ProductsOrderBookResponse> GetOrderBook(string pair);

        /// <summary>
        /// Get recent trades
        /// </summary>
        /// <param name="pair">Trading pair</param>
        /// <returns>GdaxTrade array</returns>
        Task<GdaxTrade[]> GetTrades(string pair);

        Task<ProductTicker> GetTicker(string pair);

        Task<ProductStats> GetStats(string pair);

        /// <summary>
        /// Get Balances for GDAX account
        /// </summary>
        /// <returns>Accout object</returns>
        Task<IEnumerable<GDAXSharp.Services.Accounts.Models.Account>> GetBalance();

        /// <summary>
        /// Place a limit order trade
        /// </summary>
        /// <param name="tradeParams">TradeParams for setting the trade</param>
        /// <returns>OrderResponse object</returns>
        Task<GDAXSharp.Services.Orders.Models.Responses.OrderResponse> PlaceTrade(TradeParams tradeParams);

        /// <summary>
        /// Place a stop limit trade
        /// </summary>
        /// <param name="tradeParams">TradeParams for setting the SL</param>
        /// <returns>OrderResponse object</returns>
        Task<GDAXSharp.Services.Orders.Models.Responses.OrderResponse> PlaceStopLimit(TradeParams tradeParams);

        /// <summary>
        /// Get details of an order
        /// </summary>
        /// <param name="id">Order Id</param>
        /// <returns>OrderResponse object</returns>
        Task<GDAXSharp.Services.Orders.Models.Responses.OrderResponse> GetOrder(string id);

        /// <summary>
        /// Cancel a placed trade
        /// </summary>
        /// <param name="id">Id of trade to cancel</param>
        /// <returns>CancelOrderResponse object</returns>
        Task<CancelOrderResponse> CancelTrade(string id);
    }
}
