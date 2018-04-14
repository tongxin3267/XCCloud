using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XCCloudService.BLL.CommonBLL;
using XCCloudService.CacheService;
using XCCloudService.Common;
using XCCloudService.Model.CustomModel.XCGameManager;

namespace XCCloudService.Business.XCGameMana
{
    public class DataOrderQueryBusiness
    {
        public static string SetDataOrderQueryBusiness(string mobile, out string name)
        {

            String sql = String.Format("select row_number() over(order by CreateTime desc) as [No], OrderID,StoreID,Price,Fee,OrderType,PayStatus,CONVERT(varchar(100), PayTime, 23)as PayTime,CreateTime,Descript,Mobile,BuyType,StoreName,Coins from Data_Order where Mobile='{0}' and PayStatus='1' order by CreateTime desc", mobile);
            System.Data.DataSet ds = XCGameManabll.ExecuteQuerySentence(sql, null);
            DataTable dt = ds.Tables[0];
            dt.Columns.Add("CreateTimes");
            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    DateTime time1 = Convert.ToDateTime(dt.Rows[i]["CreateTime"]);
                    string CreateTimes = time1.ToString("yyyy-MM-dd H:mm");
                    dt.Rows[i]["CreateTimes"] = CreateTimes;
                }
                var list = Utils.GetModelList<DataOrderModel>(ds.Tables[0]).ToList();
                int time = CacheExpires.CommonPageQueryDataCacheTime;
                DataOrderCaChe.Add(mobile, list, time);
                name = "1";
                return name;
            }
            name = "";
            return name;
        }
    }

}
