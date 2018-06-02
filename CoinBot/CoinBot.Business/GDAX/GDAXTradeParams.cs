using System;
using System.Collections.Generic;
using System.Text;

namespace CoinBot.Business.Entities
{
    public class GDAXTradeParams
    {
        public string type { get; set; }
        public decimal size { get; set; }
        public decimal price { get; set; }
        public string side { get; set; }
        public string product_id { get; set; }
    }
}
