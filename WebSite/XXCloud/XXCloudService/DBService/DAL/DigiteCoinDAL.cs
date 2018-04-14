using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using XCCloudService.DBService.Model;
using XCCloudService.DBService.SQLDAL;
using System.Data;
using XCCloudService.Common;
using System.Text;
using XCCloudService.Base;


namespace XCCloudService.DBService.DAL
{
    public class DigiteCoinDAL
    {
        /// <summary>
        /// 数字币销售
        /// </summary>
        /// <param name="listDetail">数字币销售明细</param>
        /// <param name="listDetailSum">数字币销售明细汇总</param>
        public static DigiteCoinSaleDataModel DigiteCoinSale(string businessDate)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(" declare @BusinessDate varchar(10) ");
            sb.Append(string.Format(" set @BusinessDate = '{0}' ", businessDate));
            sb.Append(" select a.RealTime as SaleTime,FoodName,CountNum,TotalMoney,Note,a.WorkStation from dbo.flw_digite_coin a inner join dbo.flw_digite_coin_detial b on a.ID = b.ID left join t_foods c on a.FoodID = c.FoodID where 1 = 1 and DATEDIFF(dy,a.RealTime,@BusinessDate) = 0 ");
            sb.Append(" select top 5 * from ");
            sb.Append(" ( ");
            sb.Append(" select FoodName, sum(TotalMoney) as TotalMoney from dbo.flw_digite_coin a inner join dbo.flw_digite_coin_detial b on a.ID = b.ID left join t_foods c on a.FoodID = c.FoodID where 1 = 1 and DATEDIFF(dy,a.RealTime,@BusinessDate) = 0 Group by FoodName ");
            sb.Append(" ) a ");
            DataAccess da = new DataAccess(DataAccessDB.XCGameDB);
            DataSet ds = da.ExecuteQuery(sb.ToString());
            List<DigiteCoinSaleDetailModel> listDetail = Utils.GetModelList<DigiteCoinSaleDetailModel>(ds.Tables[0]);
            List<DigiteCoinSaleDetailSumModel> listDetailSum = Utils.GetModelList<DigiteCoinSaleDetailSumModel>(ds.Tables[1]);
            return new DigiteCoinSaleDataModel(listDetail, listDetailSum);
        }

        /// <summary>
        /// 数字币销毁
        /// </summary>
        public static DigiteCoinDestroyModel DigiteCoinDestory(string businessDate, string ICCardID, int pageIndex, int pageSize, out int recordCount)
        { 
            StringBuilder sb = new StringBuilder();
            sb.Append(" declare @BusinessDate varchar(10) ");
            sb.Append(" declare @ICCardID varchar(20) ");
            sb.Append(string.Format(" set @ICCardID = '{0}' ",ICCardID));
            sb.Append(" select DestroyTime,ICCardID,UserName,Note from ");
            sb.Append(" ( ");
            sb.Append(" select Row_Number() over (order by DestroyTime desc) as RowId,DestroyTime,a.ICCardID,ISNULL(b.realName,b.UserID) as UserName,Note from Data_DigitCoinDestroy a left join Base_UserInfo b on a.UserID = b.UserID where DATEDIFF(DY,DestroyTime,@BusinessDate) = 0 and a.ICCardID = @ICCardID ");
            sb.Append(" ) a ");
            sb.Append(string.Format(" where RowId >= {0} and RowId <= {1}", pageIndex * pageSize, (pageIndex + 1) * pageSize));
            sb.Append(" select count(*) from Data_DigitCoinDestroy a left join Base_UserInfo b on a.UserID = b.UserID where DATEDIFF(DY,DestroyTime,@BusinessDate) = 0 and a.ICCardID = @ICCardID ");
            DataAccess da = new DataAccess(DataAccessDB.XCCloudDB);
            DataSet ds = da.ExecuteQuery(sb.ToString());
            List<DigiteCoinDestroyDetialModel> detail = Utils.GetModelList<DigiteCoinDestroyDetialModel>(ds.Tables[0]);
            recordCount = Convert.ToInt32(ds.Tables[1].Rows[0][0]);
            Page detailPage = new Page(pageIndex, pageSize, recordCount);
            return new DigiteCoinDestroyModel(detail,detailPage);
        }
    }
}