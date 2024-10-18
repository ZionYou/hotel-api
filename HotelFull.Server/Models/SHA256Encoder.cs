using System.Security.Cryptography;
using System.Text;

namespace HotelFull.Server.Models
{
    public class SHA256Encoder
    {
        private static readonly HashAlgorithm Crypto = null;

        static SHA256Encoder()
        {
            SHA256Encoder.Crypto = new SHA256CryptoServiceProvider();
        }

        public static string Encrypt(string originalString)
        {
            byte[] source = Encoding.Default.GetBytes(originalString);//將字串轉為Byte[]
            byte[] crypto = SHA256Encoder.Crypto.ComputeHash(source);//進行SHA256加密
            string result = string.Empty;

            for (int i = 0; i < crypto.Length; i++)
            {
                result += crypto[i].ToString("X2");
            }

            return result.ToUpper();
        }
    }
}
