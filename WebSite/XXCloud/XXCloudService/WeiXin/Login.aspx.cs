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
using XCCloudService.BLL.IBLL.XCCloud;
using XCCloudService.BLL.Container;
using XCCloudService.Business.XCCloud;
using XCCloudService.Model.XCCloud;
using XCCloudService.Model.CustomModel.XCCloud;
using XCCloudService.Business.XCGameMana;
using XCCloudService.Business.Common;

namespace XXCloudService.WeiXin
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
                    IBase_UserInfoService userInfoService = BLLContainer.Resolve<IBase_UserInfoService>();
                    if (userInfoService.Any(w => w.UnionID.ToString().Equals(unionId, StringComparison.OrdinalIgnoreCase)))
                    {
                        //进入主页面   
                        var base_UserInfoModel = userInfoService.GetModels(w => w.UnionID.ToString().Equals(unionId, StringComparison.OrdinalIgnoreCase)).FirstOrDefault<Base_UserInfo>();                        
                        var dataModel = new UserDataModel();
                        int logType = (int)RoleType.XcUser; //普通员工
                        if (base_UserInfoModel.UserType == (int)UserType.Xc && (!base_UserInfoModel.Auditor.HasValue || base_UserInfoModel.Auditor == 0))
                        {
                            logType = (int)RoleType.XcAdmin;
                        }
                        else if (base_UserInfoModel.UserType == (int)UserType.Store || base_UserInfoModel.UserType == (int)UserType.StoreBoss)
                        {
                            var storeId = base_UserInfoModel.StoreID;
                            IBase_StoreInfoService base_StoreInfoService = BLLContainer.Resolve<IBase_StoreInfoService>();
                            if (!base_StoreInfoService.Any(a => a.StoreID.Equals(storeId, StringComparison.OrdinalIgnoreCase)))
                            {
                                errMsg = "该门店编号不存在";
                                Response.Redirect(WeiXinConfig.RedirectErrorPage + "?title=" + HttpUtility.UrlEncode("登录失败") + "&message=" + HttpUtility.UrlEncode(errMsg), false);
                            }
                            string merchId = base_StoreInfoService.GetModels(p => p.StoreID.Equals(storeId, StringComparison.OrdinalIgnoreCase)).FirstOrDefault().MerchID;
                            dataModel.StoreID = storeId;
                            dataModel.MerchID = merchId;
                            logType = (int)RoleType.StoreUser;
                        }

                        token = XCCloudUserTokenBusiness.SetUserToken(base_UserInfoModel.UserID.ToString(), logType, dataModel);
                        Response.Redirect(WeiXinConfig.RedirectMainPage + "?token=" + token + "&logType=" + logType + "&userType=" + base_UserInfoModel.UserType, false);
                    }
                    else
                    {
                        IBase_MerchantInfoService base_MerchantInfoService = BLLContainer.Resolve<IBase_MerchantInfoService>();
                        if (base_MerchantInfoService.Any(w => w.WxUnionID.ToString().Equals(unionId, StringComparison.OrdinalIgnoreCase)))
                        {
                            //进入主页面
                            var base_MerchantInfoModel = base_MerchantInfoService.GetModels(w => w.WxUnionID.ToString().Equals(unionId, StringComparison.OrdinalIgnoreCase)).FirstOrDefault<Base_MerchantInfo>();
                            var dataModel = new MerchDataModel { MerchType = base_MerchantInfoModel.MerchType, CreateType = base_MerchantInfoModel.CreateType, CreateUserID = base_MerchantInfoModel.CreateUserID };
                            token = XCCloudUserTokenBusiness.SetUserToken(base_MerchantInfoModel.MerchID, (int)RoleType.MerchUser, dataModel);
                            var logType = (int)RoleType.MerchUser;
                            Response.Redirect(WeiXinConfig.RedirectMainPage + "?token=" + token + "&logType=" + logType + "&merchTag=" + base_MerchantInfoModel.MerchTag + "&userType=" + base_MerchantInfoModel.MerchType, false);
                        }
                        else
                        {
                            errMsg = "用户未注册";
                            LogHelper.SaveLog("失败:" + errMsg);
                            Response.Redirect(WeiXinConfig.RedirectErrorPage + "?title=" + HttpUtility.UrlEncode("登录失败") + "&message=" + HttpUtility.UrlEncode(errMsg), false);
                        }
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