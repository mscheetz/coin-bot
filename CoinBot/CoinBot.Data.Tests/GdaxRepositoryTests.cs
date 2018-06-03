using CoinBot.Business.Entities;
using CoinBot.Data.Interface;
using GDAXSharp.Services.Products.Types;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace CoinBot.Data.Tests
{
    public class GdaxRepositoryTests : IDisposable
    {
        private ApiInformation _exchangeApi;
        private TradeParams _tradeParams;

        public GdaxRepositoryTests()
        {
            _exchangeApi = new ApiInformation()
            {
                apiKey = "",
                apiSecret = "",
                extraValue = ""
            };
            _tradeParams = new TradeParams
            {
                side = "buy",
                price = 0.100M,
                quantity = 0.01M,
                symbol = "BTCUSD"
            };
        }

        public void Dispose()
        {

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
        public void GetGdaxBalancesRest_Test()
        {
            IFileRepository fileRepo = new FileRepository();
            var apiInfo = fileRepo.GetConfig();
            IGdaxRepository repo = new GdaxRepository();
            repo.SetExchangeApi(apiInfo, true);

            var accounts = repo.GetBalanceRest().Result;

            Assert.True(accounts != null);
        }

        [Fact]
        public void GetGdaxBalances_Test()
        {
            IFileRepository fileRepo = new FileRepository();
            var apiInfo = fileRepo.GetConfig();
            IGdaxRepository repo = new GdaxRepository();
            repo.SetExchangeApi(apiInfo, true);

            var accounts = repo.GetBalance().Result;

            Assert.True(accounts != null);
        }

        [Fact]
        public void GDAXPlaceTrade_Test()
        {
            IFileRepository fileRepo = new FileRepository();
            var apiInfo = fileRepo.GetConfig();
            IGdaxRepository repo = new GdaxRepository();
            repo.SetExchangeApi(apiInfo, true);

            var response = repo.PlaceTrade(_tradeParams).Result;

            Assert.True(response != null);
        }

        [Fact]
        public void GDAXPlaceTradeStopLoss_Test()
        {
            IFileRepository fileRepo = new FileRepository();
            var apiInfo = fileRepo.GetConfig();
            IGdaxRepository repo = new GdaxRepository();
            repo.SetExchangeApi(apiInfo, true);

            var response = repo.PlaceStopLimit(_tradeParams).Result;

            Assert.True(response != null);
        }

        [Fact]
        public void GDAXCancelAllTrades_Test()
        {
            IFileRepository fileRepo = new FileRepository();
            var apiInfo = fileRepo.GetConfig();
            IGdaxRepository repo = new GdaxRepository();
            repo.SetExchangeApi(apiInfo, true);

            var response = repo.CancelAllTrades().Result;

            Assert.True(response != null);
        }

        [Fact]
        public void GetGdaxCandlestick_Test()
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
        public void GetGdaxOrderBook_Test()
        {
            IGdaxRepository repo = new GdaxRepository();
            repo.SetExchangeApi(_exchangeApi);
            var pair = "BTCUSD";

            var candleSticks = repo.GetOrderBook(pair).Result;

            Assert.True(candleSticks != null);
        }

        [Fact]
        public void GetGdaxTicker_Test()
        {
            IGdaxRepository repo = new GdaxRepository();
            repo.SetExchangeApi(_exchangeApi);
            var pair = "BTCUSD";

            var trades = repo.GetTicker(pair).Result;

            Assert.True(trades != null);
        }

        [Fact]
        public void GetGdaxTrades_Test()
        {
            IGdaxRepository repo = new GdaxRepository();
            repo.SetExchangeApi(_exchangeApi);
            var pair = "BTCUSD";

            var trades = repo.GetTrades(pair).Result;

            Assert.True(trades != null);
        }

        [Fact]
        public void GetStats_Test()
        {
            IGdaxRepository repo = new GdaxRepository();
            repo.SetExchangeApi(_exchangeApi);
            var pair = "BTCUSD";

            var response = repo.GetStats(pair).Result;

            Assert.True(response != null);
        }
    }
}