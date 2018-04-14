using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using XCCloudService.Business.XCGameMana;
using XCCloudService.Pay.WeiXinPay.Business;
using XCCloudService.Pay.WeiXinPay.Lib;


namespace XCCloudService.Pay
{
    public partial class Payment : System.Web.UI.Page
    {

        public static string wxJsApiParam { get; set; } //H5调起JS API参数
        protected void Page_Load(object sender, EventArgs e)
        {
            MPOrderBusiness.UpdateOrderForPaySuccess("2017112323575210001000000009", "1234324");

            if (!IsPostBack)
            {
                string openid = "oNWocwVUdOOajF2CpSmnyD8uN3Nw";
                string total_fee = "5";
                //检测是否给当前页面传递了相关参数
                if (string.IsNullOrEmpty(openid) || string.IsNullOrEmpty(total_fee))
                {
                    Response.Write("<span style='color:#FF0000;font-size:20px'>" + "oNWocwVUdOOajF2CpSmnyD8uN3Nw" + "</span>");
                    Response.Write("<span style='color:#FF0000;font-size:20px'>" + "页面传参出错,请返回重试" + "</span>");
                    submit.Visible = false;
                    return;
                }

                //若传递了相关参数，则调统一下单接口，获得后续相关接口的入口参数
                JsApiPay jsApiPay = new JsApiPay(this);
                jsApiPay.openid = openid;
                jsApiPay.total_fee = int.Parse(total_fee);
                jsApiPay.body = "ceshi";
                jsApiPay.out_trade_no = System.DateTime.Now.ToString("yyyyMMddHHmmss");
                jsApiPay.device_info = "100010";


                //JSAPI支付预处理
                try
                {
                    WxPayData unifiedOrderResult = jsApiPay.GetUnifiedOrderResult();
                    wxJsApiParam = jsApiPay.GetJsApiParameters();//获取H5调起JS API参数                    

                    //在页面上显示订单信息
                    Response.Write("<span style='color:#00CD00;font-size:20px'>订单详情：</span><br/>");
                    Response.Write("<span style='color:#00CD00;font-size:20px'>" + unifiedOrderResult.ToPrintStr() + "</span>");

                }
                catch (Exception ex)
                {
                    Response.Write("<span style='color:#FF0000;font-size:20px'>" + "下单失败，请返回重试" + "</span>");
                    submit.Visible = false;
                }
            }
        }
    }
}