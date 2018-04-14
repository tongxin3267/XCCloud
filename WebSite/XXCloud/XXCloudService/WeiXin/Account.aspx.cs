using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using XCCloudService.BLL.Container;
using XCCloudService.BLL.IBLL.XCGameManager;
using XCCloudService.Common;
using XCCloudService.Model.CustomModel.XCCloud;
using XCCloudService.WeiXin.Common;
using XCCloudService.WeiXin.WeixinOAuth;

namespace XXCloudService.WeiXin
{
    public partial class Account : System.Web.UI.Page
    {
        private bool getWxUser<T>(string openId, out T userInfo, out string errMsg)
        {
            userInfo = default(T);
            errMsg = string.Empty;

            string accsess_token = string.Empty;
            if (TokenMana.GetAccessToken(out accsess_token))
            {
                string url = "https://api.weixin.qq.com/cgi-bin/user/info?access_token={0}&openid={1}";
                url = string.Format(url, accsess_token, openId);
                string str = Utils.WebClientDownloadString(url);
                Dictionary<string, object> dict = new Dictionary<string, object>();
                if (WeiXinJsonHelper.GetResponseJsonResult(str, ref dict))
                {
                    userInfo = Utils.DataContractJsonDeserializer<T>(str);
                }
                else
                {
                    errMsg = "获取用户信息出错：" + dict["errcode"].ToString() + " " + dict["errmsg"].ToString();
                    return false;
                }

                return true;
            }
            else
            {
                errMsg = "获取微信令牌出错";
                return false;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                string errMsg = string.Empty;
                string md5 = Request["state"] ?? "";
                string url = Request.Url.GetLeftPart(UriPartial.Path);
                string code = Request["code"] ?? "";
                LogHelper.SaveLog("code:" + code);

                string accsess_token = string.Empty;
                string refresh_token = string.Empty;
                string openId = string.Empty;
                string token = string.Empty;
                if (TokenMana.GetTokenForScanQR(code, out accsess_token, out refresh_token, out openId, out errMsg))
                {
                    UserInfoModel userInfo = new UserInfoModel();
                    if (getWxUser<UserInfoModel>(openId, out userInfo, out errMsg))
                    {
                        //验证用户                    
                        LogHelper.SaveLog("openId:" + openId);
                        LogHelper.SaveLog("userInfo:" + userInfo.NickName);
                        Response.Redirect(WeiXinConfig.RedirectAccountPage + "?openId=" + openId + "&nickName=" + userInfo.NickName, false);
                    }
                    else
                    {
                        LogHelper.SaveLog("错误: 获取用户基本信息失败" + errMsg);
                        Response.Redirect(WeiXinConfig.RedirectErrorPage + "?title=" + HttpUtility.UrlEncode("绑定失败") + "&message=" + HttpUtility.UrlEncode(errMsg), false);
                    }                    
                }
                else
                {
                    LogHelper.SaveLog("错误: 获取openid失败" + errMsg);
                    Response.Redirect(WeiXinConfig.RedirectErrorPage + "?title=" + HttpUtility.UrlEncode("绑定失败") + "&message=" + HttpUtility.UrlEncode(errMsg), false);
                }
            }
            catch (Exception ex)
            {
                LogHelper.SaveLog("错误:" + ex.Message);
                Response.Redirect(WeiXinConfig.RedirectErrorPage + "?title=" + HttpUtility.UrlEncode("绑定失败") + "&message=" + HttpUtility.UrlEncode(ex.Message), false);
            }
        }
    }
}