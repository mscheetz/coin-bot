using System;
using System.Collections.Generic;
using System.Text;

namespace CoinBot.Business.Entities
{
    public class Balance
    {
        public string asset { get; set; }
        public string free { get; set; }
        public string locked { get; set; }
    }
}
