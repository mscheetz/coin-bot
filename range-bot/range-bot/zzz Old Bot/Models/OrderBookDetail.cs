// -----------------------------------------------------------------------------
// <copyright file="OrderBookDetail" company="Matt Scheetz">
//     Copyright (c) Matt Scheetz All Rights Reserved
// </copyright>
// <author name="Matt Scheetz" date="2/15/2019 4:58:13 PM" />
// -----------------------------------------------------------------------------

namespace coinbot.strategies.zzz_Old_Bot
{
    #region Usings

    using System;
    using System.Collections.Generic;
    using System.Text;

    #endregion Usings

    public class OrderBookDetail
    {
        public decimal price { get; set; }
        public int precision { get; set; }
        public int position { get; set; }
    }
}