using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace HotelAPI.Service
{
    public class EcpayService
    {
        private readonly string _MerchantID;
        private readonly string _hashKey;
        private readonly string _hashIV;

        public EcpayService(IConfiguration configuration)
        {
            _MerchantID = configuration["ECpay:MerchantID"];
            _hashKey = configuration["ECpay:hashKey"];
            _hashIV = configuration["ECpay:hashIV"];  // 修正IV參數
        }

        public Dictionary<string, string> getBody(string title , Dictionary<string, string> prodlist)
        {
            int totalAmount = 0;
            string MerchantTradeNo = title + new Random().Next(0, 99999).ToString();
            string MerchantTradeDate = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
            string PaymentType = "aio";
            string TradeDesc = "訂單說明";
            string ChoosePayment = "ALL";
            string EncryptType = "1";

            Dictionary<string, string> macList = new Dictionary<string, string>();
            macList.Add("MerchantID", _MerchantID);
            macList.Add("MerchantTradeNo", MerchantTradeNo);
            macList.Add("MerchantTradeDate", MerchantTradeDate);
            macList.Add("PaymentType", PaymentType);
            macList.Add("TradeDesc", TradeDesc);
            macList.Add("ReturnURL", "https://localhost:7280/api/Ecpay/fetchEcpay");
            //macList.Add("OrderResultURL", "https://localhost:7280/api/Ecpay/fetchEcpay");       // 交易完成後跳往的畫面
            macList.Add("ChoosePayment", ChoosePayment);
            macList.Add("EncryptType", EncryptType);

            List<string> ItemNameArr = new List<string>();
            foreach (KeyValuePair<string, string> i in prodlist)
            {
                totalAmount += Int32.Parse(i.Value);
                ItemNameArr.Add(i.Key);
            }
            macList.Add("TotalAmount", totalAmount.ToString());
            macList.Add("ItemName", string.Join("#", ItemNameArr));

            // 將macList進行排序
            var sortedmacList = macList.OrderBy(pair => pair.Key);

            // 建立檢查碼前的原始字串
            string res = $"HashKey={_hashKey}&";

            foreach (var i in sortedmacList)
            {
                res += $"{i.Key}={i.Value}&";
            }
            res += $"HashIV={_hashIV}";

            // 使用 BuildCheckMacValue 來生成 CheckMacValue
            string checkMacValue = BuildCheckMacValue(res);
            macList.Add("CheckMacValue", checkMacValue);

            return macList;
        }

        private string BuildCheckMacValue(string parameters)
        {
            // Step 1: URL encode and replace special characters
            string encodedString = HttpUtility.UrlEncode(parameters).ToLower();

            // 替換特殊字符
            encodedString = encodedString.Replace("%20", "+")
                                         .Replace("%21", "!")
                                         .Replace("%2a", "*")
                                         .Replace("%28", "(")
                                         .Replace("%29", ")")
                                         .Replace("%7e", "~");

            // Step 2: SHA256 加密
            string hash = GetSHA256Hash(encodedString);

            // Step 3: 轉換為大寫
            return hash.ToUpper();
        }

        private string GetSHA256Hash(string input)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));
                StringBuilder builder = new StringBuilder();
                foreach (byte b in bytes)
                {
                    builder.Append(b.ToString("x2"));  // 轉換為十六進制格式
                }
                return builder.ToString();
            }
        }
    }
}
