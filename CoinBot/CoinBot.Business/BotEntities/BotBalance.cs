using System;
using System.Collections.Generic;
using System.Text;

namespace CoinBot.Business.Entities
{
    public class BotBalance
    {
        public string symbol { get; set; }
        public decimal quantity { get; set; }
        public DateTime timestamp { get; set; }
    }
}
