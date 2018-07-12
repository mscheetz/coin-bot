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
        private IBollingerBandTradeBuilder _bollingerBuilder;
        private IVolumeTradeBuilderOG _volumeBuilderOG;
        private IOrderBookTradeBuilder _orderBookTradeBuilder;
        private ITradeBuilder _tradeBuilder;
        private BotConfig _botConfig;
        private BotSettings _botSettings;

        public CoinBotManager(IBollingerBandTradeBuilder bollingerBuilder,
                              IVolumeTradeBuilderOG volumeBuilderOG,
                              IOrderBookTradeBuilder orderBookTradeBuilder,
                              ITradeBuilder tradeBuilder)
        {
            this._bollingerBuilder = bollingerBuilder;
            this._volumeBuilderOG = volumeBuilderOG;
            this._orderBookTradeBuilder = orderBookTradeBuilder;
            this._tradeBuilder = tradeBuilder;

            _botSettings = tradeBuilder.GetBotSettings();
            _botConfig = _tradeBuilder.GetBotConfig();
        }

        /// <summary>
        /// Validate passwords match
        /// </summary>
        /// <param name="attemptPassword">Attempted password</param>
        /// <returns>Boolean of match attempt</returns>
        public bool ValidatePassword(string attemptPassword)
        {
            ServiceReady();
            var password = _tradeBuilder.GetPassword();

            return attemptPassword.Equals(password) ? true : false;
        }

        /// <summary>
        /// Update bot password
        /// </summary>
        /// <param name="password">String of new password</param>
        /// <returns>Bool when complete</returns>
        public bool UpdatePassword(string password)
        {
            ServiceReady();
            var response = _tradeBuilder.UpdatePassword(password);
            _botSettings = _tradeBuilder.GetBotSettings();

            return response;
        }

        /// <summary>
        /// Get current BotConfig
        /// </summary>
        /// <returns>BotConfig object</returns>
        public BotConfig GetBotConfig()
        {
            ServiceReady();
            var botConfig = _tradeBuilder.GetBotConfig();

            return botConfig;
        }

        /// <summary>
        /// Get current Exchange API Key
        /// </summary>
        /// <returns>String of api key</returns>
        public string GetApiKey()
        {
            ServiceReady();
            return _tradeBuilder.GetApiKey();
        }

        /// <summary>
        /// Update BotConfig
        /// </summary>
        /// <param name="botConfig">New bot settings</param>
        /// <returns>Boolean value when complete</returns>
        public bool UpdateBotConfig(BotConfig botConfig)
        {
            ServiceReady();
            var result = _tradeBuilder.SetBotSettings(botConfig);
            _botConfig = _tradeBuilder.GetBotConfig();
            _botSettings = _tradeBuilder.GetBotSettings();

            return result;
        }

        /// <summary>
        /// Update bot settings
        /// </summary>
        /// <param name="botSettings">New bot settings</param>
        /// <returns>Boolean value when complete</returns>
        public bool UpdateApiAccess(ApiInformation apiInformation)
        {
            ServiceReady();
            var result = _tradeBuilder.SetApiInformation(apiInformation);

            return result;
        }

        /// <summary>
        /// Start trading
        /// </summary>
        /// <param name="interval">Candlestick interval</param>
        public bool StartBot(Interval interval)
        {            
            ServiceReady();
            if (_botConfig.tradingStrategy == Strategy.BollingerBands)
                _bollingerBuilder.StartTrading(interval);
            else if (_botConfig.tradingStrategy == Strategy.Volume)
                _volumeBuilderOG.StartTrading(interval);
            else if (_botConfig.tradingStrategy == Strategy.OrderBook)
                _orderBookTradeBuilder.StartTrading(interval);

            return true;
        }

        /// <summary>
        /// Stop Trading
        /// </summary>
        public bool StopBot()
        {
            var settings = new BotSettings()
            {
                runBot = false
            };
            return _tradeBuilder.SetBotSettings(settings);
        }

        /// <summary>
        /// Get last N transactions
        /// </summary>
        /// <param name="transactionCount">Count of transations to return (default 10)</param>
        /// <returns>Collection of TradeInformation</returns>
        public IEnumerable<TradeInformation> GetTransactionHistory(int transactionCount = 10)
        {
            ServiceReady();
            return _tradeBuilder.GetTradeHistory(transactionCount);
        }

        /// <summary>
        /// Get last N trade signals
        /// </summary>
        /// <param name="signalCount">Count of trade signals to return (default 10)</param>
        /// <returns>Collection of TradeSignal objects</returns>
        public IEnumerable<TradeSignal> GetTradeSignalHistory(int signalCount = 10)
        {
            ServiceReady();
            return _tradeBuilder.GetSignalHistory(signalCount);
        }

        /// <summary>
        /// Get current balance
        /// </summary>
        /// <returns>Collection of BotBalance objects</returns>
        public IEnumerable<BotBalance> GetBalance()
        {
            ServiceReady();
            return _tradeBuilder.GetBalance().FirstOrDefault();
        }

        /// <summary>
        /// Get last N balances
        /// </summary>
        /// <param name="count">Count of balances to return</param>
        /// <returns>BotBalance collection</returns>
        public IEnumerable<IEnumerable<BotBalance>> GetBalances(int count)
        {
            ServiceReady();
            return _tradeBuilder.GetBalance(count);
        }

        /// <summary>
        /// Get Balance history
        /// </summary>
        /// <returns>Collection of BotBalance objects</returns>
        public IEnumerable<IEnumerable<BotBalance>> GetBalanceHistory()
        {
            ServiceReady();
            return _tradeBuilder.GetBalance(0);
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
            ServiceReady();
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

            _tradeBuilder.SetBotSettings(_botSettings);
        }
    }
}
