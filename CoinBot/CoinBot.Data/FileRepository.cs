using CoinBot.Business.Entities;
using CoinBot.Core;
using CoinBot.Data.Interface;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CoinBot.Data
{
    public class FileRepository : IFileRepository
    {
        private string balancePath = "balance.log";
        private string configPath = "apiConfig.json";
        private string settingsPath = "botSettings.json";
        private string transactionPath = "transaction.log";
        private string errorPath = "error.log";

        /// <summary>
        /// Constructor
        /// </summary>
        public FileRepository()
        {
        }

        /// <summary>
        /// Check if config file exists
        /// </summary>
        /// <returns>Boolean of validation</returns>
        public bool ConfigExists()
        {
            return File.Exists(configPath);
        }

        /// <summary>
        /// Check if settings file exists
        /// </summary>
        /// <returns>Boolean of validation</returns>
        public bool BotSettingsExists()
        {
            return File.Exists(settingsPath);
        }

        /// <summary>
        /// Get App configuration data from file
        /// </summary>
        /// <returns>ApiInformation object</returns>
        public ApiInformation GetConfig()
        {
            using (StreamReader r = new StreamReader(configPath))
            {
                string json = r.ReadToEnd();

                var config = JsonConvert.DeserializeObject<ApiInformation>(json);

                json = null;

                return config;
            }
        }

        /// <summary>
        /// Get BotSettings
        /// </summary>
        /// <returns>BotSettings object</returns>
        public BotSettings GetSettings()
        {
            using (StreamReader r = new StreamReader(settingsPath))
            {
                string json = r.ReadToEnd();

                var settings = JsonConvert.DeserializeObject<BotSettings>(json);

                json = null;

                return settings;
            }
        }

        /// <summary>
        /// Update BotSettings file
        /// </summary>
        /// <param name="botSettings">Updated BotSettings</param>
        /// <returns>Boolean when complete</returns>
        public bool UpdateBotSettings(BotSettings botSettings)
        {
            var json = JsonConvert.SerializeObject(botSettings);

            File.WriteAllText(settingsPath, json);

            json = null;

            return true;
        }

        /// <summary>
        /// Get Transactions
        /// </summary>
        /// <returns>Collection of TradeInformation</returns>
        public List<TradeInformation> GetTransactions()
        {
            using (StreamReader r = new StreamReader(transactionPath))
            {
                string json = r.ReadToEnd();

                json = $"[{json}]";

                var tradeInfoList = JsonConvert.DeserializeObject<List<TradeInformation>>(json);

                json = null;

                return tradeInfoList;
            }
        }

        /// <summary>
        /// Write transaction to log
        /// </summary>
        /// <param name="tradeInformation">TradeInformation to write</param>
        /// <returns>Boolean when complete</returns>
        public bool LogTransaction(TradeInformation tradeInformation)
        {
            var json = JsonConvert.SerializeObject(tradeInformation);

            using (StreamWriter s = File.AppendText(transactionPath))
            {
                s.WriteLine(json + ",");

                json = null;

                return true;
            }
        }

        /// <summary>
        /// Get Transactions
        /// </summary>
        /// <returns>Collection of BotBalance</returns>
        public List<List<BotBalance>> GetBalances()
        {
            using (StreamReader r = new StreamReader(balancePath))
            {
                string json = r.ReadToEnd();

                json = $"[{json}]";

                var balanceList = JsonConvert.DeserializeObject<List<List<BotBalance>>>(json);

                json = null;

                return balanceList;
            }
        }

        /// <summary>
        /// Write balances to file
        /// </summary>
        /// <param name="botBalance">BotBalances to write</param>
        /// <returns>Boolean when complete</returns>
        public bool LogBalances(List<BotBalance> botBalance)
        {
            var json = JsonConvert.SerializeObject(botBalance);

            using (StreamWriter s = File.AppendText(balancePath))
            {
                s.WriteLine(json + ",");

                json = null;

                return true;
            }
        }

        public bool LogError<T>(string message, T obj)
        {
            var json = JsonConvert.SerializeObject(obj);

            using (StreamWriter s = File.AppendText(errorPath))
            {
                s.WriteLine($"ERROR {DateTime.UtcNow} {message}");
                s.WriteLine("    Invalid object: " + json);

                json = null;

                return true;
            }
        }
        public bool LogError(string message)
        {
            using (StreamWriter s = File.AppendText(errorPath))
            {
                s.WriteLine($"ERROR {DateTime.UtcNow} {message}");

                return true;
            }
        }
    }
}
