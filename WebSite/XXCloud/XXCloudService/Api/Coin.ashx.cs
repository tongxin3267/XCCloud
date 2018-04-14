using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using XCCloudService.Base;
using XCCloudService.DBService.Model;
using XCCloudService.DBService.DAL;
using XCCloudService.DBService.BLL;
using XCCloudService.Common;

namespace XCCloudService.Api
{
    /// <summary>
    /// 代币管理
    /// </summary>
    public class Coin : ApiBase
    {
        /// <summary>
        /// 代币库存情况分析
        /// </summary>
        public object coinStock()
        {
            return null;
        }

        /// <summary>
        /// 代币入库
        /// </summary>
        public object coinStorage(Dictionary<string, object> dicParas)
        {
            string errMsg = string.Empty;
            string businessDate = string.Empty;
            string pageIndexStr = string.Empty;
            string pageSizeStr = string.Empty;
            int pageIndex = 0;
            int pageSize = 0;

            if (!checkCoinStorageParams(dicParas, out businessDate, out pageIndex, out pageSize, out errMsg))
            {
                ResponseModel responseModel = new ResponseModel(Return_Code.T, "", Result_Code.F, errMsg);
                return responseModel;
            }
            else
            {
                int recordCount = 0;
                CoinStoragePageModel model = CoinBLL.CoinStorage(businessDate, pageIndex, pageSize, out recordCount);
                ResponseModel<CoinStoragePageModel> responseModel = new ResponseModel<CoinStoragePageModel>(model);
                return responseModel;
            }
        }

        /// <summary>
        /// 检测代币入库参数
        /// </summary>
        private bool checkCoinStorageParams(Dictionary<string, object> dicParas, out string businessDate, out int pageIndex, out int pageSize, out string errMsg)
        {
            bool isCheck = true;
            pageIndex = 0;
            pageSize = 0;
            errMsg = string.Empty;
            businessDate = dicParas.ContainsKey("businessdate") ? dicParas["businessdate"].ToString() : string.Empty;
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

            if (isCheck && !Utils.CheckDate(businessDate))
            {
                errMsg = "businessDate参数不正确";
                isCheck = false;
            }

            pageSize = (pageSize == 0) ? defaultPageSize : pageSize; 
            return isCheck;
        }

        /// <summary>
        /// 代币销毁
        /// </summary>
        /// <returns></returns>
        public object coinDestroy(Dictionary<string, object> dicParas)
        {
            string errMsg = string.Empty;
            string destroyDate = string.Empty;
            string pageIndexStr = string.Empty;
            string pageSizeStr = string.Empty;
            int pageIndex = 0;
            int pageSize = 0;

            if (!checkCoinDestroyParams(dicParas, out destroyDate, out pageIndex, out pageSize, out errMsg))
            {
                ResponseModel responseModel = new ResponseModel(Return_Code.T, "", Result_Code.F, errMsg);
                return responseModel;
            }
            else
            {
                int recordCount = 0;
                CoinDestroyPageModel model = CoinBLL.CoinDestroy(destroyDate, pageIndex, pageSize, out recordCount);
                ResponseModel<CoinDestroyPageModel> responseModel = new ResponseModel<CoinDestroyPageModel>(model);
                return responseModel;
            }
        }

        /// <summary>
        /// 检测代币入库参数
        /// </summary>
        private bool checkCoinDestroyParams(Dictionary<string, object> dicParas, out string destroyDate, out int pageIndex, out int pageSize, out string errMsg)
        {
            bool isCheck = true;
            pageIndex = 0;
            pageSize = 0;
            errMsg = string.Empty;
            destroyDate = dicParas.ContainsKey("destroydate") ? dicParas["destroydate"].ToString() : string.Empty;
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

            if (isCheck && !Utils.CheckDate(destroyDate))
            {
                errMsg = "destroyDate参数不正确";
                isCheck = false;
            }

            pageSize = (pageSize == 0) ? defaultPageSize : pageSize;
            return isCheck;
        }

        /// <summary>
        /// 代币销售情况分析
        /// </summary>
        /// <param name="dicParas">参数字典</param>
        /// <returns></returns>
        public object coinSales(Dictionary<string, object> dicParas)
        {
            string errMsg = string.Empty;
            string salesDate = string.Empty;
            string pageIndexStr = string.Empty;
            string pageSizeStr = string.Empty;
            int pageIndex = 0;
            int pageSize = 0;
            int coins = 0;

            if (!checkCoinSalesParams(dicParas, out salesDate,out coins, out pageIndex, out pageSize, out errMsg))
            {
                ResponseModel responseModel = new ResponseModel(Return_Code.T, "", Result_Code.F, errMsg);
                return responseModel;
            }
            else
            {
                int recordCount = 0;
                CoinSalesModel listInitModel = CoinBLL.CoinSales(salesDate,coins,pageIndex,pageSize,out recordCount);
                ResponseModel<CoinSalesModel> responseModel = new ResponseModel<CoinSalesModel>(listInitModel);
                return responseModel;
            }
        }

        /// <summary>
        /// 检测代币入库参数
        /// </summary>
        private bool checkCoinSalesParams(Dictionary<string, object> dicParas, out string salesDate, out int coins,out int pageIndex, out int pageSize, out string errMsg)
        {
            bool isCheck = true;
            pageIndex = 0;
            pageSize = 0;
            coins = 0;
            errMsg = string.Empty;
            salesDate = dicParas.ContainsKey("salesdate") ? dicParas["salesdate"].ToString() : string.Empty;
            string pageIndexStr = dicParas.ContainsKey("pageindex") ? dicParas["pageindex"].ToString() : "0";
            string pageSizeStr = dicParas.ContainsKey("pagesize") ? dicParas["pagesize"].ToString() : "0";
            string coinsStr = dicParas.ContainsKey("coins") ? dicParas["coins"].ToString() : "0";

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

            if (isCheck && !Utils.CheckDate(salesDate))
            {
                errMsg = "salesDate参数不正确";
                isCheck = false;
            }

            if (isCheck && !int.TryParse(coinsStr, out coins))
            {
                errMsg = "coins参数不正确";
                isCheck = false;
            }

            pageSize = (pageSize == 0) ? defaultPageSize : pageSize;
            return isCheck;
        }

        /// <summary>
        /// 代币安装
        /// </summary>
        /// <returns></returns>
        public object coinInstall()
        {
            return null;
        }

        /// <summary>
        /// 代币盘点
        /// </summary>
        /// <returns></returns>
        public object coinInventory()
        {
            return null;
        }
    }
}