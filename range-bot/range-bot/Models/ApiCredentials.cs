// -----------------------------------------------------------------------------
// <copyright file="ApiCredentials" company="Matt Scheetz">
//     Copyright (c) Matt Scheetz All Rights Reserved
// </copyright>
// <author name="Matt Scheetz" date="1/24/2019 7:56:17 PM" />
// -----------------------------------------------------------------------------

namespace range_bot.Models
{
    #region Usings

    using System;
    using System.Collections.Generic;
    using System.Text;

    #endregion Usings

    public class ApiCredentials
    {
        #region Properties

        public string ApiKey { get; set; }

        public string ApiSecret { get; set; }

        public string ApiPassword { get; set; }

        public string WIF { get; set; }

        #endregion Properties
    }
}