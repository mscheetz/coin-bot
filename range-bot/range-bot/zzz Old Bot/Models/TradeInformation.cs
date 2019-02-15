// -----------------------------------------------------------------------------
// <copyright file="TradeInformation" company="Matt Scheetz">
//     Copyright (c) Matt Scheetz All Rights Reserved
// </copyright>
// <author name="Matt Scheetz" date="2/15/2019 4:58:44 PM" />
// -----------------------------------------------------------------------------

namespace coinbot.strategies.zzz_Old_Bot
{
    #region Usings

    using System;
    using System.Collections.Generic;
    using System.Text;

    #endregion Usings

    public class TradeInformation
    {
        public string pair { get; set; }
        public string tradeType { get; set; }
        public decimal price { get; set; }
        public decimal quantity { get; set; }
        public DateTime timestamp { get; set; }
    }
}