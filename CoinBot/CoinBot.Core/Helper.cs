using System;
using System.Collections.Generic;
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
    }
}
