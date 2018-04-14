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
using XCCloudService.Business.XCCloud;
using XCCloudService.Common;
using XCCloudService.Common.Enum;
using XCCloudService.DAL;
using XCCloudService.Model.CustomModel.XCCloud;
using XCCloudService.Model.CustomModel.XCCloud.User;
using XCCloudService.Model.XCCloud;
using XCCloudService.WeiXin.Common;
using XCCloudService.WeiXin.WeixinOAuth;

namespace XXCloudService.WeiXin.Api
{
    /// <summary>
    /// Users 的摘要说明
    /// </summary>
    public class Users : ApiBase
    {
        private bool checkParas(Dictionary<string, object> dicParas, out int userId, out int authorId, out string errMsg)
        {
            errMsg = string.Empty;
            userId = 0;
            authorId = 0;

            try
            {                
                string workId = dicParas.ContainsKey("workId") ? dicParas["workId"].ToString() : string.Empty;

                if (string.IsNullOrEmpty(workId))
                {
                    errMsg = "工单号workId参数不能为空";
                    return false;
                }

                //验证工单
                IXC_WorkInfoService xC_WorkInfoService = BLLContainer.Resolve<IXC_WorkInfoService>();
                var xC_WorkInfoList = xC_WorkInfoService.GetModels(p => p.WorkID.ToString().Equals(workId, StringComparison.OrdinalIgnoreCase));
                int xC_WorkInfoCount = xC_WorkInfoList.Count<XC_WorkInfo>();
                if (xC_WorkInfoCount == 0)
                {
                    errMsg = "工单号" + workId + "不存在";
                    return false;
                }

                //验证用户
                var xC_WorkInfo = xC_WorkInfoList.FirstOrDefault<XC_WorkInfo>();
                IBase_UserInfoService userInfoService = BLLContainer.Resolve<IBase_UserInfoService>();
                var userList = userInfoService.GetModels(p => p.UserID.Equals(xC_WorkInfo.SenderID.Value));
                if (userList.Count<Base_UserInfo>() == 0)
                {
                    errMsg = "工单号" + workId + "的用户ID不存在";
                    return false;
                }

                userId = userList.FirstOrDefault<Base_UserInfo>().UserID;

                //验证审核人
                userList = userInfoService.GetModels(p => p.UserID.Equals(xC_WorkInfo.AuditorID.Value));
                if (userList.Count<Base_UserInfo>() == 0)
                {
                    errMsg = "工单号" + workId + "的审核人ID不存在";
                    return false;
                }

                authorId = userList.FirstOrDefault<Base_UserInfo>().UserID;

                return true;
            }
            catch (Exception e)
            {
                errMsg = e.Message;
                return false;
            }            
        }

        [ApiMethodAttribute(SignKeyEnum = SignKeyEnum.MethodToken)]
        public object GetUserGroup(Dictionary<string, object> dicParas)
        {
            string errMsg = string.Empty;
            int userId, authorId;
            string workId = dicParas.ContainsKey("workId") ? dicParas["workId"].ToString() : string.Empty;

            if (!checkParas(dicParas, out userId, out authorId, out errMsg))
            {
                LogHelper.SaveLog("错误:" + errMsg);
                return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
            }

            string sql = " exec SelectUserGroup @UserID";
            SqlParameter[] parameters = new SqlParameter[1];
            parameters[0] = new SqlParameter("@UserID", userId);

            System.Data.DataSet ds = XCCloudBLL.ExecuteQuerySentence(sql, parameters);
            System.Data.DataTable dt = ds.Tables[0];
            if (dt.Rows.Count > 0)
            {
                var list = Utils.GetModelList<Base_UserGroup>(ds.Tables[0]).ToList();
                return ResponseModelFactory.CreateSuccessModel(isSignKeyReturn, list);
            }
            else
            {
                return ResponseModelFactory.CreateFailModel(isSignKeyReturn, "工作组列表为空");
            }
        }        

        [ApiMethodAttribute(SignKeyEnum = SignKeyEnum.MethodToken)]
        public object GetUserGrant(Dictionary<string, object> dicParas)
        {
            string errMsg = string.Empty;
            int userId, authorId;

            if (!checkParas(dicParas, out userId, out authorId, out errMsg))
            {
                LogHelper.SaveLog("错误:" + errMsg);
                return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
            }

            string sql = " exec SelectUserGrant @UserID";
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

        [ApiMethodAttribute(SignKeyEnum = SignKeyEnum.MethodToken)]
        public object SaveUserInfo(Dictionary<string, object> dicParas)
        {
            string errMsg = string.Empty;            
            int userId, authorId;
            string workId = dicParas.ContainsKey("workId") ? dicParas["workId"].ToString() : string.Empty;
            string state = dicParas.ContainsKey("state") ? dicParas["state"].ToString() : string.Empty;
            string userType = dicParas.ContainsKey("userType") ? dicParas["userType"].ToString() : string.Empty;
            string reason = dicParas.ContainsKey("reason") ? dicParas["reason"].ToString() : string.Empty;
            string isAdmin = dicParas.ContainsKey("isAdmin") ? dicParas["isAdmin"].ToString() : string.Empty;

            if (string.IsNullOrEmpty(state))
            {
                errMsg = "审核状态state参数不能为空";
                return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
            }

            if (state == ((int)WorkState.Pass).ToString()) //审核通过
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

            if (!checkParas(dicParas, out userId, out authorId, out errMsg))
            {
                LogHelper.SaveLog("错误:" + errMsg);
                return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
            }

            //开启EF事务
            using (TransactionScope ts = new TransactionScope())
            {
                try
                {
                    if (state == ((int)WorkState.Pass).ToString()) //审核通过
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
                        var base_UserInfo = userInfoService.GetModels(p => p.UserID.Equals(userId)).FirstOrDefault<Base_UserInfo>();
                        base_UserInfo.UserGroupID = ugid;
                        base_UserInfo.Auditor = authorId;
                        base_UserInfo.AuditorTime = DateTime.Now;
                        base_UserInfo.Status = (int)UserStatus.Pass;
                        base_UserInfo.IsAdmin = !string.IsNullOrEmpty(isAdmin) ? Convert.ToInt32(isAdmin) : (int?)null;
                        base_UserInfo.UserType = !string.IsNullOrEmpty(userType) ? Convert.ToInt32(userType) : (int)UserType.Store;

                        string storeId = base_UserInfo.StoreID;
                        if (base_UserInfo.IsAdmin == 1 && userInfoService.Any(a => a.UserID != userId && a.IsAdmin == 1 && a.StoreID.Equals(storeId, StringComparison.OrdinalIgnoreCase)))
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
                                if (!dbContext.Set<Base_UserGrant>().Any(w => w.GrantID.Value.Equals(ugrid) && w.UserID.Value.Equals(userId)))
                                {
                                    var base_UserGrant = new Base_UserGrant();
                                    base_UserGrant.GrantID = ugrid;
                                    base_UserGrant.UserID = userId;
                                    base_UserGrant.GrantEN = Convert.ToInt32(ugr["grantEn"]);
                                    dbContext.Entry(base_UserGrant).State = EntityState.Added;
                                }
                                else
                                {
                                    var base_UserGrant = dbContext.Set<Base_UserGrant>().Where(p => p.GrantID == ugrid && p.UserID == userId).FirstOrDefault();
                                    base_UserGrant.GrantEN = Convert.ToInt32(ugr["grantEn"]);
                                    dbContext.Entry(base_UserGrant).State = EntityState.Modified;                                    
                                }
                            }
                        }

                        if (dbContext.SaveChanges() < 0)
                        {
                            errMsg = "保存授权功能失败";
                            return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                        }

                        //修改工单
                        IXC_WorkInfoService xC_WorkInfoService = BLLContainer.Resolve<IXC_WorkInfoService>();
                        var xC_WorkInfo = xC_WorkInfoService.GetModels(p => p.WorkID.ToString().Equals(workId, StringComparison.OrdinalIgnoreCase)).FirstOrDefault<XC_WorkInfo>();
                        xC_WorkInfo.AuditorID = authorId;
                        xC_WorkInfo.AuditTime = DateTime.Now;
                        xC_WorkInfo.WorkState = (int)WorkState.Pass;
                        xC_WorkInfo.AuditBody = "审核通过";
                        xC_WorkInfo.WorkType = (int)WorkType.UserCheck;
                        if (!xC_WorkInfoService.Update(xC_WorkInfo))
                        {
                            errMsg = "修改工单失败";
                            return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                        }

                        //添加日志
                        ILog_OperationService log_OperationService = BLLContainer.Resolve<ILog_OperationService>();
                        var log_Operation = new Log_Operation();
                        log_Operation.UserID = userId;
                        log_Operation.AuthorID = authorId;
                        log_Operation.Content = "审核通过";
                        if (!log_OperationService.Add(log_Operation))
                        {
                            errMsg = "添加日志失败";
                            return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                        }
                    }
                    else if (state == ((int)WorkState.Reject).ToString()) //审核拒绝
                    {
                        //修改工单
                        IXC_WorkInfoService xC_WorkInfoService = BLLContainer.Resolve<IXC_WorkInfoService>();
                        var xC_WorkInfo = xC_WorkInfoService.GetModels(p => p.WorkID.ToString().Equals(workId, StringComparison.OrdinalIgnoreCase)).FirstOrDefault<XC_WorkInfo>();
                        xC_WorkInfo.AuditorID = authorId;
                        xC_WorkInfo.AuditTime = DateTime.Now;
                        xC_WorkInfo.WorkState = (int)WorkState.Reject;
                        xC_WorkInfo.AuditBody = "拒绝理由：" + reason;
                        xC_WorkInfo.WorkType = (int)WorkType.UserCheck;
                        if (!xC_WorkInfoService.Update(xC_WorkInfo))
                        {
                            errMsg = "修改工单失败";
                            return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                        }

                        //添加日志
                        ILog_OperationService log_OperationService = BLLContainer.Resolve<ILog_OperationService>();
                        var log_Operation = new Log_Operation();
                        log_Operation.UserID = userId;
                        log_Operation.AuthorID = authorId;
                        log_Operation.Content = "拒绝理由：" + reason;
                        if (!log_OperationService.Add(log_Operation))
                        {
                            errMsg = "添加日志失败";
                            return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                        }
                    }
                    else
                    {
                        errMsg = "不明确的审核状态";
                        return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                    }
                    
                    ts.Complete();
                    return ResponseModelFactory.CreateSuccessModel(isSignKeyReturn);
                }
                catch(Exception ex)
                {
                    LogHelper.SaveLog("错误:" + ex.Message);
                    return ResponseModelFactory.CreateReturnModel(isSignKeyReturn, Return_Code.F, ex.Message);
                }                
            }
        }

        private bool getWxUsers<T>(string nextOpenId, out UserInfoCollection<T> userInfoCollection, out string errMsg)
        {
            userInfoCollection = null;
            errMsg = string.Empty;
            
            string accsess_token = string.Empty;
            string openId = string.Empty;
            if (TokenMana.GetAccessToken(out accsess_token))
            {
                string url = "https://api.weixin.qq.com/cgi-bin/user/get?access_token={0}&next_openid={1}";
                url = string.Format(url, accsess_token, nextOpenId);
                string list = Utils.WebClientDownloadString(url);
                Dictionary<string, object> dict = new Dictionary<string, object>();
                if (WeiXinJsonHelper.GetResponseJsonResult(list, ref dict))
                {                    
                    var openidarr = ((Dictionary<string, object>)dict["data"])["openid"] as object[];
                    if (openidarr != null && openidarr.Length > 0)
                    {
                        var userinfoarr = new object[openidarr.Length];
                        for (int i = 0; i < openidarr.Length; i++)
                        {
                            userinfoarr[i] = new { openid = openidarr[i].ToString() };
                        }
                        var pushData = new { user_list = userinfoarr };
                        string json = Utils.SerializeObject(pushData).ToString();
                        url = "https://api.weixin.qq.com/cgi-bin/user/info/batchget?access_token={0}";
                        url = string.Format(url, accsess_token);
                        string str = Utils.HttpPost(url, json);
                        if (WeiXinJsonHelper.GetResponseJsonResult(str, ref dict))
                        {
                            userInfoCollection = Utils.DataContractJsonDeserializer<UserInfoCollection<T>>(str);                            
                        }
                        else
                        {
                            errMsg = "获取用户基本信息出错：" + dict["errcode"].ToString() + " " + dict["errmsg"].ToString();
                            return false;
                        }
                    }
                }
                else
                {
                    errMsg = "获取用户列表出错：" + dict["errcode"].ToString() + " " + dict["errmsg"].ToString();
                    return false;
                }

                return true;
            }
            else
            {
                errMsg = "获取微信令牌出错";
                return false;
            }
        }

        private bool getWxFans<T>(string tagName, string nextOpenId, out UserInfoCollection<T> userInfoCollection, out string errMsg)
        {
            userInfoCollection = null;
            errMsg = string.Empty;

            string accsess_token = string.Empty;
            string openId = string.Empty;
            if (TokenMana.GetAccessToken(out accsess_token))
            {
                string url = "https://api.weixin.qq.com/cgi-bin/tags/get?access_token={0}";
                url = string.Format(url, accsess_token);
                Dictionary<string, object> dict = new Dictionary<string, object>();
                string list = Utils.WebClientDownloadString(url);
                if (WeiXinJsonHelper.GetResponseJsonResult(list, ref dict))
                {
                    var tagId = 0;
                    var tags = (object[])dict["tags"];
                    if (tags != null && tags.Length > 0)
                    {
                        foreach (IDictionary<string, object> el in tags)
                        {
                            if (el != null)
                            {
                                var dicPara = new Dictionary<string, object>(el, StringComparer.OrdinalIgnoreCase);
                                string name = dicPara.ContainsKey("name") ? dicPara["name"].ToString() : string.Empty;
                                if (name.Equals(tagName, StringComparison.OrdinalIgnoreCase))
                                {
                                    int.TryParse(dicPara["id"].ToString(), out tagId);
                                    break;
                                }
                            }
                        }
                    }

                    if (tagId == 0)
                    {
                        errMsg = "该标签不存在";
                        return false;
                    }

                    var pushData = new { tagid = tagId, next_openid = nextOpenId };
                    string json = Utils.SerializeObject(pushData).ToString();
                    url = "https://api.weixin.qq.com/cgi-bin/user/tag/get?access_token={0}";
                    url = string.Format(url, accsess_token);
                    string str = Utils.HttpPost(url, json);
                    if (WeiXinJsonHelper.GetResponseJsonResult(str, ref dict))
                    {
                        var openidarr = ((Dictionary<string, object>)dict["data"])["openid"] as object[];
                        if (openidarr != null && openidarr.Length > 0)
                        {
                            var userinfoarr = new object[openidarr.Length];
                            for (int i = 0; i < openidarr.Length; i++)
                            {
                                userinfoarr[i] = new { openid = openidarr[i].ToString() };
                            }
                            var pushData2 = new { user_list = userinfoarr };
                            json = Utils.SerializeObject(pushData2).ToString();
                            url = "https://api.weixin.qq.com/cgi-bin/user/info/batchget?access_token={0}";
                            url = string.Format(url, accsess_token);
                            str = Utils.HttpPost(url, json);
                            if (WeiXinJsonHelper.GetResponseJsonResult(str, ref dict))
                            {
                                userInfoCollection = Utils.DataContractJsonDeserializer<UserInfoCollection<T>>(str);
                            }
                            else
                            {
                                errMsg = "获取用户基本信息出错：" + dict["errcode"].ToString() + " " + dict["errmsg"].ToString();
                                return false;
                            }
                        }
                    }
                    else
                    {
                        errMsg = "获取标签粉丝信息出错:" + dict["errcode"].ToString() + " " + dict["errmsg"].ToString();
                        return false;
                    }
                }
                else
                {
                    errMsg = "获取标签列表出错：" + dict["errcode"].ToString() + " " + dict["errmsg"].ToString();
                    return false;
                }

                return true;
            }
            else
            {
                errMsg = "获取微信令牌出错";
                return false;
            }
        }

        [ApiMethodAttribute(SignKeyEnum = SignKeyEnum.MethodToken)]
        public object GetWxUserList(Dictionary<string, object> dicParas)
        {
            string errMsg = string.Empty;
            string nextOpenId = dicParas.ContainsKey("nextOpenId") ? dicParas["nextOpenId"].ToString() : string.Empty;

            UserInfoCollection<UserInfoModel> userInfoCollection = null;
            if (!getWxUsers<UserInfoModel>(nextOpenId, out userInfoCollection, out errMsg))
            {
                return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
            }

            if (userInfoCollection != null && userInfoCollection.UserInfoList != null)
            {
                var subscribeusers = userInfoCollection.UserInfoList.Exists(p => p.Subscribe.Equals(1)) ?
                    userInfoCollection.UserInfoList.Where(w => w.Subscribe.Equals(1)).ToList<UserInfoModel>() : default(List<UserInfoModel>);
                return ResponseModelFactory.CreateSuccessModel(isSignKeyReturn, subscribeusers);
            }

            return ResponseModelFactory.CreateSuccessModel(isSignKeyReturn);
        }        

        [ApiMethodAttribute(SignKeyEnum = SignKeyEnum.MethodToken)]
        public object GetWxUserInfoBatch(Dictionary<string, object> dicParas)
        {
            string errMsg = string.Empty;
            string nextOpenId = dicParas.ContainsKey("nextOpenId") ? dicParas["nextOpenId"].ToString() : string.Empty;

            UserInfoCollection<UserInfoDetailModel> userInfoCollection = null;
            if (!getWxFans<UserInfoDetailModel>("员工组", nextOpenId, out userInfoCollection, out errMsg))
            {
                return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
            }

            if (userInfoCollection != null && userInfoCollection.UserInfoList != null)
            {
                var subscribeusers = userInfoCollection.UserInfoList.Exists(p => p.Subscribe.Equals(1)) ?
                    userInfoCollection.UserInfoList.Where(w => w.Subscribe.Equals(1)).ToList<UserInfoDetailModel>() : default(List<UserInfoDetailModel>);

                //绑定UserID
                UserInfoCacheModel userInfoCacheModel = null;
                foreach (var su in subscribeusers)
                {
                    if (UserBusiness.IsEffectiveXcUser(su.OpenID, out userInfoCacheModel))
                    {
                        su.UserID = userInfoCacheModel.UserID;
                    }
                }

                return ResponseModelFactory.CreateSuccessModel(isSignKeyReturn, subscribeusers);
            }

            return ResponseModelFactory.CreateSuccessModel(isSignKeyReturn);            
        }
    }
}