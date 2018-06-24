using System;
using System.Collections.Generic;
using System.Text;

namespace CoinBot.Business.Entities.KuCoinEntities
{
    public class OrderDetail
    {
        public string coinType { get; set; }
        public decimal dealValueTotal { get; set; }
        public decimal dealPriceAverage { get; set; }
        public decimal feeTotal { get; set; }
        public string userOid { get; set; }
        public decimal dealAmount { get; set; }
        public DealOrder dealOrders { get; set; }
        public string coinTypePair { get; set; }
        public decimal orderPrice { get; set; }
        public string type { get; set; }
        public string orderOid { get; set; }
        public decimal pendingAmount { get; set; }
    }
}
