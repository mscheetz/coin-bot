using System;
using System.Collections.Generic;
using System.Text;

namespace CoinBot.Business.Entities
{
    public enum SignalType
    {
        None,
        Percent,
        Volume,
        BollingerBandUpper,
        BollingerBandLower,
        OrderBook
    }
}
