using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using XXCloudService.DBService.Model;
using XXCloudService.DBService.DAL;

namespace XXCloudService.DBService.BLL
{
    /// <summary>
    /// 商品资料
    /// </summary>
    public class GoodsBLL
    {
        /// <summary>
        /// 商品资料
        /// </summary>
        /// <returns></returns>
        public static GoodsInfoModel getGoodsInfo(int pageIndex, int pageSize, out int recordCount)
        {
            return GoodsDAL.getGoodsInfo(pageIndex,pageSize,out recordCount);
        }
    }
}