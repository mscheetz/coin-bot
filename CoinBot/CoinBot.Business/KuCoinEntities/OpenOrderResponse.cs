using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace CoinBot.Business.Entities.KuCoinEntities
{
    public class OpenOrderResponse
    {
        [JsonProperty(PropertyName = "SELL")]
        public OpenOrderDetail[] openSells { get; set; }
        [JsonProperty(PropertyName = "BUYs")]
        public OpenOrderDetail[] openBuys { get; set; }
    }
}
