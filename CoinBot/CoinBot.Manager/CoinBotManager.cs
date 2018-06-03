﻿using CoinBot.Business.Builders.Interface;
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
        private IBollingerBandTradeBuilder _bollingerBuilder;
        private IPercentageTradeBuilder _percentageBuilder;
        private ITradeBuilder _tradeBuilder;
        private Balance _balance;
        private BotSettings _botSettings;

        public CoinBotManager(IBollingerBandTradeBuilder bollingerBuilder,
                              IPercentageTradeBuilder percentageBuilder,
                              ITradeBuilder tradeBuilder)
        {
            this._bollingerBuilder = bollingerBuilder;
            this._percentageBuilder = percentageBuilder;
            this._tradeBuilder = tradeBuilder;
            _botSettings = _tradeBuilder.GetBotSettings();
        }

        /// <summary>
        /// Update bot settings
        /// </summary>
        /// <param name="botSettings">New bot settings</param>
        /// <returns>Boolean value when complete</returns>
        public bool UpdateBotSettings(BotSettings botSettings)
        {
            ServiceReady();
            var result = _tradeBuilder.SetBotSettings(botSettings);
            _botSettings = _tradeBuilder.GetBotSettings();

            return result;
        }

        /// <summary>
        /// Start trading
        /// </summary>
        /// <param name="interval">Candlestick interval</param>
        public bool StartBot(Interval interval)
        {
            ServiceReady();
            if (_botSettings.tradingStrategy == Strategy.BollingerBands)
                _bollingerBuilder.StartTrading(interval);
            else if (_botSettings.tradingStrategy == Strategy.Percentage)
                _percentageBuilder.StartTrading(interval);

            return true;
        }

        /// <summary>
        /// Stop Trading
        /// </summary>
        public bool StopBot()
        {
            ServiceReady();
            if (_botSettings.tradingStrategy == Strategy.BollingerBands)
                _bollingerBuilder.StopTrading();
            else if (_botSettings.tradingStrategy == Strategy.Percentage)
                _bollingerBuilder.StopTrading();

            return true;
        }

        /// <summary>
        /// Get all transactions
        /// </summary>
        /// <returns>Collection of TradeInformation</returns>
        public IEnumerable<TradeInformation> GetTransactionHistory()
        {
            ServiceReady();
            return _tradeBuilder.GetTradeHistory();
        }

        /// <summary>
        /// Get current balance
        /// </summary>
        /// <returns>BotBalance object</returns>
        public BotBalance GetBalance()
        {
            ServiceReady();
            return _tradeBuilder.GetBalance();
        }

        /// <summary>
        /// Get Balance history
        /// </summary>
        /// <returns>Collection of BotBalance objects</returns>
        public IEnumerable<BotBalance> GetBalanceHistory()
        {
            ServiceReady();
            return _tradeBuilder.GetBalanceHistory();
        }

        /// <summary>
        /// Get all open stop losses
        /// </summary>
        /// <returns>Collection of OpenStopLoss</returns>
        public IEnumerable<OpenStopLoss> GetStopLosses()
        {
            ServiceReady();
            return _tradeBuilder.GetStopLosses();
        }

        /// <summary>
        /// Cancel all GDAX trades
        /// </summary>
        /// <returns>Boolean when complete</returns>
        public bool CancelAllGdaxTrades()
        {
            try
            {
                _tradeBuilder.CancelTrade(0, "");
                return true;
            }
            catch
            {
                return false;
            }
        }

        private void ServiceReady()
        {
            if(!_tradeBuilder.ConfigFileExits())
            {
                throw new Exception("No Configuration file exists!");
            }

            if (!_tradeBuilder.SettingsFileExists())
            {
                throw new Exception("No BotSettings file exists!");
            }
        }
    }
}
