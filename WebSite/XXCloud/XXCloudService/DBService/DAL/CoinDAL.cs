using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using XCCloudService.DBService.SQLDAL;
using System.Data;
using XCCloudService.DBService.Model;
using XCCloudService.Common;
using XCCloudService.Base;

namespace XCCloudService.DBService.DAL
{
    public class CoinDAL
    {
        /// <summary>
        /// 代币入库
        /// </summary>
        /// <param name="businessDate">营业日期</param>
        /// <param name="pageIndex">页索引</param>
        /// <param name="pageSize">每页记录数</param>
        /// <param name="recordCount">总记录数</param>
        /// <returns>代币入库模式</returns>
        public static CoinStoragePageModel CoinStorage(string businessDate, int pageIndex, int pageSize, out int recordCount)
        {
            recordCount = 0;
            StringBuilder sb = new StringBuilder();
            sb.Append(" where 1 = 1 ");
            if (!string.IsNullOrEmpty(businessDate))
            {
                sb.Append(" and DATEDIFF(DY,DestroyTime,@BusinessDate) = 0 ");
            }
            string where = sb.ToString();

            sb.Clear();
            sb.Append(" declare @BusinessDate varchar(10) ");
            sb.Append(string.Format(" set @BusinessDate = '{0}' ", businessDate));
            sb.Append(" select StorageTime,StoreName,UserName,StorageCount,Note from ");
            sb.Append(" ( ");
            sb.Append(" select ROW_NUMBER() over(order by DestroyTime desc) as RowId, ");
            sb.Append(" DestroyTime as StorageTime,isnull(StoreName,a.StoreID) as StoreName, ");
            sb.Append(" ISNULL(a.UserID,c.RealName) as UserName,StorageCount,Note ");
            sb.Append(" from dbo.Data_CoinStorage a left join Base_StoreInfo b on a.StoreID = b.StoreID "); 
            sb.Append(" left join Base_UserInfo c on a.UserID = c.UserID ");
            sb.Append(where);
            sb.Append(" ) a ");
            sb.Append(string.Format(" where RowId >= {0} and RowId <= {1}", pageIndex * pageSize, (pageIndex + 1) * pageSize));

            sb.Append(" select count(0) from dbo.Data_CoinStorage a left join Base_StoreInfo b on a.StoreID = b.StoreID ");
            sb.Append(" left join Base_UserInfo c on a.UserID = c.UserID ");
            sb.Append(where);

            DataAccess ac = new DataAccess(DataAccessDB.XCCloudDB);
            DataSet ds = ac.ExecuteQuery(sb.ToString());
            List<CoinStorageModel> coinStorageModel = Utils.GetModelList<CoinStorageModel>(ds.Tables[0]);
            recordCount = Convert.ToInt32(ds.Tables[1].Rows[0][0]);
            Page page = new Page(pageIndex, pageSize, recordCount);
            return new CoinStoragePageModel(coinStorageModel, page);
        }

        /// <summary>
        /// 代币销毁
        /// </summary>
        /// <param name="destroyDate">销毁日期</param>
        /// <param name="pageIndex">页索引</param>
        /// <param name="pageSize">每页记录数</param>
        /// <param name="recordCount">总记录数</param>
        /// <returns>代币销毁列表</returns>
        public static CoinDestroyPageModel CoinDestroy(string destroyDate, int pageIndex, int pageSize, out int recordCount)
        {
            recordCount = 0;
            StringBuilder sb = new StringBuilder();
            sb.Append(" declare @DestroyDate varchar(10) ");
            sb.Append(" select RowId,DestroyTime,StoreName,UserName,DestroyCount,Note from ");
            sb.Append(" ( ");
            sb.Append(" select ROW_NUMBER() over(order by DestroyTime desc) as RowId,DestroyTime as DestroyTime,isnull(StoreName,a.StoreID) as StoreName,ISNULL(a.UserID,c.RealName) as UserName,StorageCount as DestroyCount,Note from Data_CoinDestory a left join Base_StoreInfo b on a.StoreID = b.StoreID left join Base_UserInfo c on a.UserID = c.UserID ");
            if (!string.IsNullOrEmpty(destroyDate))
            {
                sb.Append(string.Format(" and DATEDIFF(DY,DestroyTime,'{0}') = 0 ", destroyDate));
            }
            sb.Append(" ) a ");
            sb.Append(string.Format(" where RowId >= {0} and RowId <= {1}", pageIndex * pageSize, (pageIndex + 1) * pageSize));
            DataAccess ac = new DataAccess(DataAccessDB.XCCloudDB);
            DataSet ds = ac.ExecuteQuery(sb.ToString());
            List<CoinDestroyModel> coinDestroyDetail = Utils.GetModelList<CoinDestroyModel>(ds.Tables[0]);
            Page page = new Page(pageIndex, pageSize, recordCount);
            return new CoinDestroyPageModel(coinDestroyDetail, page);
        }

        /// <summary>
        /// 代币销售
        /// </summary>
        /// <param name="salesDate">销售日期</param>
        /// <param name="pageIndex">页索引</param>
        /// <param name="pageSize">分页大小</param>
        /// <param name="recordCount">总记录数</param>
        /// <returns></returns>
        public static CoinSalesModel CoinSales(string salesDate, int coinCount, int pageIndex, int pageSize, out int recordCount)
        {
            int coinTotalCount = 0;
            StringBuilder sb = new StringBuilder();
            //查询分页
            sb.Append(" select * from  ");
            sb.Append(" ( ");
            sb.Append(@" select ROW_NUMBER() over(order by RealTime desc) as RowId,convert(varchar,RealTime,120) as RealTime,isnull(cast(ICCardID as varchar),'') as ICCardID,Coins,WorkStation,
                        (case WorkType when 0 then '售币机加币' when 1 then '售币机清币' when 2 then '手工实物币送币' when 3 then '手工实物币提币' 
                        when 4 then '手工存币' when 5 then '电子币送币' when 6 then '电子币提币' when 7 then '售币机实物币提币' when 8 then '售币机实物币送币'  end) as WorkTypeName,
                        '' as Note from Flw_Coin_Sale ");
            sb.Append(string.Format(" where DATEDIFF(dy,realtime,'{0}') = 0 and Coins > {1} ", salesDate, coinCount));
            sb.Append(" ) a ");
            sb.Append(string.Format(" where RowId >= {0} and RowId <= {1} ", pageIndex * pageSize, (pageIndex + 1) * pageSize));

            //设备分类汇总查询
            sb.Append(" select WorkStation,SUM(Coins) as Coins from Flw_Coin_Sale");
            sb.Append(string.Format(" where DATEDIFF(dy,realtime,'{0}') = 0 and Coins > {1} ", salesDate, coinCount));
            sb.Append(" group by WorkStation ");

            //汇总
            sb.Append(" select Isnull(SUM(Coins),0) as Coins,COUNT(0) as RecordCount from Flw_Coin_Sale ");
            sb.Append(string.Format(" where DATEDIFF(dy,realtime,'{0}') = 0 and Coins > {1} ", salesDate, coinCount));

            DataAccess ac = new DataAccess(DataAccessDB.XCGameDB);
            string sql = sb.ToString();
            DataSet ds = ac.ExecuteQuery(sb.ToString());

            List<CoinSalesDetailModel> salesDetail = Utils.GetModelList<CoinSalesDetailModel>(ds.Tables[0]);
            List<CoinSalesDeviceSummaryModel> salesDeviceSummaryDetail = Utils.GetModelList<CoinSalesDeviceSummaryModel>(ds.Tables[1]);
            recordCount = Convert.ToInt32(ds.Tables[2].Rows[0]["RecordCount"]);
            coinTotalCount = Convert.ToInt32(ds.Tables[2].Rows[0]["Coins"]);
            Page salesDetailPage = new Page(pageIndex, pageSize, recordCount);
            return new CoinSalesModel(coinTotalCount, salesDetail, salesDeviceSummaryDetail, salesDetailPage);
        }
    }
}