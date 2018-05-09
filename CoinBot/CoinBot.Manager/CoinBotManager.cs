using CoinBot.Business.Entities;
using CoinBot.Data.Interface;
using CoinBot.Service;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CoinBot.Manager
{
    public class CoinBotManager : ICoinBotService
    {
        private ApiInformation _apiInformation;
        private BotSettings _botSettings;
        private IBinanceRepository _binanceRepository;
        private Balance _balance;

        public CoinBotManager(IBinanceRepository binanceRepository)
        {
            this._binanceRepository = binanceRepository;
        }
        
        /// <summary>
        /// Update ApiInformation
        /// </summary>
        /// <param name="apiInformation">New Api Information</param>
        /// <returns>Boolean value when complete</returns>
        public bool UpdateApiInformation(ApiInformation apiInformation)
        {
            this._apiInformation = apiInformation;

            return true;
        }

        /// <summary>
        /// Update bot settings
        /// </summary>
        /// <param name="botSettings">New bot settings</param>
        /// <returns>Boolean value when complete</returns>
        public bool UpdateBotSettings(BotSettings botSettings)
        {
            this._botSettings = botSettings;

            return true;
        }

        public IEnumerable<Transaction> GetTransactions(int first, int range)
        {
            var transactions = _binanceRepository.GetTransactions().Result;

            return transactions.OrderByDescending(t => t.time).Skip(first).Take(range).ToList();
        }

        public void Trade()
        {

        }

        /// <summary>
        /// Update trading pair balance from exchange
        /// </summary>
        /// <returns>Boolean value when complete</returns>
        private bool UpdateBalance()
        {
            var account = _binanceRepository.GetBalance().Result;

            _balance = account.balances.Where(b => b.asset == _botSettings.tradingPair).FirstOrDefault();

            return true;
        }

        /// <summary>
        /// Get candlesticks
        /// </summary>
        /// <returns>Collection of candlesticks</returns>
        private List<Candlestick> GetCandleSticks()
        {
            var sticks = _binanceRepository.GetCandlestick(_botSettings.tradingPair, _botSettings.chartInterval).Result;

            return sticks.ToList();
        }
    }
}
