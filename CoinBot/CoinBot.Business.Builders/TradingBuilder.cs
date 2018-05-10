using CoinBot.Business.Entities;
using CoinBot.Data.Interface;
using System;
using System.Linq;

namespace CoinBot.Business.Builders
{
    public class TradingBuilder
    {
        private IBinanceRepository _repo;

        public TradingBuilder(IBinanceRepository repo)
        {
            this._repo = repo;
        }

        /// <summary>
        /// Get 20 day Simple Moving Average
        /// </summary>
        /// <param name="symbol">String of symbol</param>
        /// <returns>double of SMA</returns>
        public double Get20DaySMA(string symbol)
        {
            double sma = 0;

            var candlesticks = _repo.GetCandlestick(symbol, Interval.OneD, 20).Result;

            sma = candlesticks.Average(c => c.close);

            return sma;
        }
    }
}
