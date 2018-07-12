using System.Collections.Generic;
using System.Net;
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

        private bool Auth(string password)
        {
            return _service.ValidatePassword(password);
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
        /// Get bot config
        /// </summary>
        /// <remarks>
        /// Get bot config
        /// </remarks>
        /// <returns>BotConfig object</returns>
        /// GET: api/coinbot/settings
        [HttpGet("config/{password}")]
        public BotConfig GetBotConfig(string password)
        {
            if (!Auth(password))
            {
                HttpContext.Response.StatusCode = 401;
                return null;
            }

            return _service.GetBotConfig();
        }

        /// <summary>
        /// Update bot config
        /// </summary>
        /// <remarks>
        /// Update bot config
        /// </remarks>
        /// <param name="botConfig">New bot config</param>
        /// <returns>Boolean value when complete</returns>
        /// POST: api/coinbot/config
        [HttpPost("config/")]
        public bool UpdateBotConfig(string password, BotConfig botConfig)
        {
            if (!Auth(password))
            {
                HttpContext.Response.StatusCode = 401;
                return false;
            }
            return _service.UpdateBotConfig(botConfig);
        }

        /// <summary>
        /// Get exchange api key
        /// </summary>
        /// <remarks>
        /// Get exchange api key
        /// </remarks>
        /// <returns>String of api key</returns>
        /// GET: api/coinbot/config/api
        [HttpGet("config/api/{password}")]
        public string GetApiKey(string password)
        {
            if (!Auth(password))
            {
                HttpContext.Response.StatusCode = 401;
                return null;
            }
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
        /// POST: api/coinbot/config/api
        [HttpPost("config/api")]
        public bool UpdateApiSettings(string password, ApiInformation apiInformation)
        {
            if (!Auth(password))
            {
                HttpContext.Response.StatusCode = 401;
                return false;
            }
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
        [HttpGet("start/{password}")]
        public bool StartBot(string password)
        {
            if (!Auth(password))
            {
                HttpContext.Response.StatusCode = 401;
                return false;
            }
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
        [HttpGet("start/{password}/{interval}")]
        public bool StartBot(string password, Interval interval)
        {
            if (!Auth(password))
            {
                HttpContext.Response.StatusCode = 401;
                return false;
            }
            return _service.StartBot(interval);
        }

        /// <summary>
        /// Stop Trading
        /// </summary>
        /// <remarks>
        /// Stop Trading
        /// </remarks>
        /// GET: api/coinbot/stop
        [HttpGet("stop/{password}")]
        public bool StopBot(string password)
        {
            if (!Auth(password))
            {
                HttpContext.Response.StatusCode = 401;
                return false;
            }
            return _service.StopBot();
        }
        
#if DEBUG
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
#endif

        /// <summary>
        /// Cancel all GDAX trades
        /// </summary>
        /// <remarks>
        /// Cancel all GDAX trades
        /// </remarks>
        /// <returns>Boolean when complete</returns>
        /// GET: api/coinbot/trades/cancel
        [HttpGet("trades/cancel/{password}")]
        public bool CancelAllTrades(string password)
        {
            if (!Auth(password))
            {
                HttpContext.Response.StatusCode = 401;
                return false;
            }
            return _service.CancelAllGdaxTrades();
        }
    }
}