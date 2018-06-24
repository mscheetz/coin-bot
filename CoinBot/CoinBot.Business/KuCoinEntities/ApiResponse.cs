using System;
using System.Collections.Generic;
using System.Text;

namespace CoinBot.Business.Entities.KuCoinEntities
{
    public class ApiResponse<T>
    {
        public bool success { get; set; }
        public string code { get; set; }
        public string msg { get; set; }
        public T data { get; set; }
    }
}
