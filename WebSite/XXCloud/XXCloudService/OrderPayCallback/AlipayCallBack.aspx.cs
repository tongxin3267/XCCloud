using Com.Alipay;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using XCCloudService.BLL.CommonBLL;
using XCCloudService.Business.XCCloud;
using XCCloudService.Common.Enum;
using XCCloudService.Model.XCCloud;
using XCCloudService.Pay.Alipay;
using XCCloudService.PayChannel.Common;

namespace XXCloudService.PayChannel
{
    public partial class AlipayCallBack : System.Web.UI.Page
    {
        public delegate void DelegRun(string data);
        protected void Page_Load(object sender, EventArgs e)
        {
            SortedDictionary<string, string> sPara = GetRequestPost();

            PayLogHelper.WriteEvent(Request.Form.ToString(), "支付宝支付");


            if (sPara.Count > 0)//判断是否有带返回参数
            {
                Notify aliNotify = new Notify(AliPayConfig.charset, AliPayConfig.sign_type, AliPayConfig.pid, AliPayConfig.mapiUrl, AliPayConfig.alipay_public_key);
                bool verifyResult = aliNotify.Verify(sPara, Request.Form["notify_id"], Request.Form["sign"]);

                if (verifyResult && CheckParams())//验证成功
                {
                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    //请在这里加上商户的业务逻辑程序代码


                    //——请根据您的业务逻辑来编写程序（以下代码仅作参考）——
                    //获取支付宝的通知返回参数，可参考技术文档中服务器异步通知参数列表

                    //商户订单号
                    string out_trade_no = Request.Form["out_trade_no"];


                    //支付宝交易号
                    string trade_no = Request.Form["trade_no"];

                    //获得支付总金额total_amount
                    string total_amount = Request.Form["total_amount"];

                    //交易状态
                    //在支付宝的业务通知中，只有交易通知状态为TRADE_SUCCESS或TRADE_FINISHED时，才是买家付款成功。
                    string trade_status = Request.Form["trade_status"];

                    //交易状态
                    if (trade_status == "TRADE_SUCCESS" || trade_status == "TRADE_FINISHED")
                    {
                        try
                        {
                            decimal amount = Convert.ToDecimal(total_amount);
                            Flw_OrderBusiness.OrderPay(out_trade_no, amount, SelttleType.AliWxPay);
                        }
                        catch (Exception ex)
                        {
                            PayLogHelper.WriteError(ex);
                        }
                        Response.Write("success");  //请不要修改或删除
                    }
                    else
                    {
                        Response.Write("fail");
                    }

                    //——请根据您的业务逻辑来编写程序（以上代码仅作参考）——

                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////
                }
                else//验证失败
                {
                    //PayLogHelper.WriteEvent("支付宝验证失败");
                    Response.Write("fail");
                }
            }
            else
            {
                Response.Write("无通知参数");
            }
        }

        /// <summary>
        /// 对支付宝异步通知的关键参数进行校验
        /// </summary>
        /// <returns></returns>
        private bool CheckParams()
        {
            //获得商户订单号out_trade_no
            string out_trade_no = Request.Form["out_trade_no"];
            //判断通知数据中的out_trade_no是否为商户系统中创建的订单号
            Flw_Order order = Flw_OrderBusiness.GetOrderModel(out_trade_no);
            if (order == null)
            {
                return false;
            }

            // ****订单金额校验放在回调业务处理中去，因为当支付金额异常时要对订单进行异常标记****
            ////获得支付总金额total_amount
            //string total_amount = Request.Form["total_amount"];
            //// 判断total_amount是否确实为该订单的实际金额
            //decimal PayCount = order.PayCount != null ? (decimal)order.PayCount : 0; //应付金额
            //decimal FreePay = order.FreePay != null ? (decimal)order.FreePay : 0;   //减免金额
            //decimal payAmount = PayCount - FreePay; //实际应支付金额
            //decimal amount = Convert.ToDecimal(total_amount);
            ////如果实际应支付金额不等于支付宝通知中的金额，就返回校验失败
            //if (payAmount != amount)
            //{
            //    return false;
            //}

            //获得卖家账号seller_email
            string seller_id = Request.Form["seller_id"].Trim();
            // 验证通知中的seller_email（或者seller_id) 是否为out_trade_no这笔单据的对应的操作方（有的时候，一个商户可能有多个seller_id / seller_email）
            if (AliPayConfig.pid.Trim() != seller_id)
            {
                return false;
            }

            //获得调用方的appid；
            //如果是非授权模式，appid是商户的appid；如果是授权模式（token调用），appid是系统商的appid
            string app_id = Request.Form["app_id"];
            // 验证app_id是否是调用方的appid
            if (AliPayConfig.appId.Trim() != app_id)
            {
                return false;
            }

            //验证上述四个参数，完全吻合则返回参数校验成功
            return true;

        }

        /// <summary>
        /// 获取支付宝POST过来通知消息，并以“参数名=参数值”的形式组成数组
        /// </summary>
        /// <returns>request回来的信息组成的数组</returns>
        public SortedDictionary<string, string> GetRequestPost()
        {
            int i = 0;
            SortedDictionary<string, string> sArray = new SortedDictionary<string, string>();
            NameValueCollection coll;
            //Load Form variables into NameValueCollection variable.
            coll = Request.Form;

            // Get names of all forms into a string array.
            String[] requestItem = coll.AllKeys;

            for (i = 0; i < requestItem.Length; i++)
            {
                sArray.Add(requestItem[i], Request.Form[requestItem[i]]);
            }

            return sArray;
        }
    }
}