using CoinBot.Business.Builders;
using CoinBot.Business.Builders.Interface;
using CoinBot.Business.Entities;
using CoinBot.Core;
using CoinBot.Data;
using CoinBot.Data.Interface;
using CoinBot.Tests.Entities;
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
        private Mock<IKuCoinRepository> _kuRepo;
        private Mock<IFileRepository> _fileRepo;
        private TestObjects _testObjects;
        private ITradeBuilder _tradeBuilder;
        private IExchangeBuilder _xchBuilder;
        private decimal _orderPrice;

        public ExchangeBuilderTests()
        {
            _testObjects = new TestObjects();
            _orderPrice = 0.001234M;
            _binanceRepo = new Mock<IBinanceRepository>();
            _gdaxRepo = new Mock<IGdaxRepository>();
            _kuRepo = new Mock<IKuCoinRepository>();
            _fileRepo = new Mock<IFileRepository>();
        }

        public void Dispose()
        {

        }

        [Fact]
        public void BuyCrypto_Success_Test()
        {
            // Arrange
            _gdaxRepo.Setup(g => g.GetTrades(It.IsAny<string>())).ReturnsAsync(_testObjects.GetGdaxTrades().ToArray());

            _xchBuilder = new ExchangeBuilder(_binanceRepo.Object, _gdaxRepo.Object, _kuRepo.Object);
            _xchBuilder.SetExchange(_testObjects.GetBotSettings());

            // Act
            var response = _xchBuilder.GetSticksFromGdaxTrades(_testObjects.GetGdaxTrades().ToArray(), 4);

            // Assert
            Assert.True(response != null);
        }

        [Fact]
        public void PlaceTrade_GDAX_Test()
        {
            // Arrange
            var settings = _testObjects.GetBotSettings();
            settings.exchange = Exchange.GDAX;
            _gdaxRepo.Setup(g => g.PlaceTrade(It.IsAny<TradeParams>()))
                     .ReturnsAsync(_testObjects.GetGDAXOrderResponse());
            _xchBuilder = new ExchangeBuilder(_binanceRepo.Object, _gdaxRepo.Object, _kuRepo.Object);
            _xchBuilder.SetExchange(settings);

            // Act
            var response = _xchBuilder.PlaceTrade(_testObjects.GetTradeParams());

            // Assert
            Assert.True(response != null);
        }
    }
}
