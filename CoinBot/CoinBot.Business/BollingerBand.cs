using System;
using System.Collections.Generic;
using System.Text;

namespace CoinBot.Business.Entities
{
    public class BollingerBand
    {
        public decimal topBand { get; set; }
        public decimal movingAvg { get; set; }
        public decimal bottomBand { get; set; }
    }
}
