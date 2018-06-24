using System;
using System.Linq;
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

        public string GetKuCoinHMCACSignature(string secretKey, string message)
        {
            return new HMACSHA256(Encoding.UTF8.GetBytes(secretKey)).ComputeHash(Encoding.UTF8.GetBytes(message)).Aggregate(new StringBuilder(), (sb, b) => sb.AppendFormat("{0:x2}", b), (sb) => sb.ToString());

            //secretKey = secretKey ?? "";
            //var encoding = new ASCIIEncoding();
            //byte[] keyByte = encoding.GetBytes(secretKey);
            //byte[] messageByte = encoding.GetBytes(message);
            //using (var mac = new HMACSHA256(keyByte))
            //{
            //    byte[] hashMessage = mac.ComputeHash(messageByte);
            //    return Convert.ToBase64String(hashMessage);
            //}

            //--------------------------------
            //var encoding = new ASCIIEncoding();
            //var msgBytes = Encoding.Default.GetBytes(message);
            //string msgString = Convert.ToBase64String(msgBytes);
            //var keyBytes = Encoding.Default.GetBytes(secretKey);

            //var encodedMsg = encoding.GetBytes(msgString);

            //var hash = new HMACSHA256(keyBytes);
            //var hashMsg = hash.ComputeHash(encodedMsg);

            //return BitConverter.ToString(hashMsg).Replace("-", "").ToLower();

            //--------------------------


            //var messageString = Convert.ToBase64String(Encoding.UTF8.GetBytes(message));

            //byte[] keyBytes = Encoding.UTF8.GetBytes(secretKey);
            //byte[] msgBytes = Encoding.UTF8.GetBytes(messageString);
            //using (var hmac = new HMACSHA256(keyBytes))
            //{
            //    byte[] msgHash = hmac.ComputeHash(msgBytes);
            //    return BitConverter.ToString(msgHash).Replace("-", "").ToLower();
            //}
        }

        public string GetHMACSignature(string secretKey, string totalParams)
        {
            //secretKey = secretKey ?? "";
            byte[] keyByte = Convert.FromBase64String(secretKey);
            byte[] messageByte = Encoding.UTF8.GetBytes(totalParams);
            using (var hmac = new HMACSHA256(keyByte))
            {
                byte[] hashMessage = hmac.ComputeHash(messageByte);
                return Convert.ToBase64String(hashMessage);
            }
        }
    }
}
