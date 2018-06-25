using System;
using System.Collections.Generic;
using System.Text;

namespace CoinBot.Business.Entities
{
    public class OrderBookDetail
    {
        public decimal price { get; set; }
        public int precision { get; set; }
        public int position { get; set; }
    }
}
