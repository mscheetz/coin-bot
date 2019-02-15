// -----------------------------------------------------------------------------
// <copyright file="BollingerBand" company="Matt Scheetz">
//     Copyright (c) Matt Scheetz All Rights Reserved
// </copyright>
// <author name="Matt Scheetz" date="2/15/2019 4:54:47 PM" />
// -----------------------------------------------------------------------------

namespace coinbot.strategies.zzz_Old_Bot
{
    #region Usings

    using System;
    using System.Collections.Generic;
    using System.Text;

    #endregion Usings

    public class BollingerBand
    {
        public decimal topBand { get; set; }
        public decimal movingAvg { get; set; }
        public decimal bottomBand { get; set; }
    }
}