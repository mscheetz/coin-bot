using CoinBot.Business.Entities;
using CoinBot.Data.Interface;
using System;
using System.Linq;
using Xunit;

namespace CoinBot.Data.Tests
{
    public class KuCoinRepositoryTests : IDisposable
    {

        public KuCoinRepositoryTests()
        {
        }

        public void Dispose()
        {
        }

        [Fact]
        public void GetCandlesticksTest()
        {
            IKuCoinRepository repo = new KuCoinRepository();
            IFileRepository fileRepo = new FileRepository();
            var apiInfo = fileRepo.GetConfig();
            repo.SetExchangeApi(apiInfo);
            var symbol = "ETH-BTC";

            var sticks = repo.GetCandlesticks(symbol, 15, 10).Result;

            Assert.True(sticks != null);
            Assert.True(sticks.close.Length > 0);
            Assert.True(sticks.open.Length > 0);
        }

        [Fact]
        public void GetAccountBalanceTest()
        {
            IKuCoinRepository repo = new KuCoinRepository();
            IFileRepository fileRepo = new FileRepository();
            var apiInfo = fileRepo.GetConfig();
            repo.SetExchangeApi(apiInfo);

            var balances = repo.GetBalance().Result;

            Assert.True(balances != null);
        }

        [Fact]
        public void GetOrderBookTest()
        {
            IKuCoinRepository repo = new KuCoinRepository();
            IFileRepository fileRepo = new FileRepository();
            var apiInfo = fileRepo.GetConfig();
            repo.SetExchangeApi(apiInfo);
            var symbol = "ETH-BTC";

            var orderBook = repo.GetOrderBook(symbol).Result;

            Assert.True(orderBook != null);
            Assert.True(orderBook.buys.Length > 0);
            Assert.True(orderBook.sells.Length > 0);
        }

        [Fact]
        public void GetTicksTest()
        {
            IKuCoinRepository repo = new KuCoinRepository();
            IFileRepository fileRepo = new FileRepository();
            var apiInfo = fileRepo.GetConfig();
            repo.SetExchangeApi(apiInfo);

            var ticks = repo.GetTicks().Result;

            Assert.True(ticks != null);
        }

        [Fact]
        public void GetTickTest()
        {
            IKuCoinRepository repo = new KuCoinRepository();
            IFileRepository fileRepo = new FileRepository();
            var apiInfo = fileRepo.GetConfig();
            repo.SetExchangeApi(apiInfo);
            var symbol = "ETH-BTC";

            var tick = repo.GetTick(symbol).Result;

            Assert.True(tick != null);
        }

        [Fact]
        public void GetOrdersTest()
        {
            IKuCoinRepository repo = new KuCoinRepository();
            IFileRepository fileRepo = new FileRepository();
            var apiInfo = fileRepo.GetConfig();
            repo.SetExchangeApi(apiInfo);
            var symbol = "DCC-BTC";

            var orders = repo.GetOrders(symbol).Result;

            Assert.True(orders != null);
        }

        [Fact]
        public void GetOpenOrdersTest()
        {
            IKuCoinRepository repo = new KuCoinRepository();
            IFileRepository fileRepo = new FileRepository();
            var apiInfo = fileRepo.GetConfig();
            repo.SetExchangeApi(apiInfo);
            var symbol = "DCC-BTC";

            var orders = repo.GetOpenOrders(symbol).Result;

            Assert.True(orders != null);
        }

        [Fact]
        public void PostTradeTest()
        {
            IKuCoinRepository repo = new KuCoinRepository();
            IFileRepository fileRepo = new FileRepository();
            var apiInfo = fileRepo.GetConfig();
            repo.SetExchangeApi(apiInfo);
            var symbol = "DCC-BTC";
            var tradeParams = new TradeParams
            {
                price = 0.00000400M,
                quantity = 3000,
                symbol = symbol,
                type = "BUY"
            };

            var orderDetail = repo.PostTrade(tradeParams).Result;

            Assert.True(orderDetail != null);
        }

        [Fact]
        public void GetAndCancelOpenTradeTest()
        {
            IKuCoinRepository repo = new KuCoinRepository();
            IFileRepository fileRepo = new FileRepository();
            var apiInfo = fileRepo.GetConfig();
            repo.SetExchangeApi(apiInfo);
            var symbol = "DCC-BTC";

            var orders = repo.GetOpenOrders(symbol).Result;

            Assert.True(orders != null);

            if (orders.openBuys.Length > 0)
            {
                var orderId = orders.openBuys[0].orderId;

                var cancelDetail = repo.DeleteTrade(symbol, orderId, orders.openBuys[0].type).Result;

                Assert.True(cancelDetail != null);
            }

            if(orders.openSells.Length > 0)
            {
                var orderId = orders.openSells[0].orderId;

                var cancelDetail = repo.DeleteTrade(symbol, orderId, orders.openSells[0].type).Result;

                Assert.True(cancelDetail != null);
            }
        }
    }
}
