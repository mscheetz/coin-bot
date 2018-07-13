using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace CoinBot.Business.Entities.GDAX
{
    public class OrderBookResponse
    {
        public long sequence { get; set; }
        [JsonProperty(PropertyName = "asks")]
        public OrderBook[] sells { get; set; }
        [JsonProperty(PropertyName = "bids")]
        public OrderBook[] buys { get; set; }
    }
}
