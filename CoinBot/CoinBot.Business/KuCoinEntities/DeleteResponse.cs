using System;
using System.Collections.Generic;
using System.Text;

namespace CoinBot.Business.Entities.KuCoinEntities
{
    public class DeleteResponse
    {
        public bool success { get; set; }
        public string code { get; set; }
        public string data { get; set; }
    }
}
