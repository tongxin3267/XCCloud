using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XCCloudService.Common
{
    public class CommonConfig
    {
        /// <summary>
        /// 短信用户名
        /// </summary>
        public static string SMSName = System.Configuration.ConfigurationManager.AppSettings["SmsName"] ?? "";
        /// <summary>
        /// 短信用户密码
        /// </summary>
        public static string SMSPassWord = System.Configuration.ConfigurationManager.AppSettings["SmsPassWord"] ?? "";
        /// <summary>
        /// 文本日志路径
        /// </summary>
        public static string TxtLogPath = System.Configuration.ConfigurationManager.AppSettings["TxtLogPath"] ?? "";
        /// <summary>
        /// 系统初始化日志
        /// </summary>
        public static string SystemInitLog = "Init/";

        public static int IconOutLockPerSecond = int.Parse(System.Configuration.ConfigurationManager.AppSettings["IconOutLockPerSecond"] ?? "10");
        /// <summary>
        /// 查询订单数量
        /// </summary>
        public static string DataOrderPageSize = System.Configuration.ConfigurationManager.AppSettings["DataOrderPageSize"] ?? "20";

        
        public static string DesKey = System.Configuration.ConfigurationManager.AppSettings["DesKey"] ?? "";

        /// <summary>
        /// 缓存key前缀
        /// </summary>
        public static string PrefixKey = "RS232";


        public static string SAppMessagePushXmlFilePath = System.Web.HttpContext.Current.Server.MapPath("/Config/SAppMessageTemplate.xml");

        /// <summary>
        /// 
        /// </summary>
        public static int RadarOffLineTimeLong = int.Parse(System.Configuration.ConfigurationManager.AppSettings["radarOffLineTimeLong"] ?? "60");
    }
}
