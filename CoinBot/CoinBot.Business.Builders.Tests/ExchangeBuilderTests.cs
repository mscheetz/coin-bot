using CoinBot.Business.Builders;
using CoinBot.Business.Builders.Interface;
using CoinBot.Business.Entities;
using CoinBot.Data;
using CoinBot.Data.Interface;
using GDAXSharp.Services.Products.Models;
using Moq;
using System;
using System.Collections.Generic;
using Xunit;

namespace CoinBot.Business.Builders.Tests
{
    public class ExchangeBuilderTests : IDisposable
    {
        private Mock<IBinanceRepository> _binanceRepo;
        private Mock<IGdaxRepository> _gdaxRepo;
        private Mock<IFileRepository> _fileRepo;
        private ITradeBuilder _tradeBuilder;
        private IExchangeBuilder _xchBuilder;
        private BotSettings _settings;
        private BotConfig _botConfig;
        private TradeResponse _tradeResponse;
        private OrderResponse _orderResponse;
        private decimal _orderPrice;
        private List<Balance> _balanceList;
        private Account _account;
        private List<BotBalance> _botBalanceList;
        private List<GdaxTrade> _gdaxTrades;


        public ExchangeBuilderTests()
        {
            _orderPrice = 0.001234M;
            _binanceRepo = new Mock<IBinanceRepository>();
            _gdaxRepo = new Mock<IGdaxRepository>();
            _fileRepo = new Mock<IFileRepository>();
            _settings = new BotSettings
            {
                stopLoss = 2,
                sellPercent = 1,
                chartInterval = Interval.OneM,
                tradingPair = "BTCUSDT",
                startBotAutomatically = false,
                exchange = Exchange.GDAX
            };
            _botConfig = new BotConfig
            {
                apiKey = It.IsAny<string>(),
                apiSecret = It.IsAny<string>(),
                privateToken = It.IsAny<string>()
            };
            _tradeResponse = new TradeResponse
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
                transactTime = 123,
                type = OrderType.LIMIT
            };
            _orderResponse = new OrderResponse
            {
                clientOrderId = "1",
                executedQty = 1,
                orderId = 1,
                origQty = 1,
                iceburgQty = 1,
                isWorking = true,
                price = _orderPrice,
                side = TradeType.BUY,
                status = OrderStatus.FILLED,
                stopPrice = 0,
                symbol = "BTCUSDT",
                time = 123,
                timeInForce = TimeInForce.GTC,
                type = OrderType.LIMIT
            };
            _balanceList = new List<Balance>
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

            _account = new Account
            {
                balances = _balanceList
            };
            _botBalanceList = new List<BotBalance>
            {
                new BotBalance
                {
                    quantity = _balanceList[0].free,
                    symbol = _balanceList[0].asset,
                    timestamp = DateTime.UtcNow

                },
                new BotBalance
                {
                    quantity = _balanceList[1].free,
                    symbol = _balanceList[1].asset,
                    timestamp = DateTime.UtcNow

                }
            };

            var times = new string[] 
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

            _gdaxTrades = new List<GdaxTrade>
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
        }

        public void Dispose()
        {

        }

        [Fact]
        public void BuyCrypto_Success_Test()
        {
            // Arrange
            _gdaxRepo.Setup(g => g.GetTrades(It.IsAny<string>())).ReturnsAsync(_gdaxTrades.ToArray());

            _xchBuilder = new ExchangeBuilder(_binanceRepo.Object, _gdaxRepo.Object);
            _xchBuilder.SetExchange(_settings);

            // Act
            var response = _xchBuilder.GetSticksFromGdaxTrades(_gdaxTrades.ToArray());

            // Assert
            Assert.True(response != null);
        }        
    }
}
