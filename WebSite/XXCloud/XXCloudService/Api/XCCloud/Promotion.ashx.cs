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
using XCCloudService.Common;
using XCCloudService.Common.Enum;
using XCCloudService.DAL;
using XCCloudService.DBService.BLL;
using XCCloudService.Model.CustomModel.XCCloud;
using XCCloudService.Model.XCCloud;

namespace XXCloudService.Api.XCCloud
{
    [Authorize(Roles = "MerchUser")]
    /// <summary>
    /// Promotion 的摘要说明
    /// </summary>
    public class Promotion : ApiBase
    {
        private bool isEveryDay(string weekDays)
        {
            return !string.IsNullOrEmpty(weekDays) && weekDays.Contains("1") && weekDays.Contains("2") && weekDays.Contains("3") && weekDays.Contains("4") && weekDays.Contains("5") && weekDays.Contains("6") && weekDays.Contains("7");
        }
        private string getWeekName(string weekDays)
        {
            return isEveryDay(weekDays) ? "每天" : (!string.IsNullOrEmpty(weekDays) ? weekDays.Replace("1", "周一").Replace("2", "周二").Replace("3", "周三").Replace("4", "周四").Replace("5", "周五").Replace("6", "周六").Replace("7", "周日") : string.Empty);
        }
        private string getTimeName(TimeSpan? startTime, TimeSpan? endTime)
        {
            var StartTime = string.Format("{0:c}", startTime).Substring(0, 5);
            var EndTime = string.Format("{0:c}", endTime).Substring(0, 5);
            return (StartTime == "00:00" && EndTime == "23:59") ? "全天" : (StartTime + "~" + EndTime);
        }
        private string getPeriodTypeName(PeriodType periodType)
        {
            return periodType.Equals(PeriodType.Custom) ? "自定义" : periodType.Equals(PeriodType.Double) ? "周末" : periodType.Equals(PeriodType.Holiday) ? "法定节假日" : periodType.Equals(PeriodType.WorkDay) ? "工作日" : string.Empty;
        }
        private bool saveFoodLevel(int iFoodId, DateTime? startTime, DateTime? endTime, object[] foodLevels, out string errMsg)
        {
            errMsg = string.Empty;
            if (foodLevels != null && foodLevels.Count() >= 0)
            {
                //先删除已有数据，后添加
                var dbContext = DbContextFactory.CreateByModelNamespace(typeof(Data_Food_Level).Namespace);
                var data_Food_Level = dbContext.Set<Data_Food_Level>().Where(p => p.FoodID == iFoodId).ToList();
                foreach (var model in data_Food_Level)
                {
                    dbContext.Entry(model).State = EntityState.Deleted;
                }

                var foodLevelList = new List<Data_Food_Level>();
                foreach (IDictionary<string, object> el in foodLevels)
                {
                    if (el != null)
                    {
                        var dicPara = new Dictionary<string, object>(el, StringComparer.OrdinalIgnoreCase);
                        string memberLevelIDs = dicPara.ContainsKey("memberLevelIDs") ? dicPara["memberLevelIDs"].ToString() : string.Empty;
                        string week = dicPara.ContainsKey("week") ? dicPara["week"].ToString() : string.Empty;
                        string start = dicPara.ContainsKey("startTime") ? dicPara["startTime"].ToString() : string.Empty;
                        string end = dicPara.ContainsKey("endTime") ? dicPara["endTime"].ToString() : string.Empty;
                        string client = dicPara.ContainsKey("clientPrice") ? dicPara["clientPrice"].ToString() : string.Empty;
                        string vip = dicPara.ContainsKey("vipPrice") ? dicPara["vipPrice"].ToString() : string.Empty;
                        string day_sale_count = dicPara.ContainsKey("day_sale_count") ? dicPara["day_sale_count"].ToString() : string.Empty;
                        string member_day_sale_count = dicPara.ContainsKey("member_day_sale_count") ? dicPara["member_day_sale_count"].ToString() : string.Empty;
                        string allowCoin = dicPara.ContainsKey("allowCoin") ? (dicPara["allowCoin"] + "") : string.Empty;
                        string coins = dicPara.ContainsKey("coins") ? (dicPara["coins"] + "") : string.Empty;
                        string allowPoint = dicPara.ContainsKey("allowPoint") ? (dicPara["allowPoint"] + "") : string.Empty;
                        string points = dicPara.ContainsKey("points") ? (dicPara["points"] + "") : string.Empty;
                        string allowLottery = dicPara.ContainsKey("allowLottery") ? (dicPara["allowLottery"] + "") : string.Empty;
                        string lottery = dicPara.ContainsKey("lottery") ? (dicPara["lottery"] + "") : string.Empty;
                        string periodType = dicPara.ContainsKey("periodType") ? (dicPara["periodType"] + "") : string.Empty;

                        #region 参数验证
                        if (string.IsNullOrEmpty(start) || string.IsNullOrEmpty(end))
                        {
                            errMsg = "优惠时段不能为空";
                            return false;
                        }
                        if (TimeSpan.Compare(TimeSpan.Parse(start), TimeSpan.Parse(end)) > 0)
                        {
                            errMsg = "优惠开始时段不能晚于结束时段";
                            return false;
                        }
                        if (string.IsNullOrEmpty(memberLevelIDs))
                        {
                            errMsg = "会员级别ID列表不能为空";
                            return false;
                        }
                        if (Convert.ToInt32(day_sale_count) < 0)
                        {
                            errMsg = "每天限购数不能为负数";
                            return false;
                        }
                        if (Convert.ToInt32(member_day_sale_count) < 0)
                        {
                            errMsg = "每人每天限购数不能为负数";
                            return false;
                        }
                        if (Convert.ToDecimal(client) < 0)
                        {
                            errMsg = "散客优惠价不能为负数";
                            return false;
                        }
                        if (Convert.ToDecimal(vip) < 0)
                        {
                            errMsg = "会员优惠价不能为负数";
                            return false;
                        }
                        if (!string.IsNullOrEmpty(coins) && Convert.ToInt32(coins) < 0)
                        {
                            errMsg = "允许支付币数不能为负数";
                            return false;
                        }
                        if (!string.IsNullOrEmpty(points) && Convert.ToInt32(points) < 0)
                        {
                            errMsg = "允许支付积分不能为负数";
                            return false;
                        }
                        if (!string.IsNullOrEmpty(lottery) && Convert.ToInt32(lottery) < 0)
                        {
                            errMsg = "允许支付彩票不能为负数";
                            return false;
                        }
                        if (string.IsNullOrEmpty(periodType))
                        {
                            errMsg = "时段类型不能为空";
                            return false;
                        }
                        int iPeriodType = Convert.ToInt32(periodType);
                        if (iPeriodType == (int)PeriodType.Custom && string.IsNullOrEmpty(week))
                        {
                            errMsg = "自定义模式周数不能为空";
                            return false;
                        }
                        #endregion

                        List<string> memberLevelIDList = memberLevelIDs.Split('|').ToList();
                        foreach (var memberLevelID in memberLevelIDList)
                        {
                            var data_Food_LevelModel = new Data_Food_Level();
                            data_Food_LevelModel.FoodID = iFoodId;
                            data_Food_LevelModel.MemberLevelID = Convert.ToInt32(memberLevelID);
                            data_Food_LevelModel.Week = week;
                            data_Food_LevelModel.StartTime = TimeSpan.Parse(start);
                            data_Food_LevelModel.EndTime = TimeSpan.Parse(end);
                            data_Food_LevelModel.ClientPrice = Convert.ToDecimal(client);
                            data_Food_LevelModel.VIPPrice = Convert.ToDecimal(vip);
                            data_Food_LevelModel.day_sale_count = Convert.ToInt32(day_sale_count);
                            data_Food_LevelModel.member_day_sale_count = Convert.ToInt32(member_day_sale_count);
                            data_Food_LevelModel.StartDate = startTime;
                            data_Food_LevelModel.EndDate = endTime;
                            data_Food_LevelModel.AllowCoin = !string.IsNullOrEmpty(allowCoin) ? Convert.ToInt32(allowCoin) : (int?)null;
                            data_Food_LevelModel.Coins = !string.IsNullOrEmpty(coins) ? Convert.ToInt32(coins) : (int?)null;
                            data_Food_LevelModel.AllowPoint = !string.IsNullOrEmpty(allowPoint) ? Convert.ToInt32(allowPoint) : (int?)null;
                            data_Food_LevelModel.Points = !string.IsNullOrEmpty(points) ? Convert.ToInt32(points) : (int?)null;
                            data_Food_LevelModel.AllowLottery = !string.IsNullOrEmpty(allowLottery) ? Convert.ToInt32(allowLottery) : (int?)null;
                            data_Food_LevelModel.Lottery = !string.IsNullOrEmpty(lottery) ? Convert.ToInt32(lottery) : (int?)null;
                            data_Food_LevelModel.PeriodType = !string.IsNullOrEmpty(periodType) ? Convert.ToInt32(periodType) : (int?)null;
                            foodLevelList.Add(data_Food_LevelModel);
                            dbContext.Entry(data_Food_LevelModel).State = EntityState.Added;
                        }                        
                    }
                    else
                    {
                        errMsg = "提交数据包含空对象";
                        return false;
                    }
                }

                //同一会员级别，时段类型为工作模式（即0~2）与自定义模式（即3）不能共存                                    
                foreach (var memberLevelId in foodLevelList.GroupBy(g => g.MemberLevelID).Select(o => o.Key))
                {
                    string memberLevelName = (from b in dbContext.Set<Data_MemberLevel>()
                                              where b.MemberLevelID == memberLevelId
                                              select b.MemberLevelName).FirstOrDefault();
                    if (foodLevelList.Any(w => w.MemberLevelID.Equals(memberLevelId) && w.PeriodType.Equals((int)PeriodType.Custom)) &&
                        foodLevelList.Any(w => w.MemberLevelID.Equals(memberLevelId) && !w.PeriodType.Equals((int)PeriodType.Custom)))
                    {
                        errMsg = string.Format("同一会员级别，工作模式与自定义模式不能共存 会员级别:{0}", memberLevelName);
                        return false;
                    }
                }

                //同一会员级别，同一个时段类型（如果是自定义模式，即同一天），同一时段只能有一个优惠策略
                foreach (var data_Food_LevelModel in foodLevelList)
                {
                    int memberLevelId = (int)data_Food_LevelModel.MemberLevelID;
                    string memberLevelName = (from b in dbContext.Set<Data_MemberLevel>()
                                              where b.MemberLevelID == memberLevelId
                                              select b.MemberLevelName).FirstOrDefault();
                    string timeSpan = Utils.TimeSpanToStr(data_Food_LevelModel.StartTime.Value) + "~" + Utils.TimeSpanToStr(data_Food_LevelModel.EndTime.Value);
                    if (data_Food_LevelModel.PeriodType == (int)PeriodType.Custom)
                    {
                        var weeks = data_Food_LevelModel.Week.Split('|').ToList();
                        foreach (var day in weeks)
                        {
                            if (foodLevelList.Where(w => w.MemberLevelID.Equals(data_Food_LevelModel.MemberLevelID) && w.Week.Contains(day) &&
                                 ((TimeSpan.Compare((TimeSpan)data_Food_LevelModel.StartTime, (TimeSpan)w.StartTime) >= 0 && TimeSpan.Compare((TimeSpan)data_Food_LevelModel.StartTime, (TimeSpan)w.EndTime) < 0) ||
                                 (TimeSpan.Compare((TimeSpan)data_Food_LevelModel.EndTime, (TimeSpan)w.StartTime) > 0 && TimeSpan.Compare((TimeSpan)data_Food_LevelModel.EndTime, (TimeSpan)w.EndTime) <= 0))).Count() > 1)
                            {
                                errMsg = string.Format("同一会员级别，同一时段只能有一个优惠策略 会员级别:{0} 自定义模式:{1} 优惠时段:{2}",
                                    memberLevelName, getWeekName(day), timeSpan);
                                return false;
                            }
                        }
                    }
                    else
                    {
                        if (foodLevelList.Where(w => w.MemberLevelID.Equals(data_Food_LevelModel.MemberLevelID) && w.PeriodType.Equals(data_Food_LevelModel.PeriodType) &&
                                 ((TimeSpan.Compare((TimeSpan)data_Food_LevelModel.StartTime, (TimeSpan)w.StartTime) >= 0 && TimeSpan.Compare((TimeSpan)data_Food_LevelModel.StartTime, (TimeSpan)w.EndTime) < 0) ||
                                 (TimeSpan.Compare((TimeSpan)data_Food_LevelModel.EndTime, (TimeSpan)w.StartTime) > 0 && TimeSpan.Compare((TimeSpan)data_Food_LevelModel.EndTime, (TimeSpan)w.EndTime) <= 0))).Count() > 1)
                        {
                            errMsg = string.Format("同一会员级别，同一时段只能有一个优惠策略 会员级别:{0} 工作模式:{1} 优惠时段:{2}",
                                memberLevelName, getPeriodTypeName((PeriodType)data_Food_LevelModel.PeriodType), timeSpan);
                            return false;
                        }
                    }
                }

                if (dbContext.SaveChanges() < 0)
                {
                    errMsg = "保存套餐级别信息失败";
                    return false;
                }
            }

            return true;
        }

        [ApiMethodAttribute(SignKeyEnum = SignKeyEnum.XCCloudUserCacheToken, SysIdAndVersionNo = false)]
        public object GetFoodInfoList(Dictionary<string, object> dicParas)
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

                string sql = @"select a.FoodID, a.FoodName, b.DictKey as FoodTypeStr, c.DictKey as RechargeTypeStr, (case a.AllowInternet when 1 then '允许' when 0 then '禁止' else '' end) as AllowInternetStr, " +
                    " (case a.AllowPrint when 1 then '允许' when 0 then '禁止' else '' end) as AllowPrintStr, (case a.ForeAuthorize when 1 then '允许' when 0 then '禁止' else '' end) as ForeAuthorizeStr, " +
                    " (case when a.StartTime is null or a.StartTime='' then '' else convert(varchar,a.StartTime,23) end) as StartTimeStr, (case when a.EndTime is null or a.EndTime='' then '' else convert(varchar,a.EndTime,23) end) as EndTimeStr from Data_FoodInfo a " +
                    " left join (select b.* from Dict_System a inner join Dict_System b on a.ID=b.PID where a.DictKey='套餐类别' and a.PID=0) b on convert(varchar, a.FoodType)=b.DictValue " +
                    " left join (select b.* from Dict_System a inner join Dict_System b on a.ID=b.PID where a.DictKey='充值方式' and a.PID=0) c on convert(varchar, a.RechargeType)=c.DictValue " +
                    " where a.MerchID=@MerchId and a.FoodType<>1 ";
                sql = sql + sqlWhere;

                IData_FoodInfoService data_FoodInfoService = BLLContainer.Resolve<IData_FoodInfoService>();
                var data_FoodInfo = data_FoodInfoService.SqlQuery<Data_FoodInfoListModel>(sql, parameters).ToList();
                
                return ResponseModelFactory.CreateSuccessModel(isSignKeyReturn, data_FoodInfo);
            }
            catch (Exception e)
            {
                return ResponseModelFactory.CreateReturnModel(isSignKeyReturn, Return_Code.F, e.Message);
            }
        }

        [ApiMethodAttribute(SignKeyEnum = SignKeyEnum.XCCloudUserCacheToken, SysIdAndVersionNo = false)]
        public object GetFoodInfo(Dictionary<string, object> dicParas)
        {
            try
            {
                string errMsg = string.Empty;               
                string foodId = dicParas.ContainsKey("foodId") ? (dicParas["foodId"] + "") : string.Empty;

                if (string.IsNullOrEmpty(foodId))
                {
                    errMsg = "套餐编号不能为空";
                    return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                }

                int iFoodId = Convert.ToInt32(foodId);                
                IData_FoodInfoService data_FoodInfoService = BLLContainer.Resolve<IData_FoodInfoService>();                
                var FoodInfo = data_FoodInfoService.GetModels(p => p.FoodID == iFoodId).FirstOrDefault();
                if(FoodInfo == null)
                {
                    errMsg = "该套餐不存在";
                    return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                }                

                var dbContext = DbContextFactory.CreateByModelNamespace(typeof(Base_UserInfo).Namespace);
                var FoodLevels = (from a in dbContext.Set<Data_Food_Level>()
                                  where a.FoodID == iFoodId
                                  group a by new { a.PeriodType, a.Week, a.StartTime, a.EndTime } into g
                                  select new
                                  {
                                      MemberLevelInfos = (from b in dbContext.Set<Data_MemberLevel>()
                                                          where g.Select(o => o.MemberLevelID).Contains(b.MemberLevelID)
                                                          select new { b.MemberLevelID, b.MemberLevelName }).ToList(),
                                      Key = g.Key,
                                      ClientPrice = g.Max(m => m.ClientPrice),
                                      VIPPrice = g.Max(m => m.VIPPrice),
                                      day_sale_count = g.Max(m => m.day_sale_count),
                                      member_day_sale_count = g.Max(m => m.member_day_sale_count),
                                      AllowCoin = g.Max(m => m.AllowCoin),
                                      Coins = g.Max(m => m.Coins),
                                      AllowPoint = g.Max(m => m.AllowPoint),
                                      Points = g.Max(m => m.Points),
                                      AllowLottery = g.Max(m => m.AllowLottery),
                                      Lottery = g.Max(m => m.Lottery)
                                  }).ToList().Select(o => new
                                  {
                                      MemberLevelIDs = string.Join("|", o.MemberLevelInfos.Select(s => s.MemberLevelID)),
                                      MemberLevels = string.Join("|", o.MemberLevelInfos.Select(s => s.MemberLevelName)),
                                      PeriodType = o.Key.PeriodType,
                                      Week = o.Key.Week,
                                      WeekStr = getWeekName(o.Key.Week),
                                      StartTime = string.Format("{0:c}", o.Key.StartTime).Substring(0, 5),
                                      EndTime = string.Format("{0:c}", o.Key.EndTime).Substring(0, 5),
                                      Time = getTimeName(o.Key.StartTime, o.Key.EndTime),
                                      ClientPrice = o.ClientPrice,
                                      VIPPrice = o.VIPPrice,
                                      day_sale_count = o.day_sale_count,
                                      member_day_sale_count = o.member_day_sale_count,
                                      AllowCoin = o.AllowCoin,
                                      Coins = o.Coins,
                                      AllowPoint = o.AllowPoint,
                                      Points = o.Points,
                                      AllowLottery = o.AllowLottery,
                                      Lottery = o.Lottery                                      
                                  });

                var FoodDetial = (from a in dbContext.Set<Data_Food_Detial>() where a.FoodID == iFoodId && a.Status == 1 select a).FirstOrDefault();
                switch (FoodInfo.FoodType)
                {

                    #region Coin
                    case (int)FoodType.Coin:
                        {
                            
                            var result = new
                            {
                                CoinSale = new
                                {
                                    FoodID = FoodInfo.FoodID,
                                    FoodName = FoodInfo.FoodName,
                                    RechargeType = FoodInfo.RechargeType,
                                    StartTime = FoodInfo.StartTime,
                                    EndTime = FoodInfo.EndTime,
                                    ImageURL = FoodInfo.ImageURL,
                                    Note = FoodInfo.Note,
                                    FoodState = FoodInfo.FoodState,
                                    AllowInternet = FoodInfo.AllowInternet,
                                    AllowPrint = FoodInfo.AllowPrint,
                                    ForeAuthorize = FoodInfo.ForeAuthorize,
                                    ClientPrice = FoodInfo.ClientPrice,
                                    MemberPrice = FoodInfo.MemberPrice,
                                    Coins = FoodDetial != null ? FoodDetial.ContainCount : 0,
                                    Days = FoodDetial != null ? FoodDetial.Days : 0,
                                    FoodLevels = FoodLevels
                                }
                            };

                            return ResponseModelFactory.CreateSuccessModel(isSignKeyReturn, result);
                        }
                    #endregion

                    #region Digit
                    case (int)FoodType.Digit:
                        {
                            var result = new
                            {
                                Digit = new
                                {
                                    FoodID = FoodInfo.FoodID,
                                    FoodName = FoodInfo.FoodName,
                                    StartTime = FoodInfo.StartTime,
                                    EndTime = FoodInfo.EndTime,
                                    ImageURL = FoodInfo.ImageURL,
                                    Note = FoodInfo.Note,
                                    FoodState = FoodInfo.FoodState,
                                    AllowPrint = FoodInfo.AllowPrint,
                                    ForeAuthorize = FoodInfo.ForeAuthorize,
                                    Price = FoodInfo.ClientPrice,
                                    Coins = FoodDetial != null ? FoodDetial.ContainCount : 0,
                                    Days = FoodDetial != null ? FoodDetial.Days : 0
                                }
                            };

                            return ResponseModelFactory.CreateSuccessModel(isSignKeyReturn, result);
                        }
                    #endregion

                    #region Good
                    case (int)FoodType.Good:
                        {
                            int GoodTypeId = dbContext.Set<Dict_System>().Where(p => p.DictKey.Equals("商品类别")).FirstOrDefault().ID;
                            var GoodInfos = from a in dbContext.Set<Base_GoodsInfo>()
                                            join b in dbContext.Set<Dict_System>().Where(p => p.PID == GoodTypeId) on (a.Status + "") equals b.DictValue into b1
                                            from b in b1.DefaultIfEmpty()
                                            join c in dbContext.Set<Data_Food_Detial>().Where(p => p.FoodID == iFoodId && p.Status == 1 && p.FoodType == (int)FoodDetailType.Good) on a.ID equals c.ContainID
                                            select new
                                            {
                                                ID = a.ID,
                                                Barcode = a.Barcode,
                                                GoodName = a.GoodName,
                                                GoodTypeStr = b != null ? b.DictKey : string.Empty,
                                                Days = c.Days
                                            };

                            var result = new
                            {
                                Good = new
                                {
                                    FoodID = FoodInfo.FoodID,
                                    FoodName = FoodInfo.FoodName,
                                    ImageURL = FoodInfo.ImageURL,
                                    FoodState = FoodInfo.FoodState,
                                    AllowInternet = FoodInfo.AllowInternet,
                                    AllowPrint = FoodInfo.AllowPrint,
                                    ForeAuthorize = FoodInfo.ForeAuthorize,
                                    ClientPrice = FoodInfo.ClientPrice,
                                    MemberPrice = FoodInfo.MemberPrice,
                                    AllowCoin = FoodInfo.AllowCoin,
                                    Coins = FoodInfo.Coins,
                                    AllowPoint = FoodInfo.AllowPoint,
                                    Points = FoodInfo.Points,
                                    AllowLottery = FoodInfo.AllowLottery,
                                    Lottery = FoodInfo.Lottery,
                                    GoodInfos = GoodInfos,
                                    FoodLevels = FoodLevels
                                }
                            };

                            return ResponseModelFactory.CreateSuccessModel(isSignKeyReturn, result);
                        }
                    #endregion

                    #region Ticket
                    case (int)FoodType.Ticket:
                        {
                            int FeeTypeId = dbContext.Set<Dict_System>().Where(p => p.DictKey.Equals("计费方式")).FirstOrDefault().ID;
                            var ProjectItem = FoodDetial != null ? (from a in dbContext.Set<Data_ProjectInfo>() where a.ID == FoodDetial.ContainID select a).FirstOrDefault() : null;
                            var TicketTypeItem = ProjectItem != null ? (from a in dbContext.Set<Dict_System>() where a.DictValue.Equals(ProjectItem.FeeType + "") select a).FirstOrDefault() : null;

                            var result = new
                            {
                                Ticket = new
                                {
                                    FoodID = FoodInfo.FoodID,
                                    ImageURL = FoodInfo.ImageURL,
                                    FoodState = FoodInfo.FoodState,
                                    AllowInternet = FoodInfo.AllowInternet,
                                    AllowPrint = FoodInfo.AllowPrint,
                                    ForeAuthorize = FoodInfo.ForeAuthorize,
                                    ClientPrice = FoodInfo.ClientPrice,
                                    MemberPrice = FoodInfo.MemberPrice,
                                    AllowCoin = FoodInfo.AllowCoin,
                                    Coins = FoodInfo.Coins,
                                    AllowPoint = FoodInfo.AllowPoint,
                                    Points = FoodInfo.Points,
                                    AllowLottery = FoodInfo.AllowLottery,
                                    Lottery = FoodInfo.Lottery,
                                    ProjectId = FoodDetial != null ? FoodDetial.ContainID : (int?)null,
                                    Count = FoodDetial != null ? FoodDetial.ContainCount : (int?)null,
                                    Days = FoodDetial != null ? FoodDetial.Days : (int?)null,
                                    TicketTypeStr = TicketTypeItem != null ? TicketTypeItem.DictKey : string.Empty,
                                    FeeCycle = ProjectItem != null ? ProjectItem.FeeCycle : (int?)null,
                                    FeeDeposit = ProjectItem != null ? ProjectItem.FeeDeposit : (int?)null,
                                    SignOutEN = ProjectItem != null ? ProjectItem.SignOutEN : (int?)null,
                                    WhenLock = ProjectItem != null ? ProjectItem.WhenLock : (int?)null,
                                    FoodLevels = FoodLevels
                                }
                            };

                            return ResponseModelFactory.CreateSuccessModel(isSignKeyReturn, result);
                        }
                    #endregion

                    #region Mixed
                    case (int)FoodType.Mixed:
                        {
                            int FoodDetailId = dbContext.Set<Dict_System>().Where(p => p.DictKey.Equals("套餐内容")).FirstOrDefault().ID;
                            int FoodDetailTypeId = dbContext.Set<Dict_System>().Where(p => p.DictKey.Equals("套餐类别") && p.PID == FoodDetailId).FirstOrDefault().ID;
                            int FeeTypeId = dbContext.Set<Dict_System>().Where(p => p.DictKey.Equals("计费方式")).FirstOrDefault().ID;
                            
                            var FoodDetials = from a in dbContext.Set<Data_Food_Detial>().Where(p => p.FoodID == iFoodId && p.Status == 1)
                                              join b in dbContext.Set<Data_ProjectInfo>() on new { ContainID = a.ContainID, FoodType = a.FoodType } equals new { ContainID = (int?)b.ID, FoodType = (int?)FoodDetailType.Ticket } into b1
                                              from b in b1.DefaultIfEmpty()
                                              join c in dbContext.Set<Base_GoodsInfo>() on new { ContainID = a.ContainID, FoodType = a.FoodType } equals new { ContainID = (int?)c.ID, FoodType = (int?)FoodDetailType.Good } into c1
                                              from c in c1.DefaultIfEmpty()
                                              join d in dbContext.Set<Dict_System>().Where(p => p.PID == FoodDetailTypeId) on (a.FoodType + "") equals d.DictValue into d1
                                              from d in d1.DefaultIfEmpty()
                                              join e in dbContext.Set<Dict_System>().Where(p => p.PID == FeeTypeId) on (b.FeeType + "") equals e.DictValue into e1
                                              from e in e1.DefaultIfEmpty()
                                              select new
                                              {
                                                  ID = a.ID,
                                                  ProjectName = (a.FoodType == (int)FoodDetailType.Coin) ? "游戏币" : (a.FoodType == (int)FoodDetailType.Digit) ? "数字币" : (a.FoodType == (int)FoodDetailType.Good) ? (c != null ? c.GoodName : string.Empty) :
                                                                (a.FoodType == (int)FoodDetailType.Ticket) ? (b != null ? b.ProjectName : string.Empty) : string.Empty,
                                                  FoodDetailType = a.FoodType,
                                                  ContainID = a.ContainID,
                                                  FoodTypeStr = (d != null ? d.DictKey : string.Empty) + (e != null ? e.DictKey : string.Empty),
                                                  ContainCount = a.ContainCount,
                                                  Days = a.Days,
                                                  WeightValue = a.WeightValue
                                              };


                            var result = new
                            {
                                Mixed = new
                                {
                                    FoodID = FoodInfo.FoodID,
                                    FoodName = FoodInfo.FoodName,
                                    StartTime = string.Format("{0:yyyy-MM-dd}", FoodInfo.StartTime),
                                    EndTime = string.Format("{0:yyyy-MM-dd}", FoodInfo.EndTime),
                                    RechargeType = FoodInfo.RechargeType,
                                    ImageURL = FoodInfo.ImageURL,
                                    FoodState = FoodInfo.FoodState,
                                    AllowInternet = FoodInfo.AllowInternet,
                                    AllowPrint = FoodInfo.AllowPrint,
                                    ForeAuthorize = FoodInfo.ForeAuthorize,
                                    ClientPrice = FoodInfo.ClientPrice,
                                    MemberPrice = FoodInfo.MemberPrice,
                                    AllowCoin = FoodInfo.AllowCoin,
                                    Coins = FoodInfo.Coins,
                                    AllowPoint = FoodInfo.AllowPoint,
                                    Points = FoodInfo.Points,
                                    AllowLottery = FoodInfo.AllowLottery,
                                    Lottery = FoodInfo.Lottery,
                                    FoodDetials = FoodDetials,
                                    FoodLevels = FoodLevels
                                }
                            };

                            return ResponseModelFactory.CreateSuccessModel(isSignKeyReturn, result);
                        }
                    #endregion                    

                }

                return ResponseModelFactory.CreateFailModel(isSignKeyReturn, "不识别的套餐类型");
            }
            catch (Exception e)
            {
                return ResponseModelFactory.CreateReturnModel(isSignKeyReturn, Return_Code.F, e.Message);
            }
        }        

        [ApiMethodAttribute(SignKeyEnum = SignKeyEnum.XCCloudUserCacheToken, SysIdAndVersionNo = false)]
        public object SaveFoodMixedInfo(Dictionary<string, object> dicParas)
        {
            try
            {
                XCCloudUserTokenModel userTokenKeyModel = (XCCloudUserTokenModel)dicParas[Constant.XCCloudUserTokenModel];
                string merchId = userTokenKeyModel.DataModel.MerchID;

                string errMsg = string.Empty;
                string foodId = dicParas.ContainsKey("foodId") ? (dicParas["foodId"] + "") : string.Empty;
                string foodName = dicParas.ContainsKey("foodName") ? (dicParas["foodName"] + "") : string.Empty;
                string startTime = dicParas.ContainsKey("startTime") ? (dicParas["startTime"] + "") : string.Empty;
                string rechargeType = dicParas.ContainsKey("rechargeType") ? (dicParas["rechargeType"] + "") : string.Empty;
                string endTime = dicParas.ContainsKey("endTime") ? (dicParas["endTime"] + "") : string.Empty;
                string foodState = dicParas.ContainsKey("foodState") ? (dicParas["foodState"] + "") : string.Empty;                
                string allowInternet = dicParas.ContainsKey("allowInternet") ? (dicParas["allowInternet"] + "") : string.Empty;
                string allowPrint = dicParas.ContainsKey("allowPrint") ? (dicParas["allowPrint"] + "") : string.Empty;
                string foreAuthorize = dicParas.ContainsKey("foreAuthorize") ? (dicParas["foreAuthorize"] + "") : string.Empty;
                string clientPrice = dicParas.ContainsKey("clientPrice") ? (dicParas["clientPrice"] + "") : string.Empty;
                string memberPrice = dicParas.ContainsKey("memberPrice") ? (dicParas["memberPrice"] + "") : string.Empty;
                string allowCoin = dicParas.ContainsKey("allowCoin") ? (dicParas["allowCoin"] + "") : string.Empty;
                string coins = dicParas.ContainsKey("coins") ? (dicParas["coins"] + "") : string.Empty;
                string allowPoint = dicParas.ContainsKey("allowPoint") ? (dicParas["allowPoint"] + "") : string.Empty;
                string points = dicParas.ContainsKey("points") ? (dicParas["points"] + "") : string.Empty;
                string allowLottery = dicParas.ContainsKey("allowLottery") ? (dicParas["allowLottery"] + "") : string.Empty;
                string lottery = dicParas.ContainsKey("lottery") ? (dicParas["lottery"] + "") : string.Empty;
                string imageUrl = dicParas.ContainsKey("imageUrl") ? (dicParas["imageUrl"] + "") : string.Empty;
                object[] foodDetials = dicParas.ContainsKey("foodDetials") ? (object[])dicParas["foodDetials"] : null;
                object[] foodLevels = dicParas.ContainsKey("foodLevels") ? (object[])dicParas["foodLevels"] : null;
                int iFoodId = 0;
                int.TryParse(foodId, out iFoodId);

                #region 验证参数

                if (string.IsNullOrEmpty(foodName))
                {
                    errMsg = "套餐名称不能为空";
                    return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                }

                if (string.IsNullOrEmpty(startTime))
                {
                    errMsg = "有效期开始时间不能为空";
                    return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                }

                if (string.IsNullOrEmpty(endTime))
                {
                    errMsg = "有效期结束时间不能为空";
                    return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                }

                if (Convert.ToDateTime(startTime) > Convert.ToDateTime(endTime))
                {
                    errMsg = "开始时间不能晚于结束时间";
                    return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                }                

                if (!string.IsNullOrEmpty(coins) && Convert.ToInt32(coins) < 0)
                {
                    errMsg = "允许支付币数不能为负数";
                    return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                }

                if (!string.IsNullOrEmpty(points) && Convert.ToInt32(points) < 0)
                {
                    errMsg = "允许支付积分不能为负数";
                    return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                }

                if (!string.IsNullOrEmpty(lottery) && Convert.ToInt32(lottery) < 0)
                {
                    errMsg = "允许支付彩票不能为负数";
                    return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                }

                if (string.IsNullOrEmpty(clientPrice))
                {
                    errMsg = "散客售价不能为空";
                    return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                }

                if (string.IsNullOrEmpty(memberPrice))
                {
                    errMsg = "会员售价不能为空";
                    return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                }

                if (Convert.ToDecimal(clientPrice) < 0)
                {
                    errMsg = "散客售价不能为负数";
                    return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                }

                if (Convert.ToDecimal(memberPrice) < 0)
                {
                    errMsg = "会员售价不能为负数";
                    return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                }

                #endregion

                //开启EF事务
                using (TransactionScope ts = new TransactionScope())
                {
                    try
                    {
                        IData_FoodInfoService data_FoodInfoService = BLLContainer.Resolve<IData_FoodInfoService>();
                        var data_FoodInfo = new Data_FoodInfo();
                        data_FoodInfo.FoodID = iFoodId;
                        data_FoodInfo.FoodName = foodName;
                        data_FoodInfo.FoodType = (int)FoodType.Mixed;
                        data_FoodInfo.FoodState = !string.IsNullOrEmpty(foodState) ? Convert.ToInt32(foodState) : (int?)null;
                        data_FoodInfo.RechargeType = !string.IsNullOrEmpty(rechargeType) ? Convert.ToInt32(rechargeType) : (int?)null;
                        data_FoodInfo.StartTime = Convert.ToDateTime(startTime);
                        data_FoodInfo.EndTime = Convert.ToDateTime(endTime);
                        data_FoodInfo.AllowPrint = !string.IsNullOrEmpty(allowPrint) ? Convert.ToInt32(allowPrint) : (int?)null;
                        data_FoodInfo.AllowInternet = !string.IsNullOrEmpty(allowInternet) ? Convert.ToInt32(allowInternet) : (int?)null;
                        data_FoodInfo.ForeAuthorize = !string.IsNullOrEmpty(foreAuthorize) ? Convert.ToInt32(foreAuthorize) : (int?)null;
                        data_FoodInfo.MemberPrice = Convert.ToDecimal(memberPrice);
                        data_FoodInfo.ClientPrice = Convert.ToDecimal(clientPrice);
                        data_FoodInfo.AllowCoin = !string.IsNullOrEmpty(allowCoin) ? Convert.ToInt32(allowCoin) : (int?)null;
                        data_FoodInfo.Coins = !string.IsNullOrEmpty(coins) ? Convert.ToInt32(coins) : (int?)null;
                        data_FoodInfo.AllowPoint = !string.IsNullOrEmpty(allowPoint) ? Convert.ToInt32(allowPoint) : (int?)null;
                        data_FoodInfo.Points = !string.IsNullOrEmpty(points) ? Convert.ToInt32(points) : (int?)null;
                        data_FoodInfo.AllowLottery = !string.IsNullOrEmpty(allowLottery) ? Convert.ToInt32(allowLottery) : (int?)null;
                        data_FoodInfo.Lottery = !string.IsNullOrEmpty(lottery) ? Convert.ToInt32(lottery) : (int?)null;
                        data_FoodInfo.MerchID = merchId;
                        data_FoodInfo.ImageURL = imageUrl;
                        if (!data_FoodInfoService.Any(a => a.FoodID == iFoodId))
                        {
                            //新增
                            if (!data_FoodInfoService.Add(data_FoodInfo))
                            {
                                errMsg = "添加套餐商品信息失败";
                                return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                            }
                        }
                        else
                        {
                            //修改
                            if (!data_FoodInfoService.Update(data_FoodInfo))
                            {
                                errMsg = "修改套餐商品信息失败";
                                return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                            }
                        }

                        iFoodId = data_FoodInfo.FoodID;

                        if (foodDetials != null && foodDetials.Count() >= 0)
                        {
                            //先删除已有数据，后添加
                            var dbContext = DbContextFactory.CreateByModelNamespace(typeof(Data_Food_Detial).Namespace);
                            var data_Food_Detial = dbContext.Set<Data_Food_Detial>().Where(p => p.FoodID == iFoodId).ToList();
                            foreach (var model in data_Food_Detial)
                            {
                                dbContext.Entry(model).State = EntityState.Deleted;
                            }

                            foreach (IDictionary<string, object> el in foodDetials)
                            {
                                if (el != null)
                                {
                                    var dicPara = new Dictionary<string, object>(el, StringComparer.OrdinalIgnoreCase);
                                    string foodDetailType = dicPara.ContainsKey("foodDetailType") ? (dicPara["foodDetailType"] + "") : string.Empty;
                                    string containId = dicPara.ContainsKey("containId") ? (dicPara["containId"] + "") : string.Empty;
                                    string weightValue = dicPara.ContainsKey("weightValue") ? (dicPara["weightValue"] + "") : string.Empty;
                                    string containCount = dicPara.ContainsKey("containCount") ? (dicPara["containCount"] + "") : string.Empty;
                                    string days = dicPara.ContainsKey("days") ? (dicPara["days"] + "") : string.Empty;
                                    int iFoodDetailType = 0;
                                    int.TryParse(foodDetailType, out iFoodDetailType);

                                    if (string.IsNullOrEmpty(foodDetailType))
                                    {
                                        errMsg = "套餐类别不能为空";
                                        return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                                    }
                                    if (!Utils.isNumber(foodDetailType))
                                    {
                                        errMsg = "套餐类别格式不正确";
                                        return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                                    }
                                    if (iFoodDetailType != (int)FoodDetailType.Coin && string.IsNullOrEmpty(containId))
                                    {
                                        errMsg = "内容ID不能为空";
                                        return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                                    }
                                    if (string.IsNullOrEmpty(containCount))
                                    {
                                        errMsg = "内容数量不能为空";
                                        return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                                    }
                                    if (!Utils.isNumber(containCount))
                                    {
                                        errMsg = "内容数量格式不正确";
                                        return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                                    }
                                    if (Convert.ToInt32(containCount) < 0)
                                    {
                                        errMsg = "内容数量不能为负数";
                                        return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                                    }
                                    if (string.IsNullOrEmpty(weightValue))
                                    {
                                        errMsg = "权重价值不能为空";
                                        return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                                    }
                                    if (!Utils.IsDecimal(weightValue))
                                    {
                                        errMsg = "权重价值格式不正确";
                                        return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                                    }
                                    if (Convert.ToDecimal(weightValue) < 0)
                                    {
                                        errMsg = "权重价值不能为负数";
                                        return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                                    }
                                    if (string.IsNullOrEmpty(days))
                                    {
                                        errMsg = "有效天数不能为空";
                                        return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                                    }
                                    if (!Utils.isNumber(days))
                                    {
                                        errMsg = "有效天数格式不正确";
                                        return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                                    }
                                    if (Convert.ToInt32(days) < 0)
                                    {
                                        errMsg = "有效天数不能为负数";
                                        return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                                    }

                                    var data_Food_DetialModel = new Data_Food_Detial();
                                    data_Food_DetialModel.FoodID = iFoodId;
                                    data_Food_DetialModel.Status = 1;
                                    data_Food_DetialModel.FoodType = iFoodDetailType;
                                    data_Food_DetialModel.ContainCount = Convert.ToInt32(containCount);
                                    data_Food_DetialModel.ContainID = !string.IsNullOrEmpty(containId) ? Convert.ToInt32(containId) : (int?)null;
                                    data_Food_DetialModel.WeightType = (int)WeightType.Money;
                                    data_Food_DetialModel.WeightValue = Convert.ToDecimal(weightValue);
                                    data_Food_DetialModel.Days = Convert.ToInt32(days);
                                    dbContext.Entry(data_Food_DetialModel).State = EntityState.Added;
                                }
                                else
                                {
                                    errMsg = "提交数据包含空对象";
                                    return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                                }
                            }

                            if (dbContext.SaveChanges() < 0)
                            {
                                errMsg = "保存套餐内容信息失败";
                                return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                            }
                        }

                        //保存套餐优惠时段信息
                        if (!saveFoodLevel(iFoodId, data_FoodInfo.StartTime, data_FoodInfo.EndTime, foodLevels, out errMsg))
                        {
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

        [ApiMethodAttribute(SignKeyEnum = SignKeyEnum.XCCloudUserCacheToken, SysIdAndVersionNo = false)]
        public object GetProjectInfo(Dictionary<string, object> dicParas)
        {
            try
            {
                string errMsg = string.Empty;
                string id = dicParas.ContainsKey("id") ? (dicParas["id"] + "") : string.Empty;
                if (string.IsNullOrEmpty(id))
                {
                    errMsg = "项目ID不能为空";
                    return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                }

                int iId = Convert.ToInt32(id);
                IData_ProjectInfoService data_ProjectInfoService = BLLContainer.Resolve<IData_ProjectInfoService>();
                var ProjectInfo = data_ProjectInfoService.GetModels(p => p.ID == iId).FirstOrDefault();
                if (ProjectInfo == null)
                {
                    errMsg = "该项目不存在";
                    return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                }

                IDict_SystemService dict_SystemService = BLLContainer.Resolve<IDict_SystemService>();
                int FeeTypeId = dict_SystemService.GetModels(p => p.DictKey.Equals("计费方式")).FirstOrDefault().ID;
                var FeeType = dict_SystemService.GetModels(p => p.PID == FeeTypeId);
                
                var result = new
                {
                    ID = ProjectInfo.ID,
                    ProjectName = ProjectInfo.ProjectName,
                    FeeTypeStr = FeeType.Any(s => s.DictValue.Equals(ProjectInfo.FeeType + "")) ? FeeType.Single(s => s.DictValue.Equals(ProjectInfo.FeeType + "")).DictKey : string.Empty,
                    FeeCycle = ProjectInfo.FeeCycle,
                    FeeDeposit = ProjectInfo.FeeDeposit,
                    SignOutEN = ProjectInfo.SignOutEN,
                    WhenLock = ProjectInfo.WhenLock,                    
                };

                return ResponseModelFactory.CreateSuccessModel(isSignKeyReturn, result);
            }
            catch (Exception e)
            {
                return ResponseModelFactory.CreateReturnModel(isSignKeyReturn, Return_Code.F, e.Message);
            }
        }

        [ApiMethodAttribute(SignKeyEnum = SignKeyEnum.XCCloudUserCacheToken, SysIdAndVersionNo = false)]
        public object GetGoodsInfoList(Dictionary<string, object> dicParas)
        {
            try
            {
                XCCloudUserTokenModel userTokenKeyModel = (XCCloudUserTokenModel)dicParas[Constant.XCCloudUserTokenModel];
                string merchId = userTokenKeyModel.DataModel.MerchID;
                string errMsg = string.Empty;                
                string goodNameOrBarcode = dicParas.ContainsKey("goodNameOrBarcode") ? (dicParas["goodNameOrBarcode"] + "") : string.Empty;

                IDict_SystemService dict_SystemService = BLLContainer.Resolve<IDict_SystemService>(resolveNew: true);
                int GoodTypeId = dict_SystemService.GetModels(p => p.DictKey.Equals("商品类别")).FirstOrDefault().ID;

                IBase_GoodsInfoService base_GoodsInfoService = BLLContainer.Resolve<IBase_GoodsInfoService>(resolveNew: true);
                var query = base_GoodsInfoService.GetModels(p => p.MerchID.Equals(merchId, StringComparison.OrdinalIgnoreCase) && p.GoodType == (int)GoodType.Good && p.Status == 1);
                if (!string.IsNullOrEmpty(goodNameOrBarcode))
                {
                    query = query.Where(w => w.GoodName.Contains(goodNameOrBarcode) || w.Barcode.Contains(goodNameOrBarcode));
                }

                var linq = from a in query
                           join b in dict_SystemService.GetModels(p => p.PID == GoodTypeId) on (a.GoodType + "") equals b.DictValue into b1
                           from b in b1.DefaultIfEmpty()
                           select new
                           {
                               ID = a.ID,
                               Barcode = a.Barcode,
                               GoodName = a.GoodName,
                               GoodTypeStr = b != null ? b.DictKey : string.Empty
                           };

                return ResponseModelFactory.CreateSuccessModel(isSignKeyReturn, linq.ToList());
            }
            catch (Exception e)
            {
                return ResponseModelFactory.CreateReturnModel(isSignKeyReturn, Return_Code.F, e.Message);
            }
        }

        [ApiMethodAttribute(SignKeyEnum = SignKeyEnum.XCCloudUserCacheToken, SysIdAndVersionNo = false)]
        public object UploadFoodPhoto(Dictionary<string, object> dicParas)
        {
            try
            {
                string errMsg = string.Empty;

                #region 验证参数

                var file = HttpContext.Current.Request.Files[0];
                if (file == null)
                {
                    errMsg = "未找到图片";
                    return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                }

                if (file.ContentLength > int.Parse(System.Configuration.ConfigurationManager.AppSettings["MaxImageSize"].ToString()))
                {
                    errMsg = "超过图片的最大限制为1M";
                    return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                }

                #endregion

                string picturePath = System.Configuration.ConfigurationManager.AppSettings["UploadImageUrl"].ToString() + "/XCCloud/Promotion/Food/";
                string path = System.Web.HttpContext.Current.Server.MapPath(picturePath);
                //如果不存在就创建file文件夹
                if (Directory.Exists(path) == false)
                {
                    Directory.CreateDirectory(path);
                }

                string fileName = Path.GetFileNameWithoutExtension(file.FileName) + Utils.ConvertDateTimeToLong(DateTime.Now) + Path.GetExtension(file.FileName);

                if (File.Exists(path + fileName))
                {
                    errMsg = "图片名称已存在，请重命名后上传";
                    return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                }

                file.SaveAs(path + fileName);

                Dictionary<string, string> dicStoreInfo = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                dicStoreInfo.Add("ImageURL", picturePath + fileName);
                return ResponseModelFactory.CreateSuccessModel(isSignKeyReturn, dicStoreInfo);
            }
            catch (Exception e)
            {
                return ResponseModelFactory.CreateReturnModel(isSignKeyReturn, Return_Code.F, e.Message);
            }
        }
    }
}