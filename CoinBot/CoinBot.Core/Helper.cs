using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
        /// Creates dashed pair (ie BTC-ETH)
        /// </summary>
        /// <param name="pair">String of pair</param>
        /// <returns>String of pair</returns>
        public string CreateDashedPair(string pair)
        {
            if (pair.IndexOf("-") < 0)
            {
                pair = pair.Substring(0, 3) + "-" + pair.Substring(3);
            }

            return pair;
        }

        /// <summary>
        /// Set first character uppercase for a string
        /// </summary>
        /// <param name="toUpperCase">String to uppercase</param>
        /// <returns>String with first character uppercased</returns>
        public string UpperCaseFirst(string toUpperCase)
        {
            if (string.IsNullOrEmpty(toUpperCase))
            {
                return string.Empty;
            }
            char[] a = toUpperCase.ToCharArray();
            a[0] = char.ToUpper(a[0]);
            return new string(a);
        }

        /// <summary>
        /// Convert an array of strings to a string
        /// </summary>
        /// <param name="myArray">Array of strings</param>
        /// <returns>String of array values</returns>
        public string ArrayToString(string[] myArray)
        {
            var qsValues = string.Empty;

            for (int i = 0; i < myArray.Length; i++)
            {
                qsValues += qsValues != string.Empty ? "&" : "";
                qsValues += myArray[i];
            }

            return qsValues;
        }

        /// <summary>
        /// Convert an object to a string of property names and values
        /// </summary>
        /// <typeparam name="T">Object type</typeparam>
        /// <param name="myObject">Object to convert</param>
        /// <returns>String of properties and values</returns>
        public string ObjectToString<T>(T myObject)
        {
            var qsValues = string.Empty;

            foreach (PropertyInfo p in myObject.GetType().GetProperties())
            {
                qsValues += qsValues != string.Empty ? "&" : "";
                qsValues += $"{p.Name}={p.GetValue(myObject, null)}";
            }

            return qsValues;
        }

        /// <summary>
        /// Create new decmial to the Nth power
        /// </summary>
        /// <param name="precision">precision of decimal</param>
        /// <param name="value">Value to set, default = 1</param>
        /// <returns>New decimal</returns>
        public decimal DecimalValueAtPrecision(int precision, int value = 1)
        {
            var pow = Math.Pow(10, precision);
            decimal newValue = value / (decimal)pow;

            return newValue;
        }
    }
}
