using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Transactions;
using System.Web;
using XCCloudService.Base;
using XCCloudService.BLL.Container;
using XCCloudService.BLL.IBLL.XCCloud;
using XCCloudService.BLL.XCCloud;
using XCCloudService.Common;
using XCCloudService.Common.Enum;
using XCCloudService.DAL;
using XCCloudService.DBService.BLL;
using XCCloudService.Model.CustomModel.XCCloud;
using XCCloudService.Model.XCCloud;

namespace XXCloudService.Api.XCCloud
{
    [Authorize(Roles = "StoreUser")]
    /// <summary>
    /// Project 的摘要说明
    /// </summary>
    public class Project : ApiBase
    {

        [ApiMethodAttribute(SignKeyEnum = SignKeyEnum.XCCloudUserCacheToken, SysIdAndVersionNo = false)]
        public object GetProjectInfoList(Dictionary<string, object> dicParas)
        {            
            try
            {
                XCCloudUserTokenModel userTokenKeyModel = (XCCloudUserTokenModel)dicParas[Constant.XCCloudUserTokenModel];
                string storeId = (userTokenKeyModel.DataModel as UserDataModel).StoreID;

                var data_ProjectInfoService = BLLContainer.Resolve<IData_ProjectInfoService>(resolveNew: true);
                var dict_SystemService = BLLContainer.Resolve<IDict_SystemService>(resolveNew: true);
                int FeeTypeId = dict_SystemService.GetModels(p => p.DictKey.Equals("计费方式")).FirstOrDefault().ID;

                var linq = from a in data_ProjectInfoService.GetModels(p => p.StoreID.Equals(storeId, StringComparison.OrdinalIgnoreCase) && p.ProjectStatus == 1)
                           join b in dict_SystemService.GetModels(p => p.PID == FeeTypeId) on (a.FeeType + "") equals b.DictValue into b1
                           from b in b1.DefaultIfEmpty()
                           select new
                           {
                               ID = a.ID,
                               ProjectName = a.ProjectName,
                               ProjectStatusStr = a.ProjectStatus == 1 ? "有效" : "无效",
                               FeeCycle = a.FeeCycle,
                               FeeDeposit = a.FeeDeposit,
                               SignOutEnStr = a.SignOutEN == 1 ? "需要签出" : "单次扣费",
                               WhenLockStr = a.WhenLock == 1 ? "是" : "否",
                               RegretTime = a.RegretTime,
                               Note = a.Note,
                               FeeTypeStr = b != null ? b.DictKey : string.Empty
                           };

                return ResponseModelFactory.CreateSuccessModel(isSignKeyReturn, linq.ToList());
            }
            catch (Exception e)
            {
                return ResponseModelFactory.CreateReturnModel(isSignKeyReturn, Return_Code.F, e.Message);
            }
        }

        [ApiMethodAttribute(SignKeyEnum = SignKeyEnum.XCCloudUserCacheToken, SysIdAndVersionNo = false)]
        public object GetProjectInfo(Dictionary<string, object> dicParas)
        {
            try
            {
                XCCloudUserTokenModel userTokenKeyModel = (XCCloudUserTokenModel)dicParas[Constant.XCCloudUserTokenModel];
                string storeId = (userTokenKeyModel.DataModel as UserDataModel).StoreID;

                string errMsg = string.Empty;
                string id = dicParas.ContainsKey("id") ? (dicParas["id"] + "") : string.Empty;
                if (string.IsNullOrEmpty(id))
                {
                    errMsg = "项目ID不能为空";
                    return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                }

                int iId = Convert.ToInt32(id);
                IData_ProjectInfoService data_ProjectInfoService = BLLContainer.Resolve<IData_ProjectInfoService>(resolveNew: true);     
                var ProjectInfo = data_ProjectInfoService.GetModels(p => p.ID == iId).FirstOrDefault();
                if (ProjectInfo == null)
                {
                    errMsg = "该项目不存在";
                    return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                }

                IDict_SystemService dict_SystemService = BLLContainer.Resolve<IDict_SystemService>(resolveNew: true);
                int DeviceTypeId = dict_SystemService.GetModels(p => p.DictKey.Equals("设备类型")).FirstOrDefault().ID;

                IData_Project_DeviceService data_Project_DeviceService = BLLContainer.Resolve<IData_Project_DeviceService>(resolveNew: true);
                IBase_DeviceInfoService base_DeviceInfoService = BLLContainer.Resolve<IBase_DeviceInfoService>(resolveNew: true);
                var ProjectDevices = from a in data_Project_DeviceService.GetModels(p => p.ProjectID == iId)
                                     join b in base_DeviceInfoService.GetModels() on a.DeviceID equals b.ID
                                     join c in dict_SystemService.GetModels(p => p.PID == DeviceTypeId) on (b.type + "") equals c.DictValue into c1
                                     from c in c1.DefaultIfEmpty()
                                     select new
                                     {
                                         DeviceID = a.DeviceID,
                                         BindType = a.BindType,
                                         DeviceName = b.DeviceName,
                                         MCUID = b.MCUID,
                                         typeStr = c != null ? c.DictKey : string.Empty
                                     };

                IData_Project_BandPriceService data_Project_BandPriceService = BLLContainer.Resolve<IData_Project_BandPriceService>(resolveNew: true);
                IData_MemberLevelService data_MemberLevelService = BLLContainer.Resolve<IData_MemberLevelService>(resolveNew: true);
                var MemberLevels = data_MemberLevelService.GetModels();
                var BandPrices = (from a in data_Project_BandPriceService.GetModels(p=>p.ProjectID == iId)
                                  group a by new { a.BandType, a.BandCount, a.BandPrice } into g
                                  select new
                                  {
                                      MemberLevelInfos = (from b in MemberLevels
                                                          where g.Select(o => o.MemberLevelID).Contains(b.MemberLevelID)
                                                          select new { b.MemberLevelID, b.MemberLevelName }).ToList(),
                                      Key = g.Key
                                  }).ToList().Select(o => new
                                  {
                                      MemberLevelIDs = string.Join("|", o.MemberLevelInfos.Select(s => s.MemberLevelID)),
                                      MemberLevels = string.Join("|", o.MemberLevelInfos.Select(s => s.MemberLevelName)),
                                      BandType = o.Key.BandType,
                                      BandTypeStr = o.Key.BandType == 0 ? "小于等于" : o.Key.BandType == 1 ? "大于等于" : string.Empty,
                                      BandCount = o.Key.BandCount,
                                      BandPrice = o.Key.BandPrice
                                  });
                                 

                var result = new
                {
                    ID = ProjectInfo.ID,
                    ProjectName = ProjectInfo.ProjectName,
                    ProjectStatus = ProjectInfo.ProjectStatus,
                    FeeType = ProjectInfo.FeeType,
                    FeeCycle = ProjectInfo.FeeCycle,
                    FeeDeposit = ProjectInfo.FeeDeposit,
                    SignOutEN = ProjectInfo.SignOutEN,
                    WhenLock = ProjectInfo.WhenLock,
                    RegretTime = ProjectInfo.RegretTime,
                    Note = ProjectInfo.Note,
                    BandPrices = BandPrices,
                    ProjectDevices = ProjectDevices
                };

                return ResponseModelFactory.CreateSuccessModel(isSignKeyReturn, result);
            }
            catch (Exception e)
            {
                return ResponseModelFactory.CreateReturnModel(isSignKeyReturn, Return_Code.F, e.Message);
            }
        }        

        [ApiMethodAttribute(SignKeyEnum = SignKeyEnum.XCCloudUserCacheToken, SysIdAndVersionNo = false)]
        public object GetBindDeviceList(Dictionary<string, object> dicParas)
        {
            try
            {
                XCCloudUserTokenModel userTokenKeyModel = (XCCloudUserTokenModel)dicParas[Constant.XCCloudUserTokenModel];
                string storeId = (userTokenKeyModel.DataModel as UserDataModel).StoreID;

                string errMsg = string.Empty;
                string type = dicParas.ContainsKey("type") ? (dicParas["type"] + "") : string.Empty;
                string mcuId = dicParas.ContainsKey("mcuId") ? (dicParas["mcuId"] + "") : string.Empty;
                var bindDeviceIDs = dicParas.ContainsKey("bindDeviceIDs") ? ((object[])dicParas["bindDeviceIDs"]).Cast<int>() : null;

                if (!string.IsNullOrEmpty(type) && !Utils.isNumber(type))
                {
                    errMsg = "设备类型格式不正确";
                    return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                }
                               
                var dbContext = DbContextFactory.CreateByModelNamespace(typeof(Base_UserInfo).Namespace);
                var data_Project_Device = from a in dbContext.Set<Data_Project_Device>()
                                          join b in dbContext.Set<Data_ProjectInfo>() on a.ProjectID equals b.ID
                                          where b.ProjectStatus == 1 
                                          select a.DeviceID;
                var base_DeviceInfo = dbContext.Set<Base_DeviceInfo>().Where(p => p.DeviceStatus == (int)DeviceStatus.Normal && p.StoreID.Equals(storeId, StringComparison.OrdinalIgnoreCase) && !data_Project_Device.Contains(p.ID));
                if (!string.IsNullOrEmpty(type))
                {
                    int iType = Convert.ToInt32(type);
                    base_DeviceInfo = base_DeviceInfo.Where(w => w.type == iType);
                }
                if (!string.IsNullOrEmpty(mcuId))
                {
                    base_DeviceInfo = base_DeviceInfo.Where(w => w.MCUID.Contains(mcuId));
                }
                if (bindDeviceIDs != null)
                {
                    base_DeviceInfo = base_DeviceInfo.Where(w => !bindDeviceIDs.Contains(w.ID));
                }

                int DeviceTypeId = dbContext.Set<Dict_System>().Where(p => p.DictKey.Equals("设备类型")).FirstOrDefault().ID;
                var linq = from a in base_DeviceInfo
                           join b in dbContext.Set<Dict_System>().Where(p=>p.PID == DeviceTypeId) on (a.type + "") equals b.DictValue into b1
                           from b in b1.DefaultIfEmpty()
                           select new
                           {
                               ID = a.ID,
                               DeviceName = a.DeviceName,
                               typeStr = b != null ? b.DictKey : string.Empty,
                               MCUID = a.MCUID
                           };

                return ResponseModelFactory.CreateSuccessModel(isSignKeyReturn, linq.ToList());
            }
            catch (Exception e)
            {
                return ResponseModelFactory.CreateReturnModel(isSignKeyReturn, Return_Code.F, e.Message);
            }
        }

        [ApiMethodAttribute(SignKeyEnum = SignKeyEnum.XCCloudUserCacheToken, SysIdAndVersionNo = false)]
        public object SaveProjectInfo(Dictionary<string, object> dicParas)
        {
            try
            {
                XCCloudUserTokenModel userTokenKeyModel = (XCCloudUserTokenModel)dicParas[Constant.XCCloudUserTokenModel];
                string storeId = (userTokenKeyModel.DataModel as UserDataModel).StoreID;

                string errMsg = string.Empty;
                string id = dicParas.ContainsKey("id") ? (dicParas["id"] + "") : string.Empty;
                string projectName = dicParas.ContainsKey("projectName") ? (dicParas["projectName"] + "") : string.Empty;
                string projectStatus = dicParas.ContainsKey("projectStatus") ? (dicParas["projectStatus"] + "") : string.Empty;
                string feeType = dicParas.ContainsKey("feeType") ? (dicParas["feeType"] + "") : string.Empty;
                string feeCycle = dicParas.ContainsKey("feeCycle") ? (dicParas["feeCycle"] + "") : string.Empty;
                string feeDeposit = dicParas.ContainsKey("feeDeposit") ? (dicParas["feeDeposit"] + "") : string.Empty;
                string signOutEn = dicParas.ContainsKey("signOutEn") ? (dicParas["signOutEn"] + "") : string.Empty;
                string whenLock = dicParas.ContainsKey("whenLock") ? (dicParas["whenLock"] + "") : string.Empty;
                string regretTime = dicParas.ContainsKey("regretTime") ? (dicParas["regretTime"] + "") : string.Empty;
                string note = dicParas.ContainsKey("note") ? (dicParas["note"] + "") : string.Empty;
                object[] bandPrices = dicParas.ContainsKey("bandPrices") ? (object[])dicParas["bandPrices"] : null;
                object[] projectDevices = dicParas.ContainsKey("projectDevices") ? (object[])dicParas["projectDevices"] : null;
                int iId = 0;
                int.TryParse(id, out iId);

                #region 验证参数

                if (string.IsNullOrEmpty(projectName))
                {
                    errMsg = "项目名称不能为空";
                    return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                }

                if (string.IsNullOrEmpty(projectStatus))
                {
                    errMsg = "项目状态不能为空";
                    return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                }

                if (string.IsNullOrEmpty(feeType))
                {
                    errMsg = "计费方式不能为空";
                    return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                }

                if (feeType == FeeType.Time.ToString())
                {
                    if (string.IsNullOrEmpty(feeCycle))
                    {
                        errMsg = "计费周期不能为空";
                        return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                    }

                    if (string.IsNullOrEmpty(feeDeposit))
                    {
                        errMsg = "预收押金不能为空";
                        return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                    }

                    if (Convert.ToInt32(feeCycle) < 0)
                    {
                        errMsg = "计费周期不能为负数";
                        return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                    }

                    if (Convert.ToInt32(feeDeposit) < 0)
                    {
                        errMsg = "预收押金不能为负数";
                        return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                    }
                }

                if (string.IsNullOrEmpty(signOutEn))
                {
                    errMsg = "是否需要签出不能为空";
                    return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                }

                if (string.IsNullOrEmpty(whenLock))
                {
                    errMsg = "入场锁定不能为空";
                    return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                }

                if (!string.IsNullOrEmpty(regretTime) && Convert.ToInt32(regretTime) < 0)
                {
                    errMsg = "后悔时间不能为负数";
                    return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                }

                #endregion

                //开启EF事务
                using (TransactionScope ts = new TransactionScope())
                {
                    try
                    {
                        IData_ProjectInfoService data_ProjectInfoService = BLLContainer.Resolve<IData_ProjectInfoService>();
                        if (data_ProjectInfoService.Any(a => a.StoreID.Equals(storeId, StringComparison.OrdinalIgnoreCase) &&
                            a.ProjectName.Equals(projectName, StringComparison.OrdinalIgnoreCase) && a.ID != iId))
                        {
                            errMsg = "该项目名称已存在";
                            return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                        }

                        var data_ProjectInfo = new Data_ProjectInfo();
                        data_ProjectInfo.ID = iId;
                        data_ProjectInfo.ProjectName = projectName;
                        data_ProjectInfo.ProjectStatus = Convert.ToInt32(projectStatus);
                        data_ProjectInfo.FeeType = Convert.ToInt32(feeType);
                        data_ProjectInfo.FeeCycle = !string.IsNullOrEmpty(feeCycle) ? Convert.ToInt32(feeCycle) : (int?)null;
                        data_ProjectInfo.FeeDeposit = !string.IsNullOrEmpty(feeDeposit) ? Convert.ToInt32(feeDeposit) : (int?)null;
                        data_ProjectInfo.RegretTime = !string.IsNullOrEmpty(regretTime) ? Convert.ToInt32(regretTime) : (int?)null;
                        data_ProjectInfo.SignOutEN = Convert.ToInt32(signOutEn);
                        data_ProjectInfo.WhenLock = Convert.ToInt32(whenLock);
                        data_ProjectInfo.Note = note;
                        data_ProjectInfo.StoreID = storeId;
                        if (!data_ProjectInfoService.Any(a => a.ID == iId))
                        {
                            //新增
                            if (!data_ProjectInfoService.Add(data_ProjectInfo))
                            {
                                errMsg = "添加门票项目信息失败";
                                return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                            }
                        }
                        else
                        {
                            //修改
                            if (!data_ProjectInfoService.Update(data_ProjectInfo))
                            {
                                errMsg = "修改门票项目信息失败";
                                return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                            }
                        }

                        iId = data_ProjectInfo.ID;

                        if (bandPrices != null && bandPrices.Count() >= 0)
                        {
                            //先删除已有数据，后添加
                            var dbContext = DbContextFactory.CreateByModelNamespace(typeof(Data_Project_BandPrice).Namespace);
                            var data_Project_BandPrice = dbContext.Set<Data_Project_BandPrice>().Where(p => p.ProjectID == iId).ToList();
                            foreach (var model in data_Project_BandPrice)
                            {
                                dbContext.Entry(model).State = EntityState.Deleted;
                            }

                            foreach (IDictionary<string, object> el in bandPrices)
                            {
                                if (el != null)
                                {
                                    var dicPara = new Dictionary<string, object>(el, StringComparer.OrdinalIgnoreCase);
                                    string memberLevelIDs = dicPara.ContainsKey("memberLevelIDs") ? dicPara["memberLevelIDs"].ToString() : string.Empty;
                                    string bandType = dicPara.ContainsKey("bandType") ? dicPara["bandType"].ToString() : string.Empty;
                                    string bandCount = dicPara.ContainsKey("bandCount") ? dicPara["bandCount"].ToString() : string.Empty;
                                    string bandPrice = dicPara.ContainsKey("bandPrice") ? dicPara["bandPrice"].ToString() : string.Empty;
                                    if (string.IsNullOrEmpty(memberLevelIDs))
                                    {
                                        errMsg = "会员级别ID列表不能为空";
                                        return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                                    }
                                    if (!Utils.isNumber(bandType))
                                    {
                                        errMsg = "档位类别格式不正确";
                                        return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                                    }
                                    if (Convert.ToInt32(bandCount) < 0)
                                    {
                                        errMsg = "档位数量不能为负数";
                                        return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                                    }
                                    if (Convert.ToInt32(bandPrice) < 0)
                                    {
                                        errMsg = "档位价格不能为负数";
                                        return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                                    }

                                    List<string> memberLevelIDList = memberLevelIDs.Split('|').ToList();
                                    foreach (var memberLevelID in memberLevelIDList)
                                    {
                                        var data_Project_BandPriceModel = new Data_Project_BandPrice();
                                        data_Project_BandPriceModel.ProjectID = iId;
                                        data_Project_BandPriceModel.MemberLevelID = Convert.ToInt32(memberLevelID);
                                        data_Project_BandPriceModel.BandType = Convert.ToInt32(bandType);
                                        data_Project_BandPriceModel.BandCount = Convert.ToInt32(bandCount);
                                        data_Project_BandPriceModel.BandPrice = Convert.ToInt32(bandPrice);
                                        dbContext.Entry(data_Project_BandPriceModel).State = EntityState.Added;
                                    }
                                }
                                else
                                {
                                    errMsg = "提交数据包含空对象";
                                    return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                                }
                            }

                            if (dbContext.SaveChanges() < 0)
                            {
                                errMsg = "保存项目波段信息失败";
                                return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                            }
                        }

                        if (projectDevices != null && projectDevices.Count() >= 0)
                        {
                            //先删除已有数据，后添加                        
                            var dbContext = DbContextFactory.CreateByModelNamespace(typeof(Data_Project_Device).Namespace);
                            var data_Project_Device = dbContext.Set<Data_Project_Device>().Where(p => p.ProjectID == iId).ToList();
                            foreach (var model in data_Project_Device)
                            {
                                dbContext.Entry(model).State = EntityState.Deleted;
                            }

                            foreach (IDictionary<string, object> el in projectDevices)
                            {
                                if (el != null)
                                {
                                    var dicPara = new Dictionary<string, object>(el, StringComparer.OrdinalIgnoreCase);
                                    string deviceId = dicPara.ContainsKey("deviceId") ? dicPara["deviceId"].ToString() : string.Empty;
                                    string bindType = dicPara.ContainsKey("bindType") ? dicPara["bindType"].ToString() : string.Empty;
                                    if (string.IsNullOrEmpty(deviceId))
                                    {
                                        errMsg = "设备ID不能为空";
                                        return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                                    }
                                    if (string.IsNullOrEmpty(bindType))
                                    {
                                        errMsg = "绑定类型不能为空";
                                        return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                                    }
                                    if (!Utils.isNumber(bindType))
                                    {
                                        errMsg = "绑定类型格式不正确";
                                        return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                                    }

                                    var data_Project_DeviceModel = new Data_Project_Device();
                                    data_Project_DeviceModel.DeviceID = Convert.ToInt32(deviceId);
                                    data_Project_DeviceModel.BindType = Convert.ToInt32(bindType);
                                    data_Project_DeviceModel.ProjectID = iId;
                                    dbContext.Entry(data_Project_DeviceModel).State = EntityState.Added;
                                }
                                else
                                {
                                    errMsg = "提交数据包含空对象";
                                    return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                                }
                            }

                            if (dbContext.SaveChanges() < 0)
                            {
                                errMsg = "保存项目设备绑定信息失败";
                                return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                            }
                        }

                        //如果不需要签出，解绑签出设备
                        if (data_ProjectInfo.SignOutEN == 0)
                        {
                            var dbContext = DbContextFactory.CreateByModelNamespace(typeof(Data_Project_Device).Namespace);
                            var data_Project_Device = dbContext.Set<Data_Project_Device>().Where(p => p.ProjectID == iId && p.BindType == (int)DeviceBindType.Out).ToList();
                            foreach (var model in data_Project_Device)
                            {
                                dbContext.Entry(model).State = EntityState.Deleted;
                            }

                            if (dbContext.SaveChanges() < 0)
                            {
                                errMsg = "解绑签出设备失败";
                                return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                            }
                        }

                        ts.Complete();
                    }
                    catch (Exception ex)
                    {
                        errMsg = ex.Message;
                        return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
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
        public object DeleteProjectInfo(Dictionary<string, object> dicParas)
        {
            try
            {
                XCCloudUserTokenModel userTokenKeyModel = (XCCloudUserTokenModel)dicParas[Constant.XCCloudUserTokenModel];
                string storeId = (userTokenKeyModel.DataModel as UserDataModel).StoreID;

                string errMsg = string.Empty;
                string projectIds = dicParas.ContainsKey("projectIds") ? (dicParas["projectIds"] + "") : string.Empty;

                if (string.IsNullOrEmpty(projectIds))
                {
                    errMsg = "项目ID不能为空";
                    return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                }

                //开启EF事务
                using (TransactionScope ts = new TransactionScope())
                {
                    try
                    {
                        var dbContext = DbContextFactory.CreateByModelNamespace(typeof(Data_ProjectInfo).Namespace);
                        List<string> projectIdList = projectIds.Split('|').ToList();
                        foreach (var projectId in projectIdList)
                        {
                            if (string.IsNullOrEmpty(projectId))
                            {
                                errMsg = "项目ID不能为空";
                                return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                            }

                            if (!Utils.isNumber(projectId))
                            {
                                errMsg = "项目ID格式不正确";
                                return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                            }

                            int iProjectId = Convert.ToInt32(projectId);
                            var data_ProjectInfoModel = dbContext.Set<Data_ProjectInfo>().Where(p => p.ID == iProjectId).FirstOrDefault();
                            if (data_ProjectInfoModel == null)
                            {
                                errMsg = "项目ID" + projectId + "不存在";
                                return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                            }

                            data_ProjectInfoModel.ProjectStatus = 0; //停用
                            dbContext.Entry(data_ProjectInfoModel).State = EntityState.Modified;
                        }

                        if (dbContext.SaveChanges() < 0)
                        {
                            errMsg = "删除项目失败";
                            return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                        }

                        ts.Complete();
                    }
                    catch (Exception ex)
                    {
                        errMsg = ex.Message;
                        return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
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