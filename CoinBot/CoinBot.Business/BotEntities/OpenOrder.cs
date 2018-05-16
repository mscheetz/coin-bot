using System;
using System.Collections.Generic;
using System.Text;

namespace CoinBot.Business.Entities
{
    public class OpenOrder
    {
        public long orderId { get; set; }
        public string clientOrderId { get; set; }
    }
}
