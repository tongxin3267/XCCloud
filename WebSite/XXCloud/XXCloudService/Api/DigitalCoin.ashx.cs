using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using XCCloudService.Base;
using XCCloudService.DBService.Model;
using XCCloudService.DBService.BLL;
using XCCloudService.Common;

namespace XCCloudService.Api
{
    /// <summary>
    /// 数字币管理
    /// </summary>
    public class DigitalCoin : ApiBase
    {
        /// <summary>
        /// 数字币入库
        /// </summary>
        public object coinStorage()
        {
            return null;
        }

        /// <summary>
        /// 数字币销售
        /// </summary>
        /// <returns></returns>
        public object coinSales(Dictionary<string, object> dicParas)
        {
            string errMsg = string.Empty;
            string businessDate = string.Empty;

            if (dicParas.ContainsKey("businessDate"))
            {
                businessDate = dicParas["businessDate"].ToString();
            }

            if (string.IsNullOrEmpty(businessDate))
            {
                errMsg = "营业日期参数不存在";
            }

            if (!string.IsNullOrEmpty(errMsg))
            {
                ResponseModel responseModel = new ResponseModel(Return_Code.T, "", Result_Code.F, errMsg);
                return responseModel;
            }
            else
            {
                DigiteCoinSaleDataModel model = DigiteCoinBLL.DigiteCoinSale(businessDate);
                ResponseModel<DigiteCoinSaleDataModel> responseModel = new ResponseModel<DigiteCoinSaleDataModel>(model);
                return responseModel;
            }
        }

        /// <summary>
        /// 数字币销毁
        /// </summary>
        /// <returns></returns>
        public object coinDestroy(Dictionary<string, object> dicParas)
        {
            string errMsg = string.Empty;
            string businessDate = string.Empty;
            string iccardid = string.Empty;
            int pageIndex = 0;
            int pageSize = 0;
            int recordCount = 0;

            if (!checkCoinDestroyParams(dicParas, out businessDate, out iccardid, out pageIndex, out pageSize, out errMsg))
            {
                ResponseModel responseModel = new ResponseModel(Return_Code.T, "", Result_Code.F, errMsg);
                return responseModel;
            }
            else
            {
                DigiteCoinDestroyModel model = DigiteCoinBLL.DigiteCoinDestory(businessDate, iccardid, pageIndex, pageSize, out recordCount);
                ResponseModel<DigiteCoinDestroyModel> responseModel = new ResponseModel<DigiteCoinDestroyModel>(model);
                return responseModel;
            }  
        }

        /// <summary>
        /// 检测代币入库参数
        /// </summary>
        private bool checkCoinDestroyParams(Dictionary<string, object> dicParas, out string businessDate, out string iccardid, out int pageIndex, out int pageSize, out string errMsg)
        {
            bool isCheck = true;
            pageIndex = 0;
            pageSize = 0;
            errMsg = string.Empty;
            string pageIndexStr = dicParas.ContainsKey("pageindex") ? dicParas["pageindex"].ToString() : "0";
            string pageSizeStr = dicParas.ContainsKey("pagesize") ? dicParas["pagesize"].ToString() : "0";
            businessDate = dicParas.ContainsKey("businessdate") ? dicParas["businessdate"].ToString() : string.Empty;
            iccardid = dicParas.ContainsKey("iccardid") ? dicParas["iccardid"].ToString() : "";

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
        /// 数字币安装
        /// </summary>
        /// <returns></returns>
        public object coinInstall(Dictionary<string, object> dicParas)
        {

            return null;
        }
    }
}