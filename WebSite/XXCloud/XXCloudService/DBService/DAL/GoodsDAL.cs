using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using XXCloudService.DBService.Model;
using System.Text;
using XXCloudService.DBService.SQLDAL;
using System.Data;
using XXCloudService.Common;
using XXCloudService.Base;

namespace XXCloudService.DBService.DAL
{
    /// <summary>
    /// 商品资料
    /// </summary>
    public class GoodsDAL
    {
        /// <summary>
        /// 获取商品信息
        /// </summary>
        /// <returns>商品信息</returns>
        public static GoodsInfoModel getGoodsInfo(int pageIndex, int pageSize, out int recordCount)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(" select Barcode,GoodName,GoodPhoteURL,GoodTypeName,MinValue,MaxValue,Price,Points,StatusName,Note from ");
            sb.Append(" ( ");
            sb.Append(@" select ROW_NUMBER() over (order by GoodName desc) as RowId, Barcode,GoodName,GoodPhoteURL,
                        case GoodType when 0 then '销售商品' when 1 then '兑换礼品' when 2 then '办公用品' 
                        when 3 then '终端设备' when 4 then '系统耗材' when 5 then '代币' 
                        when 6 then '数字币' when 7 then '会员卡' when 8 then '其他'  end as GoodTypeName,
                        MinValue,MaxValue,Price,Points,case [Status] when 0 then '停用' when 1 then '正常' end StatusName,Note 
                        from dbo.Base_GoodsInfo " );
            sb.Append(" ) a ");
            sb.Append(string.Format(" where RowId >= {0} and RowId <= {1}", pageIndex * pageSize, (pageIndex + 1) * pageSize));
            sb.Append(" select COUNT(0) from dbo.Base_GoodsInfo ");

            DataAccess ac = new DataAccess(DataAccessDB.XCCloudDB);
            string sqlStr = sb.ToString();
            DataSet ds = ac.ExecuteQuery(sqlStr);
            List<GoodsInfoDetailModel> goodsDetail = Utils.GetModelList<GoodsInfoDetailModel>(ds.Tables[0]);
            recordCount = Convert.ToInt32(ds.Tables[1].Rows[0][0]);
            Page pages = new Page(pageIndex, pageSize, recordCount);
            GoodsInfoModel goodsInfo = new GoodsInfoModel(goodsDetail, pages);
            return goodsInfo;
        }
    }
}