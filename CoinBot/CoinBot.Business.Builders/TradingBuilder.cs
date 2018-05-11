using CoinBot.Business.Builders.Interface;
using CoinBot.Business.Entities;
using CoinBot.Data.Interface;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CoinBot.Business.Builders
{
    public class TradingBuilder : ITradingBuilder
    {
        private IBinanceRepository _repo;
        private const int bbRange = 21;

        public TradingBuilder(IBinanceRepository repo)
        {
            this._repo = repo;
        }

        /// <summary>
        /// Get 21 day Simple Moving Average
        /// </summary>
        /// <param name="symbol">String of symbol</param>
        /// <returns>decimal of SMA</returns>
        public decimal Get21DaySMA(string symbol)
        {
            decimal sma = 0;

            var candlesticks = GetCandlesticks(symbol, Interval.OneD, 21);

            sma = candlesticks.Average(c => c.close);

            return sma;
        }

        public SortedList<long, Dictionary<string, decimal>> GetBollingerBands(string symbol, Interval interval)
        {
            var candlesticks = GetCandlesticks(symbol, interval, bbRange)
                                    .OrderBy(c => c.closeTime)
                                    .ToList();

            var dataList = new SortedList<long, Dictionary<string, decimal>>();
            
            foreach(var candle in candlesticks)
            {
                var dict = new Dictionary<string, decimal>
                {
                    { "close", candle.close }
                };
                dataList.Add(candle.closeTime, dict);
            }

            AddBollingerBands(ref dataList, bbRange, 2);
            var returner = dataList.Values[dataList.Count-1];
            return dataList;
        }

        private void AddBollingerBands(ref SortedList<long, Dictionary<string, decimal>> data, int period, int factor)
        {
            decimal total_average = 0;
            decimal total_squares = 0;

            for (int i = 0; i < data.Count(); i++)
            {
                total_average += data.Values[i]["close"];
                total_squares += (decimal)Math.Pow((double)data.Values[i]["close"], 2);

                if (i >= period - 1)
                {
                    decimal average = total_average / period;

                    decimal stdev = (decimal)Math.Sqrt((double)(total_squares - (decimal)Math.Pow((double)total_average, 2) / period) / period);
                    data.Values[i]["bollinger_average"] = average;
                    data.Values[i]["bollinger_top"] = average + factor * stdev;
                    data.Values[i]["bollinger_bottom"] = average - factor * stdev;

                    total_average -= data.Values[i - period + 1]["close"];
                    total_squares -= (decimal)Math.Pow((double)data.Values[i - period + 1]["close"], 2);
                }
            }
        }        

        private List<Candlestick> GetCandlesticks(string symbol, Interval interval, int range)
        {
            return _repo.GetCandlestick(symbol, interval, range).Result.ToList();
        }
    }
}
