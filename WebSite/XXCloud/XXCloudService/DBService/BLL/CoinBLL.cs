using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using XCCloudService.DBService.Model;
using XCCloudService.DBService.DAL;

namespace XCCloudService.DBService.BLL
{
    public class CoinBLL
    {
        /// <summary>
        /// 代币入库
        /// </summary>
        /// <param name="businessDate">营业日期</param>
        /// <param name="pageIndex">页索引</param>
        /// <param name="pageSize">每页记录数</param>
        /// <param name="recordCount">总记录数</param>
        /// <returns>代币入库模式</returns>
        public static CoinStoragePageModel CoinStorage(string businessDate,int pageIndex,int pageSize,out int recordCount)
        {
            return CoinDAL.CoinStorage(businessDate, pageIndex, pageSize, out recordCount);
        }

        /// <summary>
        /// 代币销毁
        /// </summary>
        /// <param name="destroyDate">销毁日期</param>
        /// <param name="pageIndex">页索引</param>
        /// <param name="pageSize">每页记录数</param>
        /// <param name="recordCount">总记录数</param>
        /// <returns>代币销毁模式</returns>
        public static CoinDestroyPageModel CoinDestroy(string destroyDate, int pageIndex, int pageSize, out int recordCount)
        {
            return CoinDAL.CoinDestroy(destroyDate, pageIndex, pageSize, out recordCount);
        }
 
        /// <summary>
        /// 代币销售
        /// </summary>
        /// <param name="salesDate">销售日期</param>
        /// <param name="coinCount">币数（大于次币数的记录）</param>
        /// <param name="pageIndex">页索引</param>
        /// <param name="pageSize">分页大小</param>
        /// <param name="recordCount">总记录数</param>
        public static CoinSalesModel CoinSales(string salesDate, int coinCount, int pageIndex, int pageSize, out int recordCount)
        {
            return CoinDAL.CoinSales(salesDate,coinCount,pageIndex,pageSize,out recordCount);
        }
    }
}