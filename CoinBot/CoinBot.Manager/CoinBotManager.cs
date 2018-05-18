using CoinBot.Business.Builders.Interface;
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
        private ITradingBuilder _tradingBuilder;
        private Balance _balance;

        public CoinBotManager(ITradingBuilder tradingBuilder)
        {
            this._tradingBuilder = tradingBuilder;
        }

        /// <summary>
        /// Update bot settings
        /// </summary>
        /// <param name="botSettings">New bot settings</param>
        /// <returns>Boolean value when complete</returns>
        public bool UpdateBotSettings(BotSettings botSettings)
        {
            ServiceReady();
            var result = _tradingBuilder.SetBotSettings(botSettings);

            return result;
        }

        /// <summary>
        /// Start trading
        /// </summary>
        /// <param name="interval">Candlestick interval</param>
        public bool StartBot(Interval interval)
        {
            ServiceReady();
            _tradingBuilder.StartTrading(interval);

            return true;
        }

        /// <summary>
        /// Stop Trading
        /// </summary>
        public bool StopBot()
        {
            ServiceReady();
            _tradingBuilder.StopTrading();

            return true;
        }

        /// <summary>
        /// Get all transactions
        /// </summary>
        /// <returns>Collection of TradeInformation</returns>
        public IEnumerable<TradeInformation> GetTransactionHistory()
        {
            ServiceReady();
            return _tradingBuilder.GetTradeHistory();
        }

        /// <summary>
        /// Get current balance
        /// </summary>
        /// <returns>BotBalance object</returns>
        public BotBalance GetBalance()
        {
            ServiceReady();
            return _tradingBuilder.GetBalance();
        }

        /// <summary>
        /// Get Balance history
        /// </summary>
        /// <returns>Collection of BotBalance objects</returns>
        public IEnumerable<BotBalance> GetBalanceHistory()
        {
            ServiceReady();
            return _tradingBuilder.GetBalanceHistory();
        }

        private void ServiceReady()
        {
            if(!_tradingBuilder.ConfigFileExits())
            {
                throw new Exception("No Configuration file exists!");
            }

            if (!_tradingBuilder.SettingsFileExists())
            {
                throw new Exception("No BotSettings file exists!");
            }
        }
    }
}
