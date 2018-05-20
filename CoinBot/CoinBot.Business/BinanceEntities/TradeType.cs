using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace CoinBot.Business.Entities
{
    public enum TradeType
    {
        [Description("BUY")]
        BUY,
        [Description("SELL")]
        SELL
    }
}
