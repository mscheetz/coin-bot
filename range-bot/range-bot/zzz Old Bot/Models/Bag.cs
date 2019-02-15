// -----------------------------------------------------------------------------
// <copyright file="Bag" company="Matt Scheetz">
//     Copyright (c) Matt Scheetz All Rights Reserved
// </copyright>
// <author name="Matt Scheetz" date="2/15/2019 4:54:06 PM" />
// -----------------------------------------------------------------------------

namespace coinbot.strategies.zzz_Old_Bot
{
    #region Usings

    using System;
    using System.Collections.Generic;
    using System.Text;

    #endregion Usings

    public class Bag
    {
        public string pair { get; set; }
        public decimal price { get; set; }
        public decimal quantity { get; set; }
        public long tradeTime { get; set; }
    }
}