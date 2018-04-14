using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XCCloudService.Pay.DinPay
{
    public class DinPayConfig
    {
        /// <summary>
        /// 接口版本
        /// </summary>
        public static string InterfaceVersion = "V3.1";

        /// <summary>
        /// 参数编码字符集
        /// </summary>
        public static string InputCharset = "UTF-8";

        /// <summary>
        /// 签名方式
        /// </summary>
        public static string SignType = "RSA-S";

        /// <summary>
        /// 商户号
        /// </summary>
        public static string MerchantId = "1111110166";

        /// <summary>
        /// 回调地址
        /// </summary>
        public static string Notify_url = "http://106.14.174.131/Notify.aspx";

        /// <summary>
        /// 商户私钥
        /// </summary>
        public static string MerPriKey = "MIICdwIBADANBgkqhkiG9w0BAQEFAASCAmEwggJdAgEAAoGBALf/+xHa1fDTCsLYPJLHy80aWq3djuV1T34sEsjp7UpLmV9zmOVMYXsoFNKQIcEzei4QdaqnVknzmIl7n1oXmAgHaSUF3qHjCttscDZcTWyrbXKSNr8arHv8hGJrfNB/Ea/+oSTIY7H5cAtWg6VmoPCHvqjafW8/UP60PdqYewrtAgMBAAECgYEAofXhsyK0RKoPg9jA4NabLuuuu/IU8ScklMQIuO8oHsiStXFUOSnVeImcYofaHmzIdDmqyU9IZgnUz9eQOcYg3BotUdUPcGgoqAqDVtmftqjmldP6F6urFpXBazqBrrfJVIgLyNw4PGK6/EmdQxBEtqqgXppRv/ZVZzZPkwObEuECQQDenAam9eAuJYveHtAthkusutsVG5E3gJiXhRhoAqiSQC9mXLTgaWV7zJyA5zYPMvh6IviX/7H+Bqp14lT9wctFAkEA05ljSYShWTCFThtJxJ2d8zq6xCjBgETAdhiH85O/VrdKpwITV/6psByUKp42IdqMJwOaBgnnct8iDK/TAJLniQJABdo+RodyVGRCUB2pRXkhZjInbl+iKr5jxKAIKzveqLGtTViknL3IoD+Z4b2yayXg6H0g4gYj7NTKCH1h1KYSrQJBALbgbcg/YbeU0NF1kibk1ns9+ebJFpvGT9SBVRZ2TjsjBNkcWR2HEp8LxB6lSEGwActCOJ8Zdjh4kpQGbcWkMYkCQAXBTFiyyImO+sfCccVuDSsWS+9jrc5KadHGIvhfoRjIj2VuUKzJ+mXbmXuXnOYmsAefjnMCI6gGtaqkzl527tw=";

        /// <summary>
        /// 商户公钥
        /// </summary>
        public static string MerPubKey = "MIGfMA0GCSqGSIb3DQEBAQUAA4GNADCBiQKBgQCWOq5aHSTvdxGPDKZWSl6wrPpn" +
                        "MHW+8lOgVU71jB2vFGuA6dwa/RpJKnz9zmoGryZlgUmfHANnN0uztkgwb+5mpgme" +
                        "gBbNLuGqqHBpQHo2EsiAhgvgO3VRmWC8DARpzNxknsJTBhkUvZdy4GyrjnUrvsAR" +
                        "g4VrFzKDWL0Yu3gunQIDAQAB";

        /// <summary>
        /// 网关地址
        /// </summary>
        public static string GatewayURL = "https://api.dinpay.com/gateway/api/";

        public static string WxPubURL = "https://pay.dinpay.com//gateway?input_charset=UTF-8";
        public static string AliH5URL = "https://pay.dinpay.com/ali?input_charset=UTF-8";
    }
}
