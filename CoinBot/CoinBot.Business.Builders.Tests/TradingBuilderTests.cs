using CoinBot.Business.Builders;
using CoinBot.Business.Builders.Interface;
using CoinBot.Business.Entities;
using CoinBot.Data;
using CoinBot.Data.Interface;
using System;
using Xunit;

namespace CoinBot.Business.Builders.Tests
{
    public class TradingBuilderTests
    {
        private ITradingBuilder _bldr;
        private IBinanceRepository _repo;

        //public TradingBuilderTests()
        //{
        //    _repo = new BinanceRepository();
        //    _bldr = new TradingBuilder(_repo);
        //}

        [Fact]
        public void BollingerBands_OneDay_Test()
        {
            var symbol = "ICXBTC";
            var interval = Interval.OneD;
            _repo = new BinanceRepository();
            _bldr = new TradingBuilder(_repo);

            var BBs = _bldr.GetBollingerBands(symbol, interval);

            Assert.NotNull(BBs);
        }

        [Fact]
        public void BollingerBands_OneMin_Test()
        {
            var symbol = "AIONBTC";
            var interval = Interval.OneM;
            _repo = new BinanceRepository();
            _bldr = new TradingBuilder(_repo);

            var BBs = _bldr.GetBollingerBands(symbol, interval);

            Assert.NotNull(BBs);
        }

        [Fact]
        public void BollingerBands_FiveMin_Test()
        {
            var symbol = "GNTBTC";
            var interval = Interval.FiveM;
            _repo = new BinanceRepository();
            _bldr = new TradingBuilder(_repo);

            var BBs = _bldr.GetBollingerBands(symbol, interval);

            Assert.NotNull(BBs);
        }
    }
}
