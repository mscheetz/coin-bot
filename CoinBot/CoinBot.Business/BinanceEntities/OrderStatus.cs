using System;
using System.Collections.Generic;
using System.Text;

namespace CoinBot.Business.Entities
{
    public enum OrderStatus
    {
        NEW,
        PARTIALLY_FILLED,
        FILLED,
        CANCELED,
        REJECTED,
        EXPIRED
    }
}
