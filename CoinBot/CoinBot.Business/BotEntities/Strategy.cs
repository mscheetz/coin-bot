using System;
using System.Collections.Generic;
using System.Text;

namespace CoinBot.Business.Entities
{
    public enum Strategy
    {
        None,
        BollingerBands,
        OrderBook,
        Percentage,
        Volume
    }
}
