using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using XCCloudService.Base;
using XCCloudService.BLL.Container;
using XCCloudService.BLL.IBLL.XCGameManager;
using XCCloudService.Business;
using XCCloudService.Business.Common;
using XCCloudService.Business.XCGameMana;
using XCCloudService.CacheService;
using XCCloudService.Common;
using XCCloudService.Model.XCGameManager;

namespace XXCloudService.Api.XCGameMana
{
    /// <summary>
    /// UserRegister 的摘要说明
    /// </summary>
    public class UserRegister : ApiBase
    {
        [ApiMethodAttribute(SignKeyEnum = SignKeyEnum.XCGameUserCacheToken)]
        public object getUserRegister(Dictionary<string, object> dicParas)
        {
            string errMsg = string.Empty;
            string xcGameDBName = string.Empty;
            string Mobile = dicParas.ContainsKey("mobile") ? dicParas["mobile"].ToString() : string.Empty;
            string StoreId = dicParas.ContainsKey("storeId") ? dicParas["storeId"].ToString() : string.Empty;
            string UserName = dicParas.ContainsKey("UserName") ? dicParas["UserName"].ToString() : string.Empty;
            string PassWord = dicParas.ContainsKey("PassWord") ? dicParas["PassWord"].ToString() : string.Empty;
            string smsCode = dicParas.ContainsKey("smsCode") ? dicParas["smsCode"].ToString() : string.Empty;
            string Pass = Utils.MD5(PassWord);
            StoreBusiness store = new StoreBusiness();
            if (!store.IsEffectiveStore(StoreId, out xcGameDBName, out errMsg))
            {
                return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, errMsg);
            }
            string key = Mobile + "_" + smsCode;
 
            //判断缓存验证码是否正确
            if (!FilterMobileBusiness.IsTestSMS && !FilterMobileBusiness.ExistMobile(Mobile))
            {
                if (!SMSCodeCache.IsExist(key))
                {
                    return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "短信验证码无效");
                }
            }
            IUserRegisterService userregisterService = BLLContainer.Resolve<IUserRegisterService>();
            //判断用户名是否存在
            var userlist = userregisterService.GetModels(p => p.UserName == UserName).ToList();
            if (userlist.Count > 0)
            {
                return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "该用户名已经存在");
            }
            //判断用户是否注册
            var menulist = userregisterService.GetModels(p => p.Mobile == Mobile && p.StoreId == StoreId).ToList();
            if (menulist.Count > 0)
            {
                return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "该用户已注册");
            }
            xcGameDBName = "XCGameManagerDB";
            string sql = "exec InsertUserRegister @UserName,@PassWord,@Mobile,@StoreId,@Return output ";
            SqlParameter[] parameters = new SqlParameter[5];
            parameters[0] = new SqlParameter("@UserName", UserName);
            parameters[1] = new SqlParameter("@PassWord", Pass);
            parameters[2] = new SqlParameter("@Mobile", Mobile);
            parameters[3] = new SqlParameter("@StoreId", StoreId);
            parameters[4] = new SqlParameter("@Return", 0);
            parameters[4].Direction = System.Data.ParameterDirection.Output;
            t_UserRegister userregister = userregisterService.SqlQuery(sql, xcGameDBName, parameters).FirstOrDefault<t_UserRegister>();
            if (userregister == null)
            {
                return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "用户添加异常");
            }
            return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.T, "");
        }
    }
}