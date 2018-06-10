using CoinBot.Core;
using System;
using Xunit;

namespace Coinbot.Core.Tests
{
    public class SecurityTests
    {
        [Fact]
        public void GdaxHmacTest_1()
        {
            // Arrange
            Security sec = new Security();
            var method = "GET";
            var nonce = "1517266319.000";
            var path = "/accounts";
            var secret = "D1/0wNj3wsKg8XcTs4KCfZUVzsHXIOW7w38Moj+YximHA5VQS7zAG47bgNSNGIGtFtYQ0vei2JiSPvX3JkBsA==";
            var expected = "jQxycBtZQKEWUZjtIaFnD1zoUDQebGeHmoIW3KHvmtg=";
            var message = nonce + method + path + "";

            // Act
            var hmac = sec.GetHMACSignature(secret, message);

            // Assert
            Assert.Equal(expected, hmac);
        }
    }
}
