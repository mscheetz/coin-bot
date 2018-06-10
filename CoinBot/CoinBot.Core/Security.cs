using System;
using System.Security.Cryptography;
using System.Text;

namespace CoinBot.Core
{
    public class Security
    {
        public string SHA256_hash(string value)
        {
            StringBuilder Sb = new StringBuilder();

            using (var hash = SHA256.Create())
            {
                Encoding enc = Encoding.UTF8;
                Byte[] result = hash.ComputeHash(enc.GetBytes(value));

                foreach (Byte b in result)
                    Sb.Append(b.ToString("x2"));
            }

            return Sb.ToString();
        }

        public string GetBinanceHMACSignature(string secretKey, string message)
        {
            ASCIIEncoding encoding = new ASCIIEncoding();
            byte[] keyBytes = encoding.GetBytes(secretKey);
            byte[] messageBytes = encoding.GetBytes(message);
            HMACSHA256 crypotgrapher = new HMACSHA256(keyBytes);

            byte[] bytes = crypotgrapher.ComputeHash(messageBytes);

            return BitConverter.ToString(bytes).Replace("-", "").ToLower();
        }

        public string GetHMACSignature(string secretKey, string totalParams)
        {
            secretKey = secretKey ?? "";
            var encoding = new ASCIIEncoding();
            byte[] keyByte = Convert.FromBase64String(secretKey);
            byte[] messageByte = encoding.GetBytes(totalParams);
            using (var mac = new HMACSHA256(keyByte))
            {
                byte[] hashMessage = mac.ComputeHash(messageByte);
                return Convert.ToBase64String(hashMessage);
            }
        }
    }
}
