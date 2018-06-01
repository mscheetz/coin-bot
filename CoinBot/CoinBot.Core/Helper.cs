using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CoinBot.Core
{
    public class Helper
    {
        /// <summary>
        /// Round a number down N decimal places
        /// </summary>
        /// <param name="number">Number to round down</param>
        /// <param name="decimalPlaces">N decimal places</param>
        /// <returns>Rounded down number</returns>
        public decimal RoundDown(decimal number, double decimalPlaces)
        {
            var power = Convert.ToDecimal(Math.Pow(10, decimalPlaces));

            return Math.Floor(number * power) / power;
        }

        /// <summary>
        /// Calculate buy percent difference
        /// </summary>
        /// <param name="currentPrice">Current price</param>
        /// <param name="lastSell">Last sell price</param>
        /// <returns>Double of percent difference</returns>
        public double GetBuyPercent(decimal currentPrice, decimal lastSell)
        {
            return GetPercent(lastSell, currentPrice);
        }

        /// <summary>
        /// Calculate sell percent difference
        /// </summary>
        /// <param name="currentPrice">Current price</param>
        /// <param name="lastBuy">Last buy price</param>
        /// <returns>Double of percent difference</returns>
        public double GetSellPercent(decimal currentPrice, decimal lastBuy)
        {
            return GetPercent(currentPrice, lastBuy);
        }

        /// <summary>
        /// Calculate percent difference from current price and last buy
        /// </summary>
        /// <param name="priceA">Price A</param>
        /// <param name="priceB">Price B</param>
        /// <returns>double of percent difference</returns>
        public double GetPercent(decimal priceA, decimal priceB)
        {
            var A = priceA;
            var B = priceB;
            var C = B == 0 ? 0 : (double)(A / B) - 1;

            return C;
        }

        /// <summary>
        /// Get zero'd satoshi count
        /// </summary>
        /// <returns>decimal of zero sats</returns>
        public decimal ZeroSats()
        {
            return 0.00000000M;
        }

        /// <summary>
        /// Get a collection from an enum
        /// </summary>
        /// <typeparam name="T">Type of enum</typeparam>
        /// <returns>Collection of enum values</returns>
        public IEnumerable<T> GetEnumValues<T>()
        {
            return Enum.GetValues(typeof(T)).Cast<T>();
        }

        /// <summary>
        /// Map one entity to another
        /// </summary>
        /// <typeparam name="F">From entity type</typeparam>
        /// <typeparam name="T">To entity type</typeparam>
        /// <param name="fromEntity">FromEntity object</param>
        /// <returns>To entity object</returns>
        public T MapEntity<F, T>(F fromEntity)
        {
            return Mapper.Map<F, T>(fromEntity);
        }

        /// <summary>
        /// Creates GDAX pair
        /// </summary>
        /// <param name="pair">String of pair</param>
        /// <returns>String of GDAX pair</returns>
        public string CreateGdaxPair(string pair)
        {
            if (pair.IndexOf("-") < 0)
            {
                pair = pair.Substring(0, 3) + "-" + pair.Substring(3);
            }

            return pair;
        }
    }
}
