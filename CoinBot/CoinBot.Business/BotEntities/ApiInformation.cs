using System;
using System.Collections.Generic;
using System.Text;

namespace CoinBot.Business.Entities
{
    public class ApiInformation
    {
        public string apiSource { get; set; }
        public string apiKey { get; set; }
        public string apiSecret { get; set; }
        public string extraValue { get; set; }
        public DateTime lastUpdate { get; set; }
    }
}
