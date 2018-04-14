using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using XCCloudService.Base;
using XCCloudService.BLL.Container;
using XCCloudService.BLL.IBLL.XCGameManager;
using XCCloudService.Business;
using XCCloudService.Business.Common;
using XCCloudService.Business.XCGame;
using XCCloudService.CacheService;
using XCCloudService.Common;
using XCCloudService.Model.CustomModel.XCGameManager;
using XCCloudService.Model.XCGameManager;

namespace XXCloudService.Api.XCGameMana
{
    /// <summary>
    /// User 的摘要说明
    /// </summary>
    public class User : ApiBase
    {


        /// <summary>
        /// 查询手机号码发送短信验证码
        /// </summary>
        /// <param name="dicParas"></param>
        /// <returns></returns>
        [ApiMethodAttribute(SignKeyEnum = SignKeyEnum.MethodToken)]
        public object getUser(Dictionary<string, object> dicParas)
        {
            string UserName = dicParas.ContainsKey("UserName") ? dicParas["UserName"].ToString() : string.Empty;
            string imgCode = dicParas.ContainsKey("imgCode") ? dicParas["imgCode"].ToString() : string.Empty;
            if (string.IsNullOrEmpty(UserName))
            {
                return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "请输入用户名");
            }
            if (string.IsNullOrEmpty(imgCode))
            {
                return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "请输入验证码");
            }

            if (!FilterMobileBusiness.IsTestSMS)
            {
                if (!ValidateImgCache.Exist(imgCode.ToUpper()))
                {
                    return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "验证码无效");
                }
            }
        
            IUserRegisterService userervice = BLLContainer.Resolve<IUserRegisterService>();
            var menulist = userervice.GetModels(p => p.UserName.Equals(UserName, StringComparison.OrdinalIgnoreCase)).FirstOrDefault<t_UserRegister>();
            if (menulist == null)
            {
                return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "未查询到该用户");
            }
            string Mobile = menulist.Mobile;

            //短信模板
            string templateId = "2";
            string key = string.Empty;
            if (!FilterMobileBusiness.IsTestSMS && !FilterMobileBusiness.ExistMobile(Mobile))
            {
                string smsCode = string.Empty;
                if (SMSBusiness.GetSMSCode(out smsCode))
                {
                    key = Mobile + "_" + smsCode;
                    SMSCodeCache.Add(key, Mobile, CacheExpires.SMSCodeExpires);
                    string errMsg = string.Empty;
                    if (SMSBusiness.SendSMSCode(templateId, Mobile, smsCode, out errMsg))
                    {
                        return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.T, "");
                    }
                    else
                    {
                        return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, errMsg);
                    }
                }
                else
                {
                    return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "发送验证码出错");
                }
            }
            else
            {
                key = Mobile + "_" + "123456";
                SMSCodeCache.Add(key, Mobile, CacheExpires.SMSCodeExpires);
                return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.T, "");
            }

        }

        /// <summary>
        /// 忘记密码修改密码
        /// </summary>
        /// <param name="dicParas"></param>
        /// <returns></returns>
        [ApiMethodAttribute(SignKeyEnum = SignKeyEnum.MethodToken)]
        public object getForgetPassWord(Dictionary<string, object> dicParas)
        {
            string UserName = dicParas.ContainsKey("UserName") ? dicParas["UserName"].ToString() : string.Empty;
            string PassWord = dicParas.ContainsKey("PassWord") ? dicParas["PassWord"].ToString() : string.Empty;
            if (string.IsNullOrEmpty(PassWord))
            {
                return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "请输入新密码");
            }
            string smsCode = dicParas.ContainsKey("smsCode") ? dicParas["smsCode"].ToString() : string.Empty;
            if (string.IsNullOrEmpty(smsCode))
            {
                return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "请输入验证码");
            }
            IUserRegisterService userervice = BLLContainer.Resolve<IUserRegisterService>();
            var menulist = userervice.GetModels(p => p.UserName.Equals(UserName, StringComparison.OrdinalIgnoreCase)).FirstOrDefault<t_UserRegister>();
            if (menulist == null)
            {
                return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "未查询到该用户");
            }
            string errMsg = string.Empty;
            string Mobile = menulist.Mobile;          
            //判断缓存验证码是否正确
            string key = Mobile + "_" + smsCode;
            if (!FilterMobileBusiness.IsTestSMS && !FilterMobileBusiness.ExistMobile(Mobile))
            {
                if (!SMSCodeCache.IsExist(key))
                {
                    return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "短信验证码无效");
                }
            }
            string pass = Utils.MD5(PassWord);
            menulist.PassWord = pass;
            userervice.Update(menulist);
            return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.T, "");
        }
        /// <summary>
        /// /登录
        /// </summary>
        /// <param name="dicParas"></param>
        /// <returns></returns>
        [ApiMethodAttribute(SignKeyEnum = SignKeyEnum.MethodToken)]
     
        public object getLoginUser(Dictionary<string, object> dicParas)
        {
            string UserName = dicParas.ContainsKey("UserName") ? dicParas["UserName"].ToString() : string.Empty;
            string PassWord = dicParas.ContainsKey("PassWord") ? dicParas["PassWord"].ToString() : string.Empty;
            if (string.IsNullOrEmpty(UserName))
            {
                return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "请输入用户名");
            }
            if (string.IsNullOrEmpty(PassWord))
            {
                return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "请输入密码");
            }
            string Pass = Utils.MD5(PassWord);
            IUserRegisterService userervice = BLLContainer.Resolve<IUserRegisterService>();
            var menulist = userervice.GetModels(p => p.UserName == UserName && p.PassWord == Pass).ToList();
            if (menulist.Count > 0)
            {
                if (menulist[0].State != 1)
                {
                    return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "该用户正在审核中");
                }
                string key = UserName;
                string token = System.Guid.NewGuid().ToString("N");
                if (!MobileTokenCache.ExistTokenByKey(key))
                {
                    MobileTokenCache.AddToken(key, token);
                }
                return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, token, Result_Code.T, "");
            }
            return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "用户名或密码有误");
        }
        /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="dicParas"></param>
        /// <returns></returns>
        [ApiMethodAttribute(SignKeyEnum = SignKeyEnum.MethodToken)]

        public object getUpdatePassWord(Dictionary<string, object> dicParas)
        {
            string UserToken = dicParas.ContainsKey("UserToken") ? dicParas["UserToken"].ToString() : string.Empty;
            string PassWord = dicParas.ContainsKey("PassWord") ? dicParas["PassWord"].ToString() : string.Empty;
            string NewsPassWord = dicParas.ContainsKey("NewsPassWord") ? dicParas["NewsPassWord"].ToString() : string.Empty;
            if (string.IsNullOrEmpty(PassWord))
            {
                return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "请输入密码");
            }
            if (string.IsNullOrEmpty(NewsPassWord))
            {
                return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "请输入新密码");
            }
            string UserName = string.Empty;
            //验证token
            if (!MobileTokenBusiness.ExistToken(UserToken, out UserName))
            {
                return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "用户token无效");
            }
            PassWord = Utils.MD5(PassWord);           
            IUserRegisterService userervice = BLLContainer.Resolve<IUserRegisterService>();
            var menlist = userervice.GetModels(p => p.UserName == UserName && p.PassWord == PassWord).ToList();
            if (menlist.Count > 0)
            {
                NewsPassWord = Utils.MD5(NewsPassWord);
                menlist[0].PassWord = NewsPassWord;
                userervice.Update(menlist[0]);
                return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.T, "");
            }
            return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "原密码输入有误");
        }
    }
}