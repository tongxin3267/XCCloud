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
using XCCloudService.BLL.IBLL.XCGameManager;
using XCCloudService.BLL.Container;
using XCCloudService.Model.XCGameManager;

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
            try
            {
                if (TokenMana.GetTokenForScanQR(code, out accsess_token, out refresh_token, out openId, out errMsg))
                {
                    bool isReg = false;
                    IMobileTokenService mobileTokenService = BLLContainer.Resolve<IMobileTokenService>();
                    var mobileTokenModel = mobileTokenService.GetModels(p => p.OpenId.Equals(openId)).FirstOrDefault<t_MobileToken>();
                    if (mobileTokenModel != null)
                    {
                        IMemberTokenService memberTokenService = BLLContainer.Resolve<IMemberTokenService>();
                        var memberTokenModel = mobileTokenService.GetModels(p => p.OpenId.Equals(openId)).FirstOrDefault<t_MobileToken>();
                        if (memberTokenModel != null)
                        {
                            isReg = true;
                        }
                    }
                    string redirectUrl = string.Format("{0}?openId={1}&isReg={2}", CommonConfig.H5WeiXinAuthRedirectUrl, openId,Convert.ToInt32(isReg));
                    Response.Redirect(redirectUrl);
                }
                else
                {
                    LogHelper.SaveLog(TxtLogType.WeiXin, TxtLogContentType.Common, TxtLogFileType.Day, "errMsg:" + errMsg);
                    //重定向的错误页面                 
                    Response.Redirect(WeiXinConfig.RedirectErrorPage);
                }
            }
            catch(Exception e)
            {
                LogHelper.SaveLog(TxtLogType.WeiXin, TxtLogContentType.Exception, TxtLogFileType.Day, "Exception:" + e.Message);
            }

        }
    }
}