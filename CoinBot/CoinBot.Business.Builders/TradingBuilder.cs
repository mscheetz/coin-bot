using CoinBot.Business.Builders.Interface;
using CoinBot.Business.Entities;
using CoinBot.Core;
using CoinBot.Data.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoinBot.Business.Builders
{
    public class TradingBuilder : ITradingBuilder
    {
        private IBinanceRepository _repo;
        private const int candlestickCount = 21;
        private BotSettings botSettings;
        private List<Bag> bags;
        private List<TradeInformation> tradeInformation;
        private decimal availableToTrade;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="repo">Repository interface</param>
        public TradingBuilder(IBinanceRepository repo)
        {
            this._repo = repo;
            bags = new List<Bag>();
            tradeInformation = new List<TradeInformation>();
            availableToTrade = 0;
        }

        public bool SetBotSettings(BotSettings settings)
        {
            botSettings = settings;

            return true;
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
            TradeType tradeType;
            var currentStick = new Candlestick();
            var previousStick = new Candlestick();

            bool trade = false;
            while(!trade)
            {
                var bbs = GetBollingerBands(pair, interval);
                int i = bbs.Length;
                currentStick = bbs[i];

                if (previousStick.close == 0)
                    previousStick = bbs[i];

                Task.Run(async () =>
                {
                    await Task.Delay(botSettings.priceCheck);


                });
            }
        }

        /// <summary>
        /// Get Bollinger Bands for a symbol
        /// </summary>
        /// <param name="pair">Trading pair</param>
        /// <param name="interval">Candlestick interval</param>
        /// <returns>Array of Candlesticks</returns>
        public Candlestick[] GetBollingerBands(string pair, Interval interval)
        {
            var candlesticks = GetCandlesticks(pair, interval, candlestickCount)
                                    .OrderByDescending(c => c.closeTime)
                                    .ToArray();
            
            AddBollingerBands(ref candlesticks);

            return candlesticks.Where(c => c.bollingerBand != null).ToArray();
        }

        /// <summary>
        /// Add Bollinger Bands and Volume data to list
        /// </summary>
        /// <param name="candlesticks">Array of Candlesticks</param>
        private void AddBollingerBands(ref Candlestick[] candlesticks)
        {
            int period = candlestickCount;
            int factor = 2;
            decimal total_average = 0;
            decimal total_squares = 0;
            decimal prev_vol = 0;

            for (int i = 0; i < candlesticks.Length; i++)
            {
                total_average += candlesticks[i].close;
                total_squares += (decimal)Math.Pow((double)candlesticks[i].close, 2);
                prev_vol = prev_vol == 0 ? candlesticks[i].volume : prev_vol;

                var volData = CalculateVolumeChanges(candlesticks[i].volume, prev_vol);

                if(volData != null)
                {
                    if(volData.ContainsKey("volumeDifference"))
                        candlesticks[i].volumeChange = volData["volumeDifference"];

                    if (volData.ContainsKey("volumePercentChange"))
                        candlesticks[i].volumePercentChange = volData["volumePercentChange"];
                }

                if (i >= period - 1)
                {
                    var bollingerBand = new BollingerBand();
                    decimal average = total_average / period;

                    decimal stdev = (decimal)Math.Sqrt((double)(total_squares - (decimal)Math.Pow((double)total_average, 2) / period) / period);
                    bollingerBand.movingAvg = average;
                    bollingerBand.topBand = average + factor * stdev;
                    bollingerBand.bottomBand = average - factor * stdev;

                    candlesticks[i].bollingerBand = bollingerBand;
                    total_average -= candlesticks[i - period + 1].close;
                    total_squares -= (decimal)Math.Pow((double)candlesticks[i - period + 1].close, 2);
                }

                prev_vol = candlesticks[i].volume;
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
                {"volumeDifference", vol_diff },
                {"volumePercentChange", vol_change }
            };            
        }

        /// <summary>
        /// Get Candlesticks from exchange API
        /// </summary>
        /// <param name="pair">Trading pair</param>
        /// <param name="interval">Candlestick Interval</param>
        /// <param name="range">Number of candlesticks to get</param>
        /// <returns>Array of Candlestick objects</returns>
        private Candlestick[] GetCandlesticks(string pair, Interval interval, int range)
        {
            return _repo.GetCandlestick(pair, interval, range).Result;
        }

        private TradeResponse PlaceTrade(TradeParams tradeParams)
        {
            return _repo.PostTrade(tradeParams).Result;
        }

        private TradeResponse CancelTrade(CancelTradeParams tradeParams)
        {
            return _repo.DeleteTrade(tradeParams).Result;
        }
    }
}
