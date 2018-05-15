using CoinBot.Business.Entities;
using System;
using System.Collections.Generic;

namespace CoinBot.Business.Builders.Interface
{
    public interface ITradingBuilder
    {

        /// <summary>
        /// Get 21 day Simple Moving Average
        /// </summary>
        /// <param name="symbol">String of symbol</param>
        /// <returns>decimal of SMA</returns>
        decimal Get21DaySMA(string symbol);

        //SortedList<long, Dictionary<string, decimal>> GetBollingerBands(string symbol, Interval interval);
        Candlestick[] GetBollingerBands(string symbol, Interval interval);
    }
}
