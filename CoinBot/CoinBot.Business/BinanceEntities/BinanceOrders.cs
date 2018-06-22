using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace CoinBot.Business.Entities
{
    [JsonConverter(typeof(Converter.ObjectToArrayConverter<Candlestick>))]
    public class BinanceOrders
    {
        [JsonProperty(Order = 1)]
        public decimal price { get; set; }
        [JsonProperty(Order = 2)]
        public decimal quantity { get; set; }
        [JsonProperty(Order = 3)]
        public string[] ignore { get; set; }
    }
}
