using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using XCCloudService.DBService.Model;
using XCCloudService.DBService.DAL;

namespace XCCloudService.DBService.BLL
{
    public class DashboardBLL
    {
        /// <summary>
        /// 今日营收总览,获取上一个营业日期的现金数与当前营业日期的现金数进行环比查询
        /// </summary>
        /// <returns></returns>
        public static MOMModel GetMOM()
        {
            return DashboardDAL.GetMOM();
        }

        /// <summary>
        /// 今日营收分析
        /// </summary>
        /// <returns></returns>
        public static TodayRevenueModel GetTodayRevenue()
        {
            return DashboardDAL.GetTodayRevenue();
        }

        /// <summary>
        /// 今日游戏机排行榜
        /// </summary>
        /// <returns></returns>
        public static List<TodayGameRevenueModel> GetTodyGameRevenue()
        {
            return DashboardDAL.GetTodayGameRevenue();
        }

        /// <summary>
        /// 今日吧台营收
        /// </summary>
        /// <returns></returns>
        public static List<TodayBarCounterRevenueModel> GetTodayBarCounterRevenue()
        {
            return DashboardDAL.GetTodayBarCounterRevenue();
        }

        /// <summary>
        /// 客流分析
        /// </summary>
        /// <returns></returns>
        public static List<PassengerFlowModel> GetTodayPassengerFlow()
        {
            return DashboardDAL.GetTodayPassengerFlow();
        }
    }
}