using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using XCCloudService.Base;
using XCCloudService.BLL.Container;
using XCCloudService.Business;
using XCCloudService.Business.Common;
using XCCloudService.Business.XCGameMana;
using XCCloudService.CacheService;
using XCCloudService.Model.CustomModel.Common;
using XCCloudService.Model.CustomModel.XCGameManager;
using XCCloudService.Model.Socket.UDP;
using XCCloudService.SocketService.UDP.Factory;

namespace XXCloudService.Api.XCGameMana
{
    /// <summary>
    /// User2 的摘要说明
    /// </summary>
    public class User2 : ApiBase
    {
        /// <summary>
        /// 获取用户注册短信验证码
        /// </summary>
        /// <param name="dicParas"></param>
        /// <returns></returns>
        [ApiMethodAttribute(SignKeyEnum = SignKeyEnum.MethodToken)]
        public object getRegisterSMSCode(Dictionary<string, object> dicParas)
        {
            try
            {
                string storeId = dicParas.ContainsKey("storeId") ? dicParas["storeId"].ToString() : string.Empty;
                string mobile = dicParas.ContainsKey("mobile") ? dicParas["mobile"].ToString() : string.Empty;
                string imgCode = dicParas.ContainsKey("imgCode") ? dicParas["imgCode"].ToString() : string.Empty;
                string errMsg = string.Empty;

                //验证码
                if (!ValidateImgCache.Exist(imgCode.ToUpper()))
                {
                    return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "验证码无效");
                }
                ValidateImgCache.Remove(imgCode.ToUpper());

                if (string.IsNullOrEmpty(storeId))
                {
                    return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "门店号码不正确");
                }
                if (string.IsNullOrEmpty(mobile))
                {
                    return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "手机号码不正确");
                }
  
                bool isSMSTest = bool.Parse(System.Configuration.ConfigurationManager.AppSettings["isSMSTest"].ToString());

                StoreBusiness sb = new StoreBusiness();
                StoreCacheModel storeModel = null;
                if (!sb.IsEffectiveStore(storeId, ref storeModel,out errMsg))
                {
                    return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, errMsg);
                }


                if (storeModel.StoreDBDeployType == 0)
                { 
                    //验证用户在分库是否存在
                    XCCloudService.BLL.IBLL.XCGame.IUserService userService = BLLContainer.Resolve<XCCloudService.BLL.IBLL.XCGame.IUserService>(storeModel.StoreDBName);
                    var gameUserModel = userService.GetModels(p => p.Mobile.Equals(mobile, StringComparison.OrdinalIgnoreCase)).FirstOrDefault<XCCloudService.Model.XCGame.u_users>();
                    if (gameUserModel == null)
                    {
                        return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "未查询到该用户");
                    }                    
                }
                else if (storeModel.StoreDBDeployType == 1)
                {
                    string sn = System.Guid.NewGuid().ToString().Replace("-", "");
                    UDPSocketCommonQueryAnswerModel answerModel = null;
                    string radarToken = string.Empty;
                    if (DataFactory.SendDataUserPhoneQuery(sn, storeModel.StoreID.ToString(), storeModel.StorePassword, mobile, out radarToken, out errMsg))
                    {

                    }
                    else
                    {
                        return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, errMsg);
                    }

                    answerModel = null;
                    int whileCount = 0;
                    while (answerModel == null && whileCount <= 25)
                    {
                        //获取应答缓存数据
                        whileCount++;
                        System.Threading.Thread.Sleep(1000);
                        answerModel = UDPSocketCommonQueryAnswerBusiness.GetAnswerModel(sn, 1);
                    }

                    if (answerModel != null)
                    {
                        UserPhoneQueryResultNotifyRequestModel model = (UserPhoneQueryResultNotifyRequestModel)(answerModel.Result);
                        //移除应答缓存数据
                        UDPSocketCommonQueryAnswerBusiness.Remove(sn);
                        if (model.Result_Code == "0")
                        {
                            return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "未查询到该用户");
                        }
                    }
                    else
                    {
                        return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "系统没有响应");
                    }
                }
                else
                {
                    return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "门店设置不正确");
                }

                string templateId = "2";

                string key = string.Empty;
                if (!isSMSTest && !FilterMobileBusiness.ExistMobile(mobile))
                {
                    string smsCode = string.Empty;
                    if (SMSBusiness.GetSMSCode(out smsCode))
                    {
                        key = mobile + "_" + smsCode;
                        SMSCodeCache.Add(key, mobile, CacheExpires.SMSCodeExpires);
                    
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
            catch(Exception e)
            {
                throw e;
            }
        }


        /// <summary>
        /// 用户注册
        /// </summary>
        /// <param name="dicParas"></param>
        /// <returns></returns>
        [ApiMethodAttribute(SignKeyEnum = SignKeyEnum.MethodToken)]
        public object registerUser(Dictionary<string, object> dicParas)
        {
            try
            {
                string storeId = dicParas.ContainsKey("storeId") ? dicParas["storeId"].ToString() : string.Empty;
                string mobile = dicParas.ContainsKey("mobile") ? dicParas["mobile"].ToString() : string.Empty;
                string smsCode = dicParas.ContainsKey("smsCode") ? dicParas["smsCode"].ToString() : string.Empty;
                string errMsg = string.Empty;
                int userId = 0;

                if (string.IsNullOrEmpty(storeId))
                {
                    return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "门店号码不正确");
                }
                if (string.IsNullOrEmpty(mobile))
                {
                    return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "手机号码不正确");
                }
                if (string.IsNullOrEmpty(smsCode))
                {
                    return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "请输入验证码");
                }

                //验证短信验证码
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

                //验证门店是否有效
                StoreBusiness sb = new StoreBusiness();
                StoreCacheModel storeModel = null;
                if (!sb.IsEffectiveStore(storeId, ref storeModel, out errMsg))
                {
                    return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, errMsg);
                }

                if (storeModel.StoreDBDeployType == 0)
                {
                    //验证用户在分库是否存在
                    XCCloudService.BLL.IBLL.XCGame.IUserService userService = BLLContainer.Resolve<XCCloudService.BLL.IBLL.XCGame.IUserService>(storeModel.StoreDBName);
                    var gameUserModel = userService.GetModels(p => p.Mobile.Equals(mobile, StringComparison.OrdinalIgnoreCase)).FirstOrDefault<XCCloudService.Model.XCGame.u_users>();
                    if (gameUserModel == null)
                    {
                        return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "未查询到该用户");
                    }
                    userId = gameUserModel.UserID;
                }
                else if(storeModel.StoreDBDeployType == 1)
                {
                    string sn = System.Guid.NewGuid().ToString().Replace("-", "");
                    UDPSocketCommonQueryAnswerModel answerModel = null;
                    string radarToken = string.Empty;
                    if (DataFactory.SendDataUserPhoneQuery(sn, storeModel.StoreID.ToString(), storeModel.StorePassword, mobile, out radarToken, out errMsg))
                    {

                    }
                    else
                    {
                        return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, errMsg);
                    }

                    answerModel = null;
                    int whileCount = 0;
                    while (answerModel == null && whileCount <= 25)
                    {
                        //获取应答缓存数据
                        whileCount++;
                        System.Threading.Thread.Sleep(1000);
                        answerModel = UDPSocketCommonQueryAnswerBusiness.GetAnswerModel(sn, 1);
                    }

                    if (answerModel != null)
                    {
                        UserPhoneQueryResultNotifyRequestModel model = (UserPhoneQueryResultNotifyRequestModel)(answerModel.Result);
                        //移除应答缓存数据
                        UDPSocketCommonQueryAnswerBusiness.Remove(sn);
                        if (model.Result_Code == "0")
                        {
                            return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "未查询到该用户");
                        }
                    }
                    else
                    {
                        return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "系统没有响应");
                    }
                    userId = 0;//本地库用0
                }
                else
                {
                    return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "门店配置无效");
                }

                //验证用户在总库是否已注册
                XCCloudService.BLL.IBLL.XCGameManager.IUserService userServiceManager = BLLContainer.Resolve<XCCloudService.BLL.IBLL.XCGameManager.IUserService>();
                var userRegisterModel = userServiceManager.GetModels(p => p.Mobile.Equals(mobile) && p.StoreId.Equals(storeId)).FirstOrDefault<XCCloudService.Model.XCGameManager.t_user>();

                if (userRegisterModel == null)
                { 
                    //添加注册用户
                    XCCloudService.Model.XCGameManager.t_user t_user = new XCCloudService.Model.XCGameManager.t_user();
                    t_user.Mobile = mobile;
                    t_user.StoreId = storeId;
                    t_user.CreateTime = System.DateTime.Now;
                    t_user.OpenId = "";

                    if (!userServiceManager.Add(t_user))
                    {
                        return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "添加注册用户失败");
                    }
                }

                //获取用户token
                //云库不用userId
                string token = XCCloudManaUserTokenBusiness.SetToken(mobile, storeId, storeModel.StoreName, userId);
                List<XCCloudManaUserTokenResultModel> list = null;
                XCCloudManaUserTokenBusiness.GetUserTokenModel(mobile, ref list);
                var obj = new
                {
                    userToken = token,
                    storeList = list
                };
                return ResponseModelFactory.CreateAnonymousSuccessModel(isSignKeyReturn, obj);
            }
            catch(Exception e)
            {
                throw e;
            }
        }
    }
}