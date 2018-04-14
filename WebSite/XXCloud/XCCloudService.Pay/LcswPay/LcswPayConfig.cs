using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XCCloudService.Pay.LcswPay
{
    /// <summary>
    /// 扫呗系统参数配置
    /// </summary>
    public class LcswPayConfig
    {
        public static string SSLCERT_PATH = "证书文件名";
        public static string SSLCERT_PASSWORD = "证书密码";

        //public static string StoreID = "852100210000005";
        //public static string DeviceID = "30050895";
        //public static string Token = "a94d34f12d474ba48d51b71e2f0e5be7";

        public static string StoreID = "852100208000715";
        public static string DeviceID = "10228422";
        public static string Token = "1cb95e74b1d94f449c9757706690f7e1";
        /// <summary>
        /// 机构代码
        /// </summary>
        public static string inst_no = "52100014";
        public static string inst_token = "63ccb4b5cb47492b94999ef373e2a5d6";
        //public static string inst_token = "1cb95e74b1d94f449c9757706690f7e1";

        public static string NotifyURL = "http://123.56.111.145:789/LCSWPayCallBack.aspx";

        public static string GatewayURL = "http://test.lcsw.cn:8045/lcsw/";
        //public static string GatewayURL = "http://pay.lcsw.cn/lcsw/";


        public static string Ver = "100";
    }
}
