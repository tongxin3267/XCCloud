using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using XCCloudService.Base;
using XCCloudService.DBService.BLL;
using XCCloudService.DBService.Model;
using XCCloudService.Common;

namespace XCCloudService.Api
{
    /// <summary>
    /// dashboard 的摘要说明
    /// </summary>
    public class Dashboard : ApiBase
    {
        /// <summary>
        /// 今日营收总览,获取上一个营业日期的现金数与当前营业日期的现金数进行环比查询
        /// </summary>
        /// <param name="dicParas">请求参数字典</param>
        /// <returns></returns>
        public object mom()
        {
            MOMModel model = DashboardBLL.GetMOM();
            ResponseModel<MOMModel> responseModel = new ResponseModel<MOMModel>();
            responseModel.Result_Data = model;
            return responseModel;
        }

        /// <summary>
        /// 今日营收分析
        /// </summary>
        /// <param name="dicParas">请求参数字典</param>
        /// <returns></returns>
        public object todayRevenue()
        {
            TodayRevenueModel model = DashboardBLL.GetTodayRevenue();
            ResponseModel<TodayRevenueModel> responseModel = new ResponseModel<TodayRevenueModel>();
            responseModel.Result_Data = model;
            return responseModel;
        }


        /// <summary>
        /// 今日游戏机排行榜
        /// </summary>
        /// <param name="dicParas">请求参数字典</param>
        /// <returns></returns>
        public object todayGameRanking()
        {
            List<TodayGameRevenueModel> model = DashboardBLL.GetTodyGameRevenue().OrderByDescending(g => g.TotalCount).ToList<TodayGameRevenueModel>();
            ResponseModel<List<TodayGameRevenueModel>> responseModel = new ResponseModel<List<TodayGameRevenueModel>>();
            responseModel.Result_Data = model;
            return responseModel;
        }

        /// <summary>
        /// 今日客流分析
        /// </summary>
        /// <returns></returns>
        public object todayPassengerFlow()
        {
            List<PassengerFlowModel> listMode = DashboardBLL.GetTodayPassengerFlow();
            string[] hourArr = new string[] { "01", "02", "03", "04", "05", "06", "07", "08", "09", "10", "11", "12", "13", "14", "15", "16", "17", "18", "19", "20", "21", "22", "23", "24" };
            List<PassengerFlowModel> list = new List<PassengerFlowModel>();

            foreach (string hour in hourArr)
            {
                PassengerFlowModel model = new PassengerFlowModel();
                var hourObj = listMode.Where<PassengerFlowModel>(p => p.HourField == Convert.ToInt32(hour).ToString());
                model.HourField = hour;
                if (hourObj != null && hourObj.ToList<PassengerFlowModel>().Count > 0)
                {
                    model.PassengerCount = hourObj.ToList<PassengerFlowModel>()[0].PassengerCount;
                }
                list.Add(model);
            }

            ResponseModel<List<PassengerFlowModel>> responseModel = new ResponseModel<List<PassengerFlowModel>>();
            responseModel.Result_Data = list;
            return responseModel;
        }

        /// <summary>
        /// 今日吧台营收
        /// </summary>
        /// <returns></returns>
        public object todayBarCounterRevenue()
        {
            List<TodayBarCounterRevenueModel> model = DashboardBLL.GetTodayBarCounterRevenue().OrderByDescending(g => g.TotalMoney).ToList<TodayBarCounterRevenueModel>();
            ResponseModel<List<TodayBarCounterRevenueModel>> responseModel = new ResponseModel<List<TodayBarCounterRevenueModel>>();
            responseModel.Result_Data = model;
            return responseModel;
        }
    }
}