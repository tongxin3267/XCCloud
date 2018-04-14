using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using XCCloudService.Pay.DinPay;

namespace XXCloudService.Pay
{
    public partial class DinPay : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string payType = Request["payType"];

            DinPayData.WxCommonPay Pay = new DinPayData.WxCommonPay();
            Pay.order_no = Guid.NewGuid().ToString().Replace("-", "");
            Pay.order_time = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            Pay.order_amount = "0.01";
            Pay.product_name = "智付支付测试";

            DinPayApi payApi = new DinPayApi();
            //构造请求
            string sHtmlText = payApi.WebPagePay(Pay, payType);
            Response.Write(sHtmlText);
        }
    }
}