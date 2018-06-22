using CoinBot.Business.Builders;
using CoinBot.Business.Builders.Interface;
using CoinBot.Business.Entities;
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
    public class VolumeTradeBuilderTests : IDisposable
    {
        private IVolumeTradeBuilder _bldr;
        private Mock<ITradeBuilder> _tradeBldr;
        private TestObjects _testObjs;
        private BotSettings _settings;

        public VolumeTradeBuilderTests()
        {
            _tradeBldr = new Mock<ITradeBuilder>();
            _testObjs = new TestObjects();
            _settings = _testObjs.GetBotSettings();
        }

        public void Dispose()
        {

        }

        [Fact]
        public void RunBot_3Cycles_Test()
        {
            var interval = Interval.OneM;
            var candlestickArray = _testObjs.GetBotSticks().ToArray();
            
            _settings.chartInterval = interval;
            _tradeBldr.Setup(f => f.SetupRepository()).Returns(true);
            var csOne = new BotStick[1] { candlestickArray[0] };
            var csTwo = new BotStick[1] { candlestickArray[1] };
            var csThree = new BotStick[1] { candlestickArray[2] };
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

            _bldr = new VolumeTradeBuilder(_tradeBldr.Object, _settings);
            //_bldr.SetBotSettings(_settings);

            var result = _bldr.RunBot(interval, 3, true);

            Assert.True(result);
        }

        [Fact]
        public void BuyPercentReached_True_Test()
        {
            decimal price = 0.00001M;
            _bldr = new VolumeTradeBuilder(_tradeBldr.Object, _settings, 0.00001M, 0.00002M);

            var result = _bldr.BuyPercentReached(price);

            Assert.True(result);
        }

        [Fact]
        public void BuyPercentReached_True_PercentLow_Test()
        {
            decimal price = 0.00001M;
            _bldr = new VolumeTradeBuilder(_tradeBldr.Object, _settings, 0.00001M, 0.000015M);

            var result = _bldr.BuyPercentReached(price);

            Assert.True(result);
        }

        [Fact]
        public void BuyPercentReached_False_Current_GT_LastSell_Test()
        {
            decimal price = 0.00002M;
            _bldr = new VolumeTradeBuilder(_tradeBldr.Object, _settings, 0.00001M, 0.000015M);

            var result = _bldr.BuyPercentReached(price);

            Assert.False(result);
        }

        [Fact]
        public void BuyPercentReached_False_Equal_Test()
        {
            decimal price = 0.00001M;
            _bldr = new VolumeTradeBuilder(_tradeBldr.Object, _settings, price, price);

            var result = _bldr.BuyPercentReached(price);

            Assert.False(result);
        }

        [Fact]
        public void SellPercentReached_True_Test()
        {
            decimal price = 0.00002M;
            _bldr = new VolumeTradeBuilder(_tradeBldr.Object, _settings, 0.00001M, 0.00001M);

            var result = _bldr.SellPercentReached(price);

            Assert.True(result);
        }

        [Fact]
        public void SellPercentReached_False_PercentLow_Test()
        {
            decimal price = 0.00001M;
            _bldr = new VolumeTradeBuilder(_tradeBldr.Object, _settings, 0.000015M, 0.000015M);

            var result = _bldr.SellPercentReached(price);

            Assert.False(result);
        }

        [Fact]
        public void SellPercentReached_True_Current_GT_LastSell_Test()
        {
            decimal price = 0.00002M;
            _bldr = new VolumeTradeBuilder(_tradeBldr.Object, _settings, 0.000015M, 0.000015M);

            var result = _bldr.SellPercentReached(price);

            Assert.True(result);
        }

        [Fact]
        public void SellPercentReached_False_Equal_Test()
        {
            decimal price = 0.00001M;
            _bldr = new VolumeTradeBuilder(_tradeBldr.Object, _settings, price, price);

            var result = _bldr.SellPercentReached(price);

            Assert.False(result);
        }

        [Fact]
        public void MooningAndTankingCheck_Mooning_Test()
        {
            var interval = Interval.OneM;
            _settings.chartInterval = interval;
            _settings.mooningTankingTime = 1;
            var mooningList = _testObjs.GetMooningList();
            var candlestickList = _testObjs.GetBotSticks();
            decimal lastPrice = 0.00001M;
            var csI = new BotStick[1] { mooningList[0] };
            var csII = new BotStick[1] { mooningList[1] };
            var csIII = new BotStick[1] { mooningList[2] };
            var csIV = new BotStick[1] { mooningList[3] };
            var csV = new BotStick[1] { mooningList[4] };
            var csVI = new BotStick[1] { mooningList[5] };
            var csVII = new BotStick[1] { mooningList[6] };
            _tradeBldr.SetupSequence(f => f.GetCandlesticks(It.IsAny<string>(), It.IsAny<Interval>(), It.IsAny<int>()))
                .Returns(csI)
                .Returns(csII)
                .Returns(csIII)
                .Returns(csIV)
                .Returns(csV)
                .Returns(csVI)
                .Returns(csVII);

            _bldr = new VolumeTradeBuilder(_tradeBldr.Object, _settings, lastPrice, lastPrice, TradeType.SELL);

            var result = _bldr.MooningAndTankingCheck(candlestickList[0], candlestickList[1], TradeType.SELL);

            Assert.True(result == TradeType.SELL);
        }

        [Fact]
        public void MooningAndTankingCheck_Tanking_Test()
        {
            var interval = Interval.OneM;
            _settings.chartInterval = interval;
            _settings.mooningTankingTime = 1;
            decimal lastPrice = 0.00001M;
            var tankingList = _testObjs.GetTankingList();
            var candlestickList = _testObjs.GetBotSticks();
            var csI = new BotStick[1] { tankingList[0] };
            var csII = new BotStick[1] { tankingList[1] };
            var csIII = new BotStick[1] { tankingList[2] };
            var csIV = new BotStick[1] { tankingList[3] };
            var csV = new BotStick[1] { tankingList[4] };
            var csVI = new BotStick[1] { tankingList[5] };
            var csVII = new BotStick[1] { tankingList[6] };
            _tradeBldr.SetupSequence(f => f.GetCandlesticks(It.IsAny<string>(), It.IsAny<Interval>(), It.IsAny<int>()))
                .Returns(csI)
                .Returns(csII)
                .Returns(csIII)
                .Returns(csIV)
                .Returns(csV)
                .Returns(csVI)
                .Returns(csVII);

            _bldr = new VolumeTradeBuilder(_tradeBldr.Object, _settings, lastPrice, lastPrice, TradeType.SELL);

            var result = _bldr.MooningAndTankingCheck(candlestickList[0], candlestickList[1], TradeType.SELL);

            Assert.True(result == TradeType.SELL);
        }
    }
}
