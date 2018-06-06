using System;
using System.Collections.Generic;
using System.Text;

namespace CoinBot.Business.Entities
{
    public class TradeSignal
    {
        public TradeType tradeType { get; set; }
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
