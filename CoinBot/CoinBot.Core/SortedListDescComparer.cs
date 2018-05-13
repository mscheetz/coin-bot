using System;
using System.Collections.Generic;
using System.Text;

namespace CoinBot.Core
{
    /// <summary>
    /// Sort SortedList descending
    /// </summary>
    /// <typeparam name="T">Generic Type</typeparam>
    public class SortedListDescComparer<T> : IComparer<T>
    {
        public int Compare(T x, T y)
        {
            return Comparer<T>.Default.Compare(y, x);
        }
    }
}
