using CoinBot.Business.Entities;
using CoinBot.Data.Interface;
using GDAXSharp.Services.Products.Types;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace CoinBot.Data.Tests
{
    public class GdaxRepositoryTests
    {
        private ApiInformation _exchangeApi;

        public GdaxRepositoryTests()
        {
            _exchangeApi = new ApiInformation()
            {
                apiKey = "",
                apiSecret = "",
            };
        }

        [Fact]
        public void GetAccountTest()
        {
            //IGdaxRepository repo = new GdaxRepository();
            //repo.SetExchangeApi(_exchangeApi);

            //var account = repo.GetBalance();

            //Assert.NotNull(account.Result);
        }

        [Fact]
        public void GetGdaxCandlestick()
        {
            IGdaxRepository repo = new GdaxRepository();
            repo.SetExchangeApi(_exchangeApi);
            var pair = "BTCUSD";
            var stickCount = 0;
            var granularity = CandleGranularity.Minutes1;

            var candleSticks = repo.GetCandleSticks(pair, stickCount, granularity).Result.ToList();

            Assert.True(candleSticks.Count > 0);
        }

        [Fact]
        public void GetGdaxOrderBook()
        {
            IGdaxRepository repo = new GdaxRepository();
            repo.SetExchangeApi(_exchangeApi);
            var pair = "BTCUSD";

            var candleSticks = repo.GetOrderBook(pair).Result;

            Assert.True(candleSticks != null);
        }

        [Fact]
        public void GetGdaxTrades()
        {
            IGdaxRepository repo = new GdaxRepository();
            repo.SetExchangeApi(_exchangeApi);
            var pair = "BTCUSD";

            //var trades = repo.GetTrades(pair).Result;

            //Assert.True(trades != null);
        }

        [Fact]
        public void WSSTicker()
        {
            IGdaxRepository repo = new GdaxRepository();
            repo.SetExchangeApi(_exchangeApi);
            var pair = "BTCUSD";

            repo.LaunchWSS(pair);
            //Task.Run(() => repo.StartWebsocket(pair));
            //Task.Run(() =>
            //{
            //    repo.StartWebsocket(pair);
            //}).ConfigureAwait(false);

            var ticker = repo.GetWSSTicker(pair);

            Assert.True(ticker != null);
        }

        [Fact]
        public void TickerTest()
        {
            IGdaxRepository repo = new GdaxRepository();
            repo.SetExchangeApi(_exchangeApi);
            var pair = "BTCUSD";

            var response = repo.GetTicker(pair).Result;

            Assert.True(response != null);
        }

        [Fact]
        public void StatsTest()
        {
            IGdaxRepository repo = new GdaxRepository();
            repo.SetExchangeApi(_exchangeApi);
            var pair = "BTCUSD";

            var response = repo.GetStats(pair).Result;

            Assert.True(response != null);
        }
    }
}