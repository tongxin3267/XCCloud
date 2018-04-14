using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;
using XCCloudService.Base;
using XCCloudService.BLL.Container;
using XCCloudService.BLL.IBLL.XCCloud;
using XCCloudService.Common;
using XCCloudService.DBService.BLL;
using XCCloudService.Model.CustomModel.XCCloud;
using XCCloudService.Model.XCCloud;
using XXCloudService.Api.XCCloud.Common;

namespace XXCloudService.Api.XCCloud
{
    [Authorize(Roles = "StoreUser")]
    /// <summary>
    /// GoodsInfo 的摘要说明
    /// </summary>
    public class GoodsInfo : ApiBase
    {
        [ApiMethodAttribute(SignKeyEnum = SignKeyEnum.XCCloudUserCacheToken, SysIdAndVersionNo = false)]
        public object QueryGoodsInfo(Dictionary<string, object> dicParas)
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

                string sql = @"select a.ID, a.GoodName, a.Price, a.Points, a.Note, c.DictKey as GoodTypeStr from Base_GoodsInfo a " +
                    " left join Base_StoreInfo b on a.MerchID=b.MerchID " +
                    " left join (select b.* from Dict_System a inner join Dict_System b on a.ID=b.PID where a.DictKey='商品类别' and a.PID=0) c on convert(varchar, a.GoodType)=c.DictValue " +
                    " where a.Status=1 and b.StoreID='" + storeId + "'";
                sql = sql + sqlWhere;
                IBase_GoodsInfoService base_GoodsInfoService = BLLContainer.Resolve<IBase_GoodsInfoService>();
                var base_GoodsInfo = base_GoodsInfoService.SqlQuery<Base_GoodsInfoList>(sql, parameters).ToList();

                return ResponseModelFactory.CreateSuccessModel(isSignKeyReturn, base_GoodsInfo);
            }
            catch (Exception e)
            {
                return ResponseModelFactory.CreateReturnModel(isSignKeyReturn, Return_Code.F, e.Message);
            }
        }

        [ApiMethodAttribute(SignKeyEnum = SignKeyEnum.XCCloudUserCacheToken, SysIdAndVersionNo = false)]
        public object GetGoodsInfo(Dictionary<string, object> dicParas)
        {
            try
            {
                string errMsg = string.Empty;
                string id = dicParas.ContainsKey("id") ? (dicParas["id"] + "") : string.Empty;
                if (string.IsNullOrEmpty(id))
                {
                    errMsg = "商品ID不能为空";
                    return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                }

                int iId = Convert.ToInt32(id);
                IBase_GoodsInfoService base_GoodsInfoService = BLLContainer.Resolve<IBase_GoodsInfoService>();
                if (!base_GoodsInfoService.Any(a => a.ID == iId))
                {
                    errMsg = "该商品信息不存在";
                    return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                }

                var result = from a in base_GoodsInfoService.GetModels(p => p.ID == iId).FirstOrDefault().AsDictionary()
                             select new
                             {
                                 name = a.Key,
                                 value = a.Value
                             };

                return ResponseModelFactory.CreateAnonymousSuccessModel(isSignKeyReturn, result);
            }
            catch (Exception e)
            {
                return ResponseModelFactory.CreateReturnModel(isSignKeyReturn, Return_Code.F, e.Message);
            }
        }

        [ApiMethodAttribute(SignKeyEnum = SignKeyEnum.XCCloudUserCacheToken, SysIdAndVersionNo = false)]
        public object SaveGoodsInfo(Dictionary<string, object> dicParas)
        {
            try
            {
                XCCloudUserTokenModel userTokenKeyModel = (XCCloudUserTokenModel)dicParas[Constant.XCCloudUserTokenModel];
                string storeId = (userTokenKeyModel.DataModel as UserDataModel).StoreID;

                string errMsg = string.Empty;
                string id = dicParas.ContainsKey("id") ? (dicParas["id"] + "") : string.Empty;
                string barCode = dicParas.ContainsKey("barCode") ? (dicParas["barCode"] + "") : string.Empty;
                string goodType = dicParas.ContainsKey("goodType") ? (dicParas["goodType"] + "") : string.Empty;
                string minValue = dicParas.ContainsKey("minValue") ? (dicParas["minValue"] + "") : string.Empty;
                string maxValue = dicParas.ContainsKey("maxValue") ? (dicParas["maxValue"] + "") : string.Empty;
                string goodName = dicParas.ContainsKey("goodName") ? (dicParas["goodName"] + "") : string.Empty;
                string price = dicParas.ContainsKey("price") ? (dicParas["price"] + "") : string.Empty;
                string points = dicParas.ContainsKey("points") ? (dicParas["points"] + "") : string.Empty;
                string note = dicParas.ContainsKey("note") ? (dicParas["note"] + "") : string.Empty;

                #region 参数验证
                if (!string.IsNullOrEmpty(id) && !Utils.isNumber(id))
                {
                    errMsg = "商品ID格式不正确";
                    return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                }

                if (string.IsNullOrEmpty(barCode))
                {
                    errMsg = "商品条码barCode不能为空";
                    return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                }

                if (string.IsNullOrEmpty(goodType))
                {
                    errMsg = "商品类别goodType不能为空";
                    return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                }

                if (string.IsNullOrEmpty(minValue) || string.IsNullOrEmpty(maxValue))
                {
                    errMsg = "库存警戒线不能为空";
                    return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                }

                if (!Utils.isNumber(minValue) || !Utils.isNumber(maxValue))
                {
                    errMsg = "库存警戒线格式不正确";
                    return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                }

                if (Convert.ToInt32(minValue) < 0 || Convert.ToInt32(maxValue) < 0)
                {
                    errMsg = "库存警戒线不能为负数";
                    return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                }

                if (Convert.ToInt32(minValue) > Convert.ToInt32(maxValue))
                {
                    errMsg = "库存警戒线下限须小于上限";
                    return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                }

                if (!string.IsNullOrEmpty(price) && !Utils.IsDecimal(price))
                {
                    errMsg = "零售价格格式不正确";
                    return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                }

                if (!string.IsNullOrEmpty(points) && (!Utils.isNumber(points) || Convert.ToInt32(points) < 0))
                {
                    errMsg = "零售积分格式不正确";
                    return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                }
                #endregion

                IBase_StoreInfoService base_StoreInfoService = BLLContainer.Resolve<IBase_StoreInfoService>();
                var merchId = base_StoreInfoService.GetModels(p => p.StoreID.Equals(storeId, StringComparison.OrdinalIgnoreCase)).FirstOrDefault().MerchID;

                IBase_GoodsInfoService base_GoodsInfoService = BLLContainer.Resolve<IBase_GoodsInfoService>();                                
                if (string.IsNullOrEmpty(id))
                {
                    if (base_GoodsInfoService.Any(a => a.Barcode.Equals(barCode, StringComparison.OrdinalIgnoreCase)))
                    {
                        errMsg = "该商品条码已使用";
                        return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                    }

                    var base_GoodsInfo = new Base_GoodsInfo();
                    Utils.GetModel(dicParas, ref base_GoodsInfo);
                    base_GoodsInfo.MerchID = merchId;
                    base_GoodsInfo.Status = 1;
                    if (!base_GoodsInfoService.Add(base_GoodsInfo))
                    {
                        errMsg = "添加商品信息失败";
                        return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                    }
                }
                else
                {
                    int iId = Convert.ToInt32(id);
                    if (!base_GoodsInfoService.Any(a => a.ID == iId))
                    {
                        errMsg = "该商品信息不存在";
                        return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                    }
                    if (base_GoodsInfoService.Any(a => a.ID != iId && a.Barcode.Equals(barCode, StringComparison.OrdinalIgnoreCase)))
                    {
                        errMsg = "该商品条码已使用";
                        return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                    }

                    var base_GoodsInfo = base_GoodsInfoService.GetModels(p => p.ID == iId).FirstOrDefault();
                    Utils.GetModel(dicParas, ref base_GoodsInfo);
                    if (!base_GoodsInfoService.Update(base_GoodsInfo))
                    {
                        errMsg = "修改商品信息失败";
                        return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
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
        public object DelGoodsInfo(Dictionary<string, object> dicParas)
        {
            try
            {
                string errMsg = string.Empty;
                string id = dicParas.ContainsKey("id") ? (dicParas["id"] + "") : string.Empty;
                if (string.IsNullOrEmpty(id))
                {
                    errMsg = "商品ID不能为空";
                    return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                }

                int iId = Convert.ToInt32(id);
                IBase_GoodsInfoService base_GoodsInfoService = BLLContainer.Resolve<IBase_GoodsInfoService>();
                if (!base_GoodsInfoService.Any(a => a.ID == iId))
                {
                    errMsg = "该商品信息不存在";
                    return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                }

                var base_GoodsInfo = base_GoodsInfoService.GetModels(p => p.ID == iId).FirstOrDefault();
                base_GoodsInfo.Status = 0;
                if (!base_GoodsInfoService.Update(base_GoodsInfo))
                {
                    errMsg = "删除游戏机信息失败";
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
        public object QueryGoodStorage(Dictionary<string, object> dicParas)
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

                string sql = @"select a.ID, a.RealTime, a.UserID, b.RealName, a.StorageCount, a.Note from Data_GoodStorage a " +
                    " left join Base_UserInfo b on a.UserID=b.UserID " +                    
                    " where a.StoreID='" + storeId + "'";
                sql = sql + sqlWhere;
                IData_GoodStorageService data_GoodStorageService = BLLContainer.Resolve<IData_GoodStorageService>();
                var data_GoodStorage = data_GoodStorageService.SqlQuery<Data_GoodStorageList>(sql, parameters).ToList();

                return ResponseModelFactory.CreateSuccessModel(isSignKeyReturn, data_GoodStorage);
            }
            catch (Exception e)
            {
                return ResponseModelFactory.CreateReturnModel(isSignKeyReturn, Return_Code.F, e.Message);
            }
        }

        [ApiMethodAttribute(SignKeyEnum = SignKeyEnum.XCCloudUserCacheToken, SysIdAndVersionNo = false)]
        public object GetGoodStorage(Dictionary<string, object> dicParas)
        {
            try
            {
                string errMsg = string.Empty;
                string id = dicParas.ContainsKey("id") ? (dicParas["id"] + "") : string.Empty;
                if (string.IsNullOrEmpty(id))
                {
                    errMsg = "入库流水号不能为空";
                    return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                }

                int iId = Convert.ToInt32(id);
                IData_GoodStorageService data_GoodStorageService = BLLContainer.Resolve<IData_GoodStorageService>();
                if (!data_GoodStorageService.Any(a => a.ID == iId))
                {
                    errMsg = "该入库信息不存在";
                    return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                }

                var result = from a in data_GoodStorageService.GetModels(p => p.ID == iId).FirstOrDefault().AsDictionary()
                             select new
                             {
                                 name = a.Key,
                                 value = a.Value
                             };

                return ResponseModelFactory.CreateAnonymousSuccessModel(isSignKeyReturn, result);
            }
            catch (Exception e)
            {
                return ResponseModelFactory.CreateReturnModel(isSignKeyReturn, Return_Code.F, e.Message);
            }
        }

        [ApiMethodAttribute(SignKeyEnum = SignKeyEnum.XCCloudUserCacheToken, SysIdAndVersionNo = false)]
        public object AddGoodStorage(Dictionary<string, object> dicParas)
        {
            try
            {
                XCCloudUserTokenModel userTokenKeyModel = (XCCloudUserTokenModel)dicParas[Constant.XCCloudUserTokenModel];
                string storeId = (userTokenKeyModel.DataModel as UserDataModel).StoreID;
                string logId = userTokenKeyModel.LogId;

                string errMsg = string.Empty;
                string barCode = dicParas.ContainsKey("barCode") ? (dicParas["barCode"] + "") : string.Empty;
                string price = dicParas.ContainsKey("price") ? (dicParas["price"] + "") : string.Empty;
                string storageCount = dicParas.ContainsKey("storageCount") ? (dicParas["storageCount"] + "") : string.Empty;
                string totalPrice = dicParas.ContainsKey("totalPrice") ? (dicParas["totalPrice"] + "") : string.Empty;
                string discount = dicParas.ContainsKey("discount") ? (dicParas["discount"] + "") : string.Empty;
                string note = dicParas.ContainsKey("note") ? (dicParas["note"] + "") : string.Empty;

                #region 参数验证

                if (string.IsNullOrEmpty(barCode))
                {
                    errMsg = "商品条码barCode不能为空";
                    return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                }

                if (string.IsNullOrEmpty(price))
                {
                    errMsg = "入库单价price不能为空";
                    return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                }

                if (string.IsNullOrEmpty(storageCount))
                {
                    errMsg = "入库数量storageCount不能为空";
                    return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                }

                if (string.IsNullOrEmpty(totalPrice))
                {
                    errMsg = "入库总额totalPrice不能为空";
                    return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                }

                if (!Utils.isNumber(storageCount))
                {
                    errMsg = "入库数量storageCount格式不正确";
                    return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                }                

                if (!Utils.IsDecimal(price))
                {
                    errMsg = "入库单价price格式不正确";
                    return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                }

                if (!Utils.IsDecimal(totalPrice))
                {
                    errMsg = "入库总额totalPrice格式不正确";
                    return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                }

                if (!string.IsNullOrEmpty(discount) && !Utils.IsDecimal(discount))
                {
                    errMsg = "优惠金额discount格式不正确";
                    return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                }

                #endregion

                IData_GoodStorageService data_GoodStorageService = BLLContainer.Resolve<IData_GoodStorageService>();
                var data_GoodStorage = new Data_GoodStorage();
                Utils.GetModel(dicParas, ref data_GoodStorage);
                data_GoodStorage.StoreID = storeId;
                data_GoodStorage.RealTime = DateTime.Now;
                data_GoodStorage.UserID = Convert.ToInt32(logId);
                if (!data_GoodStorageService.Add(data_GoodStorage))
                {
                    errMsg = "添加商品入库信息失败";
                    return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
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
        public object QueryGoodInventory(Dictionary<string, object> dicParas)
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

                string sql = @"select a.ID, c.DictKey as InventoryTypeStr, (case when a.InventoryType=1 then d.WorkStation when a.InventoryType=3 then e.HeadName else '' end) as InventoryIndexName, a.InventoryTime, a.UserID, b.RealName, a.InventoryCount, a.Note from Data_GoodInventory a " +
                    " left join Base_UserInfo b on a.UserID=b.UserID " +
                    " left join (select b.* from Dict_System a inner join Dict_System b on a.ID=b.PID where a.DictKey='盘点类别' and a.PID=0) c on convert(varchar, a.InventoryType)=c.DictValue " +
                    " left join Data_Workstation d on a.InventoryType=1 and a.InventoryIndex=d.ID " +
                    " left join Data_Head e on a.InventoryType=3 and a.InventoryIndex=e.ID " +
                    " where a.StoreID='" + storeId + "'";
                sql = sql + sqlWhere;
                IData_GoodInventoryService data_GoodInventoryService = BLLContainer.Resolve<IData_GoodInventoryService>();
                var data_GoodInventory = data_GoodInventoryService.SqlQuery<Data_GoodInventoryList>(sql, parameters).ToList();

                return ResponseModelFactory.CreateSuccessModel(isSignKeyReturn, data_GoodInventory);
            }
            catch (Exception e)
            {
                return ResponseModelFactory.CreateReturnModel(isSignKeyReturn, Return_Code.F, e.Message);
            }
        }

        [ApiMethodAttribute(SignKeyEnum = SignKeyEnum.XCCloudUserCacheToken, SysIdAndVersionNo = false)]
        public object GetGoodInventory(Dictionary<string, object> dicParas)
        {
            try
            {
                string errMsg = string.Empty;
                string id = dicParas.ContainsKey("id") ? (dicParas["id"] + "") : string.Empty;
                if (string.IsNullOrEmpty(id))
                {
                    errMsg = "盘点流水号不能为空";
                    return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                }

                int iId = Convert.ToInt32(id);
                IData_GoodInventoryService data_GoodInventoryService = BLLContainer.Resolve<IData_GoodInventoryService>();
                if (!data_GoodInventoryService.Any(a => a.ID == iId))
                {
                    errMsg = "该盘点信息不存在";
                    return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                }

                var result = from a in data_GoodInventoryService.GetModels(p => p.ID == iId).FirstOrDefault().AsDictionary()
                             select new
                             {
                                 name = a.Key,
                                 value = a.Value
                             };

                return ResponseModelFactory.CreateAnonymousSuccessModel(isSignKeyReturn, result);
            }
            catch (Exception e)
            {
                return ResponseModelFactory.CreateReturnModel(isSignKeyReturn, Return_Code.F, e.Message);
            }
        }

        [ApiMethodAttribute(SignKeyEnum = SignKeyEnum.XCCloudUserCacheToken, SysIdAndVersionNo = false)]
        public object AddGoodInventory(Dictionary<string, object> dicParas)
        {
            try
            {
                XCCloudUserTokenModel userTokenKeyModel = (XCCloudUserTokenModel)dicParas[Constant.XCCloudUserTokenModel];
                string storeId = (userTokenKeyModel.DataModel as UserDataModel).StoreID;
                string logId = userTokenKeyModel.LogId;

                string errMsg = string.Empty;
                string barCode = dicParas.ContainsKey("barCode") ? (dicParas["barCode"] + "") : string.Empty;
                string inventoryType = dicParas.ContainsKey("inventoryType") ? (dicParas["inventoryType"] + "") : string.Empty;
                string inventoryIndex = dicParas.ContainsKey("inventoryIndex") ? (dicParas["inventoryIndex"] + "") : string.Empty;
                string predictCount = dicParas.ContainsKey("predictCount") ? (dicParas["predictCount"] + "") : string.Empty;
                string inventoryCount = dicParas.ContainsKey("inventoryCount") ? (dicParas["inventoryCount"] + "") : string.Empty;
                string totalPrice = dicParas.ContainsKey("totalPrice") ? (dicParas["totalPrice"] + "") : string.Empty;
                string note = dicParas.ContainsKey("note") ? (dicParas["note"] + "") : string.Empty;

                #region 参数验证

                if (string.IsNullOrEmpty(barCode))
                {
                    errMsg = "商品条码barCode不能为空";
                    return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                }

                if (string.IsNullOrEmpty(inventoryType))
                {
                    errMsg = "盘点类别inventoryType不能为空";
                    return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                }

                if (!string.IsNullOrEmpty(inventoryIndex) && !Utils.isNumber(inventoryIndex))
                {
                    errMsg = "盘点位置索引inventoryIndex格式不正确";
                    return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                }

                if (string.IsNullOrEmpty(predictCount))
                {
                    errMsg = "预估数量predictCount不能为空";
                    return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                }

                if (string.IsNullOrEmpty(inventoryCount))
                {
                    errMsg = "盘点数量inventoryCount不能为空";
                    return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                }

                if (string.IsNullOrEmpty(totalPrice))
                {
                    errMsg = "库存金额totalPrice不能为空";
                    return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                }

                if (!Utils.isNumber(predictCount))
                {
                    errMsg = "预估数量predictCount格式不正确";
                    return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                }

                if (!Utils.isNumber(inventoryCount))
                {
                    errMsg = "盘点数量inventoryCount格式不正确";
                    return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                }

                if (!Utils.IsDecimal(totalPrice))
                {
                    errMsg = "库存金额totalPrice格式不正确";
                    return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                }

                #endregion

                IData_GoodInventoryService data_GoodInventoryService = BLLContainer.Resolve<IData_GoodInventoryService>();
                var data_GoodInventory = new Data_GoodInventory();
                Utils.GetModel(dicParas, ref data_GoodInventory);
                data_GoodInventory.StoreID = storeId;
                data_GoodInventory.InventoryTime = DateTime.Now;
                data_GoodInventory.UserID = Convert.ToInt32(logId);
                if (!data_GoodInventoryService.Add(data_GoodInventory))
                {
                    errMsg = "添加商品盘点信息失败";
                    return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
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
    }
}