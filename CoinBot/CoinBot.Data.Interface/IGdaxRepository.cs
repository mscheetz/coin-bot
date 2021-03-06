﻿using CoinBot.Business.Entities;
using CoinBot.Business.Entities.GDAX;
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
        /// <param name="sandbox">Boolean if to use sandbox (false by default)</param>
        /// <returns>Boolean when complete</returns>
        bool SetExchangeApi(ApiInformation apiInfo, bool sandbox = false);

        /// <summary>
        /// Build GDAX Client
        /// </summary>
        /// <param name="sandbox">Booelan if to use sandbox</param>
        void BuildClient(bool sandbox);

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
        /// <param name="level">Request level, default = 2</param>
        /// <returns>ProductsOrderBookResponse object</returns>
        Task<OrderBookResponse> GetOrderBook(string pair, int level = 2);

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
        /// <returns>Accout object array</returns>
        Task<GDAXAccount[]> GetBalance();
        
        /// <summary>
        /// Place a limit order trade
        /// </summary>
        /// <param name="tradeParams">TradeParams for setting the trade</param>
        /// <returns>OrderResponse object</returns>
        Task<GDAXSharp.Services.Orders.Models.Responses.OrderResponse> PlaceTrade(TradeParams tradeParams);

        /// <summary>
        /// Place a limit order trade
        /// </summary>
        /// <param name="tradeParams">GDAXTradeParams for setting the trade</param>
        /// <returns>GDAXOrderResponse object</returns>
        Task<GDAXOrderResponse> PlaceRestTrade(GDAXTradeParams tradeParams);
        
        /// <summary>
        /// Place a limit order trade
        /// </summary>
        /// <param name="tradeParams">TradeParams for setting the trade</param>
        /// <returns>OrderResponse object</returns>
        Task<GDAXSharp.Services.Orders.Models.Responses.OrderResponse> PlaceStopLimit(TradeParams tradeParams);

        /// <summary>
        /// Place a stop limit trade
        /// </summary>
        /// <param name="tradeParams">GDAXStopLostParams for setting the SL</param>
        /// <returns>GDAXOrderResponse object</returns>
        Task<GDAXOrderResponse> PlaceStopLimit(GDAXStopLossParams tradeParams);


        /// <summary>
        /// Get details of an order
        /// </summary>
        /// <param name="id">Order Id</param>
        /// <returns>OrderResponse object</returns>
        Task<GDAXSharp.Services.Orders.Models.Responses.OrderResponse> GetOrder(string id);

        /// <summary>
        /// Get details of an order
        /// </summary>
        /// <param name="id">Order Id</param>
        /// <returns>OrderResponse object</returns>
        Task<GDAXOrder> GetRestOrder(string id);

        /// <summary>
        /// Get all fills
        /// </summary>
        /// <returns>GDAXFill array</returns>
        Task<GDAXFill[]> GetRestOrders();

        /// <summary>
        /// Get all open orders
        /// </summary>
        /// <param name="pair">Trading pair</param>
        /// <returns>GDAXOrderResponse array</returns>
        Task<GDAXOrderResponse[]> GetOpenOrders(string pair = "");

        /// <summary>
        /// Cancel a placed trade
        /// </summary>
        /// <param name="id">Id of trade to cancel</param>
        /// <returns>CancelOrderResponse object</returns>
        Task<CancelOrderResponse> CancelTrade(string id);

        /// <summary>
        /// Cancel all open trades
        /// </summary>
        /// <returns>CancelOrderResponse object</returns>
        Task<CancelOrderResponse> CancelAllTrades();
        
        /// <summary>
        /// Cancel all open trades
        /// </summary>
        /// <returns>CancelOrderResponse object</returns>
        Task<CancelOrderResponse> CancelAllTradesRest();
    }
}
