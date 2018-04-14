using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using XCCloudService.Business.Common;
using XCCloudService.Common;
using XCCloudService.WeiXin.Common;
using XCCloudService.WeiXin.WeixinOAuth;

namespace XXCloudService.Boss
{
    public partial class Login : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                string errMsg = string.Empty;
                string md5 = Request["state"] ?? "";
                string url = Request.Url.GetLeftPart(UriPartial.Path);
                string code = Request["code"] ?? "";
                LogHelper.SaveLog("code:" + code);

                //if (!TokenMana.GetTokenMd5(url, md5))
                //{
                //    errMsg = url + WeiXinConfig.Md5key;
                //    LogHelper.SaveLog("错误:" + errMsg);
                //    Response.Redirect(WeiXinConfig.RedirectErrorPage + "?title=" + HttpUtility.UrlEncode("登录失败") + "&message=" + HttpUtility.UrlEncode(errMsg), false);
                //    return;
                //}       

                string accsess_token = string.Empty;
                string refresh_token = string.Empty;
                string openId = string.Empty;
                string unionId = string.Empty;
                string token = string.Empty;
                if (TokenMana.GetOpenTokenForScanQR(code, out accsess_token, out refresh_token, out openId, out unionId))
                {
                    if (string.IsNullOrEmpty(unionId))
                    {
                        if (!TokenMana.GetUnionIdFromOpen(openId, accsess_token, out unionId, out errMsg))
                        {
                            Response.Redirect(WeiXinConfig.RedirectErrorPage + "?title=" + HttpUtility.UrlEncode("登录失败") + "&message=" + HttpUtility.UrlEncode(errMsg), false);
                            return;
                        }
                    }

                    //验证用户                    
                    LogHelper.SaveLog("unionId:" + unionId);
                    if (WeiXinConfig.BossUnionId.Contains(unionId))
                    {
                        token = UnionIdTokenBusiness.SetUnionIdToken(unionId);
                        Response.Redirect(WeiXinConfig.RedirectBossPage + "?token=" + token, false);
                    }                    
                }
                else
                {
                    errMsg = "获取openId失败";
                    LogHelper.SaveLog("错误:" + errMsg);
                    Response.Redirect(WeiXinConfig.RedirectLogoutPage, false);
                }
            }
            catch (Exception ex)
            {
                LogHelper.SaveLog("错误:" + ex.Message);
                Response.Redirect(WeiXinConfig.RedirectErrorPage + "?title=" + HttpUtility.UrlEncode("登录失败") + "&message=" + HttpUtility.UrlEncode(ex.Message), false);
            }
        }
    }
}