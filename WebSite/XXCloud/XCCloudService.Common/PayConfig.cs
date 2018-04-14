//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Security.Cryptography;
//using System.Text;
//using System.Threading.Tasks;

//namespace XCCloudService.Common
//{
//    public class PayConfig
//    {
//        public static string alipay_public_key = @"MIGfMA0GCSqGSIb3DQEBAQUAA4GNADCBiQKBgQDDI6d306Q8fIfCOaTXyiUeJHkrIvYISRcc73s3vF1ZT7XN8RNPwJxo8pWaJMmvyTn9N4HQ632qJBVHf8sxHi/fEsraprwCtzvzQETrNRwVxLO5jVmRGi60j8Ue1efIlzPXV9je9mkjzOmdssymZkh2QhUrCmZYI/FCEa3/cNMW0QIDAQAB";
//        //这里要配置没有经过的原始私钥
//        public static string merchant_private_key = "MIICXQIBAAKBgQDBkwdq6jV8DsW34UTQpeVa+zTDS1q5kkoOS/up+8xJvzTbDMj9RJpLdQPJyRdm8ynssaX/A+/I/emc7WjUdQ8k06MoZ7KZgxQ53alYnP782vZHzX8mbkmyO45JBpmB5mDqf6bUZ8eF0sruIxjBPrfZKY81Y2NGMb7GpUnRxglbAwIDAQABAoGAWlZ8mrfM2CWovkTiMaGKZShhGlc7culDqGJrg4vgbfZO+39++Tuf6mEksHIpesE2qqDJgDbdG+brtHHTf3tV4iDCCyCWMGasEsDYHrJdjDNMI3TRaPnFG7c0Gl3/3WfJt6+6kUWrA4yHpJ3671oW4vqYcZLClm4vvscAJI6iq5kCQQDsLqSQ+1HSTq6R/0Z6RWubq86tQaM8mHiVt/Q568E7vof4/WRAzm4RgHJFxSICsDNWHUOcXj51FMvDh3h4w5/lAkEA0dEjwL3iWKfNMbWs+GlxonULrbz7klU7ybaLjlPpqpD6u5vtwJIUeCsCz4t5F3ebGYy17ewvpcdTcJiLNvbQxwJBAKOmBf03RpB0WF1tBgZ+x3sL5p8CFftMonELDzx68F9XO5v451hIMNgLqiJR36kBMfD/QfO9EYXKrQ2EdqMm2UkCQQC5TZPd7dWxpsE6XdFdswKBEA44audj3ZLoGyg1kzaTRWeruZLIuDsLxjC9S9iuORvp/LPQZP/P0bKHhrGs4V6RAkBcd6P8QwkeMpHG471oa9dxU9SXn8YC4nK7ePEJ/F9g023L2t8DO2heBuD/SyQrtYu1TXSuMqc3t/ZRFspUM8MF";
//        public static string merchant_public_key = @"MIGfMA0GCSqGSIb3DQEBAQUAA4GNADCBiQKBgQDBkwdq6jV8DsW34UTQpeVa+zTDS1q5kkoOS/up+8xJvzTbDMj9RJpLdQPJyRdm8ynssaX/A+/I/emc7WjUdQ8k06MoZ7KZgxQ53alYnP782vZHzX8mbkmyO45JBpmB5mDqf6bUZ8eF0sruIxjBPrfZKY81Y2NGMb7GpUnRxglbAwIDAQAB";
//        public static string appId = "2016051601406637";    //当面付APPID
//        public static string serverUrl = "https://openapi.alipay.com/gateway.do";
//        public static string mapiUrl = "https://mapi.alipay.com/gateway.do";
//        public static string monitorUrl = "http://mcloudmonitor.com/gateway.do";
//        public static string pid = "2088221884894147";


//        public static string charset = "utf-8";//"utf-8";
//        public static string sign_type = "RSA2";
//        public static string version = "1.0";

//        //微信支付所需要的
//        public static string WxAppID = "wxab2c44b2c5e18889";
//        public static string WxMchID = "1391813202";
//        public static string WxKey = "Whxckj40000515308888888888888888";
//        //public static string WxAppID = "wx86275e2035a8089d";
//        //public static string WxMchID = "1480638152";
//        //public static string WxKey = "Whxckj51530888888888888888888888";
//        public static string IP = "123.56.111.145";

//        /// <summary>
//        /// 利楚扫呗支付
//        /// </summary>
//        public static string LCStoreID = "852100210000005";
//        public static string LCDeviceID = "30050895";
//        public static string LCToken = "a94d34f12d474ba48d51b71e2f0e5be7";

//        public PayConfig()
//        {
//            //
//        }

//        public static string getMerchantPublicKeyStr()
//        {
//            StreamReader sr = new StreamReader(merchant_public_key);
//            string pubkey = sr.ReadToEnd();
//            sr.Close();
//            if (pubkey != null)
//            {
//                pubkey = pubkey.Replace("-----BEGIN PUBLIC KEY-----", "");
//                pubkey = pubkey.Replace("-----END PUBLIC KEY-----", "");
//                pubkey = pubkey.Replace("\r", "");
//                pubkey = pubkey.Replace("\n", "");
//            }
//            return pubkey;
//        }

//        public static string getMerchantPriveteKeyStr()
//        {
//            StreamReader sr = new StreamReader(merchant_private_key);
//            string pubkey = sr.ReadToEnd();
//            sr.Close();
//            if (pubkey != null)
//            {
//                pubkey = pubkey.Replace("-----BEGIN PUBLIC KEY-----", "");
//                pubkey = pubkey.Replace("-----END PUBLIC KEY-----", "");
//                pubkey = pubkey.Replace("\r", "");
//                pubkey = pubkey.Replace("\n", "");
//            }
//            return pubkey;
//        }

//        /// <summary>
//        /// DES解密字符串
//        /// </summary>
//        /// <param name="decryptString">待解密的字符串</param>
//        /// <param name="decryptKey">解密密钥,要求为8位,和加密密钥相同</param>
//        /// <returns>解密成功返回解密后的字符串，失败返源串</returns>
//        public static string DecryptDES(string pToDecrypt, string sKey)
//        {
//            DESCryptoServiceProvider des = new DESCryptoServiceProvider();

//            byte[] inputByteArray = new byte[pToDecrypt.Length / 2];
//            for (int x = 0; x < pToDecrypt.Length / 2; x++)
//            {
//                int i = (Convert.ToInt32(pToDecrypt.Substring(x * 2, 2), 16));
//                inputByteArray[x] = (byte)i;
//            }

//            des.Key = ASCIIEncoding.ASCII.GetBytes(sKey);
//            des.IV = ASCIIEncoding.ASCII.GetBytes(sKey);
//            MemoryStream ms = new MemoryStream();
//            CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(), CryptoStreamMode.Write);
//            cs.Write(inputByteArray, 0, inputByteArray.Length);
//            cs.FlushFinalBlock();

//            StringBuilder ret = new StringBuilder();

//            return System.Text.Encoding.Default.GetString(ms.ToArray());
//        }
//        /// <summary>
//        /// DES加密字符串
//        /// </summary>
//        /// <param name="pToEncrypt"></param>
//        /// <param name="sKey"></param>
//        /// <returns></returns>
//        public static string EncryptDES(string pToEncrypt, string sKey)
//        {
//            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
//            byte[] inputByteArray = Encoding.Default.GetBytes(pToEncrypt);
//            des.Key = ASCIIEncoding.ASCII.GetBytes(sKey);
//            des.IV = ASCIIEncoding.ASCII.GetBytes(sKey);
//            MemoryStream ms = new MemoryStream();
//            CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(), CryptoStreamMode.Write);
//            cs.Write(inputByteArray, 0, inputByteArray.Length);
//            cs.FlushFinalBlock();
//            StringBuilder ret = new StringBuilder();
//            foreach (byte b in ms.ToArray())
//            {
//                ret.AppendFormat("{0:X2}", b);
//            }
//            ret.ToString();
//            return ret.ToString();
//        }
//    }
//}
