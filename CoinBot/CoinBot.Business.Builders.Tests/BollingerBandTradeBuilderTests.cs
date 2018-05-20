using CoinBot.Business.Builders;
using CoinBot.Business.Builders.Interface;
using CoinBot.Business.Entities;
using CoinBot.Data;
using CoinBot.Data.Interface;
using Moq;
using System;
using Xunit;

namespace CoinBot.Business.Builders.Tests
{
    public class BollingerBandTradeBuilderTests : IDisposable
    {
        private IBollingerBandTradeBuilder _bldr;
        private IBinanceRepository _repo;
        private Mock<IFileRepository> _fileRepo;
        private BotSettings _settings;
        private BotConfig _botConfig;

        public BollingerBandTradeBuilderTests()
        {
            _fileRepo = new Mock<IFileRepository>();
            _settings = new BotSettings
            {
                stopLoss = 2,
                sellPercent = 1,
                chartInterval = Interval.OneM,
                tradingPair = "XRPBTC",
                startBotAutomatically = false
            };
            _botConfig = new BotConfig
            {
                apiKey = It.IsAny<string>(),
                apiSecret = It.IsAny<string>(),
                privateToken = It.IsAny<string>()
            };
        }

        public void Dispose()
        {

        }

        [Fact]
        public void BollingerBands_OneMin_Test()
        {
            var interval = Interval.OneM;
            _settings.chartInterval = interval;
            _fileRepo.Setup(f => f.GetSettings()).Returns(_settings);
            _fileRepo.Setup(f => f.GetConfig()).Returns(_botConfig);
            _repo = new BinanceRepository();
            _bldr = new BollingerBandTradeBuilder(_repo, _fileRepo.Object);
            _bldr.SetBotSettings(_settings);

            var BBs = _bldr.GetBollingerBands(interval);

            Assert.NotNull(BBs);
        }

        [Fact]
        public void BollingerBands_FiveMin_Test()
        {
            var interval = Interval.FiveM;
            _settings.chartInterval = interval;
            _fileRepo.Setup(f => f.GetSettings()).Returns(_settings);
            _fileRepo.Setup(f => f.GetConfig()).Returns(_botConfig);
            _repo = new BinanceRepository();
            _bldr = new BollingerBandTradeBuilder(_repo, _fileRepo.Object);
            _bldr.SetBotSettings(_settings);

            var BBs = _bldr.GetBollingerBands(interval);

            Assert.NotNull(BBs);
        }

        [Fact]
        public void BollingerBands_OneDay_Test()
        {
            var interval = Interval.OneD;
            _settings.chartInterval = interval;
            _fileRepo.Setup(f => f.GetSettings()).Returns(_settings);
            _fileRepo.Setup(f => f.GetConfig()).Returns(_botConfig);
            _repo = new BinanceRepository();
            _bldr = new BollingerBandTradeBuilder(_repo, _fileRepo.Object);
            _bldr.SetBotSettings(_settings);

            var BBs = _bldr.GetBollingerBands(interval);

            Assert.NotNull(BBs);
        }
    }
}
