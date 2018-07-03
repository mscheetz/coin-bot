using System;
using System.Collections.Generic;
using System.Text;

namespace CoinBot.Business.Entities
{
    public class CancelTradeParams
    {
        public string symbol { get; set; }
        public long orderId { get; set; }
        public string origClientOrderId { get; set; }
        public long timestamp { get; set; }
        public string type { get; set; }
    }
}
