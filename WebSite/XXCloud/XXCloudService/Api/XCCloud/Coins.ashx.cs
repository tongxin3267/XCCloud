using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Transactions;
using System.Web;
using XCCloudService.Base;
using XCCloudService.BLL.Container;
using XCCloudService.BLL.IBLL.XCCloud;
using XCCloudService.Common;
using XCCloudService.Common.Enum;
using XCCloudService.Model.CustomModel.XCCloud;
using XCCloudService.Model.XCCloud;

namespace XXCloudService.Api.XCCloud
{
    
    [Authorize(Roles = "StoreUser")]
    /// <summary>
    /// Coins 的摘要说明
    /// </summary>
    public class Coins : ApiBase
    {

        [ApiMethodAttribute(SignKeyEnum = SignKeyEnum.XCCloudUserCacheToken, SysIdAndVersionNo = false)]
        public object AddCoinStorage(Dictionary<string, object> dicParas)
        {
            try
            {
                string errMsg = string.Empty;
                string storageCount = dicParas.ContainsKey("storageCount") ? (dicParas["storageCount"] + "") : string.Empty;
                string note = dicParas.ContainsKey("note") ? (dicParas["note"] + "") : string.Empty;
                XCCloudUserTokenModel userTokenKeyModel = (XCCloudUserTokenModel)dicParas[Constant.XCCloudUserTokenModel];
                int userId = Convert.ToInt32(userTokenKeyModel.LogId);
                string storeId = (userTokenKeyModel.DataModel as UserDataModel).StoreID;

                if (string.IsNullOrEmpty(storageCount))
                {
                    errMsg = "入库数量不能为空";
                    return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                }

                if (!Utils.isNumber(storageCount))
                {
                    errMsg = "入库数量参数格式不正确";
                    return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                }

                int iStorageCount = Convert.ToInt32(storageCount);
                if (iStorageCount < 0)
                {
                    errMsg = "入库数量必不能为负数";
                    return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                }

                IData_CoinStorageService data_CoinStorageService = BLLContainer.Resolve<IData_CoinStorageService>();
                Data_CoinStorage data_CoinStorage = new Data_CoinStorage();
                data_CoinStorage.DestroyTime = DateTime.Now;
                data_CoinStorage.Note = note;
                data_CoinStorage.StorageCount = iStorageCount;
                data_CoinStorage.UserID = userId;
                data_CoinStorage.StoreID = storeId;
                if (!data_CoinStorageService.Add(data_CoinStorage))
                {
                    errMsg = "更新数据库失败";
                    return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                }                                

                return ResponseModelFactory.CreateSuccessModel(isSignKeyReturn);
            }
            catch (Exception e)
            {
                return ResponseModelFactory.CreateReturnModel(isSignKeyReturn, Return_Code.F, e.Message);
            }
        }

        [ApiMethodAttribute(SignKeyEnum = SignKeyEnum.XCCloudUserCacheToken, SysIdAndVersionNo = false)]
        public object GetCoinStorage(Dictionary<string, object> dicParas)
        {
            try
            {
                string errMsg = string.Empty;
                string destroyTime = dicParas.ContainsKey("destroyTime") ? (dicParas["destroyTime"] + "") : string.Empty;
                XCCloudUserTokenModel userTokenKeyModel = (XCCloudUserTokenModel)dicParas[Constant.XCCloudUserTokenModel];
                string storeId = (userTokenKeyModel.DataModel as UserDataModel).StoreID;

                if (!string.IsNullOrEmpty(destroyTime))
                {
                    try
                    {
                        Convert.ToDateTime(destroyTime);
                    }
                    catch
                    {
                        errMsg = "入库时间参数格式不正确";
                        return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                    }
                }
                                
                IData_CoinStorageService data_CoinStorageService = BLLContainer.Resolve<IData_CoinStorageService>();
                var query = data_CoinStorageService.GetModels(p => p.StoreID.Equals(storeId, StringComparison.OrdinalIgnoreCase));
                if (!string.IsNullOrEmpty(destroyTime))
                {
                    var dt = Convert.ToDateTime(destroyTime);
                    query = query.Where(w => DbFunctions.DiffDays(w.DestroyTime, dt) == 0);
                }

                IBase_UserInfoService base_UserInfoService = BLLContainer.Resolve<IBase_UserInfoService>();
                var linq = base_UserInfoService.GetModels(p => p.UserType == (int)UserType.Store).Select(o => new { UserID = o.UserID, LogName = o.LogName, RealName = o.RealName });

                var result = query.ToList().Select(o => new
                {
                    ID = o.ID,
                    StoreID = o.StoreID,
                    StorageCount = o.StorageCount,
                    DestroyTime = o.DestroyTime,
                    UserID = o.UserID,
                    Note = o.Note,
                    LogName = linq.SingleOrDefault(p => p.UserID == o.UserID).LogName,
                    RealName = linq.SingleOrDefault(p => p.UserID == o.UserID).RealName
                });

                return ResponseModelFactory.CreateSuccessModel(isSignKeyReturn, result.ToList());
            }
            catch (Exception e)
            {
                return ResponseModelFactory.CreateReturnModel(isSignKeyReturn, Return_Code.F, e.Message);
            }
        }

        [ApiMethodAttribute(SignKeyEnum = SignKeyEnum.XCCloudUserCacheToken, SysIdAndVersionNo = false)]
        public object AddCoinDestory(Dictionary<string, object> dicParas)
        {
            try
            {
                string errMsg = string.Empty;
                string storageCount = dicParas.ContainsKey("storageCount") ? (dicParas["storageCount"] + "") : string.Empty;
                string note = dicParas.ContainsKey("note") ? (dicParas["note"] + "") : string.Empty;
                XCCloudUserTokenModel userTokenKeyModel = (XCCloudUserTokenModel)dicParas[Constant.XCCloudUserTokenModel];
                int userId = Convert.ToInt32(userTokenKeyModel.LogId);
                string storeId = (userTokenKeyModel.DataModel as UserDataModel).StoreID;

                if (string.IsNullOrEmpty(storageCount))
                {
                    errMsg = "销毁数量不能为空";
                    return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                }

                if (!Utils.isNumber(storageCount))
                {
                    errMsg = "销毁数量参数格式不正确";
                    return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                }

                int iStorageCount = Convert.ToInt32(storageCount);
                if (iStorageCount < 0)
                {
                    errMsg = "销毁数量必不能为负数";
                    return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                }

                IData_CoinDestoryService data_CoinDestoryService = BLLContainer.Resolve<IData_CoinDestoryService>();
                Data_CoinDestory data_CoinDestory = new Data_CoinDestory();
                data_CoinDestory.DestroyTime = DateTime.Now;
                data_CoinDestory.Note = note;
                data_CoinDestory.StorageCount = iStorageCount;
                data_CoinDestory.UserID = userId;
                data_CoinDestory.StoreID = storeId;
                if (!data_CoinDestoryService.Add(data_CoinDestory))
                {
                    errMsg = "更新数据库失败";
                    return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                }

                return ResponseModelFactory.CreateSuccessModel(isSignKeyReturn);
            }
            catch (Exception e)
            {
                return ResponseModelFactory.CreateReturnModel(isSignKeyReturn, Return_Code.F, e.Message);
            }
        }

        [ApiMethodAttribute(SignKeyEnum = SignKeyEnum.XCCloudUserCacheToken, SysIdAndVersionNo = false)]
        public object GetCoinDestory(Dictionary<string, object> dicParas)
        {
            try
            {
                string errMsg = string.Empty;
                string destroyTime = dicParas.ContainsKey("destroyTime") ? (dicParas["destroyTime"] + "") : string.Empty;
                XCCloudUserTokenModel userTokenKeyModel = (XCCloudUserTokenModel)dicParas[Constant.XCCloudUserTokenModel];
                string storeId = (userTokenKeyModel.DataModel as UserDataModel).StoreID;

                if (!string.IsNullOrEmpty(destroyTime))
                {
                    try
                    {
                        Convert.ToDateTime(destroyTime);
                    }
                    catch
                    {
                        errMsg = "销毁时间参数格式不正确";
                        return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                    }
                }

                IData_CoinDestoryService data_CoinDestoryService = BLLContainer.Resolve<IData_CoinDestoryService>();
                var query = data_CoinDestoryService.GetModels(p => p.StoreID.Equals(storeId, StringComparison.OrdinalIgnoreCase));
                if (!string.IsNullOrEmpty(destroyTime))
                {
                    var dt = Convert.ToDateTime(destroyTime);
                    query = query.Where(w => DbFunctions.DiffDays(w.DestroyTime, dt) == 0);
                }

                IBase_UserInfoService base_UserInfoService = BLLContainer.Resolve<IBase_UserInfoService>();
                var linq = base_UserInfoService.GetModels(p => p.UserType == (int)UserType.Store).Select(o => new { UserID = o.UserID, LogName = o.LogName, RealName = o.RealName });

                var result = query.ToList().Select(o => new
                {
                    ID = o.ID,
                    StoreID = o.StoreID,
                    StorageCount = o.StorageCount,
                    DestroyTime = o.DestroyTime,
                    UserID = o.UserID,
                    Note = o.Note,
                    LogName = linq.SingleOrDefault(p => p.UserID == o.UserID).LogName,
                    RealName = linq.SingleOrDefault(p => p.UserID == o.UserID).RealName
                });

                return ResponseModelFactory.CreateSuccessModel(isSignKeyReturn, result.ToList());
            }
            catch (Exception e)
            {
                return ResponseModelFactory.CreateReturnModel(isSignKeyReturn, Return_Code.F, e.Message);
            }
        }

        [ApiMethodAttribute(SignKeyEnum = SignKeyEnum.XCCloudUserCacheToken, SysIdAndVersionNo = false)]
        public object AddDigitCoin(Dictionary<string, object> dicParas)
        {
            try
            {
                string errMsg = string.Empty;
                string digitLevelID = dicParas.ContainsKey("digitLevelID") ? (dicParas["digitLevelID"] + "") : string.Empty;
                string iCardID = dicParas.ContainsKey("iCardID") ? (dicParas["iCardID"] + "") : string.Empty;                
                XCCloudUserTokenModel userTokenKeyModel = (XCCloudUserTokenModel)dicParas[Constant.XCCloudUserTokenModel];
                string storeId = (userTokenKeyModel.DataModel as UserDataModel).StoreID;

                if (string.IsNullOrEmpty(digitLevelID))
                {
                    errMsg = "数字币级别编号不能为空";
                    return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                }

                if (!Utils.isNumber(digitLevelID))
                {
                    errMsg = "数字币级别编号参数格式不正确";
                    return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                }

                if (string.IsNullOrEmpty(iCardID))
                {
                    errMsg = "数字币编号不能为空";
                    return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                }

                if (iCardID.Length > 16)
                {
                    errMsg = "数字币编号长度不能超过16位";
                    return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                }

                int iDigitLevelID = Convert.ToInt32(digitLevelID);
                IData_DigitCoinService data_DigitCoinService = BLLContainer.Resolve<IData_DigitCoinService>();
                if (data_DigitCoinService.Any(a => a.ICardID.Equals(iCardID, StringComparison.OrdinalIgnoreCase)))
                {
                    errMsg = "该卡已存在";
                    return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                }

                Data_DigitCoin data_DigitCoin = new Data_DigitCoin();
                data_DigitCoin.CreateTime = DateTime.Now;
                data_DigitCoin.ICardID = iCardID;
                data_DigitCoin.DigitLevelID = iDigitLevelID;
                data_DigitCoin.StoreID = storeId;
                data_DigitCoin.Status = (int)DigitStatus.Inuse;
                if (!data_DigitCoinService.Add(data_DigitCoin))
                {
                    errMsg = "更新数据库失败";
                    return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                }

                return ResponseModelFactory.CreateSuccessModel(isSignKeyReturn);
            }
            catch (Exception e)
            {
                return ResponseModelFactory.CreateReturnModel(isSignKeyReturn, Return_Code.F, e.Message);
            }
        }

        [ApiMethodAttribute(SignKeyEnum = SignKeyEnum.XCCloudUserCacheToken, SysIdAndVersionNo = false)]
        public object GetDigitCoin(Dictionary<string, object> dicParas)
        {
            try
            {
                string errMsg = string.Empty;
                XCCloudUserTokenModel userTokenKeyModel = (XCCloudUserTokenModel)dicParas[Constant.XCCloudUserTokenModel];
                string storeId = (userTokenKeyModel.DataModel as UserDataModel).StoreID;

                IData_DigitCoinService data_DigitCoinService = BLLContainer.Resolve<IData_DigitCoinService>();
                var data_DigitCoin = data_DigitCoinService.GetModels(p => p.StoreID.Equals(storeId, StringComparison.OrdinalIgnoreCase) && p.Status != (int)DigitStatus.Cancel).OrderBy(or => or.ICardID).Select(o => o.ICardID).ToList();

                return ResponseModelFactory.CreateSuccessModel(isSignKeyReturn, data_DigitCoin);
            }
            catch (Exception e)
            {
                return ResponseModelFactory.CreateReturnModel(isSignKeyReturn, Return_Code.F, e.Message);
            }
        }

        [ApiMethodAttribute(SignKeyEnum = SignKeyEnum.XCCloudUserCacheToken, SysIdAndVersionNo = false)]
        public object AddDigitDestroy(Dictionary<string, object> dicParas)
        {
            try
            {
                string errMsg = string.Empty;
                string iCardID = dicParas.ContainsKey("iCardID") ? (dicParas["iCardID"] + "") : string.Empty;
                XCCloudUserTokenModel userTokenKeyModel = (XCCloudUserTokenModel)dicParas[Constant.XCCloudUserTokenModel];
                string storeId = (userTokenKeyModel.DataModel as UserDataModel).StoreID;
                int userId = Convert.ToInt32(userTokenKeyModel.LogId);

                if (string.IsNullOrEmpty(iCardID))
                {
                    errMsg = "数字币编号不能为空";
                    return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                }

                if (iCardID.Length > 16)
                {
                    errMsg = "数字币编号长度不能超过16位";
                    return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                }

                IData_DigitCoinService data_DigitCoinService = BLLContainer.Resolve<IData_DigitCoinService>();
                if (!data_DigitCoinService.Any(a => a.ICardID.Equals(iCardID, StringComparison.OrdinalIgnoreCase)))
                {
                    errMsg = "该卡档案不存在";
                    return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                }

                //开启EF事务
                using (TransactionScope ts = new TransactionScope())
                {
                    try
                    {
                        var data_DigitCoin = data_DigitCoinService.GetModels(p => p.ICardID.Equals(iCardID, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
                        data_DigitCoin.Status = (int)DigitStatus.Cancel;
                        if (!data_DigitCoinService.Update(data_DigitCoin))
                        {
                            errMsg = "更新数字币档案失败";
                            return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                        }

                        IData_DigitCoinDestroyService data_DigitCoinDestroyService = BLLContainer.Resolve<IData_DigitCoinDestroyService>();
                        Data_DigitCoinDestroy data_DigitCoinDestroy = new Data_DigitCoinDestroy();
                        data_DigitCoinDestroy.DestroyTime = DateTime.Now;
                        data_DigitCoinDestroy.ICCardID = iCardID;
                        data_DigitCoinDestroy.StoreID = storeId;
                        data_DigitCoinDestroy.UserID = userId;
                        if (!data_DigitCoinDestroyService.Add(data_DigitCoinDestroy))
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

        [ApiMethodAttribute(SignKeyEnum = SignKeyEnum.XCCloudUserCacheToken, SysIdAndVersionNo = false)]
        public object GetDigitDestroy(Dictionary<string, object> dicParas)
        {
            try
            {
                string errMsg = string.Empty;
                string destroyTime = dicParas.ContainsKey("destroyTime") ? (dicParas["destroyTime"] + "") : string.Empty;
                string iCardID = dicParas.ContainsKey("iCardID") ? (dicParas["iCardID"] + "") : string.Empty;
                XCCloudUserTokenModel userTokenKeyModel = (XCCloudUserTokenModel)dicParas[Constant.XCCloudUserTokenModel];
                string storeId = (userTokenKeyModel.DataModel as UserDataModel).StoreID;

                if (!string.IsNullOrEmpty(destroyTime))
                {
                    try
                    {
                        Convert.ToDateTime(destroyTime);
                    }
                    catch
                    {
                        errMsg = "销毁时间参数格式不正确";
                        return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                    }
                }

                IData_DigitCoinDestroyService data_DigitCoinDestroyService = BLLContainer.Resolve<IData_DigitCoinDestroyService>();
                var query = data_DigitCoinDestroyService.GetModels(p => p.StoreID.Equals(storeId, StringComparison.OrdinalIgnoreCase));
                if (!string.IsNullOrEmpty(destroyTime))
                {
                    var dt = Convert.ToDateTime(destroyTime);
                    query = query.Where(w => DbFunctions.DiffDays(w.DestroyTime, dt) == 0);
                }

                if (!string.IsNullOrEmpty(iCardID))
                {
                    query = query.Where(w => w.ICCardID.Contains(iCardID));
                }

                IBase_UserInfoService base_UserInfoService = BLLContainer.Resolve<IBase_UserInfoService>();
                var linq = base_UserInfoService.GetModels(p => p.UserType == (int)UserType.Store).Select(o => new { UserID = o.UserID, LogName = o.LogName, RealName = o.RealName });

                var result = query.ToList().Select(o => new
                {
                    ID = o.ID,
                    StoreID = o.StoreID,
                    ICardID = o.ICCardID,
                    DestroyTime = o.DestroyTime,
                    UserID = o.UserID,
                    Note = o.Note,
                    LogName = linq.SingleOrDefault(p => p.UserID == o.UserID).LogName,
                    RealName = linq.SingleOrDefault(p => p.UserID == o.UserID).RealName
                });                

                return ResponseModelFactory.CreateSuccessModel(isSignKeyReturn, result.ToList());
            }
            catch (Exception e)
            {
                return ResponseModelFactory.CreateReturnModel(isSignKeyReturn, Return_Code.F, e.Message);
            }
        }
    }
}