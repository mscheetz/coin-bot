using System;
using System.Collections.Generic;
using System.Text;

namespace CoinBot.Business.Entities
{
    public class WSSTicker
    {
        public string type { get; set; }
        public long sequence { get; set; }
        public string product_id { get; set; }
        public decimal price { get; set; }
        public decimal open_24h { get; set; }
        public decimal volume_24h { get; set; }
        public decimal low_24h { get; set; }
        public decimal high_24h { get; set; }
        public decimal volume_30d { get; set; }
        public decimal best_bid { get; set; }
        public decimal best_ask { get; set; }
        public string side { get; set; }
        public DateTime time { get; set; }
        public long trade_id { get; set; }
        public decimal last_size { get; set; }
    }
}
