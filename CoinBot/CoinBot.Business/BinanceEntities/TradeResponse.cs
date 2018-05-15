using System;
using System.Collections.Generic;
using System.Text;

namespace CoinBot.Business.Entities
{
    public class TradeResponse
    {
        public string symbol { get; set; }
        public long orderId { get; set; }
        public string clientOrderId { get; set; }
        public long transactTime { get; set; }
        public decimal price { get; set; }
        public decimal origQty { get; set; }
        public decimal executedQty { get; set; }
        public string status { get; set; }
        public string timeInForce { get; set; }
        public string type { get; set; }
        public string side { get; set; }
    }
}
