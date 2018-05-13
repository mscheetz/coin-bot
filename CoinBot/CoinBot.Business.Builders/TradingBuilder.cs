using CoinBot.Business.Builders.Interface;
using CoinBot.Business.Entities;
using CoinBot.Core;
using CoinBot.Data.Interface;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CoinBot.Business.Builders
{
    public class TradingBuilder : ITradingBuilder
    {
        private IBinanceRepository _repo;
        private const int candlestickCount = 21;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="repo">Repository interface</param>
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

        public void TradeCoin(string pair, Interval interval)
        {
            long last_open = 0;
            var bbs = GetBollingerBands(pair, interval);
            TradeType tradeType;

            bool trade = false;
            while(!trade)
            {
                int i = bbs.Count;
                var latestStick = bbs[i - 1];
                var prevStick = bbs[i - 2];

                //if(latestStick.Values["bollingerTop"])
            }
        }

        /// <summary>
        /// Get Bollinger Bands for a symbol
        /// </summary>
        /// <param name="pair">Trading pair</param>
        /// <param name="interval">Candlestick interval</param>
        /// <returns>SortedList of values</returns>
        public SortedList<long, Dictionary<string, decimal>> GetBollingerBands(string pair, Interval interval)
        {
            var candlesticks = GetCandlesticks(pair, interval, candlestickCount)
                                    .OrderByDescending(c => c.closeTime)
                                    .ToList();

            var dataList = new SortedList<long, Dictionary<string, decimal>>();
            
            foreach(var candle in candlesticks)
            {
                var dict = new Dictionary<string, decimal>
                {
                    { "openTime", candle.openTime },
                    { "close", candle.close },
                    { "closeTime", candle.closeTime },
                    { "volume", (decimal)candle.volume }
                };
                dataList.Add(candle.closeTime, dict);
            }

            AddBollingerBands(ref dataList);
            
            var returner = dataList.Values[dataList.Count-1];
            
            return dataList;
        }

        /// <summary>
        /// Add Bollinger Bands and Volume data to list
        /// </summary>
        /// <param name="data">SortedList to update</param>
        private void AddBollingerBands(ref SortedList<long, Dictionary<string, decimal>> data)
        {
            int period = candlestickCount;
            int factor = 2;
            decimal total_average = 0;
            decimal total_squares = 0;
            decimal prev_vol = 0;

            for (int i = 0; i < data.Count(); i++)
            {
                total_average += data.Values[i]["close"];
                total_squares += (decimal)Math.Pow((double)data.Values[i]["close"], 2);
                prev_vol = prev_vol == 0 ? data.Values[i]["volume"] : prev_vol;

                var volData = CalculateVolumeChanges(data.Values[i]["volume"], prev_vol);

                foreach(var vol in volData)
                {
                    data.Values[i].Add(vol.Key, vol.Value);
                }

                if (i >= period - 1)
                {
                    decimal average = total_average / period;

                    decimal stdev = (decimal)Math.Sqrt((double)(total_squares - (decimal)Math.Pow((double)total_average, 2) / period) / period);
                    data.Values[i]["bollingerAverage"] = average;
                    data.Values[i]["bollingerTop"] = average + factor * stdev;
                    data.Values[i]["bollingerBottom"] = average - factor * stdev;

                    total_average -= data.Values[i - period + 1]["close"];
                    total_squares -= (decimal)Math.Pow((double)data.Values[i - period + 1]["close"], 2);
                }

                prev_vol = data.Values[i]["volume"];
            }
        }

        /// <summary>
        /// Calculate volume data
        /// </summary>
        /// <param name="curr_vol">Current volume</param>
        /// <param name="prev_vol">Previous volume</param>
        /// <returns>Dictionary of values</returns>
        private Dictionary<string, decimal> CalculateVolumeChanges(decimal curr_vol, decimal prev_vol)
        {
            decimal vol_diff = curr_vol - prev_vol;
            decimal vol_change = (curr_vol / prev_vol) - 1;

            return new Dictionary<string, decimal>
            {
                {"volume_difference", vol_diff },
                {"volume_change", vol_change }
            };            
        }

        /// <summary>
        /// Get Candlesticks from exchange API
        /// </summary>
        /// <param name="pair">Trading pair</param>
        /// <param name="interval">Candlestick Interval</param>
        /// <param name="range">Number of candlesticks to get</param>
        /// <returns>List of Candlestick objects</returns>
        private List<Candlestick> GetCandlesticks(string pair, Interval interval, int range)
        {
            return _repo.GetCandlestick(pair, interval, range).Result.ToList();
        }
    }
}
