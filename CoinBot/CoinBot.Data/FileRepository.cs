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
        private string signalPath = "signal.log";

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
        /// Set App configuration data
        /// </summary>
        /// <param name="apiInformation">Updated settings</param>
        /// <returns>Boolean when completee</returns>
        public bool SetConfig(ApiInformation apiInformation, bool noWrite = true)
        {
            if (!noWrite)
            {
                var json = JsonConvert.SerializeObject(apiInformation);

                File.WriteAllText(configPath, json);

                json = null;
            }
            return true;
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
        public bool UpdateBotSettings(BotSettings botSettings, bool noWrite = true)
        {
            if (!noWrite)
            {
                var json = JsonConvert.SerializeObject(botSettings);

                File.WriteAllText(settingsPath, json);

                json = null;
            }
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
        public bool LogTransaction(TradeInformation tradeInformation, bool noWrite = true)
        {
            if (!noWrite)
            {
                var json = JsonConvert.SerializeObject(tradeInformation);

                using (StreamWriter s = File.AppendText(transactionPath))
                {
                    s.WriteLine(json + ",");

                    json = null;

                    return true;
                }
            }
            return true;
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
        public bool LogBalances(List<BotBalance> botBalance, bool noWrite = true)
        {
            if (!noWrite)
            {
                var json = JsonConvert.SerializeObject(botBalance);

                using (StreamWriter s = File.AppendText(balancePath))
                {
                    s.WriteLine(json + ",");

                    json = null;

                    return true;
                }
            }
            return true;
        }

        /// <summary>
        /// Get TradeSignals
        /// </summary>
        /// <returns>Collection of TradeSignals</returns>
        public List<TradeSignal> GetSignals()
        {
            using (StreamReader r = new StreamReader(signalPath))
            {
                string json = r.ReadToEnd();

                json = $"[{json}]";

                var signalList = JsonConvert.DeserializeObject<List<TradeSignal>>(json);

                json = null;

                return signalList;
            }
        }

        /// <summary>
        /// Write trade signal to file
        /// </summary>
        /// <param name="signal">TradeSignal to write</param>
        /// <returns>Boolean when complete</returns>
        public bool LogSignal(TradeSignal signal, bool noWrite = true)
        {
            if (!noWrite)
            {
                var json = JsonConvert.SerializeObject(signal);

                using (StreamWriter s = File.AppendText(signalPath))
                {
                    s.WriteLine(json + ",");

                    json = null;

                    return true;
                }
            }
            return true;
        }

        /// <summary>
        /// Log an error with an object
        /// </summary>
        /// <typeparam name="T">Object type to log</typeparam>
        /// <param name="message">Message to log</param>
        /// <param name="obj">Object to log</param>
        /// <returns>Boolean when complete</returns>
        public bool LogError<T>(string message, T obj, bool noWrite = true)
        {
            if (!noWrite)
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
            return true;
        }

        /// <summary>
        /// Log an error
        /// </summary>
        /// <param name="message">Message to log</param>
        /// <returns>Boolean when complete</returns>
        public bool LogError(string message, bool noWrite = true)
        {
            if (!noWrite)
            {
                using (StreamWriter s = File.AppendText(errorPath))
                {
                    s.WriteLine($"ERROR {DateTime.UtcNow} {message}");

                    return true;
                }
            }

            return true;
        }
    }
}
