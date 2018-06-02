using System;
using System.Collections.Generic;
using System.Text;

namespace CoinBot.Business.Entities
{
    public class GDAXOrderResponse
    {
        public string id { get; set; }
        public decimal price { get; set; }
        public decimal size { get; set; }
        public string product_id { get; set; }
        public string side { get; set; }
        public string stp { get; set; }
        public string type { get; set; }
        public TimeInForce time_in_force { get; set; }
        public bool post_only { get; set; }
        public DateTimeOffset created_at { get; set; }
        public decimal fill_fees { get; set; }
        public decimal filled_size { get; set; }
        public decimal executed_value { get; set; }
        public string status { get; set; }
        public bool settled { get; set; }
    }
}
