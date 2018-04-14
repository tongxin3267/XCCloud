using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using XCCloudService.Base;
using XCCloudService.BLL.CommonBLL;
using XCCloudService.BLL.Container;
using XCCloudService.BLL.IBLL.XCCloudRS232;
using XCCloudService.Business;
using XCCloudService.CacheService;
using XCCloudService.Common;
using XCCloudService.Model.CustomModel.XCCloudRS232;
using XCCloudService.Model.CustomModel.XCGameManager;
using XCCloudService.Model.XCCloudRS232;

namespace XXCloudService.Api.XCCloudRS232
{
    /// <summary>
    /// Merch 的摘要说明
    /// </summary>
    public class Merch : ApiBase
    {

        #region 校验商户手机号码是否注册及图片验证码
        /// <summary>
        /// 校验商户手机号码是否注册及图片验证码
        /// </summary>
        /// <param name="dicParas"></param>
        /// <returns></returns>
        [ApiMethodAttribute(SignKeyEnum = SignKeyEnum.MethodToken)]
        public object checkMerch(Dictionary<string, object> dicParas)
        {
            try
            {
                string errMsg = string.Empty;
                string mobile = dicParas.ContainsKey("mobile") ? dicParas["mobile"].ToString() : string.Empty;
                string code = dicParas.ContainsKey("imgCode") ? dicParas["imgCode"].ToString() : string.Empty;

                if (string.IsNullOrEmpty(mobile))
                {
                    errMsg = "mobile参数不能为空";
                }

                if (string.IsNullOrEmpty(code))
                {
                    errMsg = "验证码不能为空";
                }

                if (!string.IsNullOrEmpty(errMsg))
                {
                    return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, errMsg);
                }

                if (string.IsNullOrWhiteSpace(mobile) || !IsMobile(mobile))
                {
                    return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "请输入正确的手机号码");
                }

                //验证请求次数
                if (!RequestTotalCache.CanRequest(mobile, ApiRequestType.CheckImgCode))
                {
                    return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "已超过单日最大请求次数");
                }
                else
                {
                    RequestTotalCache.Add(mobile, ApiRequestType.CheckImgCode);
                }

                //如果用户未获取验证码
                if (!ValidateImgCache.Exist(code.ToUpper()))
                {
                    return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "验证码错误");
                }

                //验证商户手机号码是否存在
                string sql = "select Mobile,State from Base_MerchInfo where Mobile=@Mobile";
                SqlParameter[] parameters = new SqlParameter[1];
                parameters[0] = new SqlParameter("@Mobile", mobile);
                System.Data.DataSet ds = XCCloudRS232BLL.ExecuteQuerySentence(sql, parameters);
                if (ds.Tables[0].Rows.Count == 0)
                {
                    return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "手机号码未注册");
                }
                else if (ds.Tables[0].Rows[0]["state"].ToString() == "0")
                {
                    return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "该手机号已被禁用");
                }

                string key = mobile + "_" + code;
                SMSTempTokenCache.Add(key, mobile, CacheExpires.SMSTempTokenExpires);
                ValidateImgCache.Remove(code.ToUpper());

                SMSTokenModel smsTokenModel = new SMSTokenModel(mobile, code);
                return ResponseModelFactory<SMSTokenModel>.CreateModel(isSignKeyReturn, smsTokenModel);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public bool IsMobile(string mobile)
        {
            return System.Text.RegularExpressions.Regex.IsMatch(mobile, @"^1(3|5|7|8)\d{9}$");
        }

        #endregion

        #region 商户登陆
        /// <summary>
        /// 商户登陆
        /// </summary>
        /// <param name="dicParas"></param>
        /// <returns></returns>
        [ApiMethodAttribute(SignKeyEnum = SignKeyEnum.MethodToken)]
        public object merchLogin(Dictionary<string, object> dicParas)
        {
            try
            {
                string errMsg = string.Empty;
                string mobile = dicParas.ContainsKey("mobile") ? dicParas["mobile"].ToString() : string.Empty;
                string code = dicParas.ContainsKey("smsCode") ? dicParas["smsCode"].ToString() : string.Empty;

                if (string.IsNullOrWhiteSpace(mobile) || !IsMobile(mobile))
                {
                    return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "请输入正确的手机号码");
                }

                string smsCode = dicParas.ContainsKey("smsCode") ? dicParas["smsCode"].ToString() : string.Empty;
                if (string.IsNullOrEmpty(smsCode))
                {
                    return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "请输入短信验证码");
                }

                //验证短信验证码
                bool isSMSTest = bool.Parse(System.Configuration.ConfigurationManager.AppSettings["isSMSTest"].ToString());
                //判断缓存验证码是否正确
                string key = mobile + "_" + smsCode;
                if (!isSMSTest)
                {
                    if (!SMSCodeCache.IsExist(key))
                    {
                        return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "短信验证码无效");
                    }
                }

                IMerchService merchService = BLLContainer.Resolve<IMerchService>();
                var merch = merchService.GetModels(p => p.Mobile.Equals(mobile, StringComparison.OrdinalIgnoreCase)).FirstOrDefault<Base_MerchInfo>();
                if (merch == null)
                {
                    return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "商户不存在");
                }

                //如果商户token为空，就写入新的token
                if (string.IsNullOrWhiteSpace(merch.Token))
                {
                    string token = System.Guid.NewGuid().ToString("N");
                    merch.Token = token; //更新token
                    merch.State = 1; //状态激活
                    merchService.Update(merch);

                    if (!MobileTokenCache.ExistTokenByKey(mobile))
                    {
                        MobileTokenCache.AddToken(CommonConfig.PrefixKey + mobile, token);
                    }
                }

                MerchModel merchModel = new MerchModel(merch.MerchName, merch.OPName, merch.Token, merch.State);
                return ResponseModelFactory<MerchModel>.CreateModel(isSignKeyReturn, merchModel);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        #endregion

    }
}