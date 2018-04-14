using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Xml;
using XCCloudService.CacheService;
using XCCloudService.Base;
using XCCloudService.Common;
using XCCloudService.ResponseModels;
using XCCloudService.Business;

namespace XCCloudService.Api
{
    /// <summary>
    /// SMSService 的摘要说明
    /// </summary>
    public class SMSService : ApiBase
    {
        #region "发送短信"

            /// <summary>
            /// 发送短信
            /// </summary>
            /// <param name="dicParas"></param>
            /// <returns></returns>
            public object send(Dictionary<string, object> dicParas)
            {
                //广告类  1012812;验证码类 1012818
                string errMsg = string.Empty;
                string type = dicParas.ContainsKey("type") ? dicParas["type"].ToString() : "";
                string mobile = dicParas.ContainsKey("mobile") ? dicParas["mobile"].ToString() : "";
                string body = (dicParas.ContainsKey("body") ? dicParas["body"].ToString() : "") + " 退订回T 【莘宸科技】";
                
                if (!checkSendParams(dicParas, out errMsg))
                {
                    ResponseModel responseModel = new ResponseModel(Return_Code.T, "", Result_Code.F, errMsg);
                    return responseModel;
                }

                try
                {
                    string state = SMSBusiness.Send(type, mobile, body);
                    //发送成功
                    if (state == "0")
                    {
                        ResponseModel responseModel = new ResponseModel(Return_Code.T, "", Result_Code.T, "");
                        return responseModel;
                    }
                    else
                    {
                        ResponseModel responseModel = new ResponseModel(Return_Code.T, "", Result_Code.F, "发送失败");
                        return responseModel;
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

            /// <summary>
            /// 验证发送参数
            /// </summary>
            /// <param name="dicParas"></param>
            /// <param name="errMsg"></param>
            /// <returns></returns>
            private bool checkSendParams(Dictionary<string, object> dicParas, out string errMsg)
            {
                errMsg = string.Empty;
                string mobile = dicParas.ContainsKey("mobile") ? dicParas["mobile"].ToString().Trim() : "";
                string body = dicParas.ContainsKey("body") ? dicParas["body"].ToString().Trim() : "";
                string type = dicParas.ContainsKey("type") ? dicParas["type"].ToString() : "";

                if (SMSBusiness.GetSMSType(type) == string.Empty)
                {
                    errMsg = "短信类型不正确";
                    return false;
                }

                if (!Utils.CheckMobile(mobile))
                {
                    errMsg = "手机号不正确";
                    return false;
                }

                if (string.IsNullOrEmpty(body))
                {
                    errMsg = "消息内容不能为空";
                    return false;
                }

                return true;
            }

        #endregion

        #region "短信余额"

            /// <summary>
            /// 获取短信余额
            /// </summary>
            /// <param name="pid">产品编号</param>
            /// <returns></returns>
            public object balance(Dictionary<string, object> dicParas)
            {
                string pid = dicParas.ContainsKey("pid") ? dicParas["pid"].ToString() : "";
                string errMsg = string.Empty;

                if (!checkBalanceParams(dicParas, out errMsg))
                {
                    ResponseModel responseModel = new ResponseModel(Return_Code.T, "", Result_Code.F, errMsg);
                    return responseModel;
                }

                string state = string.Empty;
                int remain = 0;
                SMSBusiness.GetBalance(pid, out state, out remain);
                if (state == "0")
                {
                    SMSBalanceResponseModel smsResponseModel = new SMSBalanceResponseModel(remain);
                    ResponseModel<SMSBalanceResponseModel> responseModel = new ResponseModel<SMSBalanceResponseModel>(smsResponseModel);
                    return responseModel;
                }
                else
                {
                    ResponseModel responseModel = new ResponseModel(Return_Code.T, "", Result_Code.F, errMsg);
                    return responseModel;
                }
            }

            /// <summary>
            /// 验证获取短信余量参数
            /// </summary>
            /// <param name="dicParas"></param>
            /// <param name="errMsg"></param>
            /// <returns></returns>
            private bool checkBalanceParams(Dictionary<string, object> dicParas, out string errMsg)
            {
                errMsg = string.Empty;
                string pid = dicParas.ContainsKey("pid") ? dicParas["pid"].ToString() : "";

                if (!SMSBusiness.CheckPid(pid))
                {
                    errMsg = "短信类型不正确";
                    return false;
                }

                return true;
            }

        #endregion

        #region "获取验证码"

            /// <summary>
            /// 发送短信验证码
            /// </summary>
            /// <param name="dicParas"></param>
            /// <returns></returns>
            public static object sendSMSCode(Dictionary<string, object> dicParas)
            {
                string mobile = dicParas.ContainsKey("mobile") ? dicParas["mobile"].ToString().Trim() : "";
                string templateId = dicParas.ContainsKey("templateId") ? dicParas["templateId"].ToString().Trim() : "";

                if (!Utils.CheckMobile(mobile))
                {
                    ResponseModel responseModel = new ResponseModel(Return_Code.T, "", Result_Code.F, "手机号码无效");
                    return responseModel;
                }

                string code = string.Empty;
                if (SMSBusiness.GetSMSCode(out code))
                {
                    SMSCodeCache.Add(code, mobile, 2);
                    string errMsg = string.Empty;
                    if (SMSBusiness.SendSMSCode(templateId, mobile, code, out errMsg))
                    {
                        ResponseModel responseModel = new ResponseModel(Return_Code.T, "", Result_Code.T, "");
                        return responseModel;
                    }
                    else
                    {
                        ResponseModel responseModel = new ResponseModel(Return_Code.T, "", Result_Code.F, errMsg);
                        return responseModel;
                    }
                }
                else
                {
                    ResponseModel responseModel = new ResponseModel(Return_Code.T, "", Result_Code.F, "发送验证码出错");
                    return responseModel;
                }
            }



        #endregion

        #region "验证码校验"

            /// <summary>
            /// 验证码校验
            /// </summary>
            /// <param name="dicParas"></param>
            /// <returns></returns>
            public object checkCode(Dictionary<string, object> dicParas)
            {
                string mobile = dicParas.ContainsKey("mobile") ? dicParas["mobile"].ToString() : "";
                string code = dicParas.ContainsKey("code") ? dicParas["code"].ToString() : "";

                //验证手机号码
                if (!Utils.CheckMobile(mobile))
                {
                    ResponseModel responseModel = new ResponseModel(Return_Code.T, "", Result_Code.F,"手机号码不正确");
                    return responseModel;
                }

                //验证码是否存在于缓存中
                object codeMobile = SMSCodeCache.GetValue(code);
                if (codeMobile == null)
                {
                    ResponseModel responseModel = new ResponseModel(Return_Code.T, "", Result_Code.F, "验证码不存在或已过期");
                    return responseModel;
                }

                //缓存中的手机号码和请求的手机号码一致
                if (codeMobile.ToString().Equals(mobile))
                {
                    ResponseModel responseModel = new ResponseModel(Return_Code.T, "", Result_Code.T, "");
                    return responseModel;
                }
                else
                {
                    ResponseModel responseModel = new ResponseModel(Return_Code.T, "", Result_Code.F, "验证码无效");
                    return responseModel;
                }
            }

        #endregion
    }
}