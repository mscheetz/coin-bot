using System;
using System.Collections.Generic;
using System.Text;

namespace CoinBot.Business.Entities
{
    public class OrderBook
    {
        public long lastUpdateId { get; set; }
        public BinanceOrders[] bids { get; set; }
        public BinanceOrders[] asks { get; set; }
    }
}
