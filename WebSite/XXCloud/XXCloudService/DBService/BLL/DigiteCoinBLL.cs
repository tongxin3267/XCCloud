using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using XCCloudService.DBService.DAL;
using XCCloudService.DBService.Model;

namespace XCCloudService.DBService.BLL
{
    public class DigiteCoinBLL
    {
        /// <summary>
        /// 数字币销售
        /// </summary>
        /// <param name="listDetail">数字币销售明细</param>
        /// <param name="listDetailSum">数字币销售明细汇总</param>
        public static DigiteCoinSaleDataModel DigiteCoinSale(string businessDate)
        {
            return DigiteCoinDAL.DigiteCoinSale(businessDate);
        }

        public static DigiteCoinDestroyModel DigiteCoinDestory(string businessDate, string ICCardID, int pageIndex, int pageSize, out int recordCount)
        {
            return DigiteCoinDAL.DigiteCoinDestory(businessDate, ICCardID, pageIndex, pageSize, out recordCount);
        }
    }
}