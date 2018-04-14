using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Transactions;
using System.Web;
using XCCloudService.Base;
using XCCloudService.BLL.Container;
using XCCloudService.BLL.IBLL.XCCloud;
using XCCloudService.Common;
using XCCloudService.DBService.BLL;
using XCCloudService.Model.CustomModel.XCCloud;
using XCCloudService.Model.XCCloud;

namespace XXCloudService.Api.XCCloud
{
    [Authorize(Roles = "MerchUser")]
    /// <summary>
    /// Jackpot 的摘要说明
    /// </summary>
    public class Jackpot : ApiBase
    {

        [ApiMethodAttribute(SignKeyEnum = SignKeyEnum.XCCloudUserCacheToken, SysIdAndVersionNo = false)]
        public object QueryJackpotInfo(Dictionary<string, object> dicParas)
        {
            try
            {
                XCCloudUserTokenModel userTokenKeyModel = (XCCloudUserTokenModel)dicParas[Constant.XCCloudUserTokenModel];
                string merchId = userTokenKeyModel.DataModel.MerchID;
                string errMsg = string.Empty;                
                object[] conditions = dicParas.ContainsKey("conditions") ? (object[])dicParas["conditions"] : null;

                SqlParameter[] parameters = new SqlParameter[1];
                string sqlWhere = string.Empty;
                parameters[0] = new SqlParameter("@MerchId", merchId);

                if (conditions != null && conditions.Length > 0)
                {
                    if (!QueryBLL.GenDynamicSql(conditions, "a.", ref sqlWhere, ref parameters, out errMsg))
                    {
                        return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                    }
                }

                string sql = @"select a.* from Data_JackpotInfo a where a.MerchInfo=@MerchId";
                sql = sql + sqlWhere;

                IData_JackpotInfoService data_JackpotInfoService = BLLContainer.Resolve<IData_JackpotInfoService>();
                var data_JackpotInfo = data_JackpotInfoService.SqlQuery<Data_JackpotInfo>(sql, parameters).ToList();

                return ResponseModelFactory.CreateSuccessModel(isSignKeyReturn, data_JackpotInfo);
            }
            catch (Exception e)
            {
                return ResponseModelFactory.CreateReturnModel(isSignKeyReturn, Return_Code.F, e.Message);
            }
        }

        [ApiMethodAttribute(SignKeyEnum = SignKeyEnum.XCCloudUserCacheToken, SysIdAndVersionNo = false)]
        public object GetJackpotDic(Dictionary<string, object> dicParas)
        {
            try
            {
                XCCloudUserTokenModel userTokenKeyModel = (XCCloudUserTokenModel)dicParas[Constant.XCCloudUserTokenModel];
                string merchId = userTokenKeyModel.DataModel.MerchID;
                
                IData_JackpotInfoService data_JackpotInfoService = BLLContainer.Resolve<IData_JackpotInfoService>();
                Dictionary<int, string> pJackpotList = data_JackpotInfoService.GetModels(p=>p.MerchInfo.Equals(merchId, StringComparison.OrdinalIgnoreCase))
                    .Select(o => new { ID = o.ID, ActiveName = o.ActiveName }).Distinct()
                    .ToDictionary(d => d.ID, d => d.ActiveName);

                return ResponseModelFactory.CreateSuccessModel(isSignKeyReturn, pJackpotList);
            }
            catch (Exception e)
            {
                return ResponseModelFactory.CreateReturnModel(isSignKeyReturn, Return_Code.F, e.Message);
            }
        }

        [ApiMethodAttribute(SignKeyEnum = SignKeyEnum.XCCloudUserCacheToken, SysIdAndVersionNo = false)]
        public object GetJackpotInfo(Dictionary<string, object> dicParas)
        {
            try
            {
                string errMsg = string.Empty;
                string id = dicParas.ContainsKey("id") ? (dicParas["id"] + "") : string.Empty;
                if (string.IsNullOrEmpty(id))
                {
                    errMsg = "规则ID不能为空";
                    return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                }

                int iId = Convert.ToInt32(id);                
                IData_JackpotInfoService data_JackpotInfoService = BLLContainer.Resolve<IData_JackpotInfoService>();
                var data_JackpotInfo = data_JackpotInfoService.GetModels(p => p.ID == iId).FirstOrDefault();
                if (data_JackpotInfo == null)
                {
                    errMsg = "该抽奖规则不存在";
                    return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                }

                IData_Jackpot_LevelService data_Jackpot_LevelService = BLLContainer.Resolve<IData_Jackpot_LevelService>(resolveNew: true);
                IBase_GoodsInfoService base_GoodsInfoService = BLLContainer.Resolve<IBase_GoodsInfoService>(resolveNew: true);
                var JackpotLevels = from a in data_Jackpot_LevelService.GetModels(p => p.ActiveID == iId)
                                    join b in base_GoodsInfoService.GetModels() on a.GoodID equals b.ID
                                    select new
                                    {
                                        LevelName = a.LevelName,
                                        GoodCount = a.GoodCount,
                                        Probability = a.Probability,
                                        GoodID = a.GoodID,
                                        GoodName = b.GoodName
                                    };

                var result = new
                {
                    ID = data_JackpotInfo.ID,
                    ActiveName = data_JackpotInfo.ActiveName,
                    Threshold = data_JackpotInfo.Threshold,
                    Concerned = data_JackpotInfo.Concerned,
                    StartTime = data_JackpotInfo.StartTime,
                    EndTime = data_JackpotInfo.EndTime,
                    JackpotLevels = JackpotLevels
                };

                return ResponseModelFactory.CreateAnonymousSuccessModel(isSignKeyReturn, result);
            }
            catch (Exception e)
            {
                return ResponseModelFactory.CreateReturnModel(isSignKeyReturn, Return_Code.F, e.Message);
            }
        }

        [ApiMethodAttribute(SignKeyEnum = SignKeyEnum.XCCloudUserCacheToken, SysIdAndVersionNo = false)]
        public object SaveJackpotInfo(Dictionary<string, object> dicParas)
        {
            try
            {
                XCCloudUserTokenModel userTokenKeyModel = (XCCloudUserTokenModel)dicParas[Constant.XCCloudUserTokenModel];
                string merchId = userTokenKeyModel.DataModel.MerchID;

                string errMsg = string.Empty;
                string id = dicParas.ContainsKey("id") ? (dicParas["id"] + "") : string.Empty;
                string activeName = dicParas.ContainsKey("activeName") ? (dicParas["activeName"] + "") : string.Empty;
                string threshold = dicParas.ContainsKey("threshold") ? (dicParas["threshold"] + "") : string.Empty;
                string concerned = dicParas.ContainsKey("concerned") ? (dicParas["concerned"] + "") : string.Empty;
                string startTime = dicParas.ContainsKey("startTime") ? (dicParas["startTime"] + "") : string.Empty;
                string endTime = dicParas.ContainsKey("endTime") ? (dicParas["endTime"] + "") : string.Empty;
                object[] jackpotLevels = dicParas.ContainsKey("jackpotLevels") ? (object[])dicParas["jackpotLevels"] : null;                
                int iId = 0;
                int.TryParse(id, out iId);

                #region 验证参数
                
                if (string.IsNullOrEmpty(threshold))
                {
                    errMsg = "消费门槛不能为空";
                    return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                }

                if (!Utils.isNumber(threshold))
                {
                    errMsg = "消费门槛格式不正确";
                    return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                }

                if (Convert.ToInt32(threshold) < 0)
                {
                    errMsg = "消费门槛不能为负数";
                    return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                }

                if (string.IsNullOrEmpty(startTime) || string.IsNullOrEmpty(endTime))
                {
                    errMsg = "活动时间不能为空";
                    return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                }
                
                if (Convert.ToDateTime(startTime) > Convert.ToDateTime(endTime))
                {
                    errMsg = "开始时间不能晚于结束时间";
                    return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                }

                #endregion

                //开启EF事务
                using (TransactionScope ts = new TransactionScope())
                {
                    try
                    {
                        IData_JackpotInfoService data_JackpotInfoService = BLLContainer.Resolve<IData_JackpotInfoService>();
                        var data_JackpotInfo = data_JackpotInfoService.GetModels(p => p.ID == iId).FirstOrDefault() ?? new Data_JackpotInfo();
                        data_JackpotInfo.ActiveName = activeName;
                        data_JackpotInfo.Concerned = !string.IsNullOrEmpty(concerned) ? Convert.ToInt32(concerned) : (int?)null;
                        data_JackpotInfo.StartTime = Convert.ToDateTime(startTime);
                        data_JackpotInfo.EndTime = Convert.ToDateTime(endTime);
                        data_JackpotInfo.MerchInfo = merchId;
                        data_JackpotInfo.Threshold = Convert.ToInt32(threshold);
                        if (data_JackpotInfo.ID <= 0)
                        {
                            //新增
                            if (!data_JackpotInfoService.Add(data_JackpotInfo))
                            {
                                errMsg = "添加抽奖规则信息失败";
                                return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                            }
                        }
                        else
                        {
                            //修改
                            if (!data_JackpotInfoService.Update(data_JackpotInfo))
                            {
                                errMsg = "修改抽奖规则信息失败";
                                return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                            }
                        }

                        iId = data_JackpotInfo.ID;
                        if (jackpotLevels != null && jackpotLevels.Count() >= 0)
                        {
                            //先删除已有数据，后添加
                            IData_Jackpot_LevelService data_Jackpot_LevelService = BLLContainer.Resolve<IData_Jackpot_LevelService>();
                            var data_Jackpot_Level = data_Jackpot_LevelService.GetModels(p => p.ActiveID == iId);
                            foreach (var model in data_Jackpot_Level)
                            {
                                data_Jackpot_LevelService.DeleteModel(model);
                            }

                            foreach (IDictionary<string, object> el in jackpotLevels)
                            {
                                if (el != null)
                                {
                                    var dicPara = new Dictionary<string, object>(el, StringComparer.OrdinalIgnoreCase);
                                    string goodId = dicPara.ContainsKey("goodId") ? dicPara["goodId"].ToString() : string.Empty;
                                    string levelName = dicPara.ContainsKey("levelName") ? (dicPara["levelName"] + "") : string.Empty;
                                    string goodCount = dicPara.ContainsKey("goodCount") ? (dicPara["goodCount"] + "") : string.Empty;
                                    string probability = dicPara.ContainsKey("probability") ? (dicPara["probability"] + "") : string.Empty;
                                    if (string.IsNullOrEmpty(goodId))
                                    {
                                        errMsg = "商品ID不能为空";
                                        return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                                    }
                                    if (string.IsNullOrEmpty(levelName))
                                    {
                                        errMsg = "奖品等级不能为空";
                                        return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                                    }

                                    if (string.IsNullOrEmpty(goodCount))
                                    {
                                        errMsg = "奖品数量不能为空";
                                        return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                                    }

                                    if (!Utils.isNumber(goodCount))
                                    {
                                        errMsg = "奖品数量格式不正确";
                                        return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                                    }

                                    if (Convert.ToInt32(goodCount) < 0)
                                    {
                                        errMsg = "奖品数量不能为负数";
                                        return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                                    }

                                    if (string.IsNullOrEmpty(probability))
                                    {
                                        errMsg = "中奖概率不能为空";
                                        return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                                    }

                                    if (!Utils.IsDecimal(probability))
                                    {
                                        errMsg = "中奖概率格式不正确";
                                        return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                                    }

                                    if (Convert.ToDecimal(probability) < 0)
                                    {
                                        errMsg = "中奖概率不能为负数";
                                        return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                                    }
                                    var data_Jackpot_LevelModel = new Data_Jackpot_Level();
                                    data_Jackpot_LevelModel.ActiveID = iId;
                                    data_Jackpot_LevelModel.LevelName = levelName;
                                    data_Jackpot_LevelModel.GoodCount = Convert.ToInt32(goodCount);
                                    data_Jackpot_LevelModel.Probability = Convert.ToDecimal(probability);
                                    data_Jackpot_LevelModel.GoodID = Convert.ToInt32(goodId);
                                    data_Jackpot_LevelService.AddModel(data_Jackpot_LevelModel);                                    
                                }
                                else
                                {
                                    errMsg = "提交数据包含空对象";
                                    return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                                }
                            }

                            if (!data_Jackpot_LevelService.SaveChanges())
                            {
                                errMsg = "保存抽奖规则信息失败";
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
        public object DelJackpotInfo(Dictionary<string, object> dicParas)
        {
            try
            {
                string errMsg = string.Empty;
                string id = dicParas.ContainsKey("id") ? (dicParas["id"] + "") : string.Empty;
                if (string.IsNullOrEmpty(id))
                {
                    errMsg = "规则ID不能为空";
                    return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                }

                int iId = Convert.ToInt32(id);
                IData_Jackpot_LevelService data_Jackpot_LevelService = BLLContainer.Resolve<IData_Jackpot_LevelService>();
                IData_JackpotInfoService data_JackpotInfoService = BLLContainer.Resolve<IData_JackpotInfoService>();
                IData_Jackpot_MatrixService data_Jackpot_MatrixService = BLLContainer.Resolve<IData_Jackpot_MatrixService>();
                if (data_Jackpot_MatrixService.Any(a => a.ActiveID == iId))
                {
                    errMsg = "该抽奖规则已使用不能删除";
                    return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                }

                if (!data_JackpotInfoService.Any(a => a.ID == iId))
                {
                    errMsg = "该抽奖规则信息不存在";
                    return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                }

                //开启EF事务
                using (TransactionScope ts = new TransactionScope())
                {
                    try
                    {
                        var data_JackpotInfo = data_JackpotInfoService.GetModels(p => p.ID == iId).FirstOrDefault();
                        data_JackpotInfoService.DeleteModel(data_JackpotInfo);

                        var data_Jackpot_Level = data_Jackpot_LevelService.GetModels(p => p.ActiveID == iId);
                        foreach (var model in data_Jackpot_Level)
                        {
                            data_Jackpot_LevelService.DeleteModel(model);
                        }

                        if (!data_JackpotInfoService.SaveChanges())
                        {
                            errMsg = "删除抽奖规则信息失败";
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