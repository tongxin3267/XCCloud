using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using XCCloudService.BLL.CommonBLL;
using XCCloudService.CacheService;
using XCCloudService.Business;
using XCCloudService.Base;
using XCCloudService.Model.CustomModel.XCCloud;
using XCCloudService.Common;
using XCCloudService.BLL.IBLL.XCCloud;
using XCCloudService.BLL.Container;
using System.Data.SqlClient;
using System.Transactions;
using XCCloudService.Model.XCCloud;
using XCCloudService.Common.Enum;
using XCCloudService.DAL;
using System.Data.Entity;
using XXCloudService.Api.XCCloud.Common;

namespace XXCloudService.Api.XCCloud
{
    [Authorize(Roles = "XcUser,XcAdmin,MerchUser")]
    /// <summary>
    /// UserGroup 的摘要说明
    /// </summary>
    public class UserGroup : ApiBase
    {
        
        [ApiMethodAttribute(SignKeyEnum = SignKeyEnum.XCCloudUserCacheToken, SysIdAndVersionNo = false)]
        public object GetUserGroupList(Dictionary<string, object> dicParas)
        {
            try
            {
                string errMsg = string.Empty;
                string logId = string.Empty;
                int userId = (dicParas.ContainsKey("userId") && Utils.isNumber(dicParas["userId"])) ? Convert.ToInt32(dicParas["userId"]) : 0;

                XCCloudUserTokenModel userTokenKeyModel = (XCCloudUserTokenModel)dicParas[Constant.XCCloudUserTokenModel];

                if (userTokenKeyModel.LogType == (int)RoleType.MerchUser)
                {
                    logId = userTokenKeyModel.LogId;
                }
               
                //EF左关联
                var dbContext = DbContextFactory.CreateByModelNamespace(typeof(Base_UserGroup).Namespace);
                var base_UserGroup = (from a in dbContext.Set<Base_UserGroup>()
                                      join b in dbContext.Set<Base_UserInfo>().Where(p => p.UserID == userId) on a.ID equals b.UserGroupID into t
                                      from b in t.DefaultIfEmpty()
                                      where a.MerchID.Equals(logId, StringComparison.OrdinalIgnoreCase)
                                      select new UserGroupModel
                                      {
                                          ID = a.ID,
                                          GroupName = a.GroupName,
                                          Note = a.Note,
                                          UserState = (b.UserID > 0 ? 1 : 0)
                                      }).ToList();

                return ResponseModelFactory.CreateSuccessModel(isSignKeyReturn, base_UserGroup);
            }
            catch (Exception e)
            {
                return ResponseModelFactory.CreateReturnModel(isSignKeyReturn, Return_Code.F, e.Message);
            }
        }        

        [ApiMethodAttribute(SignKeyEnum = SignKeyEnum.XCCloudUserCacheToken, SysIdAndVersionNo = false)]
        public object GetUserGroup(Dictionary<string, object> dicParas)
        {
            try
            {
                string errMsg = string.Empty;
                string groupId = dicParas.ContainsKey("groupId") ? dicParas["groupId"].ToString() : string.Empty;
                string logId = string.Empty;

                XCCloudUserTokenModel userTokenKeyModel = (XCCloudUserTokenModel)dicParas[Constant.XCCloudUserTokenModel];

                if (userTokenKeyModel.LogType == (int)RoleType.MerchUser)
                {
                    logId = userTokenKeyModel.LogId;
                }

                string sql = " exec  SelectUserGroupGrant @GroupID,@MerchID";
                SqlParameter[] parameters = new SqlParameter[2];
                parameters[0] = new SqlParameter("@GroupID", groupId);
                parameters[1] = new SqlParameter("@MerchID", logId);
                if (userTokenKeyModel.LogType == (int)RoleType.XcAdmin)
                {
                    sql = " exec  SelectFunctionForXA @GroupID";
                    parameters = new SqlParameter[1];
                    parameters[0] = new SqlParameter("@GroupID", groupId);
                }

                //返回商户信息和功能菜单信息
                System.Data.DataSet ds = XCCloudBLL.ExecuteQuerySentence(sql, parameters);
                if (ds.Tables.Count != 2)
                {
                    errMsg = "获取数据异常";
                    return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                }

                var userGroupModel = Utils.GetModelList<UserGroupModel>(ds.Tables[0]).FirstOrDefault() ?? new UserGroupModel();
                userGroupModel.UserGroupGrants = Utils.GetModelList<UserGroupGrantModel>(ds.Tables[1]);

                //实例化一个根节点
                UserGroupGrantModel rootRoot = new UserGroupGrantModel();
                rootRoot.ParentID = 0;
                TreeHelper.LoopToAppendChildren(userGroupModel.UserGroupGrants, rootRoot);
                userGroupModel.UserGroupGrants = rootRoot.Children;

                return ResponseModelFactory.CreateSuccessModel(isSignKeyReturn, userGroupModel);
            }
            catch (Exception e)
            {
                return ResponseModelFactory.CreateReturnModel(isSignKeyReturn, Return_Code.F, e.Message);
            }
        }

        [ApiMethodAttribute(SignKeyEnum = SignKeyEnum.XCCloudUserCacheToken, SysIdAndVersionNo = false)]
        public object AddUserGroup(Dictionary<string, object> dicParas)
        {
            try
            {
                string errMsg = string.Empty;
                string logId = string.Empty;

                XCCloudUserTokenModel userTokenKeyModel = (XCCloudUserTokenModel)dicParas[Constant.XCCloudUserTokenModel];

                if (userTokenKeyModel.LogType == (int)RoleType.MerchUser)
                {
                    logId = userTokenKeyModel.LogId;
                }

                string groupName = dicParas.ContainsKey("groupName") ? dicParas["groupName"].ToString() : string.Empty;
                string note = dicParas.ContainsKey("note") ? dicParas["note"].ToString() : string.Empty;
                object[] userGroupGrants = dicParas.ContainsKey("userGroupGrants") ? (object[])dicParas["userGroupGrants"] : null;

                //验证参数
                if (string.IsNullOrEmpty(groupName))
                {
                    errMsg = "工作组名称不能为空";
                    return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                }

                if (!string.IsNullOrEmpty(groupName) && groupName.Length > 50)
                {
                    errMsg = "工作组名称不能超过50字";
                    return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                }

                if (!string.IsNullOrEmpty(note) && note.Length > 500)
                {
                    errMsg = "工作组描述不能超过500字";
                    return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                }

                //开启EF事务
                using (TransactionScope ts = new TransactionScope())
                {
                    try
                    {
                        IBase_UserGroupService base_UserGroupService = BLLContainer.Resolve<IBase_UserGroupService>();
                        var base_UserGroup = new Base_UserGroup { GroupName = groupName, MerchID = logId, Note = note };
                        if (!base_UserGroupService.Add(base_UserGroup))
                        {
                            errMsg = "更新数据库失败";
                            return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                        }

                        if (userGroupGrants != null && userGroupGrants.Count() > 0)
                        {
                            var dbContext = DbContextFactory.CreateByModelNamespace(typeof(Base_UserGroup_Grant).Namespace);
                            foreach (IDictionary<string, object> el in userGroupGrants)
                            {
                                if (el != null)
                                {
                                    var dicPara = new Dictionary<string, object>(el, StringComparer.OrdinalIgnoreCase);
                                    string functionId = dicPara.ContainsKey("functionId") ? dicPara["functionId"].ToString() : string.Empty;
                                    string isAllow = dicPara.ContainsKey("isAllow") ? dicPara["isAllow"].ToString() : string.Empty;
                                    var base_UserGroup_Grant = new Base_UserGroup_Grant { FunctionID = Convert.ToInt32(functionId), GroupID = base_UserGroup.ID, IsAllow = !string.IsNullOrEmpty(isAllow) ? Convert.ToInt32(isAllow) : default(int?) };
                                    dbContext.Entry(base_UserGroup_Grant).State = EntityState.Added;                                    
                                }
                                else
                                {
                                    errMsg = "提交数据包含空对象";
                                    return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                                }
                            }

                            if (dbContext.SaveChanges() < 0)
                            {
                                errMsg = "更新数据库失败";
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

                return ResponseModelFactory.CreateSuccessModel(isSignKeyReturn);
            }
            catch (Exception e)
            {
                return ResponseModelFactory.CreateReturnModel(isSignKeyReturn, Return_Code.F, e.Message);
            }
        }

        [ApiMethodAttribute(SignKeyEnum = SignKeyEnum.XCCloudUserCacheToken, SysIdAndVersionNo = false)]
        public object EditUserGroup(Dictionary<string, object> dicParas)
        {
            try
            {
                string errMsg = string.Empty;
                string groupId = dicParas.ContainsKey("groupId") ? dicParas["groupId"].ToString() : string.Empty;
                string groupName = dicParas.ContainsKey("groupName") ? dicParas["groupName"].ToString() : string.Empty;
                string note = dicParas.ContainsKey("note") ? dicParas["note"].ToString() : string.Empty;
                object[] userGroupGrants = dicParas.ContainsKey("userGroupGrants") ? (object[])dicParas["userGroupGrants"] : null;

                //验证参数 
                if (string.IsNullOrEmpty(groupId))
                {
                    errMsg = "工作组Id不能为空";
                    return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                }

                if (string.IsNullOrEmpty(groupName))
                {
                    errMsg = "工作组名称不能为空";
                    return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                }

                if (!string.IsNullOrEmpty(groupName) && groupName.Length > 50)
                {
                    errMsg = "工作组名称不能超过50字";
                    return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                }

                if (!string.IsNullOrEmpty(note) && note.Length > 500)
                {
                    errMsg = "工作组描述不能超过500字";
                    return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                }

                //开启EF事务
                using (TransactionScope ts = new TransactionScope())
                {
                    try
                    {
                        IBase_UserGroupService base_UserGroupService = BLLContainer.Resolve<IBase_UserGroupService>();
                        var base_UserGroup = base_UserGroupService.GetModels(p => p.ID.ToString().Equals(groupId, StringComparison.OrdinalIgnoreCase)).FirstOrDefault<Base_UserGroup>();
                        if (base_UserGroup == null)
                        {
                            errMsg = "该工作组不存在";
                            return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                        }

                        base_UserGroup.GroupName = groupName;
                        base_UserGroup.Note = note;
                        if (!base_UserGroupService.Update(base_UserGroup))
                        {
                            errMsg = "更新数据库失败";
                            return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                        }

                        if (userGroupGrants != null && userGroupGrants.Count() >= 0)
                        {
                            var dbContext = DbContextFactory.CreateByModelNamespace(typeof(Base_UserGroup_Grant).Namespace);
                            var base_UserGroup_Grants = dbContext.Set<Base_UserGroup_Grant>().Where(p => p.GroupID.ToString().Equals(groupId, StringComparison.OrdinalIgnoreCase)).ToList();
                            foreach (var base_UserGroup_Grant in base_UserGroup_Grants)
                            {
                                dbContext.Entry(base_UserGroup_Grant).State = EntityState.Deleted;
                            }

                            foreach (IDictionary<string, object> el in userGroupGrants)
                            {
                                if (el != null)
                                {
                                    var dicPara = new Dictionary<string, object>(el, StringComparer.OrdinalIgnoreCase);
                                    string functionId = dicPara.ContainsKey("functionId") ? dicPara["functionId"].ToString() : string.Empty;
                                    string isAllow = dicPara.ContainsKey("isAllow") ? dicPara["isAllow"].ToString() : string.Empty;
                                    var base_UserGroup_Grant = new Base_UserGroup_Grant();
                                    base_UserGroup_Grant.FunctionID = Convert.ToInt32(functionId);
                                    base_UserGroup_Grant.GroupID = Convert.ToInt32(groupId);
                                    base_UserGroup_Grant.IsAllow = Convert.ToInt32(isAllow);
                                    dbContext.Entry(base_UserGroup_Grant).State = EntityState.Added;                                                                   
                                }
                                else
                                {
                                    errMsg = "提交数据包含空对象";
                                    return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                                }
                            }

                            if (dbContext.SaveChanges() < 0)
                            {
                                errMsg = "更新数据库失败";
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

                return ResponseModelFactory.CreateSuccessModel(isSignKeyReturn);
            }
            catch (Exception e)
            {
                return ResponseModelFactory.CreateReturnModel(isSignKeyReturn, Return_Code.F, e.Message);
            }
        }

        [ApiMethodAttribute(SignKeyEnum = SignKeyEnum.XCCloudUserCacheToken, SysIdAndVersionNo = false)]
        public object DeleteUserGroup(Dictionary<string, object> dicParas)
        {
            try
            {
                string errMsg = string.Empty;
                string groupId = dicParas.ContainsKey("groupId") ? dicParas["groupId"].ToString() : string.Empty;

                //验证参数
                if (string.IsNullOrEmpty(groupId))
                {
                    errMsg = "工作组Id不能为空";
                    return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                }

                //开启EF事务
                using (TransactionScope ts = new TransactionScope())
                {
                    try
                    {
                        IBase_UserGroupService base_UserGroupService = BLLContainer.Resolve<IBase_UserGroupService>();
                        var base_UserGroup = base_UserGroupService.GetModels(p => p.ID.ToString().Equals(groupId, StringComparison.OrdinalIgnoreCase)).FirstOrDefault<Base_UserGroup>();
                        if (base_UserGroup == null)
                        {
                            errMsg = "该工作组不存在";
                            return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                        }

                        if (!base_UserGroupService.Delete(base_UserGroup))
                        {
                            errMsg = "更新数据库失败";
                            return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                        }

                        var dbContext = DbContextFactory.CreateByModelNamespace(typeof(Base_UserGroup_Grant).Namespace);
                        var base_UserGroup_Grants = dbContext.Set<Base_UserGroup_Grant>().Where(p => p.GroupID.ToString().Equals(groupId, StringComparison.OrdinalIgnoreCase)).ToList();
                        foreach (var base_UserGroup_Grant in base_UserGroup_Grants)
                        {
                            dbContext.Entry(base_UserGroup_Grant).State = EntityState.Deleted;
                        }

                        if (dbContext.SaveChanges() < 0)
                        {
                            errMsg = "更新数据库失败";
                            return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                        }

                        ts.Complete();
                    }
                    catch (Exception ex)
                    {
                        return ResponseModelFactory.CreateReturnModel(isSignKeyReturn, Return_Code.F, ex.Message);
                    }
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