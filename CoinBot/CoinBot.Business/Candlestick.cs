using System;
using System.Collections.Generic;
using System.Text;

namespace CoinBot.Business.Entities
{
    public class Candlestick
    {
        public long openTime { get; set; }
        public double open { get; set; }
        public double high { get; set; }
        public double low { get; set; }
        public double close { get; set; }
        public double volume { get; set; }
        public long closeTime { get; set; }
        public double quoteAssetVolume { get; set; }
        public long numberTrades { get; set; }
        public double takerBase { get; set; }
        public double takerQuote { get; set; }
        public double ignore { get; set; }
    }
}
