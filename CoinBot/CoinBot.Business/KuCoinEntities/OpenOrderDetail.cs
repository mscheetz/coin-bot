using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace CoinBot.Business.Entities.KuCoinEntities
{
    [JsonConverter(typeof(Converter.ObjectToArrayConverter<OpenOrderDetail>))]
    public class OpenOrderDetail
    {
        [JsonProperty(Order = 1)]
        public long timestamp { get; set; }
        [JsonProperty(Order = 2)]
        public string type { get; set; }
        [JsonProperty(Order = 3)]
        public decimal price { get; set; }
        [JsonProperty(Order = 4)]
        public decimal quantity { get; set; }
        [JsonProperty(Order = 5)]
        public string orderId { get; set; }
    }
}
