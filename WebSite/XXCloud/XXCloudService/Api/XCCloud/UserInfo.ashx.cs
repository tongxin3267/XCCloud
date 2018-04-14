using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Transactions;
using System.Web;
using XCCloudService.Base;
using XCCloudService.BLL.CommonBLL;
using XCCloudService.BLL.Container;
using XCCloudService.BLL.IBLL.XCCloud;
using XCCloudService.BLL.IBLL.XCGame;
using XCCloudService.BLL.XCCloud;
using XCCloudService.Business.XCCloud;
using XCCloudService.Business.XCGameMana;
using XCCloudService.Common;
using XCCloudService.Common.Enum;
using XCCloudService.DAL;
using XCCloudService.Model.CustomModel.XCCloud;
using XCCloudService.Model.CustomModel.XCCloud.Store;
using XCCloudService.Model.CustomModel.XCCloud.User;
using XCCloudService.Model.WeiXin.Message;
using XCCloudService.Model.XCCloud;
using XCCloudService.ResponseModels;
using XCCloudService.WeiXin.Message;

namespace XCCloudService.Api.XCCloud
{    
    /// <summary>
    /// UserInfo 的摘要说明
    /// </summary>
    public class UserInfo : ApiBase
    {

        #region "用户注册"

            /// <summary>
            /// 验证注册参数
            /// </summary>
            /// <param name="dicParas">参数集合</param>
            /// <param name="errMsg">错误信息</param>
            /// <returns></returns>
            public bool checkRegister(Dictionary<string, object> dicParas, out string errMsg)
            {
                errMsg = string.Empty;
                string mobile = dicParas.ContainsKey("mobile") ? dicParas["mobile"].ToString() : string.Empty;
                string realName = dicParas.ContainsKey("realname") ? dicParas["realname"].ToString() : string.Empty;
                string loginName = dicParas.ContainsKey("loginname") ? dicParas["loginname"].ToString() : string.Empty;
                string storeID = dicParas.ContainsKey("storeid") ? dicParas["storeid"].ToString() : string.Empty;
                string loginPassword = dicParas.ContainsKey("loginpassword") ? dicParas["loginpassword"].ToString() : string.Empty;

                if (!Utils.CheckMobile(mobile))
                {
                    mobile = "手机号不正确";
                    return false;
                }

                if (string.IsNullOrEmpty(realName))
                {
                    mobile = "注册真实姓名不能为空";
                    return false;
                }

                if (realName.Length > 25)
                {
                    mobile = "注册真实姓名不能超过25个字符";
                    return false;
                }

                if (string.IsNullOrEmpty(loginName))
                {
                    mobile = "注册登录名不能为空";
                    return false;
                }

                if (loginName.Length > 25)
                {
                    mobile = "注册登录名不能超过25个字符";
                    return false;
                }

                if (string.IsNullOrEmpty(loginPassword))
                {
                    mobile = "注册登录名不能为空";
                    return false;
                }

                if (loginPassword.Length > 16)
                {
                    mobile = "注册密码不能超过6个字符";
                    return false;
                }

                return true;
            }

            /// <summary>
            /// 管理员注册
            /// </summary>
            /// <param name="dicParas"></param>
            /// <returns></returns>
            public object register(Dictionary<string, object> dicParas)
            {
                string errMsg = string.Empty;
                string mobile = dicParas.ContainsKey("mobile") ? dicParas["mobile"].ToString() : string.Empty;
                string realName = dicParas.ContainsKey("realname") ? dicParas["realname"].ToString() : string.Empty;
                string loginName = dicParas.ContainsKey("loginname") ? dicParas["loginname"].ToString() : string.Empty;
                string storeID = dicParas.ContainsKey("storeid") ? dicParas["storeid"].ToString() : string.Empty;
                string loginPassword = dicParas.ContainsKey("loginpassword") ? dicParas["loginpassword"].ToString() : string.Empty;

                //验证注册信息
                if (!checkRegister(dicParas, out errMsg))
                {
                    ResponseModel responseModel = new ResponseModel(Return_Code.T, "", Result_Code.F, errMsg);
                    return responseModel;
                }

                IBase_UserInfoService userInfoService = BLLContainer.Resolve<IBase_UserInfoService>();
                var model = userInfoService.GetModels(p => p.Mobile.Equals(mobile, StringComparison.OrdinalIgnoreCase)).FirstOrDefault<Base_UserInfo>();
                if (model == null)
                {
                    //如果未注册，注册用户
                    string sql = " exec  SP_UserRegister @StoreId,@LogName,@LogPassword,@RealName,@Mobile,@Return output ";
                    SqlParameter[] parameters = new SqlParameter[6];
                    parameters[0] = new SqlParameter("@StoreId", storeID);
                    parameters[1] = new SqlParameter("@LogName", loginName);
                    parameters[2] = new SqlParameter("@LogPassword", loginPassword);
                    parameters[3] = new SqlParameter("@RealName", realName);
                    parameters[4] = new SqlParameter("@Mobile", mobile);
                    parameters[5] = new SqlParameter("@Return", 0);
                    parameters[5].Direction = System.Data.ParameterDirection.Output;

                    //返回新注册的用户信息和门店信息
                    System.Data.DataSet ds = XCCloudBLL.ExecuteQuerySentence(sql, parameters);
                    UserInfoRegisterResponseModel userInfoRegisterResponseModel = Utils.GetModelList<UserInfoRegisterResponseModel>(ds.Tables[0])[0];
                    ResponseModel<UserInfoRegisterResponseModel> responseModel = new ResponseModel<UserInfoRegisterResponseModel>(userInfoRegisterResponseModel);
                    return responseModel;
                }
                else
                {
                    //如果已注册，返回用户主要信息(用户信息和门店信息)
                    string sql = " exec  SP_GetRegisterUser @Mobile ";
                    SqlParameter[] parameters = new SqlParameter[1];
                    parameters[0] = new SqlParameter("@Mobile", mobile);

                    //返回新注册的用户信息和门店信息
                    System.Data.DataSet ds = XCCloudBLL.ExecuteQuerySentence(sql, parameters);
                    UserInfoRegisterResponseModel userInfoRegisterResponseModel = Utils.GetModelList<UserInfoRegisterResponseModel>(ds.Tables[0])[0];
                    ResponseModel<UserInfoRegisterResponseModel> responseModel = new ResponseModel<UserInfoRegisterResponseModel>(userInfoRegisterResponseModel);
                    return responseModel;
                }
            } 

        #endregion

        #region"获取用户信息"

            /// <summary>
            ///  获取用户信息
            /// </summary>
            /// <param name="dicParas"></param>
            /// <returns></returns>
            public object getUserInfo(Dictionary<string, object> dicParas)
            {
                string mobile = dicParas.ContainsKey("mobile") ? dicParas["mobile"].ToString() : string.Empty;

                if (!Utils.CheckMobile(mobile))
                {
                    ResponseModel responseModel = new ResponseModel(Return_Code.T, "", Result_Code.F, "手机号码错误");
                    return responseModel;
                }
                else 
                {
                    string sql = " exec  SP_GetUserInfoByMobile @Mobile ";
                    SqlParameter[] parameters = new SqlParameter[1];
                    parameters[0] = new SqlParameter("@Mobile", mobile);

                    //返回新注册的用户信息和门店信息
                    System.Data.DataSet ds = XCCloudBLL.ExecuteQuerySentence(sql, parameters);
                    UserInfoResponseModel userInfoRegisterResponseModel = Utils.GetModelList<UserInfoResponseModel>(ds.Tables[0])[0];
                    List<Base_StoreInfoModel> storeList = Utils.GetModelList<Base_StoreInfoModel>(ds.Tables[1]);
                    userInfoRegisterResponseModel.StoreList = storeList;
                    ResponseModel<UserInfoResponseModel> responseModel = new ResponseModel<UserInfoResponseModel>(userInfoRegisterResponseModel);
                    return responseModel;
                }
            }

        #endregion

        #region "吧台用户登录"
            [ApiMethodAttribute(SignKeyEnum = SignKeyEnum.MethodToken,SysIdAndVersionNo=false)]
            public object barLogin(Dictionary<string, object> dicParas)
            {
                try
                {
                    string errMsg = string.Empty;
                    int currentSchedule = 0;
                    string openTime = string.Empty; 
                    string loginName = dicParas.ContainsKey("loginName") ? dicParas["loginName"].ToString() : string.Empty;
                    string password = dicParas.ContainsKey("password") ? dicParas["password"].ToString() : string.Empty;
                    string workStation = dicParas.ContainsKey("workStation") ? dicParas["workStation"].ToString() : string.Empty;
                    string dogId = dicParas.ContainsKey("dogId") ? dicParas["dogId"].ToString() : string.Empty;

                    if (string.IsNullOrEmpty(loginName))
                    {
                        return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.F, "", Result_Code.T, "用户名不能为空");
                    }

                    if (string.IsNullOrEmpty(password))
                    {
                        return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.F, "", Result_Code.T, "密码不能为空");
                    }

                    if (string.IsNullOrEmpty(workStation))
                    {
                        return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.F, "", Result_Code.T, "工作站信息不完整");
                    }

                    //验证用户信息
                    IBase_UserInfoService userService = BLLContainer.Resolve<IBase_UserInfoService>();
                    var userModel = userService.GetModels(p => p.LogName.Equals(loginName, StringComparison.OrdinalIgnoreCase)).FirstOrDefault<Base_UserInfo>();
                    if (userModel == null)
                    {
                        return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "用户名不存在");
                    }
                    //验证密码
                    if (!userModel.LogPassword.Equals(Utils.MD5(password), StringComparison.OrdinalIgnoreCase))
                    {
                        return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "用户名或密码错误");
                    }
                    //开班
                    if (!ScheduleBusiness.OpenSchedule(userModel.StoreID, userModel.UserID, "", workStation, out currentSchedule,out openTime, out errMsg))
                    {
                        return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, errMsg);
                    }

                    StoreInfoCacheModel storeInfo = null;
                    if(!XCCloudStoreBusiness.IsEffectiveStore(userModel.StoreID,ref storeInfo))
                    {
                        return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "门店缓存数据不存在");
                    }

                    //设置用户token
                    StoreIDDataModel storeIDDataModel = new StoreIDDataModel(userModel.StoreID, password, workStation);
                    XCCloudUserTokenBusiness.RemoveStoreUserTokenByWorkStaion(userModel.UserID.ToString(), (int)RoleType.StoreUser, workStation);
                    string userToken = XCCloudUserTokenBusiness.SetUserToken(userModel.UserID.ToString(), (int)RoleType.StoreUser, storeIDDataModel);

                    var dataObj = new {
                        userToken = userToken,
                        storeId = userModel.StoreID,
                        storeName = storeInfo.StoreName,
                        scheduleId = currentSchedule,
                        openTime = openTime
                    };
                    return ResponseModelFactory.CreateAnonymousSuccessModel(isSignKeyReturn, dataObj);
                }
                catch (Exception e)
                {
                    throw e;
                }
            }

        #endregion

        #region "吧台用户退出"

            [ApiMethodAttribute(SignKeyEnum = SignKeyEnum.XCCloudUserCacheToken, SysIdAndVersionNo = false)]
            public object barExist(Dictionary<string, object> dicParas)
            {
                try
                {
                    string errMsg = string.Empty;
                    string token = dicParas["userToken"].ToString();
                    XCCloudUserTokenBusiness.RemoveToken(token);
                    return ResponseModelFactory.CreateSuccessModel(isSignKeyReturn);
                }
                catch (Exception e)
                {
                    throw e;
                }
            }

        #endregion

        #region "后台用户管理"

            private void NewPasswordMessagePush(string openId, string userName, string password)
            {
                string errMsg = string.Empty;
                XcUserNewPasswordDataModel dataModel = new XcUserNewPasswordDataModel(userName, password);
                if (MessageMana.PushMessage(WeiXinMesageType.XcUserNewPassword, openId, dataModel, out errMsg))
                {
                    LogHelper.SaveLog(TxtLogType.WeiXin, TxtLogContentType.Common, TxtLogFileType.Day, "true");
                }
                else
                {
                    LogHelper.SaveLog(TxtLogType.WeiXin, TxtLogContentType.Common, TxtLogFileType.Day, errMsg);
                }
            }

            private void ResetPasswordMessagePush(string openId, string userName, string password)
            {
                string errMsg = string.Empty;
                XcUserResetPasswordDataModel dataModel = new XcUserResetPasswordDataModel(userName, password);
                if (MessageMana.PushMessage(WeiXinMesageType.XcUserResetPassword, openId, dataModel, out errMsg))
                {
                    LogHelper.SaveLog(TxtLogType.WeiXin, TxtLogContentType.Common, TxtLogFileType.Day, "true");
                }
                else
                {
                    LogHelper.SaveLog(TxtLogType.WeiXin, TxtLogContentType.Common, TxtLogFileType.Day, errMsg);
                }
            }

            [Authorize(Roles = "XcAdmin")]
            [ApiMethodAttribute(SignKeyEnum = SignKeyEnum.XCCloudUserCacheToken, SysIdAndVersionNo = false)]
            public object GetXcUserGrant(Dictionary<string, object> dicParas)
            {
                string errMsg = string.Empty;
                string userId = dicParas.ContainsKey("userId") ? dicParas["userId"].ToString() : string.Empty;
                XCCloudUserTokenModel userTokenKeyModel = (XCCloudUserTokenModel)dicParas[Constant.XCCloudUserTokenModel];
                
                string sql = " exec SelectXcUserGrant @UserID";
                SqlParameter[] parameters = new SqlParameter[1];
                parameters[0] = new SqlParameter("@UserID", userId);

                System.Data.DataSet ds = XCCloudBLL.ExecuteQuerySentence(sql, parameters);
                System.Data.DataTable dt = ds.Tables[0];
                if (dt.Rows.Count > 0)
                {
                    var list = Utils.GetModelList<UserGrantModel>(ds.Tables[0]).ToList();
                    return ResponseModelFactory.CreateSuccessModel(isSignKeyReturn, list);
                }
                else
                {
                    return ResponseModelFactory.CreateFailModel(isSignKeyReturn, "授权功能列表为空");
                }
            }

            [Authorize(Roles = "XcAdmin")]
            [ApiMethodAttribute(SignKeyEnum = SignKeyEnum.XCCloudUserCacheToken, SysIdAndVersionNo = false)]
            public object GetXcUserInfo(Dictionary<string, object> dicParas)
            {
                try
                {
                    string errMsg = string.Empty;
                    string userId = dicParas.ContainsKey("userId") ? dicParas["userId"].ToString() : string.Empty;

                    XCCloudUserTokenModel userTokenKeyModel = (XCCloudUserTokenModel)dicParas[Constant.XCCloudUserTokenModel];

                    if (string.IsNullOrEmpty(userId))
                    {
                        errMsg = "userId参数不能为空";
                        return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                    }

                    if (!Utils.isNumber(userId))
                    {
                        errMsg = "userId参数格式不正确";
                        return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                    }

                    IBase_UserInfoService base_UserInfoService = BLLContainer.Resolve<IBase_UserInfoService>();
                    var base_UserInfoModel = base_UserInfoService.GetModels(p => p.UserID.ToString().Equals(userId)).FirstOrDefault();
                    if (base_UserInfoModel == null)
                    {
                        errMsg = "该用户不存在";
                        return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                    }

                    var xcUserInfoModel = Utils.GetCopy<XcUserInfoModel, Base_UserInfo>(base_UserInfoModel);                    
                    
                    return ResponseModelFactory.CreateSuccessModel(isSignKeyReturn, xcUserInfoModel);
                }
                catch (Exception e)
                {
                    return ResponseModelFactory.CreateReturnModel(isSignKeyReturn, Return_Code.F, e.Message);
                }
            }

            [Authorize(Roles = "XcAdmin")]
            [ApiMethodAttribute(SignKeyEnum = SignKeyEnum.XCCloudUserCacheToken, SysIdAndVersionNo = false)]
            public object SaveXcUserInfo(Dictionary<string, object> dicParas)
            {
                try
                {
                    string errMsg = string.Empty;
                    string pwd = string.Empty;
                    string userId = dicParas.ContainsKey("userId") ? dicParas["userId"].ToString() : string.Empty;
                    string logName = dicParas.ContainsKey("logName") ? dicParas["logName"].ToString() : string.Empty;
                    string openId = dicParas.ContainsKey("openId") ? dicParas["openId"].ToString() : string.Empty;
                    string realName = dicParas.ContainsKey("realName") ? dicParas["realName"].ToString() : string.Empty;
                    string mobile = dicParas.ContainsKey("mobile") ? dicParas["mobile"].ToString() : string.Empty;
                    string iCCardId = dicParas.ContainsKey("iCCardId") ? dicParas["iCCardId"].ToString() : string.Empty;
                    string status = dicParas.ContainsKey("status") ? dicParas["status"].ToString() : string.Empty;
                    string unionId = dicParas.ContainsKey("unionId") ? dicParas["unionId"].ToString() : string.Empty;
                    string userGroupId = dicParas.ContainsKey("userGroupId") ? dicParas["userGroupId"].ToString() : string.Empty;
                    object[] userGrants = dicParas.ContainsKey("userGrants") ? (object[])dicParas["userGrants"] : null;

                    XCCloudUserTokenModel userTokenKeyModel = (XCCloudUserTokenModel)dicParas[Constant.XCCloudUserTokenModel];

                    #region 验证参数

                    if (!string.IsNullOrEmpty(userId) && !Utils.isNumber(userId))
                    {
                        errMsg = "userId格式不正确";
                        return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                    }

                    if (!Utils.CheckMobile(mobile))
                    {
                        errMsg = "手机号不正确";
                        return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                    }

                    if (string.IsNullOrEmpty(realName))
                    {
                        errMsg = "员工姓名不能为空";
                        return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                    }

                    if (realName.Length > 25)
                    {
                        errMsg = "员工姓名不能超过25个字符";
                        return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                    }

                    if (string.IsNullOrEmpty(logName))
                    {
                        errMsg = "登录名不能为空";
                        return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                    }

                    if (logName.Length > 25)
                    {
                        errMsg = "登录名不能超过25个字符";
                        return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                    }

                    if (!string.IsNullOrEmpty(iCCardId) && iCCardId.Length > 16)
                    {
                        errMsg = "员工卡号不能超过16个字符";
                        return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                    }

                    if (string.IsNullOrEmpty(status))
                    {
                        errMsg = "员工状态不能为空";
                        return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                    }

                    if (!Utils.isNumber(status))
                    {
                        errMsg = "员工状态格式不正确";
                        return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                    }

                    if (string.IsNullOrEmpty(openId))
                    {
                        errMsg = "openId不能为空";
                        return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                    }

                    if (string.IsNullOrEmpty(userGroupId))
                    {
                        errMsg = "工作组Id不能为空";
                        return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                    }

                    if (!Utils.isNumber(userGroupId))
                    {
                        errMsg = "工作组Id格式不正确";
                        return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                    }
                    #endregion

                    //开启EF事务
                    using (TransactionScope ts = new TransactionScope())
                    {
                        try
                        {
                            var iUserId = 0;
                            int.TryParse(userId, out iUserId);
                            IBase_UserInfoService base_UserInfoService = BLLContainer.Resolve<IBase_UserInfoService>();
                            var base_UserInfoModel = base_UserInfoService.GetModels(p => p.UserID == iUserId).FirstOrDefault() ?? new Base_UserInfo();                            
                            base_UserInfoModel.LogName = logName;                            
                            base_UserInfoModel.RealName = realName;
                            base_UserInfoModel.Mobile = mobile;
                            base_UserInfoModel.ICCardID = iCCardId;
                            base_UserInfoModel.Status = Convert.ToInt32(status);                            
                            base_UserInfoModel.UserGroupID = Convert.ToInt32(userGroupId);
                            if (base_UserInfoModel.UserID == 0)
                            {
                                pwd = Utils.GetCheckCode(6);
                                base_UserInfoModel.OpenID = openId;
                                base_UserInfoModel.UnionID = unionId;
                                base_UserInfoModel.UserType = (int)UserType.Xc;
                                base_UserInfoModel.CreateTime = DateTime.Now;                                
                                base_UserInfoModel.LogPassword = Utils.MD5(pwd);
                                base_UserInfoModel.Auditor = Convert.ToInt32(userTokenKeyModel.LogId); //管理员授权给普通员工
                                base_UserInfoModel.AuditorTime = DateTime.Now;
                                if (!base_UserInfoService.Add(base_UserInfoModel))
                                {
                                    errMsg = "更新数据库失败";
                                    return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                                }
                            }
                            else
                            {
                                if (!base_UserInfoService.Update(base_UserInfoModel))
                                {
                                    errMsg = "更新数据库失败";
                                    return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                                }
                            }

                            iUserId = base_UserInfoModel.UserID;                                                       
                            
                            if (userGrants != null && userGrants.Count() >= 0)
                            {
                                //先删除后添加授权功能表
                                var dbContext = DbContextFactory.CreateByModelNamespace(typeof(Base_UserGrant).Namespace);
                                var base_UserGrantList = dbContext.Set<Base_UserGrant>().Where(p => p.UserID == iUserId).ToList();
                                foreach (var item in base_UserGrantList)
                                {
                                    dbContext.Entry(item).State = EntityState.Deleted;
                                }

                                foreach (IDictionary<string, object> iUgr in userGrants)
                                {
                                    if (iUgr != null)
                                    {
                                        var ugr = new Dictionary<string, object>(iUgr, StringComparer.OrdinalIgnoreCase);
                                        int ugrid = Convert.ToInt32(ugr["id"]);
                                        var base_UserGrant = new Base_UserGrant();
                                        base_UserGrant.GrantID = ugrid;
                                        base_UserGrant.UserID = iUserId;
                                        base_UserGrant.GrantEN = Convert.ToInt32(ugr["grantEn"]);
                                        dbContext.Entry(base_UserGrant).State = EntityState.Added;                                        
                                    }
                                    else
                                    {
                                        errMsg = "提交的数据包含空值";
                                        return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                                    }
                                }

                                if (dbContext.SaveChanges() < 0)
                                {
                                    errMsg = "保存授权功能失败";
                                    return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                                }
                            }

                            ts.Complete();
                        }
                        catch (Exception ex)
                        {
                            return ResponseModelFactory.CreateReturnModel(isSignKeyReturn, Return_Code.F, ex.Message);
                        }                        
                    }

                    if (!string.IsNullOrEmpty(pwd))
                    {
                        NewPasswordMessagePush(openId, logName, pwd);
                    }

                    //更新缓存
                    UserBusiness.XcUserInit();

                    return ResponseModelFactory.CreateSuccessModel(isSignKeyReturn);
                }
                catch (Exception e)
                {
                    return ResponseModelFactory.CreateReturnModel(isSignKeyReturn, Return_Code.F, e.Message);
                }
            }

            [Authorize(Roles = "XcAdmin")]
            [ApiMethodAttribute(SignKeyEnum = SignKeyEnum.XCCloudUserCacheToken, SysIdAndVersionNo = false)]
            public object ResetPassword(Dictionary<string, object> dicParas)
            {
                try
                {
                    string errMsg = string.Empty;
                    string openId = dicParas.ContainsKey("openId") ? dicParas["openId"].ToString() : string.Empty;

                    if (string.IsNullOrEmpty(openId))
                    {
                        errMsg = "openId不能为空";
                        return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                    }

                    UserInfoCacheModel userInfoCacheModel = null;
                    if (!UserBusiness.IsEffectiveXcUser(openId, out userInfoCacheModel))
                    {
                        errMsg = "该用户不存在";
                        return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                    }

                    string userId = userInfoCacheModel.UserID.ToString();
                    string pwd = Utils.GetCheckCode(6);
                    IBase_UserInfoService base_UserInfoService = BLLContainer.Resolve<IBase_UserInfoService>();
                    var base_UserInfoModel = base_UserInfoService.GetModels(p => p.UserID.ToString().Equals(userId, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
                    base_UserInfoModel.LogPassword = Utils.MD5(pwd);
                    if (!base_UserInfoService.Update(base_UserInfoModel))
                    {
                        errMsg = "更新数据库失败";
                        return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                    }
                    
                    //推送微信消息
                    ResetPasswordMessagePush(openId, base_UserInfoModel.LogName, pwd);

                    return ResponseModelFactory.CreateSuccessModel(isSignKeyReturn);
                }
                catch (Exception e)
                {
                    return ResponseModelFactory.CreateReturnModel(isSignKeyReturn, Return_Code.F, e.Message);
                }
            }

        #endregion

        #region "门店用户管理"

            [Authorize(Merches = "Normal,Heavy")]
            [ApiMethodAttribute(SignKeyEnum = SignKeyEnum.XCCloudUserCacheToken, SysIdAndVersionNo = false)]
            public object GetStoreUserList(Dictionary<string, object> dicParas)
            {
                try
                {
                    string errMsg = string.Empty;
                    string merchId = string.Empty;
                    XCCloudUserTokenModel userTokenKeyModel = (XCCloudUserTokenModel)dicParas[Constant.XCCloudUserTokenModel];
                    merchId = userTokenKeyModel.LogId;

                    IBase_StoreInfoService base_StoreInfoService = BLLContainer.Resolve<IBase_StoreInfoService>();
                    var storeIds = base_StoreInfoService.GetModels(p => p.MerchID.Equals(merchId, StringComparison.OrdinalIgnoreCase)).Select(o => o.StoreID).ToList();                    
                    IDict_SystemService dict_SystemService = BLLContainer.Resolve<IDict_SystemService>();
                    int UserStatusId = dict_SystemService.GetModels(p => p.DictKey.Equals("员工状态")).FirstOrDefault().ID;

                    var dbContext = DbContextFactory.CreateByModelNamespace(typeof(Base_UserInfo).Namespace);
                    var linq = from a in dbContext.Set<Base_UserInfo>().Where(p => storeIds.Contains(p.StoreID))
                                join b in dbContext.Set<Base_StoreInfo>() on a.StoreID equals b.StoreID into b1
                                from b in b1.DefaultIfEmpty()
                                join c in dbContext.Set<Base_UserGroup>() on a.UserGroupID equals c.ID into c1
                                from c in c1.DefaultIfEmpty()
                                join d in dbContext.Set<Dict_System>().Where(p => p.PID == UserStatusId) on (a.Status + "") equals d.DictValue into d1
                                from d in d1.DefaultIfEmpty()
                                select new
                                {
                                    UserID = a.UserID,
                                    RealName = a.RealName,
                                    LogName = a.LogName,
                                    Mobile = a.Mobile,
                                    IsAdminStr = a.IsAdmin == 1 ? "是" : "否",
                                    StoreName = b != null ? b.StoreName : string.Empty,
                                    UserGroupName = c != null ? c.GroupName : string.Empty,
                                    UserStatusStr = d != null ? d.DictKey : string.Empty
                                };

                    return ResponseModelFactory.CreateSuccessModel(isSignKeyReturn, linq.ToList());
                }
                catch (Exception e)
                {
                    return ResponseModelFactory.CreateReturnModel(isSignKeyReturn, Return_Code.F, e.Message);
                }
            }

            [Authorize(Merches = "Normal,Heavy")]
            [ApiMethodAttribute(SignKeyEnum = SignKeyEnum.XCCloudUserCacheToken, SysIdAndVersionNo = false)]
            public object GetStoreUserInfo(Dictionary<string, object> dicParas)
            {
                try
                {
                    string errMsg = string.Empty;
                    string userId = dicParas.ContainsKey("userId") ? dicParas["userId"].ToString() : string.Empty;

                    if (string.IsNullOrEmpty(userId))
                    {
                        errMsg = "userId参数不能为空";
                        return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                    }

                    if (!Utils.isNumber(userId))
                    {
                        errMsg = "userId参数格式不正确";
                        return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                    }

                    int iUserId = Convert.ToInt32(userId);
                    IBase_UserInfoService base_UserInfoService = BLLContainer.Resolve<IBase_UserInfoService>();
                    if (!base_UserInfoService.Any(p => p.UserID == iUserId))
                    {
                        errMsg = "该用户不存在";
                        return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                    }

                    IDict_SystemService dict_SystemService = BLLContainer.Resolve<IDict_SystemService>();
                    int UserStatusId = dict_SystemService.GetModels(p => p.DictKey.Equals("员工状态")).FirstOrDefault().ID;
                    int UserTypeId = dict_SystemService.GetModels(p => p.DictKey.Equals("用户类型")).FirstOrDefault().ID;
                    var dbContext = DbContextFactory.CreateByModelNamespace(typeof(Base_UserInfo).Namespace);
                    var linq = from a in dbContext.Set<Base_UserInfo>().Where(p => p.UserID == iUserId)
                               join c in dbContext.Set<Base_UserGroup>() on a.UserGroupID equals c.ID into c1
                               from c in c1.DefaultIfEmpty()
                               join d in dbContext.Set<Dict_System>().Where(p => p.PID == UserStatusId) on (a.Status + "") equals d.DictValue into d1
                               from d in d1.DefaultIfEmpty()
                               join b in dbContext.Set<Dict_System>().Where(p => p.PID == UserTypeId) on (a.UserType + "") equals b.DictValue into b1
                               from b in b1.DefaultIfEmpty()
                               select new
                               {
                                   UserID = a.UserID,
                                   RealName = a.RealName,
                                   LogName = a.LogName,
                                   Mobile = a.Mobile,
                                   IsAdmin = a.IsAdmin,
                                   IsAdminStr = a.IsAdmin == 1 ? "是" : "否",    
                                   UserGroupID = a.UserGroupID,
                                   UserGroupName = c != null ? c.GroupName : string.Empty,
                                   UserStatus = a.Status,
                                   UserStatusStr = d != null ? d.DictKey : string.Empty,
                                   UserType = a.UserType,
                                   UserTypeStr = b != null ? b.DictKey : string.Empty
                               };

                    return ResponseModelFactory.CreateSuccessModel(isSignKeyReturn, linq.FirstOrDefault());
                }
                catch (Exception e)
                {
                    return ResponseModelFactory.CreateReturnModel(isSignKeyReturn, Return_Code.F, e.Message);
                }
            }

            [Authorize(Merches = "Normal,Heavy")]
            [ApiMethodAttribute(SignKeyEnum = SignKeyEnum.XCCloudUserCacheToken, SysIdAndVersionNo = false)]
            public object SaveStoreUserInfo(Dictionary<string, object> dicParas)
            {
                string errMsg = string.Empty;
                string userId = dicParas.ContainsKey("userId") ? (dicParas["userId"] + "") : string.Empty;
                string realName = dicParas.ContainsKey("realName") ? (dicParas["realName"] + "") : string.Empty;
                string logName = dicParas.ContainsKey("logName") ? (dicParas["logName"] + "") : string.Empty;
                string mobile = dicParas.ContainsKey("mobile") ? (dicParas["mobile"] + "") : string.Empty;
                string status = dicParas.ContainsKey("status") ? (dicParas["status"] + "") : string.Empty;
                string isAdmin = dicParas.ContainsKey("isAdmin") ? (dicParas["isAdmin"] + "") : string.Empty;
                string userType = dicParas.ContainsKey("userType") ? (dicParas["userType"] + "") : string.Empty;
                int iUserId = Convert.ToInt32(userId);

                if (string.IsNullOrEmpty(userId))
                {
                    errMsg = "userId参数不能为空";
                    return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                }

                if (string.IsNullOrEmpty(status))
                {
                    errMsg = "status参数不能为空";
                    return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                }

                if (string.IsNullOrEmpty(realName))
                {
                    errMsg = "realName参数不能为空";
                    return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                }

                if (string.IsNullOrEmpty(logName))
                {
                    errMsg = "logName参数不能为空";
                    return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                }

                if (!Utils.CheckMobile(mobile))
                {
                    errMsg = "手机格式不正确";
                    return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                }
                
                //开启EF事务
                using (TransactionScope ts = new TransactionScope())
                {
                    try
                    {
                        //修改用户信息                                                                                                
                        IBase_UserInfoService userInfoService = BLLContainer.Resolve<IBase_UserInfoService>();
                        var base_UserInfo = userInfoService.GetModels(p => p.UserID == iUserId).FirstOrDefault();
                        if (base_UserInfo == null)
                        {
                            errMsg = "该用户不存在";
                            return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                        }

                        if (dicParas.ContainsKey("userGroup") && dicParas["userGroup"] != null)
                        {
                            Dictionary<string, object> userGroup = new Dictionary<string, object>((IDictionary<string, object>)dicParas["userGroup"], StringComparer.OrdinalIgnoreCase);
                            IBase_UserGroupService base_UserGroupService = BLLContainer.Resolve<IBase_UserGroupService>();
                            int ugid = Convert.ToInt32(userGroup["id"]);
                            if (!base_UserGroupService.Any(w => w.ID.Equals(ugid)))
                            {
                                errMsg = "工作组" + userGroup["groupName"] + "不存在";
                                return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                            }

                            base_UserInfo.UserGroupID = ugid;
                        }
                        
                        base_UserInfo.IsAdmin = !string.IsNullOrEmpty(isAdmin) ? Convert.ToInt32(isAdmin) : (int?)null;
                        base_UserInfo.Status = !string.IsNullOrEmpty(status) ? Convert.ToInt32(status) : (int?)null;
                        base_UserInfo.UserType = !string.IsNullOrEmpty(userType) ? Convert.ToInt32(userType) : (int)UserType.Store;
                        base_UserInfo.RealName = realName;
                        base_UserInfo.LogName = logName;
                        base_UserInfo.Mobile = mobile;

                        string storeId = base_UserInfo.StoreID;
                        if (base_UserInfo.IsAdmin == 1 && userInfoService.Any(a => a.UserID != iUserId && a.IsAdmin == 1 && a.StoreID.Equals(storeId, StringComparison.OrdinalIgnoreCase)))
                        {
                            errMsg = "同一个门店只能有一个管理员";
                            return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                        }

                        if (!userInfoService.Update(base_UserInfo))
                        {
                            errMsg = "修改用户信息失败";
                            return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                        }

                        if (dicParas.ContainsKey("userGrant") && dicParas["userGrant"] != null)
                        {
                            //添加或修改授权功能表
                            var dbContext = DbContextFactory.CreateByModelNamespace(typeof(Base_UserGrant).Namespace);
                            var userGrant = (object[])dicParas["userGrant"];
                            foreach (IDictionary<string, object> iUgr in userGrant)
                            {
                                if (iUgr != null)
                                {
                                    var ugr = new Dictionary<string, object>(iUgr, StringComparer.OrdinalIgnoreCase);
                                    int ugrid = Convert.ToInt32(ugr["id"]);
                                    if (!dbContext.Set<Base_UserGrant>().Any(w => w.GrantID == ugrid && w.UserID == iUserId))
                                    {
                                        var base_UserGrant = new Base_UserGrant();
                                        base_UserGrant.GrantID = ugrid;
                                        base_UserGrant.UserID = iUserId;
                                        base_UserGrant.GrantEN = Convert.ToInt32(ugr["grantEn"]);
                                        dbContext.Entry(base_UserGrant).State = EntityState.Added;
                                    }
                                    else
                                    {
                                        var base_UserGrant = dbContext.Set<Base_UserGrant>().Where(p => p.GrantID == ugrid && p.UserID == iUserId).FirstOrDefault();
                                        base_UserGrant.GrantEN = Convert.ToInt32(ugr["grantEn"]);
                                        dbContext.Entry(base_UserGrant).State = EntityState.Modified;                                        
                                    }
                                }
                                else
                                {
                                    errMsg = "提交的数据包含空值";
                                    return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                                }
                            }

                            if (dbContext.SaveChanges() < 0)
                            {
                                errMsg = "保存授权功能失败";
                                return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                            }
                        }
                        
                        ts.Complete();

                        return ResponseModelFactory.CreateSuccessModel(isSignKeyReturn);
                    }
                    catch (Exception ex)
                    {
                        LogHelper.SaveLog("错误:" + ex.Message);
                        return ResponseModelFactory.CreateReturnModel(isSignKeyReturn, Return_Code.F, ex.Message);
                    }
                }
            }

            [Authorize(Merches = "Normal,Heavy")]
            [ApiMethodAttribute(SignKeyEnum = SignKeyEnum.XCCloudUserCacheToken, SysIdAndVersionNo = false)]
            public object CheckStoreUser(Dictionary<string, object> dicParas)
            {
                string errMsg = string.Empty;
                string userId = dicParas.ContainsKey("userId") ? (dicParas["userId"] + "") : string.Empty;
                string state = dicParas.ContainsKey("state") ? (dicParas["state"] + "") : string.Empty;
                string reason = dicParas.ContainsKey("reason") ? (dicParas["reason"] + "") : string.Empty;
                string isAdmin = dicParas.ContainsKey("isAdmin") ? (dicParas["isAdmin"] + "") : string.Empty;
                string userType = dicParas.ContainsKey("userType") ? (dicParas["userType"] + "") : string.Empty;
                int iUserId, iState; 

                if (string.IsNullOrEmpty(userId))
                {
                    errMsg = "userId参数不能为空";
                    return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                }

                if (string.IsNullOrEmpty(state))
                {
                    errMsg = "审核状态state参数不能为空";
                    return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                }

                iUserId = Convert.ToInt32(userId);
                iState = Convert.ToInt32(state);
                if (iState == (int)WorkState.Pass) //审核通过
                {
                    if (!dicParas.ContainsKey("userGroup") || dicParas["userGroup"] == null)
                    {
                        errMsg = "工作组userGroup参数不能为空";
                        return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                    }

                    if (!dicParas.ContainsKey("userGrant") || dicParas["userGrant"] == null)
                    {
                        errMsg = "授权功能列表userGrant参数不能为空";
                        return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                    }
                }

                //开启EF事务
                using (TransactionScope ts = new TransactionScope())
                {
                    try
                    {
                        if (iState == (int)WorkState.Pass) //审核通过
                        {
                            //修改用户信息
                            Dictionary<string, object> userGroup = new Dictionary<string, object>((IDictionary<string, object>)dicParas["userGroup"], StringComparer.OrdinalIgnoreCase);
                            IBase_UserGroupService base_UserGroupService = BLLContainer.Resolve<IBase_UserGroupService>();
                            int ugid = Convert.ToInt32(userGroup["id"]);
                            if (!base_UserGroupService.Any(w => w.ID.Equals(ugid)))
                            {
                                errMsg = "工作组" + userGroup["groupName"] + "不存在";
                                return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                            }

                            IBase_UserInfoService userInfoService = BLLContainer.Resolve<IBase_UserInfoService>();
                            var base_UserInfo = userInfoService.GetModels(p => p.UserID.Equals(iUserId)).FirstOrDefault();
                            if (base_UserInfo == null)
                            {
                                errMsg = "该用户不存在";
                                return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                            }

                            base_UserInfo.UserGroupID = ugid;
                            base_UserInfo.AuditorTime = DateTime.Now;
                            base_UserInfo.Status = (int)UserStatus.Pass;
                            base_UserInfo.IsAdmin = !string.IsNullOrEmpty(isAdmin) ? Convert.ToInt32(isAdmin) : (int?)null;
                            base_UserInfo.UserType = !string.IsNullOrEmpty(userType) ? Convert.ToInt32(userType) : (int)UserType.Store;

                            string storeId = base_UserInfo.StoreID;
                            if (base_UserInfo.IsAdmin == 1 && userInfoService.Any(a => a.UserID != iUserId && a.IsAdmin == 1 && a.StoreID.Equals(storeId, StringComparison.OrdinalIgnoreCase)))
                            {
                                errMsg = "同一个门店只能有一个管理员";
                                return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                            }

                            if (!userInfoService.Update(base_UserInfo))
                            {
                                errMsg = "修改用户信息失败";
                                return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                            }

                            //添加或修改授权功能表
                            var dbContext = DbContextFactory.CreateByModelNamespace(typeof(Base_UserGrant).Namespace);
                            var userGrant = (object[])dicParas["userGrant"];
                            foreach (IDictionary<string, object> iUgr in userGrant)
                            {
                                if (iUgr != null)
                                {
                                    var ugr = new Dictionary<string, object>(iUgr, StringComparer.OrdinalIgnoreCase);
                                    int ugrid = Convert.ToInt32(ugr["id"]);
                                    if (!dbContext.Set<Base_UserGrant>().Any(w => w.GrantID == ugrid && w.UserID == iUserId))
                                    {
                                        var base_UserGrant = new Base_UserGrant();
                                        base_UserGrant.GrantID = ugrid;
                                        base_UserGrant.UserID = iUserId;
                                        base_UserGrant.GrantEN = Convert.ToInt32(ugr["grantEn"]);
                                        dbContext.Entry(base_UserGrant).State = EntityState.Added;
                                    }
                                    else
                                    {
                                        var base_UserGrant = dbContext.Set<Base_UserGrant>().Where(p => p.GrantID == ugrid && p.UserID == iUserId).FirstOrDefault();
                                        base_UserGrant.GrantEN = Convert.ToInt32(ugr["grantEn"]);
                                        dbContext.Entry(base_UserGrant).State = EntityState.Modified;
                                    }
                                }
                                else
                                {
                                    errMsg = "提交的数据包含空值";
                                    return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                                }
                            }

                            if (dbContext.SaveChanges() < 0)
                            {
                                errMsg = "保存授权功能失败";
                                return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                            }

                            //修改工单
                            IXC_WorkInfoService xC_WorkInfoService = BLLContainer.Resolve<IXC_WorkInfoService>();
                            var xC_WorkInfo = xC_WorkInfoService.GetModels(p => p.WorkType == (int)WorkType.UserCheck && p.SenderID == iUserId && p.WorkState == (int)WorkState.Pending).FirstOrDefault();
                            if (xC_WorkInfo == null)
                            {
                                errMsg = "未找到审核工单";
                                return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                            }

                            xC_WorkInfo.AuditTime = DateTime.Now;
                            xC_WorkInfo.WorkState = (int)WorkState.Pass;
                            xC_WorkInfo.AuditBody = "审核通过";
                            if (!xC_WorkInfoService.Update(xC_WorkInfo))
                            {
                                errMsg = "修改工单失败";
                                return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                            }

                            //添加日志
                            ILog_OperationService log_OperationService = BLLContainer.Resolve<ILog_OperationService>();
                            var log_Operation = new Log_Operation();
                            log_Operation.UserID = iUserId;
                            log_Operation.AuthorID = xC_WorkInfo.AuditorID;
                            log_Operation.Content = "审核通过";
                            if (!log_OperationService.Add(log_Operation))
                            {
                                errMsg = "添加日志失败";
                                return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                            }
                        }
                        else if (iState == (int)WorkState.Reject) //审核拒绝
                        {
                            //修改工单
                            IXC_WorkInfoService xC_WorkInfoService = BLLContainer.Resolve<IXC_WorkInfoService>();
                            var xC_WorkInfo = xC_WorkInfoService.GetModels(p => p.WorkType == (int)WorkType.UserCheck && p.SenderID == iUserId && p.WorkState == (int)WorkState.Pending).FirstOrDefault();
                            xC_WorkInfo.AuditTime = DateTime.Now;
                            xC_WorkInfo.WorkState = (int)WorkState.Reject;
                            xC_WorkInfo.AuditBody = "拒绝理由：" + reason;
                            if (!xC_WorkInfoService.Update(xC_WorkInfo))
                            {
                                errMsg = "修改工单失败";
                                return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                            }

                            //添加日志
                            ILog_OperationService log_OperationService = BLLContainer.Resolve<ILog_OperationService>();
                            var log_Operation = new Log_Operation();
                            log_Operation.UserID = iUserId;
                            log_Operation.AuthorID = xC_WorkInfo.AuditorID;
                            log_Operation.Content = "拒绝理由：" + reason;
                            if (!log_OperationService.Add(log_Operation))
                            {
                                errMsg = "添加日志失败";
                                return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                            }
                        }

                        ts.Complete();
                        return ResponseModelFactory.CreateSuccessModel(isSignKeyReturn);
                    }
                    catch (Exception ex)
                    {
                        LogHelper.SaveLog("错误:" + ex.Message);
                        return ResponseModelFactory.CreateReturnModel(isSignKeyReturn, Return_Code.F, ex.Message);
                    }
                }
            }

            [Authorize(Merches = "Normal,Heavy")]
            [ApiMethodAttribute(SignKeyEnum = SignKeyEnum.XCCloudUserCacheToken, SysIdAndVersionNo = false)]
            public object GetStoreUserGroup(Dictionary<string, object> dicParas)
            {
                string errMsg = string.Empty;
                string userId = dicParas.ContainsKey("userId") ? dicParas["userId"].ToString() : string.Empty;
                int iUserId;

                if (string.IsNullOrEmpty(userId))
                {
                    errMsg = "userId参数不能为空";
                    return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                }

                if (!Utils.isNumber(userId))
                {
                    errMsg = "userId参数格式不正确";
                    return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                }

                iUserId = Convert.ToInt32(userId);
                string sql = " exec SelectUserGroup @UserID";
                SqlParameter[] parameters = new SqlParameter[1];
                parameters[0] = new SqlParameter("@UserID", userId);

                IBase_UserInfoService base_UserInfoService = BLLContainer.Resolve<IBase_UserInfoService>();
                System.Data.DataSet ds = XCCloudBLL.ExecuteQuerySentence(sql, parameters);
                System.Data.DataTable dt = ds.Tables[0];
                if (dt.Rows.Count > 0)
                {
                    var UserInfoModel = base_UserInfoService.GetModels(p => p.UserID == iUserId).FirstOrDefault();
                    var list = Utils.GetModelList<Base_UserGroup>(ds.Tables[0]).ToList();
                    var linq = list.Select(o => new
                    {
                        ID = o.ID,
                        MerchID = o.MerchID,
                        GroupName = o.GroupName,
                        Note = o.Note,
                        Selected = (UserInfoModel.UserGroupID == o.ID) ? 1 : 0
                    });

                    return ResponseModelFactory.CreateSuccessModel(isSignKeyReturn, linq.ToList());
                }
                else
                {
                    return ResponseModelFactory.CreateFailModel(isSignKeyReturn, "工作组列表为空");
                }
            }

            [Authorize(Merches = "Normal,Heavy")]
            [ApiMethodAttribute(SignKeyEnum = SignKeyEnum.XCCloudUserCacheToken, SysIdAndVersionNo = false)]
            public object GetStoreUserGrant(Dictionary<string, object> dicParas)
            {
                string errMsg = string.Empty;
                string userId = dicParas.ContainsKey("userId") ? dicParas["userId"].ToString() : string.Empty;

                if (string.IsNullOrEmpty(userId))
                {
                    errMsg = "userId参数不能为空";
                    return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                }

                if (!Utils.isNumber(userId))
                {
                    errMsg = "userId参数格式不正确";
                    return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                }

                string sql = " exec SelectUserGrant @UserID";
                SqlParameter[] parameters = new SqlParameter[1];
                parameters[0] = new SqlParameter("@UserID", userId);

                System.Data.DataSet ds = XCCloudBLL.ExecuteQuerySentence(sql, parameters);
                System.Data.DataTable dt = ds.Tables[0];
                if (dt.Rows.Count <= 0)
                {
                    errMsg = "授权功能列表为空";
                    return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);                    
                }

                var list = Utils.GetModelList<UserGrantModel>(ds.Tables[0]);
                return ResponseModelFactory.CreateSuccessModel(isSignKeyReturn, list);
            }

        #endregion
    }
}