using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using XCCloudService.CacheService;
using XCCloudService.Base;
using XCCloudService.BLL.Container;
using XCCloudService.BLL.IBLL.XCGame;
using XCCloudService.BLL.IBLL.XCGameManager;
using XCCloudService.Business;
using XCCloudService.Business.Common;
using XCCloudService.Business.XCGame;
using XCCloudService.Business.XCGameMana;
using XCCloudService.Common;
using XCCloudService.Model.CustomModel.XCGame;
using XCCloudService.Model.CustomModel.XCGameManager;
using XCCloudService.Model.XCGame;
using XCCloudService.Model.XCGameManager;
using XCCloudService.ResponseModels;
using System.Data.SqlClient;
using XCCloudService.BLL.CommonBLL;

namespace XCCloudService.Api.XCGameMana
{
    /// <summary>
    /// Token 的摘要说明
    /// </summary>
    public class Token : ApiBase
    {
        #region "校验验证码，获取短信临时TOKEN"

        /// <summary>
        /// 校验验证码
        /// </summary>
        /// <param name="dicParas"></param>
        /// <returns></returns>
        [ApiMethodAttribute(SignKeyEnum = SignKeyEnum.MethodToken)]
        public object checkImgCode(Dictionary<string, object> dicParas)
        {
            try
            {
                string errMsg = string.Empty;
                string mobile = dicParas.ContainsKey("mobile") ? dicParas["mobile"].ToString() : string.Empty;
                string code = dicParas.ContainsKey("code") ? dicParas["code"].ToString() : string.Empty;
                string sysId = dicParas.ContainsKey("sysId") ? dicParas["sysId"].ToString() : string.Empty;

                //验证请求次数
                if (!RequestTotalCache.CanRequest(mobile, ApiRequestType.CheckImgCode))
                {
                    return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "已超过单日最大请求次数");
                }
                else
                {
                    RequestTotalCache.Add(mobile, ApiRequestType.CheckImgCode);
                }

                if (!checkCodeParams(dicParas, out errMsg))
                {
                    return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, errMsg);
                }

                //如果用户未获取验证码
                if (!ValidateImgCache.Exist(code.ToUpper()))
                {
                    return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "验证码无效");
                }

                string key = mobile + "_" + code;
                SMSTempTokenCache.Add(key, mobile, CacheExpires.SMSTempTokenExpires);
                ValidateImgCache.Remove(code.ToUpper());

                SMSTokenModel smsTokenModel = new SMSTokenModel(mobile, code);
                return ResponseModelFactory<SMSTokenModel>.CreateModel(isSignKeyReturn, smsTokenModel);
            }
            catch(Exception e)
            {
                throw e;
            }
        }

        private bool checkCodeParams(Dictionary<string, object> dicParas, out string errMsg)
        {
            bool isCheck = true;
            errMsg = string.Empty;
            string mobile = dicParas.ContainsKey("mobile") ? dicParas["mobile"].ToString() : string.Empty;
            string code = dicParas.ContainsKey("code") ? dicParas["code"].ToString() : string.Empty;

            if (isCheck && string.IsNullOrEmpty(mobile))
            {
                errMsg = "mobile参数不能为空";
                isCheck = false;
            }

            if (isCheck && !Utils.CheckMobile(mobile))
            {
                errMsg = "mobile参数无效";
                isCheck = false;
            }

            if (string.IsNullOrEmpty(code))
            {
                errMsg = "code参数不能为空";
                isCheck = false;
            }

            return isCheck;
        }         

        #endregion

        #region "发送短信验证码"

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
                //是否模拟短信测试（1-模拟短信测试，不发送固定短信，不做短信验证）
 
                string mobile = dicParas.ContainsKey("mobile") ? dicParas["mobile"].ToString().Trim() : "";
                string templateId = "2";//短信模板
                string token = dicParas.ContainsKey("token") ? dicParas["token"].ToString().Trim() : "";

                //验证请求次数
                if (!FilterMobileBusiness.IsTestSMS && !FilterMobileBusiness.ExistMobile(mobile))
                { 
                    if (!RequestTotalCache.CanRequest(mobile, ApiRequestType.SendSMSCode))
                    {
                        return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "已超过单日最大请求次数");
                    }
                    else
                    {
                        RequestTotalCache.Add(mobile, ApiRequestType.SendSMSCode);
                    }                    
                }

                if (!Utils.CheckMobile(mobile))
                {
                    return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "手机号码无效");
                }

                //验证短信验证码
                string key = mobile + "_" + token;
                object smsTempTokenCacheObj = SMSTempTokenCache.GetValue(key);
                if (smsTempTokenCacheObj == null)
                {
                    return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "验证码无效");
                }

                if (!smsTempTokenCacheObj.ToString().Equals(mobile))
                {
                    return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "验证码无效");
                }

                //发送短信，并添加缓存成功
                if (!FilterMobileBusiness.IsTestSMS && !FilterMobileBusiness.ExistMobile(mobile))
                {
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

        #endregion

        #region "校验短信验证码,获取手机token"

        [ApiMethodAttribute(SignKeyEnum = SignKeyEnum.MethodToken)]
        public object getMobileToken(Dictionary<string, object> dicParas)
        {
            try
            {
                string mobile = dicParas.ContainsKey("mobile") ? dicParas["mobile"].ToString().Trim() : "";
                string smsCode = dicParas.ContainsKey("smsCode") ? dicParas["smsCode"].ToString().Trim() : "";

                string key = mobile + "_" + smsCode;
                if (!FilterMobileBusiness.IsTestSMS)
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

                string token = MobileTokenBusiness.SetMobileToken(mobile);
                MobileTokenResponseModel tokenModel = new MobileTokenResponseModel(mobile, token);
                return ResponseModelFactory<MobileTokenResponseModel>.CreateModel(isSignKeyReturn, tokenModel);
            }
            catch(Exception e)
            {
                throw e;
            }
        }

        #endregion

        #region "获取会员的token"

        /// <summary>
        /// 获取会员的token
        /// </summary>
        /// <param name="dicParas"></param>
        /// <returns></returns>
        [ApiMethodAttribute(SignKeyEnum = SignKeyEnum.MobileToken)]
        public object getMemberToken(Dictionary<string, object> dicParas)
        {
            bool isMember = false;
            string errMsg = string.Empty;
            string storeId = dicParas.ContainsKey("storeId") ? dicParas["storeId"].ToString() : string.Empty;
            try
            {
                MobileTokenModel mobileTokenModel = (MobileTokenModel)(dicParas[Constant.MobileTokenModel]);
                //获取终端号是否存在
                StoreCacheModel storeModel = null; 
                StoreBusiness storeBusiness = new StoreBusiness();
                if (!storeBusiness.IsEffectiveStore(storeId, ref storeModel, out errMsg))
                {
                    return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, errMsg);
                }

                //是否注册会员
                MemberResponseModel memberResponseModel = null;
                System.Data.DataSet ds = null;
                if (storeModel.StoreType == 0)
                {
                    XCCloudService.BLL.IBLL.XCGame.IMemberService memberService = BLLContainer.Resolve<XCCloudService.BLL.IBLL.XCGame.IMemberService>(storeModel.StoreDBName);
                    var count = memberService.GetModels(p => p.Mobile.Equals(mobileTokenModel.Mobile, StringComparison.OrdinalIgnoreCase)).Count<XCCloudService.Model.XCGame.t_member>();
                    if (count == 0)
                    {
                        string sql = " exec RegisterMember @Mobile,@MemberPassword,@WXOpenID,@Return output ";
                        SqlParameter[] parameters = new SqlParameter[4];
                        parameters[0] = new SqlParameter("@Mobile", mobileTokenModel.Mobile);
                        parameters[1] = new SqlParameter("@MemberPassword", "888888");
                        parameters[2] = new SqlParameter("@WXOpenID", "");
                        parameters[3] = new SqlParameter("@Return", 0);
                        parameters[3].Direction = System.Data.ParameterDirection.Output;
                        ds = XCGameBLL.ExecuteQuerySentence(sql, storeModel.StoreDBName, parameters);
                    }
                    else
                    {
                        string sql = " exec GetMember @Mobile,@ICCardID";
                        SqlParameter[] parameters = new SqlParameter[2];
                        parameters[0] = new SqlParameter("@Mobile", mobileTokenModel.Mobile);
                        parameters[1] = new SqlParameter("@ICCardID", "0");
                        ds = XCGameBLL.ExecuteQuerySentence(sql, storeModel.StoreDBName, parameters);
                    }
                    memberResponseModel = Utils.GetModelList<MemberResponseModel>(ds.Tables[0])[0];
                    memberResponseModel.MemberState = MemberBusiness.GetMemberStateName(memberResponseModel.MemberState);
                    isMember = true;
                }
                else if (storeModel.StoreType == 1)
                {
                    XCCloudService.BLL.IBLL.XCCloudRS232.IMemberService memberService = BLLContainer.Resolve<XCCloudService.BLL.IBLL.XCCloudRS232.IMemberService>();
                    var count = memberService.GetModels(p => p.Mobile.Equals(mobileTokenModel.Mobile, StringComparison.OrdinalIgnoreCase)).Count<XCCloudService.Model.XCCloudRS232.t_member>();
                    if (count == 0)
                    {
                        string sql = " exec RegisterMember @Mobile,@MerchId,@MemberPassword,@WXOpenID,@Return output ";
                        SqlParameter[] parameters = new SqlParameter[5];
                        parameters[0] = new SqlParameter("@Mobile", mobileTokenModel.Mobile);
                        parameters[1] = new SqlParameter("@MerchId", storeId);
                        parameters[2] = new SqlParameter("@MemberPassword", "888888");
                        parameters[3] = new SqlParameter("@WXOpenID", "");
                        parameters[4] = new SqlParameter("@Return", 0);
                        parameters[4].Direction = System.Data.ParameterDirection.Output;
                        ds = XCCloudRS232BLL.ExecuteQuerySentence(sql, parameters);
                    }
                    else
                    {
                        string sql = " exec GetMember @Mobile,@MerchId,@ICCardID";
                        SqlParameter[] parameters = new SqlParameter[3];
                        parameters[0] = new SqlParameter("@Mobile", mobileTokenModel.Mobile);
                        parameters[1] = new SqlParameter("@MerchId", storeId);
                        parameters[2] = new SqlParameter("@ICCardID", "0");
                        ds = XCCloudRS232BLL.ExecuteQuerySentence(sql, parameters);
                    }
                    memberResponseModel = Utils.GetModelList<MemberResponseModel>(ds.Tables[0])[0];
                    memberResponseModel.MemberState = "有效";
                    isMember = true;
                }
                else
                {
                    isMember = false;
                }
             
                //设置会员token
                string token = string.Empty;
                if (isMember)
                {
                    token = MemberTokenBusiness.SetMemberToken(storeId, mobileTokenModel.Mobile, memberResponseModel.MemberLevelName, storeModel.StoreName, memberResponseModel.ICCardID.ToString(), memberResponseModel.EndDate);
                }

                MemberTokenResponseModel memberTokenModel = new MemberTokenResponseModel(storeId,storeModel.StoreName, token, isMember, memberResponseModel);
                return ResponseModelFactory<MemberTokenResponseModel>.CreateModel(isSignKeyReturn, memberTokenModel);
            }
            catch(Exception e)
            {
                throw e;
            }
        }

        private bool checkMemberParams(Dictionary<string, object> dicParas, out string errMsg)
        {
            bool isCheck = true;
            errMsg = string.Empty;
            string mobile = dicParas.ContainsKey("mobile") ? dicParas["mobile"].ToString() : string.Empty;

            if (isCheck && string.IsNullOrEmpty(mobile))
            {
                errMsg = "mobile参数不能为空";
                isCheck = false;
            }

            if (isCheck && !Utils.CheckMobile(mobile))
            {
                errMsg = "mobile参数无效";
                isCheck = false;
            }

            return isCheck;
        }

        #endregion

        #region "获取设备token"

        /// <summary>
        /// 获取会员的token
        /// </summary>
        /// <param name="dicParas"></param>
        /// <returns></returns>
        [ApiMethodAttribute(SignKeyEnum = SignKeyEnum.MethodToken)]
        public object getRouteToken(Dictionary<string, object> dicParas)
        {
            try
            {
                string errMsg = string.Empty;
                string dbName = string.Empty;
                string password = string.Empty;
                string storeId = dicParas.ContainsKey("storeId") ? dicParas["storeId"].ToString() : string.Empty;
                string segment = dicParas.ContainsKey("segment") ? dicParas["segment"].ToString() : string.Empty;
                string token = dicParas.ContainsKey("token") ? dicParas["token"].ToString() : string.Empty;

                //验证参数
                if (!checkGetRouteTokenParams(dicParas, out errMsg))
                {
                    return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, errMsg);
                }

                //验证店Id
                StoreBusiness storeBusiness = new StoreBusiness();
                if (!storeBusiness.IsEffectiveStore(storeId, out dbName, out password, out errMsg))
                {
                    return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, errMsg);
                }

                //验证MD5
                string md5token = Utils.MD5(storeId + segment + password);
                if (!token.Equals(md5token))
                {
                    return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "token不正确");
                }

                //验证segment
                if (!new DeviceBusiness().IsEffectiveStoreSegment(dbName, segment, out errMsg))
                {
                    return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, errMsg);
                }

                //设置路由设备token
                string radarToken = XCGameRadarDeviceTokenBusiness.SetRadarDeviceToken(storeId, segment);

                RadarTokenModel radarTokenModel = new RadarTokenModel(storeId, segment, radarToken);
                return ResponseModelFactory<RadarTokenModel>.CreateModel(isSignKeyReturn, radarTokenModel);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        private bool checkGetRouteTokenParams(Dictionary<string, object> dicParas, out string errMsg)
        {
            errMsg = string.Empty;
            string storeId = dicParas.ContainsKey("storeId") ? dicParas["storeId"].ToString() : string.Empty;
            string segment = dicParas.ContainsKey("segment") ? dicParas["segment"].ToString() : string.Empty;
            string token = dicParas.ContainsKey("token") ? dicParas["token"].ToString() : string.Empty;

            //验证店Id
            if (string.IsNullOrEmpty(storeId))
            {
                errMsg = "店Id无效";
                return false;
            }

            //验证路由段地址
            if (string.IsNullOrEmpty(segment))
            {
                errMsg = "路由段地址无效";
                return false;
            }

            //验证token
            if (string.IsNullOrEmpty(token))
            {
                errMsg = "token无效";
                return false;
            }

            return true;
        }

        #endregion 
    }
}