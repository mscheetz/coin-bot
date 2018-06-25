using CoinBot.Core;
using System;
using Xunit;

namespace Coinbot.Core.Tests
{
    public class HelperTests
    {
        [Fact]
        public void DecimalValueAtPrecisionTest()
        {
            var helper = new Helper();
            var precision = 7;

            // Act
            var newDecimal = helper.DecimalValueAtPrecision(precision);

            // Assert
            Assert.True(newDecimal > 0);
        }
    }
}
