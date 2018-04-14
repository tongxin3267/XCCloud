using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using XCCloudService.Base;
using XCCloudService.BLL.Container;
using XCCloudService.BLL.IBLL.XCGameManager;
using XCCloudService.Business;
using XCCloudService.Business.Common;
using XCCloudService.CacheService;
using XCCloudService.Common;
using XCCloudService.Model.CustomModel.XCGameManager;

namespace XXCloudService.Api.XCGameMana
{
    /// <summary>
    /// Account 的摘要说明
    /// </summary>
    public class Account : ApiBase
    {
        /// <summary>
        /// 校验验证码
        /// </summary>
        /// <param name="dicParas"></param>
        /// <returns></returns>
        [ApiMethodAttribute(SignKeyEnum = SignKeyEnum.MethodToken)]
        public object checkStoreImgCode(Dictionary<string, object> dicParas)
        {
            try
            {
                string errMsg = string.Empty;
                string storeId = dicParas.ContainsKey("storeId") ? dicParas["storeId"].ToString() : string.Empty;
                string code = dicParas.ContainsKey("code") ? dicParas["code"].ToString() : string.Empty;
                string sysId = dicParas.ContainsKey("sysId") ? dicParas["sysId"].ToString() : string.Empty;

                if (string.IsNullOrEmpty(storeId))
                {
                    errMsg = "storeId参数不能为空";
                    return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, errMsg);
                }

                if (!Utils.isNumber(storeId))
                {
                    return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "店号的格式不正确");
                }

                if (string.IsNullOrEmpty(code))
                {
                    errMsg = "code参数不能为空";
                    return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, errMsg);
                }

                int iStoreId = Convert.ToInt32(storeId);
                IStoreService storeService = BLLContainer.Resolve<IStoreService>();
                if (!storeService.Any(a => a.id == iStoreId))
                {
                    return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "该门店不存在");
                }

                var storeModel = storeService.GetModels(p => p.id == iStoreId).FirstOrDefault();
                string mobile = storeModel.phone;
                if (string.IsNullOrEmpty(mobile))
                {
                    errMsg = "该门店手机号为空";
                    return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, errMsg);
                }
                if (!Utils.CheckMobile(mobile))
                {
                    return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "手机格式不正确");
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
                    return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "验证码已过期");
                }

                string key = mobile + "_" + code;
                SMSTempTokenCache.Add(key, mobile, CacheExpires.SMSTempTokenExpires);
                ValidateImgCache.Remove(code.ToUpper());

                string token = MobileTokenBusiness.SetMobileToken(mobile);
                var tokenModel = new { 
                    token = token,
                    mobile = mobile.Substring(0, 3) + "****" + mobile.Substring(7), //屏蔽中间4位手机号码
                    code = code
                };

                return ResponseModelFactory.CreateAnonymousSuccessModel(isSignKeyReturn, tokenModel);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// 发送短信验证码
        /// </summary>
        /// <param name="dicParas"></param>
        /// <returns></returns>
        [ApiMethodAttribute(SignKeyEnum = SignKeyEnum.MethodToken)]
        public object sendSMSCode(Dictionary<string, object> dicParas)
        {
            try
            {
                string token = dicParas.ContainsKey("token") ? dicParas["token"].ToString().Trim() : "";
                string code = dicParas.ContainsKey("code") ? dicParas["code"].ToString().Trim() : "";
                string templateId = "4";//短信模板    

                //获取手机号码
                string mobile = string.Empty;
                if (!MobileTokenBusiness.ExistToken(token, out mobile))
                {
                    return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "手机令牌无效");
                }                

                if (!Utils.CheckMobile(mobile))
                {
                    return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "手机号码无效");
                }

                //验证短信验证码
                string key = mobile + "_" + code;
                object smsTempTokenCacheObj = SMSTempTokenCache.GetValue(key);
                if (smsTempTokenCacheObj == null)
                {
                    return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "验证码已过期");
                }

                if (!smsTempTokenCacheObj.ToString().Equals(mobile))
                {
                    return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "验证码无效");
                }

                //发送短信，并添加缓存成功
                if (!FilterMobileBusiness.IsTestSMS && !FilterMobileBusiness.ExistMobile(mobile))
                {
                    //验证请求次数
                    if (!RequestTotalCache.CanRequest(mobile, ApiRequestType.SendSMSCode))
                    {
                        return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "已超过单日最大请求次数");
                    }
                    else
                    {
                        RequestTotalCache.Add(mobile, ApiRequestType.SendSMSCode);
                    }

                    string smsCode = string.Empty;
                    if (SMSBusiness.GetSMSCode(out smsCode))
                    {
                        key = mobile + "_" + smsCode;
                        SMSCodeCache.Add(key, mobile, CacheExpires.SMSCodeExpires);
                        string errMsg = string.Empty;
                        if (SMSBusiness.SendSMSCode(templateId, mobile, smsCode, out errMsg))
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
                    key = mobile + "_" + "123456";
                    SMSCodeCache.Add(key, mobile, CacheExpires.SMSCodeExpires);
                    return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.T, "");
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        [ApiMethodAttribute(SignKeyEnum = SignKeyEnum.MethodToken)]
        public object getMobileToken(Dictionary<string, object> dicParas)
        {
            try
            {
                string token = dicParas.ContainsKey("token") ? dicParas["token"].ToString().Trim() : "";
                string smsCode = dicParas.ContainsKey("smsCode") ? dicParas["smsCode"].ToString().Trim() : "";
                string storeId = dicParas.ContainsKey("storeId") ? dicParas["storeId"].ToString().Trim() : "";
                string openId = dicParas.ContainsKey("openId") ? dicParas["openId"].ToString().Trim() : "";

                //获取手机号码
                string mobile = string.Empty;
                if (!MobileTokenBusiness.ExistToken(token, out mobile))
                {
                    return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "手机令牌无效");
                }

                string key = mobile + "_" + smsCode;
                if (!FilterMobileBusiness.IsTestSMS && !FilterMobileBusiness.ExistMobile(mobile))
                {
                    if (!SMSCodeCache.IsExist(key))
                    {
                        return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "短信验证码无效");
                    }
                }

                if (SMSCodeCache.IsExist(key))
                {
                    SMSCodeCache.Remove(key);
                }

                //绑定openId
                if (string.IsNullOrEmpty(storeId))
                {
                    return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "storeId参数不能为空");
                }

                if (!Utils.isNumber(storeId))
                {
                    return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "店号的格式不正确");
                }

                if (string.IsNullOrEmpty(openId))
                {
                    return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "openId参数不能为空");
                }

                int iStoreId = Convert.ToInt32(storeId);
                IStoreService storeService = BLLContainer.Resolve<IStoreService>();
                if (!storeService.Any(a => a.id == iStoreId))
                {
                    return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "该门店不存在");
                }

                var storeModel = storeService.GetModels(p => p.id == iStoreId).FirstOrDefault();
                if (!string.IsNullOrEmpty(storeModel.openId))
                {
                    return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "门店已绑定微信，不能重复绑定");
                }

                storeModel.openId = openId;
                if (!storeService.Update(storeModel))
                {
                    return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "绑定openId失败");
                }

                token = MobileTokenBusiness.SetMobileToken(mobile);
                var tokenModel = new
                {
                    mobileToken = token
                };

                return ResponseModelFactory.CreateAnonymousSuccessModel(isSignKeyReturn, tokenModel);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        [ApiMethodAttribute(SignKeyEnum = SignKeyEnum.MethodToken)]
        public object getStores(Dictionary<string, object> dicParas)
        {
            try
            {
                string openId = dicParas.ContainsKey("openId") ? dicParas["openId"].ToString() : string.Empty;

                if (string.IsNullOrEmpty(openId))
                {
                    return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "openId参数不能为空");
                }

                IStoreService storeService = BLLContainer.Resolve<IStoreService>();
                var linq = from a in storeService.GetModels(p => p.openId.Equals(openId, StringComparison.OrdinalIgnoreCase))
                           select new
                           {
                               storeId = a.id,
                               companyname = a.companyname,
                               address = a.address
                           };

                return ResponseModelFactory.CreateAnonymousSuccessModel(isSignKeyReturn, linq.ToList());
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        [ApiMethodAttribute(SignKeyEnum = SignKeyEnum.MethodToken)]
        public object unBindStore(Dictionary<string, object> dicParas)
        {
            try
            {
                string storeId = dicParas.ContainsKey("storeId") ? dicParas["storeId"].ToString() : string.Empty;

                if (string.IsNullOrEmpty(storeId))
                {
                    return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "storeId参数不能为空");
                }

                int iStoreId = Convert.ToInt32(storeId);
                IStoreService storeService = BLLContainer.Resolve<IStoreService>();
                if (!storeService.Any(a => a.id == iStoreId))
                {
                    return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "该门店不存在");
                }

                var storeInfo = storeService.GetModels(p => p.id == iStoreId).FirstOrDefault();
                storeInfo.openId = string.Empty;
                if (!storeService.Update(storeInfo))
                {
                    return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "解绑门店失败");
                }

                return ResponseModelFactory.CreateSuccessModel(isSignKeyReturn);
            }
            catch (Exception e)
            {
                return ResponseModelFactory.CreateReturnModel(isSignKeyReturn, Return_Code.F, e.Message);
            }
        }
        
    }
}