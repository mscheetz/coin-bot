using System;
using System.Collections.Generic;
using System.Text;

namespace CoinBot.Business.Entities
{
    public enum OrderType
    {
        LIMIT,
        MARKET,
        STOP_LOSS,
        STOP_LOSS_LIMIT,
        TAKE_PROFIT,
        TAKE_PROFIT_LIMIT,
        LIMIT_MAKER
    }
}
