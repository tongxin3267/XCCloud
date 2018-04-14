using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using XCCloudService.Base;
using XCCloudService.BLL.CommonBLL;
using XCCloudService.BLL.Container;
using XCCloudService.BLL.IBLL.XCCloud;
using XCCloudService.BLL.XCCloud;
using XCCloudService.Business.XCCloud;
using XCCloudService.Business.XCGameMana;
using XCCloudService.Common;
using XCCloudService.Common.Enum;
using XCCloudService.Model.CustomModel.XCCloud;
using XCCloudService.Model.XCCloud;

namespace XXCloudService.Api.XCCloud
{
    /// <summary>
    /// Login 的摘要说明
    /// </summary>
    public class Login : ApiBase
    {
        [ApiMethodAttribute(SignKeyEnum = SignKeyEnum.MethodToken, SysIdAndVersionNo = false)]
        public object CheckUser(Dictionary<string, object> dicParas)
        {
            try
            {
                string errMsg = string.Empty;
                string token = string.Empty;
                string userName = dicParas.ContainsKey("userName") ? dicParas["userName"].ToString() : string.Empty;
                string password = dicParas.ContainsKey("password") ? dicParas["password"].ToString() : string.Empty;

                if (string.IsNullOrWhiteSpace(userName))
                {
                    errMsg = "用户名不能为空";
                    return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                }

                if (string.IsNullOrWhiteSpace(password))
                {
                    errMsg = "密码不能为空";
                    return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                }

                password = Utils.MD5(password);
                UserLogResponseModel userLogResponseModel = new UserLogResponseModel();
                IBase_UserInfoService base_UserInfoService = BLLContainer.Resolve<IBase_UserInfoService>();                                                
                if (base_UserInfoService.Any(p => p.LogName.Equals(userName, StringComparison.OrdinalIgnoreCase) && p.LogPassword.Equals(password, StringComparison.OrdinalIgnoreCase)))
                {
                    var base_UserInfoModel = base_UserInfoService.GetModels(p => p.LogName.Equals(userName, StringComparison.OrdinalIgnoreCase) && p.LogPassword.Equals(password, StringComparison.OrdinalIgnoreCase)).FirstOrDefault<Base_UserInfo>();
                    int userId = base_UserInfoModel.UserID;
                    int userType = (int)base_UserInfoModel.UserType;
                    int logType = (int)RoleType.XcUser; //默认普通员工登录
                    int isXcAdmin = 0; int.TryParse(Convert.ToString(base_UserInfoModel.Auditor), out isXcAdmin);

                    if (userType == (int)UserType.Xc && isXcAdmin == 0)
                    {
                        logType = (int)RoleType.XcAdmin;
                        userLogResponseModel.Token = XCCloudUserTokenBusiness.SetUserToken(userId.ToString(), logType);
                    }
                    else if (userType == (int)UserType.Store || base_UserInfoModel.UserType == (int)UserType.StoreBoss)
                    {
                        logType = (int)RoleType.StoreUser;
                        string storeId = base_UserInfoModel.StoreID;
                        IBase_StoreInfoService base_StoreInfoService = BLLContainer.Resolve<IBase_StoreInfoService>();
                        if (!base_StoreInfoService.Any(a => a.StoreID.Equals(storeId, StringComparison.OrdinalIgnoreCase)))
                        {
                            errMsg = "该门店ID不存在";
                            return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                        }
                        string merchId = base_StoreInfoService.GetModels(p => p.StoreID.Equals(storeId, StringComparison.OrdinalIgnoreCase)).FirstOrDefault().MerchID;
                        var dataModel = new UserDataModel { StoreID = storeId, MerchID = merchId };                        
                        userLogResponseModel.Token = XCCloudUserTokenBusiness.SetUserToken(userId.ToString(), logType, dataModel);
                    }
                    else
                    {
                        logType = (int)RoleType.MerchUser;
                        string merchId = base_UserInfoModel.MerchID;                        
                        IBase_MerchantInfoService base_MerchantInfoService = BLLContainer.Resolve<IBase_MerchantInfoService>();
                        if (!base_MerchantInfoService.Any(p => p.MerchID.Equals(merchId, StringComparison.OrdinalIgnoreCase)))
                        {
                            errMsg = "该商户ID不存在";
                            return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                        }
                        var base_MerchantInfoModel = base_MerchantInfoService.GetModels(p => p.MerchID.Equals(merchId, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
                        var dataModel = new MerchDataModel { MerchType = base_MerchantInfoModel.MerchType, CreateType = base_MerchantInfoModel.CreateType, CreateUserID = base_MerchantInfoModel.CreateUserID };
                        userLogResponseModel.Token = XCCloudUserTokenBusiness.SetUserToken(userId.ToString(), logType, dataModel);
                    }
                    
                    userLogResponseModel.LogType = logType;
                    userLogResponseModel.UserType = userType;
                    return ResponseModelFactory.CreateSuccessModel(isSignKeyReturn, userLogResponseModel);
                }
                else
                {
                    errMsg = "用户名或密码错误";
                    return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                }                
            }
            catch (Exception e)
            {
                return ResponseModelFactory.CreateReturnModel(isSignKeyReturn, Return_Code.F, e.Message);
            } 
        }

    }
}