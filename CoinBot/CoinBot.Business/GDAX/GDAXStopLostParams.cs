using System;
using System.Collections.Generic;
using System.Text;

namespace CoinBot.Business.Entities
{
    public class GDAXStopLossParams : GDAXTradeParams
    {
        public string stop { get; set; }
        public decimal stop_price { get; set; }
    }
}
