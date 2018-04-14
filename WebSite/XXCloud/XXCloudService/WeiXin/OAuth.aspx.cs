using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using XCCloudService.Common;
using XCCloudService.WeiXin.Common;
using XCCloudService.WeiXin.WeixinOAuth;
using XCCloudService.WeiXin.Message;
using XCCloudService.Pay.PPosPay;
using XCCloudService.Common.Enum;
using XCCloudService.Business.Common;

namespace XXCloudService.WeiXin
{
    public partial class OAuth : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
         {
            try
            {
                string code = Request["code"] ?? "";
                string state = Request["state"] ?? "";

                switch (state)
                {
                    case "h5_Auth_Common": H5AuthCommon(code); break;
                    default: break;
                }
            }
            catch (Exception ex)
            {
                LogHelper.SaveLog(TxtLogType.WeiXin, TxtLogContentType.Exception, TxtLogFileType.Day, "errMsg:" + ex.Message);
            }
        }

        /// <summary>
        /// H5页面微信授权
        /// </summary>
        private void H5AuthCommon(string code)
        {
            string accsess_token = string.Empty;
            string refresh_token = string.Empty;
            string openId = string.Empty;
            string errMsg = string.Empty;
            if (TokenMana.GetTokenForScanQR(code, out accsess_token, out refresh_token, out openId, out errMsg))
            {
                string urls = "https://api.weixin.qq.com/cgi-bin/token?grant_type=client_credential&appid={0}&secret={1}";
                urls = string.Format(urls, WeiXinConfig.AppId, WeiXinConfig.AppSecret);
                string list = Utils.WebClientDownloadString(urls);
                LogHelper.SaveLog("accsess_token:" + accsess_token);
                LogHelper.SaveLog("refresh_token:" + refresh_token);
                LogHelper.SaveLog("openId:" + openId);
                string redirectUrl = string.Format("http://mp.4000051530.com/WeiXin/Test.aspx?openId={0}",openId);
                Response.Redirect(redirectUrl);
            }
            else
            {
                LogHelper.SaveLog(TxtLogType.WeiXin, TxtLogContentType.Common, TxtLogFileType.Day, "errMsg:" + errMsg);
                //重定向的错误页面                 
                Response.Redirect(WeiXinConfig.RedirectErrorPage);
            }
        }
    }
}