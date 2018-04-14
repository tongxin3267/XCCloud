using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XCCloudService.CacheService
{
    public class CacheType
    {
        public static string SMSTempToken = "SMSTempToken";

        public static string SMSCode = "SMSCode";

        public static string ImgCode = "ImgCode";

        public static string DeviceState = "DeviceState";

        public static string MobileToken = "MobileToken";

        public static string TCPAnswerOrder = "TCPAnswerOrder";

        public static string MemberCardQuery = "MemberCardQuery";

        public static string OrderPayCache = "OrderPayCache";

        /// <summary>
        /// 出币锁定缓存
        /// </summary>
        public static string IconOutLock = "IconOutLock";

        public static string WeiXinSAppSession = "WeiXinSAppSession";

        public static string ResetPassword = "ResetPassword";

        public static string UDPSocketStoreQueryAnswerBusiness = "UDPSocketStoreQueryAnswerBusiness";
    }
}