using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using XCCloudService.Base;
using XCCloudService.DBService.Model;
using XCCloudService.DBService.BLL;
using XCCloudService.BLL.IBLL.XCGame;
using XCCloudService.BLL.Container;
using XCCloudService.Model;
using XCCloudService.Common;
using XCCloudService.ResponseModels;
using XCCloudService.Model.XCGame;



namespace XCCloudService.Api
{
    /// <summary>
    /// Goods 的摘要说明
    /// </summary>
    public class Goods : ApiBase
    {
        /// <summary>
        /// 获取商品信息
        /// </summary>
        /// <param name="dicParas">参数字典</param>
        /// <returns>商品信息</returns>
        public object goodsInfo(Dictionary<string, object> dicParas)
        {
            string errMsg = string.Empty;
            string destroyDate = string.Empty;
            string pageIndexStr = string.Empty;
            string pageSizeStr = string.Empty;
            int pageIndex = 0;
            int pageSize = 0;

            if (!checkGoodsParams(dicParas, out pageIndex, out pageSize, out errMsg))
            {
                ResponseModel responseModel = new ResponseModel(Return_Code.T, "", Result_Code.F, errMsg);
                return responseModel;
            }
            else
            {
                int recordCount = 0;
                IGoodsService goodsService = BLLContainer.Resolve<IGoodsService>();
                List<t_goods> goods = goodsService.GetModelsByPage<string>(pageSize, pageIndex, false, p => p.UpdateTime, p => p.UserID == 1, out recordCount).ToList<t_goods>();
                List<GoodsInfoResponseModel> goodsResponse = Utils.GetCopyList<GoodsInfoResponseModel, t_goods>(goods);
                GoodsInfoPageResponseModel model = new GoodsInfoPageResponseModel(goodsResponse, new Page(pageIndex, pageSize, recordCount));
                ResponseModel<GoodsInfoPageResponseModel> responseModel = new ResponseModel<GoodsInfoPageResponseModel>(model);
                return responseModel;
            }
        }

        /// <summary>
        /// 检测获取商品信息参数
        /// </summary>
        /// <param name="dicParas"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="errMsg"></param>
        /// <returns></returns>
        private bool checkGoodsParams(Dictionary<string, object> dicParas, out int pageIndex, out int pageSize, out string errMsg)
        {
            bool isCheck = true;
            pageIndex = 0;
            pageSize = 0;
            errMsg = string.Empty;

            string pageIndexStr = dicParas.ContainsKey("pageindex") ? dicParas["pageindex"].ToString() : "0";
            string pageSizeStr = dicParas.ContainsKey("pagesize") ? dicParas["pagesize"].ToString() : "0";

            if (isCheck && !int.TryParse(pageIndexStr, out pageIndex))
            {
                errMsg = "pageIndex参数不正确";
                isCheck = false;
            }

            if (isCheck && !int.TryParse(pageSizeStr, out pageSize))
            {
                errMsg = "pageSize参数不正确";
                isCheck = false;
            }

            pageSize = (pageSize == 0) ? defaultPageSize : pageSize;
            return isCheck;
        }
    
        
    }
}