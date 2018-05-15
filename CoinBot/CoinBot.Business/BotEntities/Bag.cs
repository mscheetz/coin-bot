using System;
using System.Collections.Generic;
using System.Text;

namespace CoinBot.Business.Entities
{
    public class Bag
    {
        public string pair { get; set; }
        public decimal price { get; set; }
        public decimal quantity { get; set; }
        public long tradeTime { get; set; }
    }
}
