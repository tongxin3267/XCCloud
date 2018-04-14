using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XCCloudService.WeiXin.Common
{
    public class WeiXinConfig
    {
        //微信Host
        public static string WeiXinHost = System.Configuration.ConfigurationManager.AppSettings["WeiXinHost"].ToString();

        //微信http协议HostUrl
        public static string WeiXinHttpHostUrl = "http://" + System.Configuration.ConfigurationManager.AppSettings["WeiXinHost"].ToString();

        //微信https协议HostUrl
        public static string WeiXinHttpsHostUrl = "https://" + System.Configuration.ConfigurationManager.AppSettings["WeiXinHost"].ToString();
        //微信开发者ID
        public static string AppId = System.Configuration.ConfigurationManager.AppSettings["AppId"].ToString();
        //开发者密码
        public static string AppSecret = System.Configuration.ConfigurationManager.AppSettings["AppSecret"].ToString();

        public static string MCHID = System.Configuration.ConfigurationManager.AppSettings["MCHID"].ToString();

        public static string KEY = System.Configuration.ConfigurationManager.AppSettings["KEY"].ToString();

        /// <summary>
        /// Md5key
        /// </summary>
        public static string Md5key = System.Configuration.ConfigurationManager.AppSettings["Md5Key"].ToString();
        //重定向
        public static string RedirectErrorPage = System.Configuration.ConfigurationManager.AppSettings["RedirectErrorPage"].ToString();

        public static string RedirectSuccessPage = System.Configuration.ConfigurationManager.AppSettings["RedirectSuccessPage"].ToString();

        public static string RedirectMainPage = System.Configuration.ConfigurationManager.AppSettings["RedirectMainPage"].ToString();

        public static string RedirectLogoutPage = System.Configuration.ConfigurationManager.AppSettings["RedirectLogoutPage"].ToString();

        public static string RedirectAccountPage = System.Configuration.ConfigurationManager.AppSettings["RedirectAccountPage"].ToString();

        public static string WXSmallAppId = System.Configuration.ConfigurationManager.AppSettings["WXSmallAppId"].ToString();

        public static string WXSmallAppSecret = System.Configuration.ConfigurationManager.AppSettings["WXSmallAppSecret"].ToString();

        public static int WXSmallAppSessionTime = int.Parse(System.Configuration.ConfigurationManager.AppSettings["WXSmallAppSessionTime"].ToString() ?? "600");

        public static string WXPayCallBackUrl = System.Configuration.ConfigurationManager.AppSettings["WXPayCallBackUrl"].ToString();

        public static string OpenAppId = System.Configuration.ConfigurationManager.AppSettings["OpenAppId"].ToString();

        public static string OpenAppSecret = System.Configuration.ConfigurationManager.AppSettings["OpenAppSecret"].ToString();

        public static string BossUnionId = System.Configuration.ConfigurationManager.AppSettings["BossUnionId"].ToString();

        public static string RedirectBossPage = System.Configuration.ConfigurationManager.AppSettings["RedirectBossPage"].ToString();

        //云平台http协议HostUrl
        public static string CloudHttpHostUrl = "http://" + System.Configuration.ConfigurationManager.AppSettings["CloudHost"].ToString();

        //云平台https协议HostUrl
        public static string CloudHttpsHostUrl = "https://" + System.Configuration.ConfigurationManager.AppSettings["CloudHost"].ToString();
    }
}
