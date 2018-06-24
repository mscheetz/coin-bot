using System;
using System.Collections.Generic;
using System.Text;

namespace CoinBot.Business.Entities.KuCoinEntities
{
    public class DeleteRequest
    {
        public string orderOid { get; set; }
        public string type { get; set; }
    }
}
