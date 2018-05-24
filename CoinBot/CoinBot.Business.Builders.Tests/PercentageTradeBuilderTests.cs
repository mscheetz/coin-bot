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
        private BotConfig _botConfig;
        private List<Candlestick> _candlestickList;
        private List<Candlestick> _mooningList;
        private List<Candlestick> _tankingList;

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
            _botConfig = new BotConfig
            {
                apiKey = It.IsAny<string>(),
                apiSecret = It.IsAny<string>(),
                privateToken = It.IsAny<string>()
            };
            _candlestickList = new List<Candlestick>
            {
                new Candlestick
                {
                    close = 0.000010M
                },
                new Candlestick
                {
                    close = 0.00002M
                },
                new Candlestick
                {
                    close = 0.000010M
                },
                new Candlestick
                {
                    close = 0.000010201M
                },
            };
            _mooningList = new List<Candlestick>
            {
                new Candlestick { close = 0.00002M },
                new Candlestick { close = 0.000021M },
                new Candlestick { close = 0.000022M },
                new Candlestick { close = 0.000023M },
                new Candlestick { close = 0.000024M },
                new Candlestick { close = 0.000025M },
                new Candlestick { close = 0.000024M },
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
            _fileRepo.Setup(f => f.GetConfig()).Returns(_botConfig);
            _tradeBldr.Setup(f => f.SetupRepository()).Returns(true);
            var csOne = new Candlestick[1] { _candlestickList[0] };
            var csTwo = new Candlestick[1] { _candlestickList[1] };
            var csThree = new Candlestick[1] { _candlestickList[2] };
            _tradeBldr.SetupSequence(f => f.GetCandlesticks(It.IsAny<string>(), It.IsAny<Interval>(), It.IsAny<int>()))
                .Returns(csOne)
                .Returns(csTwo)
                .Returns(csThree);
            _tradeBldr.SetupSequence(t => t.StoppedOutCheck(It.IsAny<decimal>()))
                .Returns(null)
                .Returns(null)
                .Returns(null);
            _tradeBldr.SetupSequence(f => f.BuyCrypto(It.IsAny<decimal>()))
                .Returns(true)
                .Returns(true);
            _tradeBldr.Setup(f => f.SellCrypto(It.IsAny<decimal>()))
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
        public void BuyPercentReached_False_PercentLow_Test()
        {
            decimal price = 0.00001M;
            _bldr = new PercentageTradeBuilder(_repo.Object, _fileRepo.Object, _tradeBldr.Object, _settings, 0.00001M, 0.000015M);

            var result = _bldr.BuyPercentReached(price);

            Assert.False(result);
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
        public void SellPercentReached_False_Current_GT_LastSell_Test()
        {
            decimal price = 0.00002M;
            _bldr = new PercentageTradeBuilder(_repo.Object, _fileRepo.Object, _tradeBldr.Object, _settings, 0.000015M, 0.000015M);

            var result = _bldr.SellPercentReached(price);

            Assert.False(result);
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
            var csI = new Candlestick[1] { _mooningList[0] };
            var csII = new Candlestick[1] { _mooningList[1] };
            var csIII = new Candlestick[1] { _mooningList[2] };
            var csIV = new Candlestick[1] { _mooningList[3] };
            var csV = new Candlestick[1] { _mooningList[4] };
            var csVI = new Candlestick[1] { _mooningList[5] };
            var csVII = new Candlestick[1] { _mooningList[6] };
            _tradeBldr.SetupSequence(f => f.GetCandlesticks(It.IsAny<string>(), It.IsAny<Interval>(), It.IsAny<int>()))
                .Returns(csI)
                .Returns(csII)
                .Returns(csIII)
                .Returns(csIV)
                .Returns(csV)
                .Returns(csVI)
                .Returns(csVII);

            _bldr = new PercentageTradeBuilder(_repo.Object, _fileRepo.Object, _tradeBldr.Object, _settings, lastPrice, lastPrice, TradeType.SELL);

            var result = _bldr.MooningAndTankingCheck(_candlestickList[0]);

            Assert.True(result);
        }

        [Fact]
        public void MooningAndTankingCheck_Tanking_Test()
        {
            var interval = Interval.OneM;
            _settings.chartInterval = interval;
            _settings.mooningTankingTime = 1;
            decimal lastPrice = 0.00001M;
            var csI = new Candlestick[1] { _tankingList[0] };
            var csII = new Candlestick[1] { _tankingList[1] };
            var csIII = new Candlestick[1] { _tankingList[2] };
            var csIV = new Candlestick[1] { _tankingList[3] };
            var csV = new Candlestick[1] { _tankingList[4] };
            var csVI = new Candlestick[1] { _tankingList[5] };
            var csVII = new Candlestick[1] { _tankingList[6] };
            _tradeBldr.SetupSequence(f => f.GetCandlesticks(It.IsAny<string>(), It.IsAny<Interval>(), It.IsAny<int>()))
                .Returns(csI)
                .Returns(csII)
                .Returns(csIII)
                .Returns(csIV)
                .Returns(csV)
                .Returns(csVI)
                .Returns(csVII);

            _bldr = new PercentageTradeBuilder(_repo.Object, _fileRepo.Object, _tradeBldr.Object, _settings, lastPrice, lastPrice, TradeType.SELL);

            var result = _bldr.MooningAndTankingCheck(_candlestickList[0]);

            Assert.True(result);
        }
    }
}
