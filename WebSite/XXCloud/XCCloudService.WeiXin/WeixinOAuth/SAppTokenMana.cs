using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XCCloudService.Business.WeiXin;
using XCCloudService.Common;
using XCCloudService.WeiXin.Common;

namespace XCCloudService.WeiXin.WeixinOAuth
{
    public class SAppTokenMana
    {
        public static bool GetAccessToken(out string accessToken,out string errMsg)
        {
            accessToken = string.Empty;
            errMsg = string.Empty;
            if (SAppAccessTokenBusiness.GetAccessToken(out accessToken))
            {
                //如果缓存中存在访问access_token,直接返回缓存的access_token
                return true;
            }
            else
            {
                //如果缓存中不存在访问access_token，调用微信接口获取，并写入缓存
                string url = string.Format("https://api.weixin.qq.com/cgi-bin/token?grant_type=client_credential&appid={0}&secret={1}", WeiXinConfig.WXSmallAppId, WeiXinConfig.WXSmallAppSecret);
                string json = Utils.WebClientDownloadString(url);
                Dictionary<string, object> dict = new Dictionary<string, object>();
                if (WeiXinJsonHelper.GetResponseJsonResult(json, ref dict))
                {
                    accessToken = dict["access_token"].ToString();
                    int expires = int.Parse(dict["expires_in"].ToString());
                    SAppAccessTokenBusiness.AddAccessToken(accessToken, expires - 60);
                    return true;
                }
                else
                {
                    errMsg = dict["errmsg"].ToString();
                    return false;
                }
            }
        }
    }
}
