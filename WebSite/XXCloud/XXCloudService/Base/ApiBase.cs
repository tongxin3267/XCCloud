using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Reflection;
using XCCloudService.Common;
using System.Runtime.Serialization.Json;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using XCCloudService.Business.XCGame;
using XCCloudService.Business.Common;
using XCCloudService.CacheService;
using XCCloudService.Model.CustomModel.XCGame;
using XCCloudService.Common.Enum;
using XCCloudService.Model.CustomModel.XCCloud;
using XCCloudService.Business.XCCloud;
using XCCloudService.Business.XCGameMana;

namespace XCCloudService.Base
{

    public class ApiBase : IHttpHandler
    {
        protected System.Web.Script.Serialization.JavaScriptSerializer jss = new System.Web.Script.Serialization.JavaScriptSerializer();
        protected bool isSignKeyReturn = false;
        protected int defaultPageSize = int.Parse(System.Configuration.ConfigurationManager.AppSettings["defaultPageSize"].ToString());
        protected string sysId = string.Empty;
        protected string versionNo = string.Empty;
 
        /// <summary>
        /// 处理请求,完成安全验证,调用接口方法
        /// </summary>
        /// <param name="context"></param>
        public void ProcessRequest(HttpContext context)
        {
            ApiRequestLog ar = new ApiRequestLog();
            context.Response.AddHeader("Access-Control-Allow-Origin", "*");

            //验证请求参数
            string errMsg = string.Empty;//异常错误
            string signKeyToken = string.Empty;//
            string postJson = string.Empty;//json
            int apiType = 0;//0-XCloud项目，1-XCGame项目,2-xcgamemana项目
            ApiMethodAttribute apiMethodAttribute = new ApiMethodAttribute();
            AuthorizeAttribute authorizeAttribute = new AuthorizeAttribute();
            MethodInfo requestMethodInfo = null;
            Dictionary<string, object> dicParas = null;
            string requestUrl = string.Empty;
            string action = RequestHelper.GetString("action");
            try
            {
                //获取请求的方法信息

                GetMethodInfo(this, action, ref requestMethodInfo, ref apiMethodAttribute, ref authorizeAttribute);

                if (requestMethodInfo == null)
                {
                    isSignKeyReturn = IsSignKeyReturn(apiMethodAttribute.SignKeyEnum);
                    errMsg = "请求方法无效";
                    FailResponseOutput(context, apiMethodAttribute, errMsg, signKeyToken);
                    return;
                }

                //验证请求参数
                if (!CheckRequestParam(context, apiMethodAttribute, ref dicParas, out errMsg, out postJson, out apiType, out requestUrl, out sysId, out versionNo))
                {
                    FailResponseOutput(context, apiMethodAttribute, errMsg, signKeyToken);
                    ar.show(apiType, requestUrl + "?action=" + action, postJson, Return_Code.F, errMsg, sysId);
                    return;
                }

                //验证参数签名
                if (!CheckSignKey(apiMethodAttribute.SignKeyEnum, dicParas, out signKeyToken, out errMsg))
                {
                    FailResponseOutput(context, apiMethodAttribute, errMsg, signKeyToken);
                    ar.show(apiType, requestUrl + "?action=" + action, postJson, Return_Code.F, errMsg, sysId);
                    return;
                }

                //验证访问权限
                if (!CheckAuthorize(authorizeAttribute, apiMethodAttribute.SignKeyEnum, dicParas, out errMsg))
                {
                    FailResponseOutput(context, apiMethodAttribute, errMsg, signKeyToken);
                    ar.show(apiType, requestUrl + "?action=" + action, postJson, Return_Code.F, errMsg, sysId);
                    return;
                }

                //验证是否锁定接口
                //if(!CheckIconOutputLock(apiMethodAttribute,dicParas,out errMsg))
                //{
                //    ar.show(apiType, requestUrl + "?action=" + action, postJson, Return_Code.F, errMsg, sysId);
                //    var obj = ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, errMsg);
                //    SuccessResponseOutput(context, apiMethodAttribute, obj, signKeyToken);
                //    return;
                //}

                //调用请求方法
                object[] paras = null;
                if (requestMethodInfo.GetParameters().Count<object>() > 0)
                {
                    paras = new object[1] { dicParas };
                }
                object resObj = requestMethodInfo.Invoke(this, paras);
                SuccessResponseOutput(context, apiMethodAttribute, resObj, signKeyToken);

                
                string return_code;
                string return_msg;
                string result_code;
                string result_msg;
                GetResObjInfo(resObj, out return_code, out return_msg, out result_code, out result_msg);
                ar.show(apiType, requestUrl + "?action=" + action, postJson, return_code, return_msg, sysId, result_msg);
            }
            catch (Exception ex)
            {
                FailResponseOutput(context, apiMethodAttribute, ex.Message, signKeyToken);
                LogHelper.SaveLog(TxtLogType.Api, TxtLogContentType.Exception, TxtLogFileType.Day, Utils.GetException(ex));
                ar.show(apiType, requestUrl + "?action=" + action, postJson, Return_Code.F, Utils.GetException(ex), sysId);
            }
        }


        /// <summary>
        /// 是否返回带签名的结果
        /// </summary>
        /// <param name="signKeyEnum"></param>
        /// <returns></returns>
        private bool IsSignKeyReturn(SignKeyEnum signKeyEnum)
        {
            switch (signKeyEnum)
            {
                case SignKeyEnum.DogNoToken: return true;
                case SignKeyEnum.MethodToken: return false;
                case SignKeyEnum.XCGameMemberToken: return false;
                default: return true;
            }
        }


        private bool CheckIconOutputLock(ApiMethodAttribute apiMethodAttribute, Dictionary<string, object> dicParas, out string errMsg)
        {
            TokenType tokenType = TokenType.Mobile;
            string mobile = string.Empty;
            errMsg = string.Empty;

            if (apiMethodAttribute.IconOutputLock == false)
            {
                return true;
            }

            if (apiMethodAttribute.SignKeyEnum == SignKeyEnum.XCGameMemberToken || apiMethodAttribute.SignKeyEnum == SignKeyEnum.MobileToken || apiMethodAttribute.SignKeyEnum == SignKeyEnum.XCGameMemberOrMobileToken)
            {
                if (!MeberAndMobileTokenBusiness.GetTokenData(dicParas, out tokenType, out mobile, out errMsg))
                {
                    return false;
                }

                if (IconOutLockBusiness.Exist(mobile))
                {
                    errMsg = "正在出币中，请稍后再进行操作。";
                    return false;
                }

                return true;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// 失败响应
        /// </summary>
        /// <param name="context"></param>
        /// <param name="apiMethodAttribute"></param>
        /// <param name="errMsg"></param>
        private void FailResponseOutput(HttpContext context, ApiMethodAttribute apiMethodAttribute, string errMsg, string signKeyToken)
        {
            if (apiMethodAttribute.RespDataTypeEnum == RespDataTypeEnum.Json)
            {
                object resModel = ResponseModelFactory.CreateReturnModel(isSignKeyReturn, Return_Code.F, errMsg);
                ResponseJsonOutput(context, resModel, signKeyToken);
            }
            else if (apiMethodAttribute.RespDataTypeEnum == RespDataTypeEnum.ImgStream)
            {

            }
        }

        /// <summary>
        /// 成功响应
        /// </summary>
        /// <param name="context"></param>
        /// <param name="apiMethodAttribute"></param>
        /// <param name="resObj"></param>
        /// <param name="signKeyToken"></param>
        private void SuccessResponseOutput(HttpContext context, ApiMethodAttribute apiMethodAttribute, object resObj,string signKeyToken)
        {
            if (apiMethodAttribute.RespDataTypeEnum == RespDataTypeEnum.Json)
            {
                ResponseJsonOutput(context, resObj, signKeyToken);
            }
            else if (apiMethodAttribute.RespDataTypeEnum == RespDataTypeEnum.ImgStream)
            {
                ResponseImgStreamOutput(context, (byte[])resObj, signKeyToken);
            }
        }


        private int GetSysType(string requestUrl)
        {
            string RequestUrl = RequestHelper.GetUrl();//获取路径
            string apiType = Convert.ToString(RequestUrl.Split('/')[4]);//获取项目
            int num = 0;//判断是那个项目
            switch (apiType.ToLower())
            {
                case "xccloud":
                    num = 0;
                    break;
                case "xcgame":
                    num = 1;
                    break;
                case "xcgamemana":
                    num = 3;
                    break;
            }
            return num;
        }

        /// <summary>
        /// 获取请求的方法对象
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="action"></param>
        /// <param name="requestMethodInfo"></param>
        /// <param name="signKeyEnum"></param>
        private void GetMethodInfo(object obj, string action, ref MethodInfo requestMethodInfo, ref ApiMethodAttribute apiMethodAttribute, ref AuthorizeAttribute authorizeAttribute)
        {
            Type type = obj.GetType();
            requestMethodInfo = type.GetMethod(action);
            if (requestMethodInfo == null)
                return;

            Attribute attribute = requestMethodInfo.GetCustomAttribute(typeof(ApiMethodAttribute));
            if (attribute != null)
            {
                ApiMethodAttribute apiMethodAttr = (ApiMethodAttribute)attribute;
                apiMethodAttribute.SignKeyEnum = apiMethodAttr.SignKeyEnum;
                apiMethodAttribute.RespDataTypeEnum = apiMethodAttr.RespDataTypeEnum;
                apiMethodAttribute.SysIdAndVersionNo = apiMethodAttr.SysIdAndVersionNo;
                apiMethodAttribute.IconOutputLock = apiMethodAttr.IconOutputLock;
            }
            else
            {
                apiMethodAttribute.SignKeyEnum = SignKeyEnum.DogNoToken;
                apiMethodAttribute.RespDataTypeEnum = RespDataTypeEnum.Json;
                apiMethodAttribute.SysIdAndVersionNo = true;
                apiMethodAttribute.IconOutputLock = false;
            }

            //类对象授权验证
            attribute = type.GetCustomAttribute(typeof(AuthorizeAttribute));
            if (attribute != null)
            {
                AuthorizeAttribute authorizeAttr = (AuthorizeAttribute)attribute;
                authorizeAttribute.Roles = authorizeAttr.Roles;
                authorizeAttribute.Users = authorizeAttr.Users;
                authorizeAttribute.Merches = authorizeAttr.Merches;
            }

            //方法授权验证
            attribute = requestMethodInfo.GetCustomAttribute(typeof(AuthorizeAttribute));
            if (attribute != null)
            {                
                AuthorizeAttribute authorizeAttr = (AuthorizeAttribute)attribute;
                authorizeAttribute.Roles = (authorizeAttribute.Roles + "," + authorizeAttr.Roles).Trim(',');
                authorizeAttribute.Users = (authorizeAttribute.Users + "," + authorizeAttr.Users).Trim(',');
                authorizeAttribute.Merches = (authorizeAttribute.Merches + "," + authorizeAttr.Merches).Trim(','); 
            }

            //匿名授权验证
            attribute = requestMethodInfo.GetCustomAttribute(typeof(AllowAnonymousAttribute));
            if (attribute != null)
            {
                authorizeAttribute.Roles = string.Empty;
                authorizeAttribute.Users = string.Empty;
                authorizeAttribute.Merches = string.Empty;
            }
        }        


        /// <summary>
        /// 验证请求参数
        /// </summary>
        /// <param name="context">上下文信息</param>
        /// <param name="errMsg">错误信息</param>
        /// <returns></returns>
        private bool CheckRequestParam(HttpContext context,ApiMethodAttribute apiMethodAttribute,ref Dictionary<string, object> dicParas, out string errMsg, out string postJson, out int apiType, out string requestUrl, out string sysId, out string versionNo)
        {
            errMsg = string.Empty;
            postJson = string.Empty;
            requestUrl = context.Request.Url.AbsolutePath;
            sysId = string.Empty;
            versionNo = string.Empty;
            apiType = GetSysType(requestUrl);
            //接收的Post请求参数集合不存在
            if (context.Request.HttpMethod == "GET")
            {
                postJson = Utils.GetNameValueCollection(context.Request.QueryString);
            }
            else if (context.Request.HttpMethod == "POST")
            {
                if (context.Request.Form.Count == 0 && context.Request.InputStream.Length == 0)
                {
                    errMsg = "没有业务请求参数";
                    return false;
                }
                else if (context.Request.InputStream.Length > 0)
                {
                    postJson = GetJsonByRequestStream(context);
                }
                else if (context.Request.Form.Count > 0)
                {
                    postJson = Utils.GetNameValueCollection(context.Request.Form);
                }
            }


            //接收的Post请求参数集合的第一参数为空
            if (string.IsNullOrEmpty(postJson))
            {
                errMsg = "没有业务请求参数";
                return false;
            }

            //Post请求的Json字符串格式的参数，转换为数据字典
            dicParas = GetJsonObject(postJson);
            //if (dicParas == null || dicParas.Count == 0)
            //{
            //    errMsg = "请求参数无效";
            //    return false;
            //}

            if (apiMethodAttribute.SysIdAndVersionNo)
            { 
                if (!dicParas.ContainsKey("sysId"))
                {
                    errMsg = "系统Id参数无效";
                    return false;
                }

                if (!dicParas.ContainsKey("versionNo"))
                {
                    errMsg = "系统版本号参数无效";
                    return false;
                }
                sysId = dicParas["sysId"].ToString();
                versionNo = dicParas["versionNo"].ToString();                
            }

            return true;
        }        

        /// <summary>
        /// 从请求流中获取JSON格式字符串
        /// </summary>
        /// <returns>JSON字符串</returns>
        private string GetJsonByRequestStream(HttpContext context)
        {
            //判断是否文件流
            if (context.Request.Files != null && context.Request.Files.Count > 0)
            {
                return Utils.GetNameValueCollection(context.Request.Form);
            }

            int count = 0;
            System.IO.Stream s = context.Request.InputStream;
            byte[] buffer = new byte[1024];
            StringBuilder builder = new StringBuilder();
            while ((count = s.Read(buffer, 0, 1024)) > 0)
            {
                builder.Append(Encoding.UTF8.GetString(buffer, 0, count));
            }
            string postJson = HttpUtility.UrlDecode(builder.ToString());
            string regex = @"{(.*)}";
            Match m = Regex.Match(postJson, regex);
            if (m != Match.Empty)
            {
                return m.ToString();
            }
            else
            {
                return Utils.GetNameValueCollection(context.Request.Form);
            }
        }

        //验证签名
        private bool CheckSignKey(SignKeyEnum signKeyEnum, Dictionary<string, object> dicParas, out string signkeyToken, out string errMsg)
        {
            errMsg = string.Empty;
            signkeyToken = string.Empty;
            if (signKeyEnum == SignKeyEnum.MobileToken)
            {
                string mobile = string.Empty;
                string mobileToken = Utils.GetDictionaryValue<string>(dicParas, "mobileToken").ToString();
                string storeId = Utils.GetDictionaryValue<string>(dicParas, "storeId").ToString();
                //如果是手机token
                if (MobileTokenBusiness.ExistToken(mobileToken, out mobile))
                {
                    MobileTokenModel mobileTokenTokenModel = new MobileTokenModel(mobile);
                    dicParas.Add(Constant.MobileTokenModel, mobileTokenTokenModel);
                    return true;
                }
                else
                {
                    errMsg = "手机令牌无效";
                    return false;
                }
            }
            else if (signKeyEnum == SignKeyEnum.XCGameMemberToken)
            {
                string token = dicParas["memberToken"].ToString();
                //验证token
                XCGameMemberTokenModel memberTokenKeyModel = MemberTokenBusiness.GetMemberTokenModel(token);
                if (memberTokenKeyModel == null)
                {
                    errMsg = "token无效";
                    return false;
                }
                else
                {
                    dicParas.Add(Constant.XCGameMemberTokenModel, memberTokenKeyModel);
                    return true;
                }
            }
            else if (signKeyEnum == SignKeyEnum.XCGameMemberOrMobileToken)
            {
                string mobile = string.Empty;
                string mobileToken = Utils.GetDictionaryValue<string>(dicParas,"mobileToken").ToString();
                string memberToken = Utils.GetDictionaryValue<string>(dicParas,"memberToken").ToString();
                if (string.IsNullOrEmpty(mobileToken) && string.IsNullOrEmpty(memberToken))
                {
                    errMsg = "手机令牌或会员令牌不正确";
                    return false;
                }
                else if (!string.IsNullOrEmpty(mobileToken) && string.IsNullOrEmpty(memberToken))
                {
                    //如果是手机token
                    if (MobileTokenBusiness.ExistToken(mobileToken, out mobile))
                    {
                        MobileTokenModel mobileTokenTokenModel = new MobileTokenModel(mobile);
                        dicParas.Add(Constant.MobileTokenModel, mobileTokenTokenModel);
                        return true;
                    }
                    else
                    {
                        errMsg = "手机令牌无效";
                        return false;
                    }
                }
                else if (!string.IsNullOrEmpty(memberToken) && string.IsNullOrEmpty(mobileToken))
                {
                    XCGameMemberTokenModel memberTokenModel = MemberTokenBusiness.GetMemberTokenModel(memberToken);
                    if (memberTokenModel != null)
                    {
                        dicParas.Add(Constant.XCGameMemberTokenModel, memberTokenModel);
                        return true;
                    }
                    else
                    {
                        errMsg = "会员令牌无效";
                        return false;
                    }
                }
                else if (!string.IsNullOrEmpty(memberToken) && !string.IsNullOrEmpty(mobileToken))
                {
                    //手机token验证
                    
                    if (!MobileTokenBusiness.ExistToken(mobileToken, out mobile))
                    {
                        errMsg = "手机令牌无效";
                        return false;
                    }
                    MobileTokenModel mobileTokenModel = new MobileTokenModel(mobile);
                    dicParas.Add(Constant.MobileTokenModel, mobileTokenModel);

                    //会员token
                    XCGameMemberTokenModel memberTokenModel = MemberTokenBusiness.GetMemberTokenModel(memberToken);
                    if (memberTokenModel == null)
                    {
                        errMsg = "会员令牌无效";
                        return false;
                    }
                    dicParas.Add(Constant.XCGameMemberTokenModel, memberTokenModel);

                    //会员token和手机token手机号对比
                    if (!memberTokenModel.Mobile.Equals(mobileTokenModel.Mobile))
                    {
                        errMsg = "手机令牌和会员令牌手机号不一致";
                        return false;
                    }
                    return true;
                }
                return true;
            }
            else if (signKeyEnum == SignKeyEnum.XCGameUserCacheToken)
            {
                string mobile = dicParas.ContainsKey("mobile") ? dicParas["mobile"].ToString() : string.Empty;
                string UserName = dicParas.ContainsKey("UserName") ? dicParas["UserName"].ToString() : string.Empty;
                string PassWord = dicParas.ContainsKey("PassWord") ? dicParas["PassWord"].ToString() : string.Empty;
                string storeId = dicParas.ContainsKey("storeId") ? dicParas["storeId"].ToString() : string.Empty;
                if (string.IsNullOrEmpty(storeId))
                {
                    errMsg = "门店Id不能为空";
                    return false;
                }
                if (string.IsNullOrEmpty(UserName))
                {
                    errMsg = "用户名不能为空";
                    return false;
                }
                if (string.IsNullOrEmpty(PassWord))
                {
                    errMsg = "密码不能为空";
                    return false;
                }
                if (string.IsNullOrEmpty(mobile))
                {
                    errMsg = "手机号码不能为空";
                    return false;
                }
                return true;
            }
            else if (signKeyEnum == SignKeyEnum.XCCloudUserCacheToken)
            {
                string token = dicParas["userToken"].ToString();

                //验证token
                XCCloudUserTokenModel userTokenKeyModel = XCCloudUserTokenBusiness.GetUserTokenModel(token);
                if (userTokenKeyModel == null)
                {
                    errMsg = "token无效";
                    return false;
                }
                else
                {
                    dicParas.Add(Constant.XCCloudUserTokenModel, userTokenKeyModel);
                    return true;
                }
            }
            else if (signKeyEnum == SignKeyEnum.MethodToken)
            {
                return true;
            }
            else if (signKeyEnum == SignKeyEnum.XCGameAdminToken)
            {
                string userToken = dicParas.ContainsKey("userToken") ? dicParas["userToken"].ToString() : string.Empty;
                string configUnionId = System.Configuration.ConfigurationManager.AppSettings["BossUnionId"].ToString();
                string unionId = string.Empty;
                if (!UnionIdTokenBusiness.ExistToken(userToken, out unionId))
                {
                    errMsg = "用户令牌无效";
                    return false;
                }
                if (!configUnionId.Contains(unionId))
                {
                    errMsg = "用户没有授权";
                    return false;
                }
                return true;
            }
            else if (signKeyEnum == SignKeyEnum.XCGameManaUserToken)
            {
                string userToken = dicParas.ContainsKey("userToken") ? dicParas["userToken"].ToString() : string.Empty;
                XCCloudManaUserTokenModel userTokenModel = XCCloudManaUserTokenBusiness.GetManaUserTokenModel(userToken);
                if (userTokenModel == null)
                {
                    errMsg = "用户没有授权";
                    return false;
                }
                dicParas.Add(Constant.XCGameManaUserToken, userTokenModel);
                return true;
            }
            else if (signKeyEnum == SignKeyEnum.XCGameManamAdminUserToken)
            {
                string userToken = dicParas.ContainsKey("userToken") ? dicParas["userToken"].ToString() : string.Empty;
                XCGameManaAdminUserTokenModel tokenModel = XCGameManaAdminUserTokenBusiness.GetTokenModel(userToken);
                if (tokenModel == null)
                {
                    errMsg = "用户没有授权";
                    return false;
                }
                dicParas.Add(Constant.XCGameManamAdminUserToken, tokenModel);
                return true;
            }
            else
            {
                string token = dicParas["token"].ToString();
                string signKey = dicParas["signkey"].ToString();
                errMsg = string.Empty;
                signkeyToken = "default";
                return true;
            }
        }

        /// <summary>
        /// 验证访问权限
        /// </summary>
        /// <param name="context">上下文信息</param>
        /// <param name="errMsg">错误信息</param>
        /// <returns></returns>
        private bool CheckAuthorize(AuthorizeAttribute authorizeAttribute, SignKeyEnum signKeyEnum, Dictionary<string, object> dicParas, out string errMsg)
        {
            errMsg = string.Empty;

            switch (signKeyEnum)
            {
                case SignKeyEnum.MobileToken: break;
                case SignKeyEnum.XCGameMemberToken: break;
                case SignKeyEnum.XCGameMemberOrMobileToken: break;
                case SignKeyEnum.XCGameUserCacheToken: break;
                case SignKeyEnum.XCCloudUserCacheToken:
                    {
                        string token = dicParas["userToken"].ToString();

                        //验证token
                        XCCloudUserTokenModel userTokenKeyModel = XCCloudUserTokenBusiness.GetUserTokenModel(token);
                        if (userTokenKeyModel == null)
                        {
                            errMsg = "token无效";
                            return false;
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(authorizeAttribute.Roles))
                            {
                                string roleName = Enum.GetName(typeof(RoleType), userTokenKeyModel.LogType);
                                if (!authorizeAttribute.Roles.Contains(roleName))
                                {
                                    errMsg = "当前用户无权访问";
                                    return false;
                                }
                            }

                            if (!string.IsNullOrEmpty(authorizeAttribute.Merches))
                            {
                                var merchDataModel = userTokenKeyModel.DataModel as MerchDataModel;
                                if (merchDataModel == null)
                                {
                                    errMsg = "当前用户无权访问";
                                    return false;
                                }

                                string merchType = Enum.GetName(typeof(MerchType), merchDataModel.MerchType);
                                if (!authorizeAttribute.Merches.Contains(merchType))
                                {
                                    errMsg = "当前用户无权访问";
                                    return false;
                                }
                            }
                        }

                        break;
                    }
                case SignKeyEnum.MethodToken: break;
                default: break;
            }
            
            return true;            
        }

        /// <summary>
        /// 获取json对象
        /// </summary>
        /// <param name="json">POST请求的Json格式字符串</param>
        /// <returns></returns>
        private Dictionary<string, object> GetJsonObject(string json)
        {
            //获取对象
            object obj = jss.DeserializeObject(json);
            if (obj == null)
                return null;

            //历便对象的属性集合,转换为, 并不区分大小写
            Dictionary<string, object> gd = (Dictionary<string, object>)obj;
            gd = (from entry in gd
                  orderby entry.Key ascending
                  select entry).ToDictionary(pair => pair.Key, pair => pair.Value, StringComparer.OrdinalIgnoreCase);

            return gd;
        }


        private void ResponseJsonOutput(HttpContext context, object model, string signKeyToken)
        {
            string json = GetResponseJson(model, signKeyToken);
            context.Response.Write(json);
        }

        private void ResponseImgStreamOutput(HttpContext context, byte[] bytes, string signKeyToken)
        {
            HttpContext.Current.Response.ClearContent();
            HttpContext.Current.Response.ContentType = "image/png";
            HttpContext.Current.Response.BinaryWrite(bytes);  
        }

        /// <summary>
        /// 获取相应输出Json格式字符串
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        private string GetResponseJson(ResponseModel model, string signKeyToken)
        {
            string json = string.Empty;
            //如果返回代码为F
            if (model.Return_Code.Equals(Return_Code.F))
            {
                string signKey = Utils.MD5(model.Return_Code + (string.IsNullOrEmpty(model.Return_Msg) ? "" : model.Return_Msg) + signKeyToken);
                var resModel = new
                {
                    return_code = model.Return_Code,
                    return_msg = string.IsNullOrEmpty(model.Return_Msg) ? "" : model.Return_Msg,
                    signKey = signKey
                };
                DataContractJsonSerializer js = new DataContractJsonSerializer(resModel.GetType());
                return jss.Serialize(resModel);
            }
            //如果结果代码为F
            else if (model.Result_Code.Equals(Result_Code.F))
            {
                string signKey = Utils.MD5(model.Return_Code + (string.IsNullOrEmpty(model.Return_Msg) ? "" : model.Return_Msg) +
                    model.Result_Code + (string.IsNullOrEmpty(model.Result_Msg) ? "" : model.Result_Msg) + signKeyToken);
                var resModel = new
                {
                    return_code = model.Return_Code,
                    return_msg = string.IsNullOrEmpty(model.Return_Msg) ? "" : model.Return_Msg,
                    result_code = model.Result_Code,
                    result_msg = string.IsNullOrEmpty(model.Result_Msg) ? "" : model.Result_Msg,
                    signKey = signKey
                };
                DataContractJsonSerializer js = new DataContractJsonSerializer(resModel.GetType());
                return jss.Serialize(resModel);
            }
            //如果结果代码和返回代码都为T
            else
            {
                string signKey = Utils.MD5(model.Return_Code + (string.IsNullOrEmpty(model.Return_Msg) ? "" : model.Return_Msg) +
                        model.Result_Code + (string.IsNullOrEmpty(model.Result_Msg) ? "" : model.Result_Msg) + signKeyToken);
                model.Result_Msg = string.IsNullOrEmpty(model.Return_Msg) ? "" : model.Return_Msg;
                model.Return_Msg = string.IsNullOrEmpty(model.Result_Msg) ? "" : model.Result_Msg;
                model.SignKey = signKey;

                List<Type> list = new List<Type>();
                list.Add(typeof(XCCloudService.DBService.Model.MOMModel));
                list.Add(typeof(XCCloudService.DBService.Model.TodayRevenueModel));
                IEnumerable<Type> iEnum = list;

                DataContractJsonSerializer serializer = new DataContractJsonSerializer(model.GetType(), iEnum);
                byte[] byteArr;
                using (MemoryStream ms = new MemoryStream())
                {
                    serializer.WriteObject(ms, model);
                    byteArr = ms.ToArray();
                }

                string jsonStr = Encoding.UTF8.GetString(byteArr);
                return jss.Serialize(jsonStr);
            }
        }


        /// <summary>
        /// 获取相应输出Json格式字符串
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        private string GetResponseJson(object resObj, string signKeyToken)
        {
            string json = string.Empty;

            //设置响应对象签名
            if (isSignKeyReturn && !string.IsNullOrEmpty(signKeyToken))
            {
                SetSignKey(resObj, signKeyToken);
            }

            //转化为JSON格式
            if (resObj.GetType().FullName.IndexOf("[[System.Object") >= 0 || resObj.GetType().FullName.IndexOf("f__AnonymousType") >= 0)
            {
                string jsonStr = jss.Serialize(resObj);
                return jsonStr;
            }           
            else
            {
                return Utils.DataContractJsonSerializer(resObj);
            }
        }

        /// <summary>
        /// 设置对象签名
        /// </summary>
        /// <param name="resObj">响应泛型类对象</param>
        /// <param name="dogNO">加密狗号</param>
        private void SetSignKey(object resObj, string dogNO)
        {
            string return_code = resObj.GetType().GetProperty("Return_Code").GetValue(resObj, null).ToString();
            string return_msg = resObj.GetType().GetProperty("Return_Msg").GetValue(resObj, null).ToString();
            string result_code = resObj.GetType().GetProperty("Result_Code").GetValue(resObj, null).ToString();
            string result_msg = resObj.GetType().GetProperty("Result_Msg").GetValue(resObj, null).ToString();            
            string signKey = return_code + return_msg + result_code + result_msg + dogNO;
            signKey = Utils.MD5(signKey);
            resObj.GetType().GetProperty("SignKey").SetValue(resObj, signKey, null);
        }

        private void GetResObjInfo(object resObj, out string return_code, out string return_msg, out string result_code, out string result_msg)
        {
            var return_codeprop = resObj.GetType().GetProperty("Return_Code");
            var return_msgprop = resObj.GetType().GetProperty("Return_Msg");
            var result_codeprop = resObj.GetType().GetProperty("Result_Code");
            var result_msgprop = resObj.GetType().GetProperty("Result_Msg");
            return_code = return_codeprop != null ? Convert.ToString(return_codeprop.GetValue(resObj, null)) : string.Empty;
            return_msg = return_msgprop != null ? Convert.ToString(return_msgprop.GetValue(resObj, null)) : string.Empty;
            result_code = result_codeprop != null ? Convert.ToString(result_codeprop.GetValue(resObj, null)) : string.Empty;
            result_msg = result_msgprop != null ? Convert.ToString(result_msgprop.GetValue(resObj, null)) : string.Empty;            
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}