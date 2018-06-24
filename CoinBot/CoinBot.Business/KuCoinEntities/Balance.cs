using System;
using System.Collections.Generic;
using System.Text;

namespace CoinBot.Business.Entities.KuCoinEntities
{
    public class Balance
    {
        public string coinType { get; set; }
        public decimal balance { get; set; }
        public decimal freezeBalance { get; set; }
    }
}
