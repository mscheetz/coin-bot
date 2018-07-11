using CoinBot.Business.Builders;
using CoinBot.Business.Builders.Interface;
using CoinBot.Business.Entities;
using CoinBot.Core;
using CoinBot.Data;
using CoinBot.Data.Interface;
using CoinBot.Tests.Entities;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace CoinBot.Business.Builders.Tests
{
    public class TradeBuilderTests : IDisposable
    {
        private Mock<IBinanceRepository> _binanceRepo;
        private Mock<IFileRepository> _fileRepo;
        private IFileRepository _fileRepoLive;
        private Mock<IExchangeBuilder> _exchangeBldr;
        private TestObjects _testObjects;
        private ITradeBuilder _tradeBuilder;
        private DateTimeHelper _dtHelper;

        public TradeBuilderTests()
        {
            _binanceRepo = new Mock<IBinanceRepository>();
            _fileRepo = new Mock<IFileRepository>();
            _exchangeBldr = new Mock<IExchangeBuilder>();
            _testObjects = new TestObjects();
            _dtHelper = new DateTimeHelper();
        }

        public void Dispose()
        {

        }

        [Fact]
        public void BuyCrypto_Success_Test()
        {
            // Arrange
            var orderPrice = 0.101M;
            _exchangeBldr.Setup(b => b.PlaceTrade(It.IsAny<TradeParams>())).Returns(_testObjects.GetTradeResponse());
            _exchangeBldr.Setup(b => b.GetOrderDetail(It.IsAny<TradeResponse>(), It.IsAny<string>())).Returns(_testObjects.GetOrderResponse());
            _exchangeBldr.Setup(b => b.PlaceTrade(It.IsAny<TradeParams>())).Returns(_testObjects.GetTradeResponse());
            _exchangeBldr.Setup(b => b.GetBalance(It.IsAny<string>(), It.IsAny<string>())).Returns(_testObjects.GetBalanceList());
            _fileRepo.Setup(f => f.LogTransaction(It.IsAny<TradeInformation>(), It.IsAny<bool>())).Returns(true);
            _fileRepo.Setup(f => f.GetSettings()).Returns(_testObjects.GetBotSettings());

            _tradeBuilder = new TradeBuilder(_fileRepo.Object, _exchangeBldr.Object, _testObjects.GetBotBalanceList());
            
            // Act
            var response = _tradeBuilder.BuyCrypto(orderPrice, TradeType.BUY);

            // Assert
            Assert.True(response);
        }

        [Fact]
        public void CaptureTransaction_Live_Test()
        {
            // Arrange
            _fileRepoLive = new FileRepository();
            var price = 500.0M;
            var quantity = 12.0M;
            var ts = _dtHelper.UTCtoUnixTime();
            var tsDT = _dtHelper.UnixTimeToUTC(ts);

            _tradeBuilder = new TradeBuilder(_fileRepoLive, _exchangeBldr.Object);

            // Act
            var response = _tradeBuilder.CaptureTransaction(price, quantity, ts, TradeType.BUY);

            // Assert
            Assert.True(response);

            // Act
            var lastTrade = _tradeBuilder.GetTradeHistory(1).FirstOrDefault();

            // Assert
            Assert.True(lastTrade.price == price);
            Assert.True(lastTrade.quantity == quantity);
            Assert.True(lastTrade.timestamp == tsDT);
        }

        [Fact]
        public void PricePadding_GDAX_Buy_Types_Test()
        {
            // Arrange
            var settings = _testObjects.GetBotSettings();
            var price = 100.0M;
            var tradeType = TradeType.BUY;
            settings.exchange = Exchange.GDAX;
            _fileRepo.Setup(f => f.GetSettings()).Returns(settings);
            _fileRepo.Setup(f => f.GetConfig()).Returns(_testObjects.GetApiInfo());
            _tradeBuilder = new TradeBuilder(_fileRepo.Object, _exchangeBldr.Object);

            // Act
            var paddedPrice = _tradeBuilder.GetPricePadding(tradeType, price);

            // Assert
            Assert.True(paddedPrice == 99.99M);
            
            // Act
            tradeType = TradeType.VOLUMEBUY;
            price = 200.00M;
            paddedPrice = _tradeBuilder.GetPricePadding(tradeType, price);

            // Assert
            Assert.True(paddedPrice == 199.99M);

            // Act
            tradeType = TradeType.VOLUMESELLBUYOFF;
            price = 300.00M;
            paddedPrice = _tradeBuilder.GetPricePadding(tradeType, price);

            // Assert
            Assert.True(paddedPrice == 299.99M);
        }
        [Fact]
        public void PricePadding_GDAX_Sell_Types_Test()
        {
            // Arrange
            var settings = _testObjects.GetBotSettings();
            var price = 100.0M;
            var tradeType = TradeType.SELL;
            settings.exchange = Exchange.GDAX;
            _fileRepo.Setup(f => f.GetSettings()).Returns(settings);
            _fileRepo.Setup(f => f.GetConfig()).Returns(_testObjects.GetApiInfo());
            _tradeBuilder = new TradeBuilder(_fileRepo.Object, _exchangeBldr.Object);

            // Act
            var paddedPrice = _tradeBuilder.GetPricePadding(tradeType, price);

            // Assert
            Assert.True(paddedPrice == 100.01M);

            // Act
            tradeType = TradeType.VOLUMESELL;
            price = 200.00M;
            paddedPrice = _tradeBuilder.GetPricePadding(tradeType, price);

            // Assert
            Assert.True(paddedPrice == 200.01M);

            // Act
            tradeType = TradeType.VOLUMEBUYSELLOFF;
            price = 300.00M;
            paddedPrice = _tradeBuilder.GetPricePadding(tradeType, price);

            // Assert
            Assert.True(paddedPrice == 300.01M);
        }
    }
}
