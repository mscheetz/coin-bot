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
        /// <remarks>
        /// Check if service is running
        /// </remarks>
        /// GET: api/coinbot/status
        [HttpGet("status")]
        public bool Status()
        {
            return true;
        }

        /// <summary>
        /// Get bot settings
        /// </summary>
        /// <remarks>
        /// Get bot settings
        /// </remarks>
        /// <returns>BotSettings object</returns>
        /// GET: api/coinbot/settings
        [HttpGet("settings")]
        public BotSettings GetBotSettings()
        {
            return _service.GetBotSettings();
        }

        /// <summary>
        /// Update bot settings
        /// </summary>
        /// <remarks>
        /// Update bot settings
        /// </remarks>
        /// <param name="botSettings">New bot settings</param>
        /// <returns>Boolean value when complete</returns>
        /// POST: api/coinbot/settings
        [HttpPost("settings")]
        public bool UpdateBotSettings(BotSettings botSettings)
        {
            return _service.UpdateBotSettings(botSettings);
        }

        /// <summary>
        /// Get exchange api key
        /// </summary>
        /// <remarks>
        /// Get exchange api key
        /// </remarks>
        /// <returns>String of api key</returns>
        /// GET: api/coinbot/settings/api
        [HttpGet("settings/api")]
        public string GetApiKey()
        {
            return _service.GetApiKey();
        }

        /// <summary>
        /// Update Exchange api settings
        /// </summary>
        /// <remarks>
        /// Update Exchange api settings
        /// </remarks>
        /// <param name="apiInformation">New api settings</param>
        /// <returns>Boolean value when complete</returns>
        /// POST: api/coinbot/settings/api
        [HttpPost("settings/api")]
        public bool UpdateApiSettings(ApiInformation apiInformation)
        {
            return _service.UpdateApiAccess(apiInformation);
        }

        /// <summary>
        /// Start trading with 1 Minute interval on candlesticks
        /// </summary>
        /// <remarks>
        /// Start trading with 1 Minute interval on candlesticks
        /// </remarks>
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
        /// <remarks>
        /// Start trading
        /// </remarks>
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
        /// <remarks>
        /// Stop Trading
        /// </remarks>
        /// GET: api/coinbot/stop
        [HttpGet("stop")]
        public bool StopBot()
        {
            return _service.StopBot();
        }

        /// <summary>
        /// Get last 10 transactions
        /// </summary>
        /// <remarks>
        /// Get last 10 transactions
        /// </remarks>
        /// <returns>Collection of TradeInformation</returns>
        /// GET: api/coinbot/transactions
        [HttpGet("transactions")]
        public IEnumerable<TradeInformation> GetTransactionHistory()
        {
            return _service.GetTransactionHistory();
        }

        /// <summary>
        /// Get last N transactions
        /// </summary>
        /// <remarks>
        /// Get last N transactions
        /// </remarks>
        /// <param name="transactionCount">Count of transations to return</param>
        /// <returns>Collection of TradeInformation</returns>
        /// GET: api/coinbot/transactions
        [HttpGet("transactions/{transactionCount}")]
        public IEnumerable<TradeInformation> GetTransactionHistory(int transactionCount)
        {
            return _service.GetTransactionHistory(transactionCount);
        }

        /// <summary>
        /// Get last 10 trade signals
        /// </summary>
        /// <remarks>
        /// Get last 10 trade signals
        /// </remarks>
        /// <returns>Collection of TradeSignal objects</returns>
        /// GET: api/coinbot/signals
        [HttpGet("signals")]
        public IEnumerable<TradeSignal> GetTradeSignalHistory()
        {
            return _service.GetTradeSignalHistory();
        }

        /// <summary>
        /// Get last N trade signals
        /// </summary>
        /// <remarks>
        /// Get last N trade signals
        /// </remarks>
        /// <param name="signalCount">Count of trade signals to return</param>
        /// <returns>Collection of TradeSignal objects</returns>
        /// GET: api/coinbot/signals
        [HttpGet("signals/{signalCount}")]
        public IEnumerable<TradeSignal> GetTradeSignalHistory(int signalCount)
        {
            return _service.GetTradeSignalHistory(signalCount);
        }

        /// <summary>
        /// Get current balance
        /// </summary>
        /// <remarks>
        /// Get current balance
        /// </remarks>
        /// <returns>Collection of BotBalance objects</returns>
        /// GET: api/coinbot/balance
        [HttpGet("balance")]
        public IEnumerable<BotBalance> GetBalance()
        {
            return _service.GetBalance();
        }

        /// <summary>
        /// Get last N balances
        /// </summary>
        /// <remarks>
        /// Get last N balances (Default 10)
        /// </remarks>
        /// <param name="count">Count of balances to return (default 10)</param>
        /// <returns>BotBalance object</returns>
        /// GET: api/coinbot/balance
        [HttpGet("balance/{count}")]
        public IEnumerable<IEnumerable<BotBalance>> GetLastBalances(int count = 10)
        {
            return _service.GetBalances(count);
        }

        /// <summary>
        /// Get Balance history
        /// </summary>
        /// <remarks>
        /// Get Balance history
        /// </remarks>
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
        /// <remarks>
        /// Get Stop losses
        /// </remarks>
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
        /// <remarks>
        /// Cancel all GDAX trades
        /// </remarks>
        /// <returns>Boolean when complete</returns>
        /// GET: api/coinbot/trades/cancel
        [HttpGet("trades/cancel")]

        public bool CancelAllTrades()
        {
            return _service.CancelAllGdaxTrades();
        }
    }
}