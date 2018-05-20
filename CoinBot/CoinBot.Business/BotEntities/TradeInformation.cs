using System;
using System.Collections.Generic;
using System.Text;

namespace CoinBot.Business.Entities
{
    public class TradeInformation
    {
        public string pair { get; set; }
        public string tradeType { get; set; }
        public decimal price { get; set; }
        public decimal quantity { get; set; }
        public DateTime timestamp { get; set; }
    }
}
