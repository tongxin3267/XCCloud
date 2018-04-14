using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Data.SqlClient;
using System.Globalization;
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
    /// PushRule 的摘要说明
    /// </summary>
    public class PushRule : ApiBase
    {        

        [ApiMethodAttribute(SignKeyEnum = SignKeyEnum.XCCloudUserCacheToken, SysIdAndVersionNo = false)]
        public object QueryPushRule(Dictionary<string, object> dicParas)
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

                string sql = @"select b.GameID, b.GameName, a.ID, a.MemberLevelName, a.Week, a.Allow_Out, a.Allow_In, a.Coin, a.Level, a.StartTime as ST, a.EndTime as ET from Data_Push_Rule a " +
                    " inner join Data_GameInfo b on a.GameIndexID=b.ID " +                   
                    " where b.StoreID='" + storeId + "'";
                sql = sql + sqlWhere;
                IData_Push_RuleService data_Push_RuleService = BLLContainer.Resolve<IData_Push_RuleService>();
                var data_Push_Rule = data_Push_RuleService.SqlQuery<Data_Push_RuleList>(sql, parameters).ToList();

                return ResponseModelFactory.CreateSuccessModel(isSignKeyReturn, data_Push_Rule);
            }
            catch (Exception e)
            {
                return ResponseModelFactory.CreateReturnModel(isSignKeyReturn, Return_Code.F, e.Message);
            }
        }

        [ApiMethodAttribute(SignKeyEnum = SignKeyEnum.XCCloudUserCacheToken, SysIdAndVersionNo = false)]
        public object AddPushRule(Dictionary<string, object> dicParas)
        {
            try
            {
                string errMsg = string.Empty;
                var gameIndexIds = dicParas.ContainsKey("gameIndexIds") ? ((object[])dicParas["gameIndexIds"]).ToList().Select<object, int>(x => Convert.ToInt32(x)) : null;
                string memberLevelId = dicParas.ContainsKey("memberLevelId") ? (dicParas["memberLevelId"] + "") : string.Empty;
                string memberLevelName = dicParas.ContainsKey("memberLevelName") ? (dicParas["memberLevelName"] + "") : string.Empty;
                string allow_Out = dicParas.ContainsKey("allow_Out") ? (dicParas["allow_Out"] + "") : string.Empty;
                string allow_In = dicParas.ContainsKey("allow_In") ? (dicParas["allow_In"] + "") : string.Empty;
                var weeks = dicParas.ContainsKey("weeks") ? ((object[])dicParas["weeks"]).ToList().Select<object, int>(x => Convert.ToInt32(x)) : null;
                string coin = dicParas.ContainsKey("coin") ? (dicParas["coin"] + "") : string.Empty;
                string level = dicParas.ContainsKey("level") ? (dicParas["level"] + "") : string.Empty;
                string startTime = dicParas.ContainsKey("startTime") ? (dicParas["startTime"] + "") : string.Empty;
                string endTime = dicParas.ContainsKey("endTime") ? (dicParas["endTime"] + "") : string.Empty;

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

                if (string.IsNullOrEmpty(memberLevelName))
                {
                    errMsg = "会员级别名称memberLevelName不能为空";
                    return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                }

                if (string.IsNullOrEmpty(allow_Out))
                {
                    errMsg = "是否允许退币allow_Out不能为空";
                    return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                }

                if (!Utils.isNumber(allow_Out))
                {
                    errMsg = "是否允许退币allow_Out格式不正确";
                    return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                }

                if (string.IsNullOrEmpty(allow_In))
                {
                    errMsg = "是否允许投币allow_In不能为空";
                    return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                }

                if (!Utils.isNumber(allow_In))
                {
                    errMsg = "是否允许投币allow_In格式不正确";
                    return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                }

                if (weeks == null || weeks.Count() == 0)
                {
                    errMsg = "周数组weeks不能为空";
                    return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                }

                if (string.IsNullOrEmpty(coin))
                {
                    errMsg = "投币数coin不能为空";
                    return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                }

                if (!Utils.isNumber(coin))
                {
                    errMsg = "投币数coin格式不正确";
                    return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                }

                if (string.IsNullOrEmpty(level))
                {
                    errMsg = "优先级level不能为空";
                    return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                }

                if (!Utils.isNumber(level))
                {
                    errMsg = "优先级level格式不正确";
                    return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                }

                if (string.IsNullOrEmpty(startTime))
                {
                    errMsg = "生效时段startTime不能为空";
                    return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                }

                TimeSpan st;
                try
                {
                    //st = Utils.StrToTimeSpan(startTime);
                    DateTime t = DateTime.ParseExact(startTime, "HH:mm:ss", CultureInfo.InvariantCulture); //24小时制，hh:mm:ss 12小时制
                    st = t - t.Date;
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

                TimeSpan et;
                try
                {
                    //et = Utils.StrToTimeSpan(endTime);
                    DateTime t = DateTime.ParseExact(endTime, "HH:mm:ss", CultureInfo.InvariantCulture); //24小时制，hh:mm:ss 12小时制
                    et = t - t.Date;                    
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

                #endregion

                IData_Push_RuleService data_Push_RuleService = BLLContainer.Resolve<IData_Push_RuleService>();

                //开启EF事务
                using (TransactionScope ts = new TransactionScope())
                {
                    try
                    {
                        foreach (int gameIndexId in gameIndexIds)
                        {
                            foreach (int week in weeks)
                            {
                                var data_Push_Rule = new Data_Push_Rule();
                                data_Push_Rule.Allow_In = Convert.ToInt32(allow_In);
                                data_Push_Rule.Allow_Out = Convert.ToInt32(allow_Out);
                                data_Push_Rule.Coin = Convert.ToInt32(coin);
                                data_Push_Rule.Week = Convert.ToInt32(week);
                                data_Push_Rule.Level = Convert.ToInt32(level);
                                data_Push_Rule.StartTime = st;
                                data_Push_Rule.EndTime = et;
                                data_Push_Rule.MemberLevelID = Convert.ToInt32(memberLevelId);
                                data_Push_Rule.MemberLevelName = memberLevelName;
                                data_Push_Rule.GameIndexID = gameIndexId;
                                data_Push_RuleService.AddModel(data_Push_Rule);                                
                            }                            
                        }

                        if (!data_Push_RuleService.SaveChanges())
                        {
                            errMsg = "添加投币规则失败";
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
        public object DelPushRule(Dictionary<string, object> dicParas)
        {
            try
            {
                string errMsg = string.Empty;
                string id = dicParas.ContainsKey("id") ? (dicParas["id"] + "") : string.Empty;
                if (string.IsNullOrEmpty(id))
                {
                    errMsg = "投币规则流水号不能为空";
                    return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                }

                int iId = Convert.ToInt32(id);
                IData_Push_RuleService data_Push_RuleService = BLLContainer.Resolve<IData_Push_RuleService>();
                if (!data_Push_RuleService.Any(a => a.ID == iId))
                {
                    errMsg = "该投币规则信息不存在";
                    return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                }

                var data_Push_Rule = data_Push_RuleService.GetModels(p => p.ID == iId).FirstOrDefault();
                if (!data_Push_RuleService.Delete(data_Push_Rule))
                {
                    errMsg = "删除投币规则信息失败";
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