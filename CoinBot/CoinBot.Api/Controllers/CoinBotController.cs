using System.Collections.Generic;
using CoinBot.Business.Entities;
using CoinBot.Service;
using Microsoft.AspNetCore.Mvc;

namespace CoinBot.Api.Controllers
{
    [Produces("application/json")]
    [Route("api/coinbot")]
    public class CoinBotController : Controller
    {
        private ICoinBotService _service;

        public CoinBotController(ICoinBotService service)
        {
            _service = service;
        }

        /// <summary>
        /// Check if service is running
        /// </summary>
        /// GET: api/coinbot/status
        [HttpGet("status")]
        public bool Status()
        {
            return true;
        }

        /// <summary>
        /// Update bot settings
        /// </summary>
        /// <param name="botSettings">New bot settings</param>
        /// <returns>Boolean value when complete</returns>
        /// POST: api/coinbot/settings
        [HttpPost("settings")]
        public bool UpdateBotSettings(BotSettings botSettings)
        {
            return _service.UpdateBotSettings(botSettings);
        }

        /// <summary>
        /// Start trading with 1 Minute interval on candlesticks
        /// </summary>
        /// <param name="interval">Candlestick interval</param>
        /// GET: api/coinbot/start
        [HttpGet("start")]
        public bool StartBot()
        {
            return _service.StartBot(Interval.OneM);
        }

        /// <summary>
        /// Start trading
        /// </summary>
        /// <param name="interval">Candlestick interval</param>
        /// GET: api/coinbot/start/{interval}
        [HttpGet("start/{interval}")]
        public bool StartBot(Interval interval)
        {
            return _service.StartBot(interval);
        }

        /// <summary>
        /// Stop Trading
        /// </summary>
        /// GET: api/coinbot/stop
        [HttpGet("stop")]
        public bool StopBot()
        {
            return _service.StopBot();
        }

        /// <summary>
        /// Get last 10 transactions
        /// </summary>
        /// <returns>Collection of TradeInformation</returns>
        /// GET: api/coinbot/transactions
        [HttpGet("transactions")]
        public IEnumerable<TradeInformation> GetTransactionHistory()
        {
            return _service.GetTransactionHistory();
        }

        /// <summary>
        /// Get last 10 transactions
        /// </summary>
        /// <param name="transactionCount">Count of transations to return</param>
        /// <returns>Collection of TradeInformation</returns>
        /// GET: api/coinbot/transactions
        [HttpGet("transactions/{transactionCount}")]
        public IEnumerable<TradeInformation> GetTransactionHistory(int transactionCount)
        {
            return _service.GetTransactionHistory(transactionCount);
        }

        /// <summary>
        /// Get current balance
        /// </summary>
        /// <returns>Collection of BotBalance objects</returns>
        /// GET: api/coinbot/balance
        [HttpGet("balance")]
        public IEnumerable<BotBalance> GetBalance()
        {
            return _service.GetBalance();
        }

        /// <summary>
        /// Get current balance
        /// </summary>
        /// <param name="count">Count of balances to return</param>
        /// <returns>BotBalance object</returns>
        /// GET: api/coinbot/balance
        [HttpGet("balance/{count}")]
        public IEnumerable<IEnumerable<BotBalance>> GetLastBalances(int count)
        {
            return _service.GetBalances(count);
        }

        /// <summary>
        /// Get Balance history
        /// </summary>
        /// <returns>Collection of BotBalance objects</returns>
        /// GET: api/coinbot/balance/history
        [HttpGet("balance/history")]
        public IEnumerable<IEnumerable<BotBalance>> GetBalanceHistory()
        {
            return _service.GetBalanceHistory();
        }

        /// <summary>
        /// Get Stop losses
        /// </summary>
        /// <returns>Collection of OpenStopLoss objects</returns>
        /// GET: api/coinbot/stoploss
        [HttpGet("stoploss")]

        public IEnumerable<OpenStopLoss> GetStopLoss()
        {
            return _service.GetStopLosses();
        }

        /// <summary>
        /// Cancel all GDAX trades
        /// </summary>
        /// <returns>Boolean when complete</returns>
        /// GET: api/coinbot/trades/cancel
        [HttpGet("trades/cancel")]

        public bool CancelAllTrades()
        {
            return _service.CancelAllGdaxTrades();
        }
    }
}