// -----------------------------------------------------------------------------
// <copyright file="IRangeService" company="Matt Scheetz">
//     Copyright (c) Matt Scheetz All Rights Reserved
// </copyright>
// <author name="Matt Scheetz" date="1/29/2019 8:38:12 PM" />
// -----------------------------------------------------------------------------

namespace coinbot.strategies.Contracts.Interfaces
{
    #region Usings

    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Threading.Tasks;

    #endregion Usings

    public interface IRangeService
    {
        /// <summary>
        /// Run bot
        /// </summary>
        Task RunBot();
    }
}