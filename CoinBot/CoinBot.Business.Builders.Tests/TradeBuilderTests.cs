using CoinBot.Business.Builders;
using CoinBot.Business.Builders.Interface;
using CoinBot.Business.Entities;
using CoinBot.Data;
using CoinBot.Data.Interface;
using Moq;
using System;
using System.Collections.Generic;
using Xunit;

namespace CoinBot.Business.Builders.Tests
{
    public class TradeBuilderTests : IDisposable
    {
        private Mock<IBinanceRepository> _binanceRepo;
        private Mock<IFileRepository> _fileRepo;
        private Mock<IExchangeBuilder> _exchangeBldr;
        private ITradeBuilder _tradeBuilder;
        private BotSettings _settings;
        private BotConfig _botConfig;
        private TradeResponse _tradeResponse;
        private OrderResponse _orderResponse;
        private decimal _orderPrice;
        private List<Balance> _balanceList;
        private Account _account;
        private List<BotBalance> _botBalanceList;


        public TradeBuilderTests()
        {
            _orderPrice = 0.001234M;
            _binanceRepo = new Mock<IBinanceRepository>();
            _fileRepo = new Mock<IFileRepository>();
            _exchangeBldr = new Mock<IExchangeBuilder>();
            _settings = new BotSettings
            {
                stopLoss = 2,
                sellPercent = 1,
                chartInterval = Interval.OneM,
                tradingPair = "BTCUSDT",
                startBotAutomatically = false
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
        }

        public void Dispose()
        {

        }

        [Fact]
        public void BuyCrypto_Success_Test()
        {
            _exchangeBldr.Setup(b => b.PlaceTrade(It.IsAny<TradeParams>())).Returns(_tradeResponse);
            _exchangeBldr.Setup(b => b.GetOrderDetail(It.IsAny<TradeResponse>(), It.IsAny<string>())).Returns(_orderResponse);
            _exchangeBldr.Setup(b => b.PlaceTrade(It.IsAny<TradeParams>())).Returns(_tradeResponse);
            _exchangeBldr.Setup(b => b.GetBalance()).Returns(_account.balances);
            _fileRepo.Setup(f => f.LogTransaction(It.IsAny<TradeInformation>())).Returns(true);
            _fileRepo.Setup(f => f.GetSettings()).Returns(_settings);

            _tradeBuilder = new TradeBuilder(_fileRepo.Object, _exchangeBldr.Object, _botBalanceList);
            
            var response = _tradeBuilder.BuyCrypto(_orderPrice, TradeType.BUY);

            Assert.True(response);
        }        
    }
}
