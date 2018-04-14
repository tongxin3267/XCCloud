using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Data.SqlClient;
using System.Linq;
using System.Transactions;
using System.Web;
using XCCloudService.Base;
using XCCloudService.BLL.Container;
using XCCloudService.BLL.IBLL.XCCloud;
using XCCloudService.Common;
using XCCloudService.Common.Enum;
using XCCloudService.DBService.BLL;
using XCCloudService.Model.CustomModel.XCCloud;
using XCCloudService.Model.XCCloud;
using XXCloudService.Api.XCCloud.Common;

namespace XXCloudService.Api.XCCloud
{
    [Authorize(Roles = "StoreUser")]
    /// <summary>
    /// Gift 的摘要说明
    /// </summary>
    public class DeviceInfo : ApiBase
    {
        [ApiMethodAttribute(SignKeyEnum = SignKeyEnum.XCCloudUserCacheToken, SysIdAndVersionNo = false)]
        public object QueryReloadGifts(Dictionary<string, object> dicParas)
        {
            try
            {
                XCCloudUserTokenModel userTokenKeyModel = (XCCloudUserTokenModel)dicParas[Constant.XCCloudUserTokenModel];
                string storeId = (userTokenKeyModel.DataModel as UserDataModel).StoreID;

                string errMsg = string.Empty;
                object[] conditions = dicParas.ContainsKey("conditions") ? (object[])dicParas["conditions"] : null;

                SqlParameter[] parameters = new SqlParameter[0];
                string sqlWhere = string.Empty;

                if (conditions != null && conditions.Length > 0)
                {
                    if (!QueryBLL.GenDynamicSql(conditions, "a.", ref sqlWhere, ref parameters, out errMsg))
                    {
                        return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                    }
                }

                string sql = @"select a.ID, (case when a.RealTime is null or a.RealTime='' then '' else convert(varchar,a.RealTime,23) end) as RealTime, c.HeadName, d.DictKey as ReloadTypeName, b.LogName, a.ReloadCount, a.Note from Data_Reload a " +
                    " left join Base_UserInfo b on a.UserID=b.UserID " +
                    " left join Data_Head c on a.DeviceID=c.ID " +
                    " left join (select b.* from Dict_System a inner join Dict_System b on a.ID=b.PID where a.DictKey='安装类别' and a.PID=0) d on convert(varchar, a.ReloadType)=d.DictValue " +
                    " where b.StoreID='" + storeId + "' and deviceType=2 and ReloadType=3 " + 
                    " order by c.HeadName, a.RealTime desc";
                sql = sql + sqlWhere;
                IData_ReloadService data_ReloadService = BLLContainer.Resolve<IData_ReloadService>();
                var data_Reload = data_ReloadService.SqlQuery<Data_ReloadModelList>(sql, parameters).ToList();

                return ResponseModelFactory.CreateSuccessModel(isSignKeyReturn, data_Reload);
            }
            catch (Exception e)
            {
                return ResponseModelFactory.CreateReturnModel(isSignKeyReturn, Return_Code.F, e.Message);
            }
        }

        [ApiMethodAttribute(SignKeyEnum = SignKeyEnum.XCCloudUserCacheToken, SysIdAndVersionNo = false)]
        public object ReloadDevice(Dictionary<string, object> dicParas)
        {
            try
            {
                XCCloudUserTokenModel userTokenKeyModel = (XCCloudUserTokenModel)dicParas[Constant.XCCloudUserTokenModel];
                string storeId = (userTokenKeyModel.DataModel as UserDataModel).StoreID;
                string userId = userTokenKeyModel.LogId;
                int iUserId = 0;
                int.TryParse(userId, out iUserId);

                string errMsg = string.Empty;
                string goodId = dicParas.ContainsKey("goodId") ? Convert.ToString(dicParas["goodId"]) : string.Empty;
                string deviceId = dicParas.ContainsKey("deviceId") ? Convert.ToString(dicParas["deviceId"]) : string.Empty;
                string deviceType = dicParas.ContainsKey("deviceType") ? Convert.ToString(dicParas["deviceType"]) : string.Empty;
                string reloadType = dicParas.ContainsKey("reloadType") ? Convert.ToString(dicParas["reloadType"]) : string.Empty;
                string reloadCount = dicParas.ContainsKey("reloadCount") ? Convert.ToString(dicParas["reloadCount"]) : string.Empty;
                string note = dicParas.ContainsKey("note") ? Convert.ToString(dicParas["note"]) : string.Empty;
                int iDeviceId = 0, iGoodId = 0;
                int.TryParse(deviceId, out iDeviceId);
                int.TryParse(goodId, out iGoodId);

                #region 参数验证

                if (string.IsNullOrEmpty(goodId))
                {
                    errMsg = "商品索引goodId不能为空";
                    return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                }

                //if (string.IsNullOrEmpty(deviceId))
                //{
                //    errMsg = "设备索引deviceId不能为空";
                //    return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                //}

                if (string.IsNullOrEmpty(deviceType))
                {
                    errMsg = "设备类型deviceType不能为空";
                    return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                }

                if (string.IsNullOrEmpty(reloadType))
                {
                    errMsg = "安装类别reloadType不能为空";
                    return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                }

                if (string.IsNullOrEmpty(reloadCount))
                {
                    errMsg = "安装数量reloadCount不能为空";
                    return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                }

                if (!Utils.isNumber(reloadCount))
                {
                    errMsg = "安装数量reloadCount格式不正确";
                    return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                }

                int iReloadCount = Convert.ToInt32(reloadCount);
                if (iReloadCount < 0)
                {
                    errMsg = "安装数量reloadCount不能为负数";
                    return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                }                

                #endregion

                IBase_StorageInfoService base_StorageInfoService = BLLContainer.Resolve<IBase_StorageInfoService>();
                IData_Storage_RecordService data_Storage_RecordService = BLLContainer.Resolve<IData_Storage_RecordService>();
                IData_ReloadService data_ReloadService = BLLContainer.Resolve<IData_ReloadService>();
                IData_HeadService data_HeadService = BLLContainer.Resolve<IData_HeadService>(resolveNew: true);
                IData_GameInfoService data_GameInfoService = BLLContainer.Resolve<IData_GameInfoService>(resolveNew: true);
                int? iDepotId = (from a in data_HeadService.GetModels(p => p.ID == iDeviceId)
                               join b in data_GameInfoService.GetModels() on a.GameIndexID equals b.ID
                               select b.DepotID).FirstOrDefault();

                //开启EF事务
                using (TransactionScope ts = new TransactionScope())
                {
                    try
                    {
                        var data_Reload = new Data_Reload();
                        data_Reload.StoreID = storeId;
                        data_Reload.GoodID = Convert.ToInt32(goodId);
                        data_Reload.DeviceID = !string.IsNullOrEmpty(deviceId) ? Convert.ToInt32(deviceId) : (int?)null;
                        data_Reload.DeviceType = Convert.ToInt32(deviceType);
                        data_Reload.ReloadType = Convert.ToInt32(reloadType);
                        data_Reload.ReloadCount = iReloadCount;
                        data_Reload.Note = note;
                        data_Reload.RealTime = DateTime.Now;
                        data_Reload.UserID = iUserId;
                        if (!data_ReloadService.Add(data_Reload))
                        {
                            errMsg = "添加设备安装信息失败";
                            return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                        }

                        //更新库存
                        if (base_StorageInfoService.Any(a => a.DepotID == iDepotId && a.GoodID == iGoodId))
                        {
                            var base_StorageInfo = base_StorageInfoService.GetModels(p => p.DepotID == iDepotId && p.GoodID == iGoodId).FirstOrDefault();
                            base_StorageInfo.Surplus -= iReloadCount;
                            if (!base_StorageInfoService.Update(base_StorageInfo))
                            {
                                errMsg = "更新库存信息失败";
                                return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                            }

                            //添加库存记录
                            var data_Storage_Record = new Data_Storage_Record();
                            data_Storage_Record.StorageCount = iReloadCount;
                            data_Storage_Record.StorageFlag = (int)StockFlag.Out;
                            data_Storage_Record.StorageID = base_StorageInfo.ID;
                            data_Storage_Record.CreateTime = DateTime.Now;
                            if (!data_Storage_RecordService.Add(data_Storage_Record))
                            {
                                errMsg = "添加库存记录失败";
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
            catch (DbEntityValidationException e)
            {
                return ResponseModelFactory.CreateFailModel(isSignKeyReturn, e.EntityValidationErrors.ToErrors());
            }
            catch (Exception e)
            {
                return ResponseModelFactory.CreateReturnModel(isSignKeyReturn, Return_Code.F, e.Message);
            }
        }

        [ApiMethodAttribute(SignKeyEnum = SignKeyEnum.XCCloudUserCacheToken, SysIdAndVersionNo = false)]
        public object CheckDevice(Dictionary<string, object> dicParas)
        {
            try
            {
                string errMsg = string.Empty;
                string id = dicParas.ContainsKey("id") ? Convert.ToString(dicParas["id"]) : string.Empty;
                string realCount = dicParas.ContainsKey("realCount") ? Convert.ToString(dicParas["realCount"]) : string.Empty;

                #region 参数验证

                if (string.IsNullOrEmpty(id))
                {
                    errMsg = "安装索引id不能为空";
                    return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                }

                if (string.IsNullOrEmpty(realCount))
                {
                    errMsg = "实点数realCount不能为空";
                    return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                }

                if (!Utils.isNumber(realCount))
                {
                    errMsg = "实点数realCount格式不正确";
                    return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                }

                int iRealCount = Convert.ToInt32(realCount);
                if (iRealCount < 0)
                {
                    errMsg = "实点数realCount不能为负数";
                    return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                }  

                #endregion
                

                return ResponseModelFactory.CreateSuccessModel(isSignKeyReturn);
            }
            catch (DbEntityValidationException e)
            {
                return ResponseModelFactory.CreateFailModel(isSignKeyReturn, e.EntityValidationErrors.ToErrors());
            }
            catch (Exception e)
            {
                return ResponseModelFactory.CreateReturnModel(isSignKeyReturn, Return_Code.F, e.Message);
            }
        }
        
    }
}