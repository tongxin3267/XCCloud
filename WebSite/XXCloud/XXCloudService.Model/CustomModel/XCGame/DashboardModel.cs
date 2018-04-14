using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace XCCloudService.Model.CustomModel.XCGame
{
    /// <summary>
    /// 今日营收总览
    /// </summary>
    [DataContract] 
    public class MOMModel
    {
        /// <summary>
        /// 搜索数据的名称（环比）
        /// </summary>
        [DataMember(Name="search",Order=1)]
        public string Search { set; get; }

        /// <summary>
        /// 当日营收金额
        /// </summary>
        [DataMember(Name="total",Order=2)]
        public decimal Total { set; get; }

        /// <summary>
        /// 昨日营收金额
        /// </summary>
        [DataMember(Name="lasttotal",Order=3)]
        public decimal LastTotal { set; get; }
    }

    /// <summary>
    /// 今日营收
    /// </summary>
    [DataContract] 
    public class TodayRevenueModel
    {
        /// <summary>
        /// 套餐销售
        /// </summary>
        [DataMember(Name="foodtotal",Order=1)]
        public decimal FoodTotal { set; get; }

        /// <summary>
        /// 商品销售
        /// </summary>
        [DataMember(Name="goodtotal",Order=2)]
        public decimal GoodTotal { set; get; }

        /// <summary>
        /// 售数字币
        /// </summary>
        [DataMember(Name="digitetotal",Order=3)]
        public decimal DigiteTotal { set; get; }
    }

    /// <summary>
    /// 游戏机今日营收
    /// </summary>
    [DataContract]
    public class TodayGameRevenueModel
    {
        /// <summary>
        /// 游戏机ID
        /// </summary>
        public string GameID { set; get; }

        /// <summary>
        /// 游戏机名称
        /// </summary>
        [DataMember(Name = "gamename", Order = 1)]
        public string GameName { set; get; }

        /// <summary>
        /// 游戏币数
        /// </summary>
        [DataMember(Name = "totalcount", Order = 2)]
        public int TotalCount { set; get; }
    }

    /// <summary>
    /// 今日吧台营收
    /// </summary>
    [DataContract]
    public class TodayBarCounterRevenueModel
    {
        /// <summary>
        /// 吧台或机器名称
        /// </summary>
        [DataMember(Name = "name", Order = 1)]
        public string WorkStation { set; get; }

        /// <summary>
        /// 营收金额
        /// </summary>
        [DataMember(Name = "totalmoney", Order = 2)]
        public decimal TotalMoney { set; get; }
    }

    /// <summary>
    /// 客流分析(每个时段的会员卡只计入一次)
    /// </summary>
    [DataContract]
    public class PassengerFlowModel
    {
        /// <summary>
        /// 时段
        /// </summary>
        [DataMember(Name = "hour", Order = 1)]
        public string HourField { set; get; }

        /// <summary>
        /// 乘客数量
        /// </summary>
        [DataMember(Name = "count", Order = 2)]
        public int PassengerCount { set; get; }
    }
}