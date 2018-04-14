using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using XCCloudService.BLL.CommonBLL;
using XCCloudService.BLL.Container;
using XCCloudService.BLL.IBLL.XCCloud;
using XCCloudService.CacheService;
using XCCloudService.Common;
using XCCloudService.Common.Enum;
using XCCloudService.Model.WeiXin.Message;
using XCCloudService.Model.XCCloud;
using XCCloudService.WeiXin.Common;
using XCCloudService.WeiXin.Message;
using XCCloudService.WeiXin.WeixinOAuth;

namespace XXCloudService.WeiXin
{
    public partial class Register : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                string openId = string.Empty;
                string errMsg = string.Empty;
                string storeId = string.Empty;
                string merchId = string.Empty;
                string revOpenId = string.Empty;
                int userType = (int)UserType.Store;
                string code = Request["code"] != null ? Request["code"].ToString() : string.Empty;
                string state = Request["state"] != null ? Request["state"].ToString() : string.Empty;
                string mobile = Request["mobile"] != null ? Request["mobile"].ToString() : string.Empty;
                string username = Request["username"] != null ? Request["username"].ToString() : string.Empty;
                string realname = Request["realname"] != null ? Request["realname"].ToString() : string.Empty;
                string password = Request["password"] != null ? Request["password"].ToString() : string.Empty;
                string message = Request["message"] != null ? Request["message"].ToString() : string.Empty;

                //获取用户openid
                if (!TokenMana.GetOpenId(code, state, out openId))
                {
                    errMsg = "获取openId失败";
                    LogHelper.SaveLog("错误:" + errMsg);
                    Response.Redirect(WeiXinConfig.RedirectErrorPage + "?title=" + HttpUtility.UrlEncode("注册失败") + "&message=" + HttpUtility.UrlEncode(errMsg), false);
                    return;
                }

                LogHelper.SaveLog("openId:" + openId);

                //获取用户基本信息
                string unionId = string.Empty;
                if (!TokenMana.GetUnionId(openId, out unionId, out errMsg))
                {
                    LogHelper.SaveLog("错误:" + errMsg);
                    Response.Redirect(WeiXinConfig.RedirectErrorPage + "?title=" + HttpUtility.UrlEncode("注册失败") + "&message=" + HttpUtility.UrlEncode(errMsg), false);
                    return;
                }                       

                if (!checkRegisterParas(openId, out storeId, out merchId, out userType, out revOpenId, out errMsg))
                {
                    Response.Redirect(WeiXinConfig.RedirectErrorPage + "?title=" + HttpUtility.UrlEncode("注册失败") + "&message=" + HttpUtility.UrlEncode(errMsg), false);
                    return;
                }                

                LogHelper.SaveLog("unionId:" + unionId);
                LogHelper.SaveLog("storeId:" + storeId);              

                //注册成功后，给商户管理员发送审核消息模板
                string sql = " exec SP_RegisterUserFromWx @StoreId,@MerchId,@UserType,@Mobile,@Username,@Realname,@UserPassword,@Message,@WXOpenID,@UnionID,@WorkID output,@Return output ";
                SqlParameter[] parameters = new SqlParameter[12];
                parameters[0] = new SqlParameter("@StoreId", storeId);
                parameters[1] = new SqlParameter("@MerchId", merchId);
                parameters[2] = new SqlParameter("@UserType", userType);
                parameters[3] = new SqlParameter("@Mobile", mobile);
                parameters[4] = new SqlParameter("@Username", username);
                parameters[5] = new SqlParameter("@Realname", realname);
                parameters[6] = new SqlParameter("@UserPassword", Utils.MD5(password));
                parameters[7] = new SqlParameter("@Message", message);
                parameters[8] = new SqlParameter("@WXOpenID", openId);
                parameters[9] = new SqlParameter("@UnionID", unionId);
                parameters[10] = new SqlParameter("@WorkID", 0);
                parameters[10].Direction = ParameterDirection.Output;
                parameters[11] = new SqlParameter("@Return", 0);
                parameters[11].Direction = ParameterDirection.Output;
                System.Data.DataSet ds = XCCloudBLL.ExecuteQuerySentence(sql, parameters);
                var workId = parameters[10].Value + "";
                var ret = parameters[11].Value + "";                
                if (ret == "1")
                {
                    IBase_UserInfoService userInfoService = BLLContainer.Resolve<IBase_UserInfoService>();
                    var userList = userInfoService.GetModels(p => p.OpenID.ToString().Equals(openId, StringComparison.OrdinalIgnoreCase));
                    var userInfo = userList.FirstOrDefault<Base_UserInfo>();
                    MessagePush(revOpenId, username, userInfo.CreateTime.Value.ToString("f"), workId, userType, message);

                    var succMsg = "已递交工单，等待管理员审核";
                    LogHelper.SaveLog("成功:" + succMsg);
                    Response.Redirect(WeiXinConfig.RedirectSuccessPage + "?realname=" + HttpUtility.UrlEncode(realname) + "&openid=" + openId 
                        + "&title=" + HttpUtility.UrlEncode("注册成功") + "&message=" + HttpUtility.UrlEncode(succMsg), false);
                }
                else
                {
                    errMsg = "注册失败";
                    LogHelper.SaveLog("错误:" + errMsg);
                    Response.Redirect(WeiXinConfig.RedirectErrorPage + "?title=" + HttpUtility.UrlEncode("注册失败") + "&message=" + HttpUtility.UrlEncode(errMsg), false);
                }
            }
            catch (Exception ex)
            {
                LogHelper.SaveLog("错误:" + ex.Message);
                Response.Redirect(WeiXinConfig.RedirectErrorPage + "?title=" + HttpUtility.UrlEncode("注册失败") + "&message=" + HttpUtility.UrlEncode(ex.Message), false);
            }
        }

        /// <summary>
        /// 验证注册参数
        /// </summary>
        /// <returns></returns>
        private bool checkRegisterParas(string openId, out string storeId, out string merchId, out int userType, out string revOpenId, out string errMsg)
        {
            errMsg = string.Empty;
            storeId = string.Empty;
            merchId = string.Empty;
            revOpenId = string.Empty;
            userType = (int)UserType.Store;
            string scode = Request["scode"] != null ? Request["scode"].ToString() : string.Empty;
            string mobile = Request["mobile"] != null ? Request["mobile"].ToString() : string.Empty;
            string storeOrMerchId = Request["storeOrMerchId"] != null ? Request["storeOrMerchId"].ToString() : string.Empty;            
            string username = Request["username"] != null ? Request["username"].ToString() : string.Empty;
            string password = Request["password"] != null ? Request["password"].ToString() : string.Empty;

            #region 验证参数
            //如果用户未获取短信验证码
            string key = mobile + "_" + scode;
            if (!SMSCodeCache.IsExist(key))
            {
                errMsg = "短信验证码无效";
                return false;
            }

            if (string.IsNullOrEmpty(storeOrMerchId))
            {
                errMsg = "门店ID或商户ID参数不能为空";
                return false;
            }

            if (string.IsNullOrEmpty(openId))
            {
                errMsg = "用户openId参数不能为空";
                return false;
            }

            if (openId.Length > 64)
            {
                errMsg = "用户openId参数长度不能超过64个字符";
                return false;
            }

            if (string.IsNullOrEmpty(username))
            {
                errMsg = "用户名参数不能为空";
                return false;
            }

            if (username.Length > 20)
            {
                errMsg = "用户名参数长度不能超过20个字符";
                return false;
            }

            if (!Utils.CheckMobile(mobile))
            {
                errMsg = "手机号码参数不正确";
                return false;
            }

            if (string.IsNullOrEmpty(password))
            {
                errMsg = "密码参数不能为空";
                return false;
            }
            #endregion

            //验证商户
            var mId = storeOrMerchId;
            IBase_MerchantInfoService base_MerchantInfoService = BLLContainer.Resolve<IBase_MerchantInfoService>();
            if (!base_MerchantInfoService.Any(p => p.MerchID.Equals(mId, StringComparison.OrdinalIgnoreCase)))
            {
                //验证门店
                var sId = storeOrMerchId;
                IBase_StoreInfoService storeInfoService = BLLContainer.Resolve<IBase_StoreInfoService>();
                if (!storeInfoService.Any(p => p.StoreID.Equals(sId, StringComparison.OrdinalIgnoreCase)))
                {
                    errMsg = "门店不存在";
                    return false;
                }

                mId = storeInfoService.GetModels(p => p.StoreID.Equals(sId, StringComparison.OrdinalIgnoreCase)).FirstOrDefault().MerchID;
                storeId = storeOrMerchId;
            }
            else
            {
                merchId = storeOrMerchId;
            }

            //验证发起人
            IBase_UserInfoService userInfoService = BLLContainer.Resolve<IBase_UserInfoService>();
            var userList = userInfoService.GetModels(p => p.OpenID.ToString().Equals(openId, StringComparison.OrdinalIgnoreCase));
            int userInfoCount = userList.Count<Base_UserInfo>();
            if (userInfoService.Any(p => p.OpenID.ToString().Equals(openId, StringComparison.OrdinalIgnoreCase)))
            {
                errMsg = "用户不能重复注册";
                return false;
            }

            //验证接收人
            if (!base_MerchantInfoService.Any(a => a.MerchID.Equals(mId, StringComparison.OrdinalIgnoreCase)))
            {
                errMsg = "接收商户不存在";
                return false;
            }

            var base_MerchantInfoModel = base_MerchantInfoService.GetModels(p => p.MerchID.Equals(mId, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
            var wxOpenId = base_MerchantInfoModel.WxOpenID;
            revOpenId = wxOpenId;
            if (!userInfoService.Any(p=>p.OpenID.Equals(wxOpenId, StringComparison.OrdinalIgnoreCase)))
            {
                errMsg = "接收人不存在";
                return false;
            }

            //验证用户类别
            if (!string.IsNullOrEmpty(merchId))
            {
                userType = (int)base_MerchantInfoModel.MerchType;
            }
            else
            {
                userType = (int)UserType.Store;
            }

            return true;
        }

        private void MessagePush(string openId, string userName, string registerTime, string workId, int userType, string message)
        {
            string errMsg = string.Empty;
            LogHelper.SaveLog(TxtLogType.WeiXin, TxtLogContentType.Common, TxtLogFileType.Day, "MessagePush");
            UserRegisterRemindDataModel dataModel = new UserRegisterRemindDataModel(userName, registerTime, workId, userType, message);
            if (MessageMana.PushMessage(WeiXinMesageType.UserRegisterRemind, openId, dataModel, out errMsg))
            {
                LogHelper.SaveLog(TxtLogType.WeiXin, TxtLogContentType.Common, TxtLogFileType.Day, "true");
            }
            else
            {
                LogHelper.SaveLog(TxtLogType.WeiXin, TxtLogContentType.Common, TxtLogFileType.Day, errMsg);
            }
        }
    }
}