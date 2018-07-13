using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace CoinBot.Business.Entities.GDAX
{
    [JsonConverter(typeof(Converter.ObjectToArrayConverter<OrderBook>))]
    public class OrderBook
    {
        [JsonProperty(Order = 1)]
        public decimal price { get; set; }
        [JsonProperty(Order = 2)]
        public decimal size { get; set; }
        [JsonProperty(Order = 3)]
        public int orders { get; set; }
    }
}
