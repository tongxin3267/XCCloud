using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XCCloudService.Business.WeiXin;
using XCCloudService.Common;
using XCCloudService.Common.Enum;
using XCCloudService.Model.WeiXin.Session;
using XCCloudService.WeiXin.Common;

namespace XCCloudService.WeiXin.WeixinOAuth
{
    public class WeiXinUserMana
    {
        public static bool GetWeiXinSAppUserSession(string code, ref WeiXinSAppSessionModel sessionModel, out string serverSessionKey, out string errMsg)
        {
            sessionModel = null;
            errMsg = string.Empty;
            serverSessionKey = string.Empty;
            string url = string.Format("https://api.weixin.qq.com/sns/jscode2session?appid={0}&secret={1}&js_code={2}&grant_type=authorization_code", WeiXinConfig.WXSmallAppId, WeiXinConfig.WXSmallAppSecret, code);
            string str = Utils.WebClientDownloadString(url);
            Dictionary<string, object> dict = new Dictionary<string, object>();
            LogHelper.SaveLog(TxtLogType.WeiXin, TxtLogContentType.Common, TxtLogFileType.Day, str);
            if (WeiXinJsonHelper.GetResponseJsonResult(str, ref dict))
            {
                sessionModel = new WeiXinSAppSessionModel();
                sessionModel.OpenId = dict["openid"].ToString();
                sessionModel.SessionKey = dict["session_key"].ToString();
                sessionModel.UnionId = dict.ContainsKey("unionid") ? dict["unionid"].ToString():"";
                if (WeiXinSAppSessionBussiness.Add(sessionModel, WeiXinConfig.WXSmallAppSessionTime, out serverSessionKey))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                errMsg = dict["errmsg"].ToString();
                return false;
            }
        }



    }
}
