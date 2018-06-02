using System;
using System.Collections.Generic;
using System.Text;

namespace CoinBot.Business.Entities
{
    public class GDAXStopLostParams : GDAXTradeParams
    {
        public string stop { get; set; }
        public decimal stop_price { get; set; }
    }
}
