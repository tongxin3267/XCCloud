using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using XCCloudService.Business.WeiXin;
using XCCloudService.Common;
using XCCloudService.WeiXin.Common;
using System.Web;
using XCCloudService.Business.XCCloud;

namespace XCCloudService.WeiXin.WeixinOAuth
{
    public class TokenMana
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="code"></param>
        /// <param name="accsess_token"></param>
        /// <param name="refresh_token"></param>
        /// <param name="openId"></param>
        /// <returns></returns>
        public static bool GetTokenForScanQR(string code,out string accsess_token,out string refresh_token,out string openId)
        {
            accsess_token = string.Empty;
            refresh_token = string.Empty;
            openId = string.Empty;

            string url = "https://api.weixin.qq.com/sns/oauth2/access_token?appid={0}&secret={1}&code={2}&grant_type=authorization_code";
            url = string.Format(url, WeiXinConfig.AppId, WeiXinConfig.AppSecret, code);
            string str = Utils.WebClientDownloadString(url);
            Dictionary<string, object> dict = new Dictionary<string, object>();
            if (WeiXinJsonHelper.GetResponseJsonResult(str, ref dict))
            {
                accsess_token = dict["access_token"].ToString();
                refresh_token = dict["refresh_token"].ToString();
                openId = dict["openid"].ToString();
            
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool GetTokenForScanQR(string code, out string accsess_token, out string refresh_token, out string openId,out string errMsg)
        {
            accsess_token = string.Empty;
            refresh_token = string.Empty;
            openId = string.Empty;
            errMsg = string.Empty;
            string url = "https://api.weixin.qq.com/sns/oauth2/access_token?appid={0}&secret={1}&code={2}&grant_type=authorization_code";
            url = string.Format(url, WeiXinConfig.AppId, WeiXinConfig.AppSecret, code);
            string str = Utils.WebClientDownloadString(url);
            Dictionary<string, object> dict = new Dictionary<string, object>();
            if (WeiXinJsonHelper.GetResponseJsonResult(str, ref dict))
            {
                accsess_token = dict["access_token"].ToString();
                refresh_token = dict["refresh_token"].ToString();
                openId = dict["openid"].ToString();

                return true;
            }
            else
            {
                errMsg = dict["errmsg"].ToString();
                return false;
            }
        }

        public static bool GetOpenTokenForScanQR(string code, out string accsess_token, out string refresh_token, out string openId, out string unionId)
        {
            accsess_token = string.Empty;
            refresh_token = string.Empty;
            openId = string.Empty;
            unionId = string.Empty;

            string url = "https://api.weixin.qq.com/sns/oauth2/access_token?appid={0}&secret={1}&code={2}&grant_type=authorization_code";
            url = string.Format(url, WeiXinConfig.OpenAppId, WeiXinConfig.OpenAppSecret, code);
            string str = Utils.WebClientDownloadString(url);
            Dictionary<string, object> dict = new Dictionary<string, object>();
            if (WeiXinJsonHelper.GetResponseJsonResult(str, ref dict))
            {
                accsess_token = dict["access_token"].ToString();
                refresh_token = dict["refresh_token"].ToString();
                openId = dict["openid"].ToString();
                //unionId当且仅当该网站应用已获得该用户的userinfo授权时，才会出现该字段。
                unionId = dict.ContainsKey("unionid") ? dict["unionid"].ToString() : string.Empty;
                return true;
            }
            else
            {
                return false;
            }
        } 

        public static bool GetTokenMd5(string url,string md5)
        {
            string Md5Url = Utils.MD5(url+ WeiXinConfig.Md5key);
            if (Md5Url != md5)
            {
                return false;
            }
            return true;
        }
        
        /// <summary>
        /// 获取微信访问token
        /// </summary>
        /// <returns></returns>
        public static bool GetAccessToken(out string accessToken)
        {
            accessToken = string.Empty;
            if (WeiXinAccessTokenBusiness.GetAccessToken(out accessToken) && IsValid(accessToken))
            {
                //如果缓存中存在访问access_token,直接返回缓存的access_token
                return true;
            }
            else
            {
                //如果缓存中不存在访问access_token，调用微信接口获取，并写入缓存
                string url = string.Format("https://api.weixin.qq.com/cgi-bin/token?grant_type=client_credential&appid={0}&secret={1}", WeiXinConfig.AppId, WeiXinConfig.AppSecret);
                string json = Utils.WebClientDownloadString(url);
                Dictionary<string,object> dict =new Dictionary<string,object>();
                if (WeiXinJsonHelper.GetResponseJsonResult(json, ref dict))
                {
                    accessToken = dict["access_token"].ToString();
                    int expires = int.Parse(dict["expires_in"].ToString());
                    WeiXinAccessTokenBusiness.AddAccessToken(accessToken, expires - 60);
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// 获取微信访问openid
        /// </summary>
        /// <returns></returns>
        public static bool GetOpenId(string code, string key, out string openId)
        {
            openId = string.Empty;
            if (RegisterOpenIDBusiness.GetOpenId(Constant.WeiXinOpenId + key, out openId))
            {
                //如果缓存中存在访问openId,直接返回缓存的openId
                return true;
            }
            else
            {
                //如果缓存中不存在访问openId，调用微信接口获取，并写入缓存
                string url = string.Format("https://api.weixin.qq.com/sns/oauth2/access_token?appid={0}&secret={1}&code={2}&grant_type=authorization_code", WeiXinConfig.AppId, WeiXinConfig.AppSecret, code);
                string json = Utils.WebClientDownloadString(url);
                Dictionary<string, object> dict = new Dictionary<string, object>();

                if (WeiXinJsonHelper.GetResponseJsonResult(json, ref dict))
                {
                    openId = dict["openid"].ToString();
                    int expires = int.Parse(dict["expires_in"].ToString());
                    RegisterOpenIDBusiness.AddOpenId(Constant.WeiXinOpenId + key, openId, expires - 60);
                    return true;
                }
                else
                {
                    return false;
                }
            }            
        }

        public static bool IsValid(string access_token)
        {
            string url = "https://api.weixin.qq.com/cgi-bin/getcallbackip?access_token={0}";
            url = string.Format(url, access_token);
            string list = Utils.WebClientDownloadString(url);
            Dictionary<string, object> dict = new Dictionary<string, object>();
            //LogHelper.SaveLog("access_token：" + access_token);            
            if (WeiXinJsonHelper.GetResponseJsonResult(list, ref dict))
            {
                //LogHelper.SaveLog("ip_list.length：" + ((object[])dict["ip_list"]).Length);
                return true;
            }
            else
            {
                LogHelper.SaveLog("errcode：" + dict["errcode"].ToString() + " " + dict["errmsg"].ToString());
                return false;
            }
        }

        public static bool GetUnionId(string openId, out string unionId, out string errMsg)
        {
            unionId = string.Empty;
            errMsg = string.Empty;
            if (XCCloudUnionIDBusiness.GetUnionID(openId, out unionId))
            {
                //如果缓存中存在访问unionId,直接返回缓存的unionId
                return true;
            }
            else
            {
                //获取用户基本信息
                string access_token = string.Empty;                
                if (TokenMana.GetAccessToken(out access_token))
                {
                    string url = "https://api.weixin.qq.com/cgi-bin/user/info?access_token={0}&openid={1}";
                    url = string.Format(url, access_token, openId);
                    string list = Utils.WebClientDownloadString(url);
                    Dictionary<string, object> dict = new Dictionary<string, object>();
                    if (WeiXinJsonHelper.GetResponseJsonResult(list, ref dict))
                    {
                        if (dict.ContainsKey("errcode"))
                        {
                            errMsg = dict["errmsg"].ToString();
                            return false;
                        }

                        if (dict["subscribe"].ToString().Equals("0"))
                        {
                            errMsg = "该用户未关注微信公众号";
                            return false;
                        }

                        unionId = dict["unionid"].ToString();
                        XCCloudUnionIDBusiness.SetUnionID(openId, unionId);
                        return true;
                    }
                    else
                    {
                        errMsg = "获取用户基本信息出错";
                        return false;
                    }
                }
                else
                {
                    errMsg = "获取微信令牌出错";
                    return false;
                }
            }            
        }

        public static bool GetUnionIdFromOpen(string openId, string accsess_token, out string unionId, out string errMsg)
        {
            unionId = string.Empty;
            errMsg = string.Empty;
            if (XCCloudUnionIDBusiness.GetUnionID(openId, out unionId))
            {
                //如果缓存中存在访问unionId,直接返回缓存的unionId
                return true;
            }
            else
            {
                //获取用户基本信息
                string url = "https://api.weixin.qq.com/sns/userinfo?access_token={0}&openid={1}";
                url = string.Format(url, accsess_token, openId);
                string list = Utils.WebClientDownloadString(url);
                Dictionary<string, object> dict = new Dictionary<string, object>();
                if (WeiXinJsonHelper.GetResponseJsonResult(list, ref dict))
                {
                    if (dict.ContainsKey("errcode"))
                    {
                        errMsg = dict["errmsg"].ToString();
                        return false;
                    }

                    unionId = dict["unionid"].ToString();
                    return true;
                }
                else
                {
                    errMsg = "获取用户基本信息出错";
                    return false;
                }
            }
        }
    }
}
