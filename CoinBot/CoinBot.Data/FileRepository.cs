using CoinBot.Business.Entities;
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
        private string configPath = "config.json";
        private string transactionPath = "transaction.log";

        /// <summary>
        /// Constructor
        /// </summary>
        public FileRepository()
        {
        }

        /// <summary>
        /// Get BotSettings
        /// </summary>
        /// <returns>BotSettings object</returns>
        public BotSettings GetConfig()
        {
            using (StreamReader r = new StreamReader(configPath))
            {
                string json = r.ReadToEnd();

                BotSettings settings = JsonConvert.DeserializeObject<BotSettings>(json);

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

            File.WriteAllText(configPath, json);

            return true;
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
                s.WriteLine(json);

                return true;
            }
        }
    }
}
