using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XCCloudService.Pay.PPosPay
{
    public class PPosPayConfig
    {
        public const string Version = "V1.0.0";
        public const string SignType = "MD5";
        public const string Character = "00";
        public const string SSLCERT_PATH = "";
        public const string SSLCERT_PASSWORD = "";

        /// <summary>
        /// 商户号
        /// </summary>
        public static string MerchNo = "800521000000141";
        /// <summary>
        /// 终端号
        /// </summary>
        public static string TerminalNo = "95014865";
        /// <summary>
        /// 令牌
        /// </summary>
        public static string Token = "B6AF5CA3A37AD60EDA90F80EB3A0B5BE";
        /// <summary>
        /// 机构号
        /// </summary>
        public static string InstNo = "1726";
        /// <summary>
        /// 请求网关
        /// </summary>
        public static string GatewayURL = "http://gateway.starpos.com.cn/adpweb/ehpspos3/";

        //public static string GatewayURL = "http://139.196.77.69:8280/adpweb/ehpspos3/";

        //H5支付网关地址
        public static string Gateway_Mobile = "https://gateway.starpos.com.cn/sysmng/bhpspos4/5533020.do";
    }

    public enum OSType
    {
        ANDROID = 0,
        IOS = 1,
        WINDOWS = 2,
        DIRECT = 3,
    }
}
