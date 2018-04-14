using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using XCCloudService.Common;
using XCCloudService.WeiXin.Common;

namespace XCCloudService.WeiXin.WeixinOAuth
{
    public class WeiXinQR
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static string GetRQImageBySnsapi_Login()
        {
            
            string oauthPageUrl = WeiXinConfig.WeiXinHttpHostUrl + "/WeiXin/OAuth.aspx";
            string md5key = oauthPageUrl + WeiXinConfig.Md5key;
            string md5 = Utils.MD5(md5key);
            string url = string.Format("https://open.weixin.qq.com/connect/oauth2/authorize?appid={0}&redirect_uri={1}&response_type=code&scope=snsapi_userinfo&state={2}#wechat_redirect ",
                WeiXinConfig.AppId,HttpUtility.UrlEncode(oauthPageUrl), md5);
            return url;
        }

        public static string GetRQImageBySnsapi_Base(string state)
        {
            string oauthPageUrl = WeiXinConfig.WeiXinHttpHostUrl + "/WeiXin/OAuth.aspx";
            string url = string.Format("https://open.weixin.qq.com/connect/oauth2/authorize?appid={0}&redirect_uri={1}&response_type=code&scope=snsapi_base&state={2}#wechat_redirect ",
                WeiXinConfig.AppId, HttpUtility.UrlEncode(oauthPageUrl), state);
            return url;
        }

        public static string GetRQImageByMerch_Login()
        {

            string oauthPageUrl = WeiXinConfig.CloudHttpHostUrl + "/WeiXin/Login.aspx";
            string md5key = oauthPageUrl + WeiXinConfig.Md5key;
            string md5 = Utils.MD5(md5key);
            string url = string.Format("https://open.weixin.qq.com/connect/qrconnect?appid={0}&redirect_uri={1}&response_type=code&scope=snsapi_login&state={2}#wechat_redirect ",
                WeiXinConfig.OpenAppId, HttpUtility.UrlEncode(oauthPageUrl), md5);
            return url;
        }
    }
}
