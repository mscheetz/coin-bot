using System;
using System.Collections.Generic;
using System.Text;

namespace CoinBot.Business.Entities
{
    public class GDAXOrder
    {
        public string id { get; set; }
        public decimal size { get; set; }
        public string product_id { get; set; }
        public string side { get; set; }
        public string stp { get; set; }
        public decimal funds { get; set; }
        public decimal specified_funds { get; set; }
        public string type { get; set; }
        public bool post_only { get; set; }
        public DateTimeOffset crated_at { get; set; }
        public DateTimeOffset done_at { get; set; }
        public string done_reason { get; set; }
        public decimal fill_fees { get; set; }
        public decimal fill_size { get; set; }
        public decimal executed_value { get; set; }
        public string status { get; set; }
        public bool settled { get; set; }
    }
}
