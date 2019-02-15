// -----------------------------------------------------------------------------
// <copyright file="TradeSignal" company="Matt Scheetz">
//     Copyright (c) Matt Scheetz All Rights Reserved
// </copyright>
// <author name="Matt Scheetz" date="2/15/2019 4:53:33 PM" />
// -----------------------------------------------------------------------------

namespace coinbot.strategies.zzz_Old_Bot
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;
    #region Usings

    using System;
    using System.Collections.Generic;
    using System.Text;

    #endregion Usings

    public class TradeSignal
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public TradeType tradeType { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public SignalType signal { get; set; }
        public string pair { get; set; }
        public decimal price { get; set; }
        public decimal lastBuy { get; set; }
        public decimal lastSell { get; set; }
        public decimal currentVolume { get; set; }
        public decimal bandUpper { get; set; }
        public decimal bandLower { get; set; }
        public DateTime transactionDate { get; set; }
    }
}