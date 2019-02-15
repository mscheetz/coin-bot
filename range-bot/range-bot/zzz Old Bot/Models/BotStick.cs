// -----------------------------------------------------------------------------
// <copyright file="BotStick" company="Matt Scheetz">
//     Copyright (c) Matt Scheetz All Rights Reserved
// </copyright>
// <author name="Matt Scheetz" date="2/15/2019 4:55:50 PM" />
// -----------------------------------------------------------------------------

namespace coinbot.strategies.zzz_Old_Bot
{
    #region Usings

    using System;
    using System.Collections.Generic;
    using System.Text;

    #endregion Usings

    public class BotStick
    {
        public long openTime { get; set; }
        public decimal open { get; set; }
        public decimal high { get; set; }
        public decimal low { get; set; }
        public decimal close { get; set; }
        public decimal volume { get; set; }
        public long closeTime { get; set; }
        public decimal volumeChange { get; set; }
        public decimal volumePercentChange { get; set; }
        public BollingerBand bollingerBand { get; set; }
    }
}