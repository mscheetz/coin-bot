using CoinBot.Business.Builders.Interface;
using CoinBot.Business.Entities;
using CoinBot.Core;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace CoinBot.Tests.Entities
{
    public class TestObjects
    {
        private DateTimeHelper _dtHelper;
        private decimal _orderPrice;
        private DateTime _utcNow;
        private long _unixNow;

        public TestObjects()
        {
            _orderPrice = 0.001234M;
            _dtHelper = new DateTimeHelper();

            _utcNow = DateTime.UtcNow;
            _unixNow = _dtHelper.UTCtoUnixTime();
        }

        public BotSettings GetBotSettings()
        {
            var settings = new BotSettings
            {
                buyPercent = 1.0,
                lastBuy = 100.0M,
                mooningTankingPercent = 1.0,
                mooningTankingTime = 1000,
                priceCheck = 1000,
                startingAmount = 0,
                tradePercent = 1.0,
                tradingStatus = TradeStatus.LiveTrading,
                tradingStrategy = Strategy.Percentage,
                stopLoss = 2,
                sellPercent = 1,
                chartInterval = Interval.OneM,
                tradingPair = "BTCUSDT",
                startBotAutomatically = false,
                exchange = Exchange.GDAX
            };

            return settings;
        }

        public BotConfig GetBotConfig()
        {
            var botConfig = new BotConfig
            {
                apiKey = It.IsAny<string>(),
                apiSecret = It.IsAny<string>(),
                privateToken = It.IsAny<string>()
            };

            return botConfig;
        }

        public ApiInformation GetApiInfo()
        {
            var apiInfo = new ApiInformation
            {
                apiKey = It.IsAny<string>(),
                apiSecret = It.IsAny<string>(),
                extraValue = It.IsAny<string>()
            };

            return apiInfo;
        }

        public TradeResponse GetTradeResponse()
        {
            var tradeResponse = new TradeResponse
            {
                clientOrderId = "1",
                executedQty = 1,
                orderId = 1,
                origQty = 1,
                price = _orderPrice,
                side = TradeType.BUY,
                status = OrderStatus.FILLED,
                symbol = "BTCUSDT",
                timeInForce = TimeInForce.GTC,
                transactTime = _unixNow,
                type = OrderType.LIMIT
            };

            return tradeResponse;
        }

        public OrderResponse GetOrderResponse()
        {
            var orderResponse = new OrderResponse
            {
                clientOrderId = "1",
                executedQty = 1,
                orderId = 1,
                origQty = 1,
                icebergQty = 1,
                isWorking = true,
                price = _orderPrice,
                side = TradeType.BUY,
                status = OrderStatus.FILLED,
                stopPrice = 0,
                symbol = "BTCUSDT",
                time = _unixNow,
                timeInForce = TimeInForce.GTC,
                type = OrderType.LIMIT
            };

            return orderResponse;
        }

        public GDAXSharp.Services.Orders.Models.Responses.OrderResponse GetGDAXOrderResponse()
        {
            var gdaxOrderResponse = new GDAXSharp.Services.Orders.Models.Responses.OrderResponse
            {
                CreatedAt = _utcNow,
                DoneAt = _utcNow,
                DoneReason = "Filled",
                ExecutedValue = 0.0000M,
                FilledSize = 0.000M,
                FillFees = 0.000M,
                Id = new Guid(),
                OrderType = GDAXSharp.Services.Orders.Types.OrderType.Limit,
                PostOnly = true,
                Price = 0.000M,
                ProductId = GDAXSharp.Shared.Types.ProductType.BtcUsd,
                Settled = true,
                Side = GDAXSharp.Services.Orders.Types.OrderSide.Buy,
                Size = 0.000M,
                SpecifiedFunds = 0.000M,
                Status = GDAXSharp.Services.Orders.Types.OrderStatus.Done,
                Stp = string.Empty,
                TimeInForce = GDAXSharp.Services.Orders.Types.TimeInForce.Gtc
            };

            return gdaxOrderResponse;
        }
        public Account GetAccount()
        {
            var account = new Account
            {
                balances = GetBalanceList()
            };

            return account;
        }

        public List<Balance> GetBalanceList()
        {
            var balanceList = new List<Balance>
            {
                new Balance
                {
                    asset = "BTC",
                    free = 1,
                    locked = 0
                },
                new Balance
                {
                    asset = "USDT",
                    free = 0,
                    locked = 0
                },
            };

            return balanceList;
        }

        public List<BotBalance> GetBotBalanceList()
        {
            var balanceList = GetBalanceList();

            var botBalanceList = new List<BotBalance>
            {
                new BotBalance
                {
                    quantity = balanceList[0].free,
                    symbol = balanceList[0].asset,
                    timestamp = DateTime.UtcNow

                },
                new BotBalance
                {
                    quantity = balanceList[1].free,
                    symbol = balanceList[1].asset,
                    timestamp = DateTime.UtcNow

                }
            };

            return botBalanceList;
        }

        public string[] GetTimes()
        {
            var times  = new string[]
            {
                "01/01/2018 10:05:45.20",// 0
                "01/01/2018 10:05:42.30",// 1
                "01/01/2018 10:05:41.01",// 2
                "01/01/2018 10:05:35.20",// 3
                "01/01/2018 10:05:23.20",// 4
                "01/01/2018 10:05:23.10",// 5
                "01/01/2018 10:05:23.01",// 6
                "01/01/2018 10:04:45.20",// 7
                "01/01/2018 10:04:42.30",// 8
                "01/01/2018 10:04:41.01",// 9
                "01/01/2018 10:04:35.20",//10
                "01/01/2018 10:04:23.20",//11
                "01/01/2018 10:04:23.10",//12
                "01/01/2018 10:04:23.01",//13
            };
            return times;
        }

        public List<GdaxTrade> GetGdaxTrades()
        {
            var times = GetTimes();
            var gdaxTrades = new List<GdaxTrade>
            {
                new GdaxTrade
                {
                    Price = 100.00M,
                    Side = "buy",
                    Size = 0.2M,
                    Time = Convert.ToDateTime(times[0]),
                },
                new GdaxTrade
                {
                    Price = 101.00M,
                    Side = "buy",
                    Size = 0.3M,
                    Time = Convert.ToDateTime(times[1]),
                },
                new GdaxTrade
                {
                    Price = 99.00M,
                    Side = "buy",
                    Size = 1M,
                    Time = Convert.ToDateTime(times[2]),
                },
                new GdaxTrade
                {
                    Price = 100.00M,
                    Side = "buy",
                    Size = 0.25M,
                    Time = Convert.ToDateTime(times[3]),
                },
                new GdaxTrade
                {
                    Price = 100.00M,
                    Side = "buy",
                    Size = 0.4M,
                    Time = Convert.ToDateTime(times[4]),
                },
                new GdaxTrade
                {
                    Price = 100.00M,
                    Side = "buy",
                    Size = 4M,
                    Time = Convert.ToDateTime(times[5]),
                },
                new GdaxTrade
                {
                    Price = 100.00M,
                    Side = "buy",
                    Size = 20M,
                    Time = Convert.ToDateTime(times[6]),
                },
                new GdaxTrade
                {
                    Price = 100.00M,
                    Side = "buy",
                    Size = 0.023M,
                    Time = Convert.ToDateTime(times[7]),
                },
                new GdaxTrade
                {
                    Price = 400.00M,
                    Side = "buy",
                    Size = 0.023M,
                    Time = Convert.ToDateTime(times[8]),
                },
                new GdaxTrade
                {
                    Price = 100.00M,
                    Side = "buy",
                    Size = 0.023M,
                    Time = Convert.ToDateTime(times[9]),
                },
                new GdaxTrade
                {
                    Price = 100.00M,
                    Side = "buy",
                    Size = 0.023M,
                    Time = Convert.ToDateTime(times[10]),
                },
                new GdaxTrade
                {
                    Price = 100.00M,
                    Side = "buy",
                    Size = 0.023M,
                    Time = Convert.ToDateTime(times[11]),
                },
                new GdaxTrade
                {
                    Price = 100.00M,
                    Side = "buy",
                    Size = 0.023M,
                    Time = Convert.ToDateTime(times[12]),
                },
                new GdaxTrade
                {
                    Price = 100.00M,
                    Side = "buy",
                    Size = 0.023M,
                    Time = Convert.ToDateTime(times[13]),
                }
            };

            return gdaxTrades;
        }

        public TradeParams GetTradeParams()
        {
            var tradeParams = new TradeParams
            {
                icebergQty = 0.00M,
                price = 0.00M,
                quantity = 0.00M,
                side = "buy",
                stopPrice = 0.00M,
                symbol = "BTCUSD",
                timeInForce = "GTC",
                timestamp = _unixNow,
                type = "buy"
            };

            return tradeParams;
        }

        public List<BotStick> GetBotSticks()
        {
            var candlestickList = new List<BotStick>
            {
                new BotStick
                {
                    close = 0.000010M
                },
                new BotStick
                {
                    close = 0.00002M
                },
                new BotStick
                {
                    close = 0.000010M
                },
                new BotStick
                {
                    close = 0.000010201M
                },
            };

            return candlestickList;
        }

        public List<BotStick> GetMooningList()
        {
            var mooningList = new List<BotStick>
            {
                new BotStick { close = 0.00002M },
                new BotStick { close = 0.000021M },
                new BotStick { close = 0.000022M },
                new BotStick { close = 0.000023M },
                new BotStick { close = 0.000024M },
                new BotStick { close = 0.000025M },
                new BotStick { close = 0.000024M },
            };

            return mooningList;
        }

        public List<BotStick> GetTankingList()
        {
            var tankingList = GetMooningList()
                                .OrderByDescending(m => m.close)
                                .ToList();

            return tankingList;
        }
    }
}
