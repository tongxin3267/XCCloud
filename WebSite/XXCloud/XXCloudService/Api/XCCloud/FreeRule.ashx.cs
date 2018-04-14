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
using XCCloudService.DBService.BLL;
using XCCloudService.Model.CustomModel.XCCloud;
using XCCloudService.Model.XCCloud;
using XXCloudService.Api.XCCloud.Common;

namespace XXCloudService.Api.XCCloud
{
    [Authorize(Roles = "StoreUser")]
    /// <summary>
    /// FreeRule 的摘要说明
    /// </summary>
    public class FreeRule : ApiBase
    {
        [ApiMethodAttribute(SignKeyEnum = SignKeyEnum.XCCloudUserCacheToken, SysIdAndVersionNo = false)]
        public object QueryFreeRule(Dictionary<string, object> dicParas)
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

                string sql = @"select c.ID, b.GameID, b.GameName, d.MemberLevelName as MemberLevelName, (case c.RuleType when 0 then '模拟机' when 1 then '博彩机' else '' end) as RuleTypeStr, " +
                    " c.NeedCoin, c.FreeCoin, c.ExitCoin, c.StartTime as ST, c.EndTime as ET, c.State from Data_GameFreeRule_List a " +
                    " inner join Data_GameInfo b on a.GameIndexID=b.ID " +
                    " inner join Data_GameFreeRule c on a.RuleID=c.ID " +
                    " left join Data_MemberLevel d on c.MemberLevelID=d.MemberLevelID" +
                    " where b.StoreID='" + storeId + "'";
                sql = sql + sqlWhere;
                IData_GameFreeRuleService data_GameFreeRuleService = BLLContainer.Resolve<IData_GameFreeRuleService>();
                var data_GameFreeRule = data_GameFreeRuleService.SqlQuery<Data_GameFreeRuleList>(sql, parameters).ToList();
                var linq = from a in data_GameFreeRule
                           group a by new { ID = a.ID } into g
                           select new { 
                               g.Key.ID,
                               GameID = string.Join("|", g.Select(p => p.GameID)),
                               GameName = string.Join("|", g.Select(p => p.GameName)),
                               MemberLevelName = g.Max(p=>p.MemberLevelName),
                               RuleTypeStr = g.Max(p => p.RuleTypeStr),
                               NeedCoin = g.Max(p => p.NeedCoin),
                               FreeCoin = g.Max(p => p.FreeCoin),
                               ExitCoin = g.Max(p => p.ExitCoin),
                               StartTime = g.Max(p => p.StartTime),
                               EndTime = g.Max(p => p.EndTime),
                               State = g.Max(p => p.State)
                           };

                return ResponseModelFactory.CreateAnonymousSuccessModel(isSignKeyReturn, linq);
            }
            catch (Exception e)
            {
                return ResponseModelFactory.CreateReturnModel(isSignKeyReturn, Return_Code.F, e.Message);
            }
        }

        [ApiMethodAttribute(SignKeyEnum = SignKeyEnum.XCCloudUserCacheToken, SysIdAndVersionNo = false)]
        public object AddFreeRule(Dictionary<string, object> dicParas)
        {
            try
            {
                string errMsg = string.Empty;
                var gameIndexIds = dicParas.ContainsKey("gameIndexIds") ? ((object[])dicParas["gameIndexIds"]).ToList().Select<object, int>(x => Convert.ToInt32(x)) : null;
                string memberLevelId = dicParas.ContainsKey("memberLevelId") ? (dicParas["memberLevelId"] + "") : string.Empty;
                string ruleType = dicParas.ContainsKey("ruleType") ? (dicParas["ruleType"] + "") : string.Empty;
                string needCoin = dicParas.ContainsKey("needCoin") ? (dicParas["needCoin"] + "") : string.Empty;
                string freeCoin = dicParas.ContainsKey("freeCoin") ? (dicParas["freeCoin"] + "") : string.Empty;
                string exitCoin = dicParas.ContainsKey("exitCoin") ? (dicParas["exitCoin"] + "") : string.Empty;
                string startTime = dicParas.ContainsKey("startTime") ? (dicParas["startTime"] + "") : string.Empty;
                string endTime = dicParas.ContainsKey("endTime") ? (dicParas["endTime"] + "") : string.Empty;
                string times = dicParas.ContainsKey("times") ? (dicParas["times"] + "") : string.Empty;

                #region 参数验证

                if (gameIndexIds == null || gameIndexIds.Count() == 0)
                {
                    errMsg = "游戏机数组gameIndexIds不能为空";
                    return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                }

                if (string.IsNullOrEmpty(memberLevelId))
                {
                    errMsg = "会员级别memberLevelId不能为空";
                    return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                }
                
                if (string.IsNullOrEmpty(ruleType))
                {
                    errMsg = "规则类别ruleType不能为空";
                    return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                }

                if (string.IsNullOrEmpty(needCoin))
                {
                    errMsg = "扣币（局）数needCoin不能为空";
                    return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                }

                if (!Utils.isNumber(needCoin))
                {
                    errMsg = "扣币（局）数needCoin格式不正确";
                    return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                }

                int iNeedCoin = Convert.ToInt32(needCoin);
                if (iNeedCoin < 0)
                {
                    errMsg = "扣币（局）数不能为负数";
                    return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                }

                if (string.IsNullOrEmpty(freeCoin))
                {
                    errMsg = "送币（局）数freeCoin不能为空";
                    return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                }

                if (!Utils.isNumber(freeCoin))
                {
                    errMsg = "送币（局）数freeCoin格式不正确";
                    return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                }

                int iFreeCoin = Convert.ToInt32(freeCoin);
                if (iFreeCoin < 0)
                {
                    errMsg = "送币（局）数不能为负数";
                    return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                }

                int iRuleType = Convert.ToInt32(ruleType);
                if (iRuleType == 1 && string.IsNullOrEmpty(exitCoin)) //博彩机
                {
                    errMsg = "最小退币数exitCoin不能为空";
                    return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                }

                int iExitCoin = 0;
                if (!string.IsNullOrEmpty(exitCoin))
                {
                    if (!Utils.isNumber(exitCoin))
                    {
                        errMsg = "最小退币数exitCoin格式不正确";
                        return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                    }
                    else
                    {
                        if ((iExitCoin = Convert.ToInt32(exitCoin)) < 0)
                        {
                            errMsg = "最小退币数exitCoin不能为负数";
                            return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                        }
                    }
                }

                int iTimes = 0;
                if (!string.IsNullOrEmpty(times))
                {
                    if (!Utils.isNumber(times))
                    {
                        errMsg = "重复次数times格式不正确";
                        return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                    }
                    else
                    {
                        if ((iTimes = Convert.ToInt32(times)) < 0)
                        {
                            errMsg = "重复次数times不能为负数";
                            return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                        }
                    }
                }                                
                
                if (string.IsNullOrEmpty(startTime))
                {
                    errMsg = "生效时段startTime不能为空";
                    return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                }

                DateTime st;
                try
                {
                    st = Convert.ToDateTime(startTime);
                }
                catch
                {
                    errMsg = "生效时段startTime格式不正确";
                    return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                }


                if (string.IsNullOrEmpty(endTime))
                {
                    errMsg = "生效时段endTime不能为空";
                    return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                }

                DateTime et;
                try
                {
                    et = Convert.ToDateTime(endTime);
                }
                catch
                {
                    errMsg = "生效时段endTime格式不正确";
                    return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                }

                if (st.CompareTo(et) > 0)
                {
                    errMsg = "生效时段开始时间startTime须小于结束时间endTime";
                    return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                }

                if (et.Subtract(st).Hours > 23)
                {
                    errMsg = "生效时段须在24小时之内";
                    return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                }

                #endregion

                IData_GameFreeRule_ListService data_GameFreeRule_ListService = BLLContainer.Resolve<IData_GameFreeRule_ListService>();
                IData_GameFreeRuleService data_GameFreeRuleService = BLLContainer.Resolve<IData_GameFreeRuleService>();

                //开启EF事务
                using (TransactionScope ts = new TransactionScope())
                {
                    try
                    {                        
                        for (int i = 0; i <= iTimes; i++)
                        {                            
                            var data_GameFreeRule = new Data_GameFreeRule();
                            data_GameFreeRule.MemberLevelID = Convert.ToInt32(memberLevelId);
                            data_GameFreeRule.RuleType = iRuleType;
                            data_GameFreeRule.State = 1;
                            data_GameFreeRule.NeedCoin = iNeedCoin;
                            data_GameFreeRule.FreeCoin = iFreeCoin;
                            data_GameFreeRule.ExitCoin = iExitCoin;
                            data_GameFreeRule.StartTime = st;
                            data_GameFreeRule.EndTime = et;
                            data_GameFreeRuleService.AddModel(data_GameFreeRule);
                            if (!data_GameFreeRuleService.SaveChanges())
                            {
                                errMsg = "添加送局规则失败";
                                return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                            }

                            foreach (int gameIndexId in gameIndexIds)
                            {
                                var data_GameFreeRule_List = new Data_GameFreeRule_List();
                                data_GameFreeRule_List.GameIndexID = gameIndexId;
                                data_GameFreeRule_List.RuleID = data_GameFreeRule.ID;
                                data_GameFreeRule_ListService.AddModel(data_GameFreeRule_List);
                            }

                            st = st.AddDays(1);
                            et = et.AddDays(1);
                        }

                        if (!data_GameFreeRule_ListService.SaveChanges())
                        {
                            errMsg = "添加送局规则失败";
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
        public object DelFreeRule(Dictionary<string, object> dicParas)
        {
            try
            {
                string errMsg = string.Empty;
                string id = dicParas.ContainsKey("id") ? (dicParas["id"] + "") : string.Empty;
                if (string.IsNullOrEmpty(id))
                {
                    errMsg = "送局规则ID不能为空";
                    return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                }

                int iId = Convert.ToInt32(id);
                IData_GameFreeRuleService data_GameFreeRuleService = BLLContainer.Resolve<IData_GameFreeRuleService>();
                IData_GameFreeRule_ListService data_GameFreeRule_ListService = BLLContainer.Resolve<IData_GameFreeRule_ListService>();
                IFlw_Game_FreeService flw_Game_FreeService = BLLContainer.Resolve<IFlw_Game_FreeService>();
                if (flw_Game_FreeService.Any(a => a.RuleID == iId))
                {
                    errMsg = "该送局规则已使用不能删除";
                    return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                }

                if (!data_GameFreeRuleService.Any(a => a.ID == iId))
                {
                    errMsg = "该送局规则信息不存在";
                    return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                }

                //开启EF事务
                using (TransactionScope ts = new TransactionScope())
                {
                    try
                    {
                        var data_GameFreeRule = data_GameFreeRuleService.GetModels(p => p.ID == iId).FirstOrDefault();
                        data_GameFreeRuleService.DeleteModel(data_GameFreeRule);                        

                        var data_GameFreeRule_List = data_GameFreeRule_ListService.GetModels(p => p.RuleID == iId);
                        foreach (var data_GameFreeRule_ListMode in data_GameFreeRule_List)
                        {
                            data_GameFreeRule_ListService.DeleteModel(data_GameFreeRule_ListMode);
                        }

                        if (!data_GameFreeRuleService.SaveChanges())
                        {
                            errMsg = "删除送局规则信息失败";
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
        public object EnFreeRule(Dictionary<string, object> dicParas)
        {
            try
            {
                string errMsg = string.Empty;
                string id = dicParas.ContainsKey("id") ? (dicParas["id"] + "") : string.Empty;
                string state = dicParas.ContainsKey("state") ? (dicParas["state"] + "") : string.Empty;

                if (string.IsNullOrEmpty(id))
                {
                    errMsg = "送局规则ID不能为空";
                    return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                }

                if (string.IsNullOrEmpty(state))
                {
                    errMsg = "启用状态state不能为空";
                    return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                }

                int iState = Convert.ToInt32(state);
                int iId = Convert.ToInt32(id);
                IData_GameFreeRuleService data_GameFreeRuleService = BLLContainer.Resolve<IData_GameFreeRuleService>();
                if (!data_GameFreeRuleService.Any(a => a.ID == iId))
                {
                    errMsg = "该送局规则信息不存在";
                    return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                }

                var data_GameFreeRule = data_GameFreeRuleService.GetModels(p => p.ID == iId).FirstOrDefault();
                data_GameFreeRule.State = iState;
                if (!data_GameFreeRuleService.Update(data_GameFreeRule))
                {
                    errMsg = "更新送局规则信息失败";
                    return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
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