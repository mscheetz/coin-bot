// -----------------------------------------------------------------------------
// <copyright file="Transaction" company="Matt Scheetz">
//     Copyright (c) Matt Scheetz All Rights Reserved
// </copyright>
// <author name="Matt Scheetz" date="1/24/2019 8:12:24 PM" />
// -----------------------------------------------------------------------------

namespace range_bot.Models
{
    #region Usings

    using System;
    using System.Collections.Generic;
    using System.Text;

    #endregion Usings

    public class Transaction
    {
        #region Properties

        public string Pair { get; set; }

        public string Side { get; set; }

        public DateTime Date { get; set; }

        public decimal Price { get; set; }

        public decimal Quantity { get; set; }

        public string OrderId { get; set; }

        public bool Complete { get; set; }

        #endregion Properties
    }
}