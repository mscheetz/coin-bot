using CoinBot.Business.Entities;
using CoinBot.Data.Interface;
using System;
using System.Linq;
using Xunit;

namespace CoinBot.Data.Tests
{
    public class BinanceRepositoryTests : IDisposable
    {
        private ApiInformation _exchangeApi;

        public BinanceRepositoryTests()
        {
            _exchangeApi = new ApiInformation()
            {
                apiKey = "",
                apiSecret = "",
            };
        }

        public void Dispose()
        {

        }

        [Fact]
        public void GetAccountTest()
        {
            IBinanceRepository repo = new BinanceRepository();
            repo.SetExchangeApi(_exchangeApi);

            var account = repo.GetBalance();

            Assert.NotNull(account.Result);
        }

        [Fact]
        public void GetBinanceTimeTest()
        {
            IBinanceRepository repo = new BinanceRepository();
            repo.SetExchangeApi(_exchangeApi);

            var binanceTime = repo.GetBinanceTime();

            Assert.True(binanceTime > 0);
        }

        [Fact]
        public void GetBinanceCandlestick()
        {
            IBinanceRepository repo = new BinanceRepository();
            repo.SetExchangeApi(_exchangeApi);
            var pair = "BTCUSDT";
            var interval = Interval.OneM;

            var candleSticks = repo.GetCandlestick(pair, interval).Result.ToList();

            Assert.True(candleSticks.Count > 0);
        }
    }
}