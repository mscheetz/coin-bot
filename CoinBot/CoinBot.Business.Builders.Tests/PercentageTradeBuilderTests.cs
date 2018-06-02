using CoinBot.Business.Builders;
using CoinBot.Business.Builders.Interface;
using CoinBot.Business.Entities;
using CoinBot.Data;
using CoinBot.Data.Interface;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace CoinBot.Business.Builders.Tests
{
    public class PercentageTradeBuilderTests : IDisposable
    {
        private IPercentageTradeBuilder _bldr;
        private Mock<IBinanceRepository> _repo;
        private Mock<ITradeBuilder> _tradeBldr;
        private Mock<IFileRepository> _fileRepo;
        private BotSettings _settings;
        private ApiInformation _apiInfo;
        private List<BotStick> _candlestickList;
        private List<BotStick> _mooningList;
        private List<BotStick> _tankingList;

        public PercentageTradeBuilderTests()
        {
            _fileRepo = new Mock<IFileRepository>();
            _repo = new Mock<IBinanceRepository>();
            _tradeBldr = new Mock<ITradeBuilder>();
            _settings = new BotSettings
            {
                stopLoss = 2,
                buyPercent = 1,
                sellPercent = 1,
                chartInterval = Interval.OneM,
                tradingPair = "XRPBTC",
                startBotAutomatically = false,
                mooningTankingTime = 0
            };
            _apiInfo = new ApiInformation
            {
                apiKey = It.IsAny<string>(),
                apiSecret = It.IsAny<string>(),
                extraValue = It.IsAny<string>()
            };
            _candlestickList = new List<BotStick>
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
            _mooningList = new List<BotStick>
            {
                new BotStick { close = 0.00002M },
                new BotStick { close = 0.000021M },
                new BotStick { close = 0.000022M },
                new BotStick { close = 0.000023M },
                new BotStick { close = 0.000024M },
                new BotStick { close = 0.000025M },
                new BotStick { close = 0.000024M },
            };
            _tankingList = _mooningList
                                .OrderByDescending(m => m.close)
                                .ToList();
        }

        public void Dispose()
        {

        }

        [Fact]
        public void RunBot_3Cycles_Test()
        {
            var interval = Interval.OneM;
            var candlestickArray = _candlestickList.ToArray();
            _settings.chartInterval = interval;
            _fileRepo.Setup(f => f.GetSettings()).Returns(_settings);
            _fileRepo.Setup(f => f.GetConfig()).Returns(_apiInfo);
            _tradeBldr.Setup(f => f.SetupRepository()).Returns(true);
            var csOne = new BotStick[1] { _candlestickList[0] };
            var csTwo = new BotStick[1] { _candlestickList[1] };
            var csThree = new BotStick[1] { _candlestickList[2] };
            _tradeBldr.SetupSequence(f => f.GetCandlesticks(It.IsAny<string>(), It.IsAny<Interval>(), It.IsAny<int>()))
                .Returns(csOne)
                .Returns(csTwo)
                .Returns(csThree);
            _tradeBldr.SetupSequence(t => t.StoppedOutCheck(It.IsAny<decimal>()))
                .Returns(null)
                .Returns(null)
                .Returns(null);
            _tradeBldr.SetupSequence(f => f.BuyCrypto(It.IsAny<decimal>(), It.IsAny<TradeType>()))
                .Returns(true)
                .Returns(true);
            _tradeBldr.Setup(f => f.SellCrypto(It.IsAny<decimal>(), It.IsAny<TradeType>()))
                .Equals(true);

            _bldr = new PercentageTradeBuilder(_repo.Object, _fileRepo.Object, _tradeBldr.Object, _settings);
            //_bldr.SetBotSettings(_settings);

            var result = _bldr.RunBot(interval, 3, true);

            Assert.True(result);
        }

        [Fact]
        public void BuyPercentReached_True_Test()
        {
            decimal price = 0.00001M;
            _bldr = new PercentageTradeBuilder(_repo.Object, _fileRepo.Object, _tradeBldr.Object, _settings, 0.00001M, 0.00002M);

            var result = _bldr.BuyPercentReached(price);

            Assert.True(result);
        }

        [Fact]
        public void BuyPercentReached_True_PercentLow_Test()
        {
            decimal price = 0.00001M;
            _bldr = new PercentageTradeBuilder(_repo.Object, _fileRepo.Object, _tradeBldr.Object, _settings, 0.00001M, 0.000015M);

            var result = _bldr.BuyPercentReached(price);

            Assert.True(result);
        }

        [Fact]
        public void BuyPercentReached_False_Current_GT_LastSell_Test()
        {
            decimal price = 0.00002M;
            _bldr = new PercentageTradeBuilder(_repo.Object, _fileRepo.Object, _tradeBldr.Object, _settings, 0.00001M, 0.000015M);

            var result = _bldr.BuyPercentReached(price);

            Assert.False(result);
        }

        [Fact]
        public void BuyPercentReached_False_Equal_Test()
        {
            decimal price = 0.00001M;
            _bldr = new PercentageTradeBuilder(_repo.Object, _fileRepo.Object, _tradeBldr.Object, _settings, price, price);

            var result = _bldr.BuyPercentReached(price);

            Assert.False(result);
        }

        [Fact]
        public void SellPercentReached_True_Test()
        {
            decimal price = 0.00002M;
            _bldr = new PercentageTradeBuilder(_repo.Object, _fileRepo.Object, _tradeBldr.Object, _settings, 0.00001M, 0.00001M);

            var result = _bldr.SellPercentReached(price);

            Assert.True(result);
        }

        [Fact]
        public void SellPercentReached_False_PercentLow_Test()
        {
            decimal price = 0.00001M;
            _bldr = new PercentageTradeBuilder(_repo.Object, _fileRepo.Object, _tradeBldr.Object, _settings, 0.000015M, 0.000015M);

            var result = _bldr.SellPercentReached(price);

            Assert.False(result);
        }

        [Fact]
        public void SellPercentReached_True_Current_GT_LastSell_Test()
        {
            decimal price = 0.00002M;
            _bldr = new PercentageTradeBuilder(_repo.Object, _fileRepo.Object, _tradeBldr.Object, _settings, 0.000015M, 0.000015M);

            var result = _bldr.SellPercentReached(price);

            Assert.True(result);
        }

        [Fact]
        public void SellPercentReached_False_Equal_Test()
        {
            decimal price = 0.00001M;
            _bldr = new PercentageTradeBuilder(_repo.Object, _fileRepo.Object, _tradeBldr.Object, _settings, price, price);

            var result = _bldr.SellPercentReached(price);

            Assert.False(result);
        }

        [Fact]
        public void MooningAndTankingCheck_Mooning_Test()
        {
            var interval = Interval.OneM;
            _settings.chartInterval = interval;
            _settings.mooningTankingTime = 1;
            decimal lastPrice = 0.00001M;
            var csI = new BotStick[1] { _mooningList[0] };
            var csII = new BotStick[1] { _mooningList[1] };
            var csIII = new BotStick[1] { _mooningList[2] };
            var csIV = new BotStick[1] { _mooningList[3] };
            var csV = new BotStick[1] { _mooningList[4] };
            var csVI = new BotStick[1] { _mooningList[5] };
            var csVII = new BotStick[1] { _mooningList[6] };
            _tradeBldr.SetupSequence(f => f.GetCandlesticks(It.IsAny<string>(), It.IsAny<Interval>(), It.IsAny<int>()))
                .Returns(csI)
                .Returns(csII)
                .Returns(csIII)
                .Returns(csIV)
                .Returns(csV)
                .Returns(csVI)
                .Returns(csVII);

            _bldr = new PercentageTradeBuilder(_repo.Object, _fileRepo.Object, _tradeBldr.Object, _settings, lastPrice, lastPrice, TradeType.SELL);

            var result = _bldr.MooningAndTankingCheck(_candlestickList[0], TradeType.SELL);

            Assert.True(result == TradeType.SELL);
        }

        [Fact]
        public void MooningAndTankingCheck_Tanking_Test()
        {
            var interval = Interval.OneM;
            _settings.chartInterval = interval;
            _settings.mooningTankingTime = 1;
            decimal lastPrice = 0.00001M;
            var csI = new BotStick[1] { _tankingList[0] };
            var csII = new BotStick[1] { _tankingList[1] };
            var csIII = new BotStick[1] { _tankingList[2] };
            var csIV = new BotStick[1] { _tankingList[3] };
            var csV = new BotStick[1] { _tankingList[4] };
            var csVI = new BotStick[1] { _tankingList[5] };
            var csVII = new BotStick[1] { _tankingList[6] };
            _tradeBldr.SetupSequence(f => f.GetCandlesticks(It.IsAny<string>(), It.IsAny<Interval>(), It.IsAny<int>()))
                .Returns(csI)
                .Returns(csII)
                .Returns(csIII)
                .Returns(csIV)
                .Returns(csV)
                .Returns(csVI)
                .Returns(csVII);

            _bldr = new PercentageTradeBuilder(_repo.Object, _fileRepo.Object, _tradeBldr.Object, _settings, lastPrice, lastPrice, TradeType.SELL);

            var result = _bldr.MooningAndTankingCheck(_candlestickList[0], TradeType.SELL);

            Assert.True(result == TradeType.SELL);
        }
    }
}
