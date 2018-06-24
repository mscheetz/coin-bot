using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace CoinBot.Business.Entities.KuCoinEntities
{
    public class DealOrder
    {
        public int total { get; set; }
        public bool firstPage { get; set; }
        public bool lastPage { get; set; }
        [JsonProperty(PropertyName = "datas")]
        public OrderFill[] orderFills { get; set; }
        public int currPageNo { get; set; }
        public int limit { get; set; }
        public int pageNos { get; set; }
    }
}
