using Aop.Api;
using Aop.Api.Request;
using Aop.Api.Response;
using Com.Alipay;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using XCCloudService.Business.Common;
using XCCloudService.Business.XCGameMana;
using XCCloudService.Common;
using XCCloudService.Common.Enum;
using XCCloudService.Model.XCGameManager;
using XCCloudService.Pay.Alipay;
using XCCloudService.PayChannel.Common;

namespace XXCloudService.CallBack
{
    public partial class AliPayNotify : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            SortedDictionary<string, string> sPara = GetRequestPost();

            if (sPara.Count > 0)//判断是否有带返回参数
            {
                //PayLogHelper.WritePayLog(Request.Form.ToString());
                //Notify aliNotify = new Notify();
                Notify aliNotify = new Notify(AliPayConfig.charset, AliPayConfig.sign_type, AliPayConfig.pid, AliPayConfig.mapiUrl, AliPayConfig.alipay_miniapp_public_key);
                bool verifyResult = aliNotify.Verify(sPara, Request.Form["notify_id"], Request.Form["sign"]);

                //商户订单号
                string out_trade_no = Request.Form["out_trade_no"];
                Data_Order order = null;

                if (verifyResult && CheckParams(out order))//验证成功
                {
                    ///////////////////////////////////////////////////////////////////////////////////////////////////////////
                    //请在这里加上商户的业务逻辑程序代码


                    //——请根据您的业务逻辑来编写程序（以下代码仅作参考）——
                    //获取支付宝的通知返回参数，可参考技术文档中服务器异步通知参数列表

                    //支付宝交易号
                    string trade_no = Request.Form["trade_no"];

                    //交易状态
                    //在支付宝的业务通知中，只有交易通知状态为TRADE_SUCCESS或TRADE_FINISHED时，才是买家付款成功。
                    string trade_status = Request.Form["trade_status"];

                    //交易状态
                    if (trade_status == "TRADE_SUCCESS" || trade_status == "TRADE_FINISHED")
                    {
                        try
                        {
                            if (MPOrderBusiness.UpdateOrderForPaySuccess(out_trade_no, trade_no))
                            {
                                LogHelper.SaveLog(TxtLogType.AliPay, TxtLogContentType.Debug, TxtLogFileType.Day, "应用：莘拍档 订单号：" + out_trade_no + " 支付成功！");

                                //支付宝买家用户id
                                string buyer_id = Request.Form["buyer_id"];
                                //支付时间
                                string gmt_payment = HttpUtility.UrlDecode(Request.Form["gmt_payment"]);

                                string aliId = string.Empty;
                                string msg = string.Empty;
                                if (!MobileTokenBusiness.GetAliId(order.Mobile, out aliId, out msg))
                                {
                                    bool ret = MobileTokenBusiness.UpdateAliBuyerId(order.Mobile, buyer_id);
                                }

                                IAopClient client = new DefaultAopClient(AliPayConfig.serverUrl, AliPayConfig.miniAppId, AliPayConfig.merchant_miniapp_private_key, "json", "1.0", "RSA2", AliPayConfig.alipay_miniapp_public_key, AliPayConfig.charset, false);
                                AlipayOpenAppMiniTemplatemessageSendRequest request = new AlipayOpenAppMiniTemplatemessageSendRequest();
                                request.BizContent = "{" +
                                "\"to_user_id\":\"" + buyer_id + "\"," +
                                "\"form_id\":\"" + trade_no + "\"," +
                                "\"user_template_id\":\"" + AliPayConfig.MiniAppTemplateId + "\"," +
                                "\"page\":\"pages/login/login\"," +
                                "\"data\":\"{\\\"keyword1\\\":{\\\"value\\\":\\\"" + out_trade_no + "\\\"},\\\"keyword2\\\":{\\\"value\\\":\\\"" + order.Descript + "\\\"},\\\"keyword3\\\":{\\\"value\\\":\\\"" + order.Price.ToString("0.00") + "\\\"},\\\"keyword4\\\":{\\\"value\\\":\\\"" + order.CreateTime.ToString("yyyy-MM-dd HH:mm:ss") + "\\\"},\\\"keyword5\\\":{\\\"value\\\":\\\"" + gmt_payment + "\\\"}}\"" +
                                "}";
                                AlipayOpenAppMiniTemplatemessageSendResponse response = client.Execute(request);
                            }
                            else
                            {
                                LogHelper.SaveLog(TxtLogType.AliPay, TxtLogContentType.Debug, TxtLogFileType.Day, "应用：莘拍档 订单号：" + out_trade_no + " 已支付订单更新失败！！！");
                            }

                        }
                        catch (Exception ex)
                        {
                            //PayLogHelper.WriteError(ex);
                            LogHelper.SaveLog(TxtLogType.AliPay, ex.InnerException != null ? ex.InnerException.Message : ex.Message);
                        }

                        //判断是否在商户网站中已经做过了这次通知返回的处理
                        //如果没有做过处理，那么执行商户的业务程序
                        //如果有做过处理，那么不执行商户的业务程序
                        Response.Write("success");  //请不要修改或删除
                    }
                    else
                    {
                        Response.Write("fail");
                    }

                    //——请根据您的业务逻辑来编写程序（以上代码仅作参考）——

                    ///////////////////////////////////////////////////////////////////////////////////////////////////////////
                }
                else//验证失败
                {
                    LogHelper.SaveLog(TxtLogType.AliPay, TxtLogContentType.Debug, TxtLogFileType.Day, "应用：莘拍档 订单号：" + out_trade_no + " 警告：支付回调验证失败！！！");
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
        private bool CheckParams(out Data_Order order)
        {
            //获得商户订单号out_trade_no
            string out_trade_no = Request.Form["out_trade_no"];
            //判断通知数据中的out_trade_no是否为商户系统中创建的订单号
            order = MPOrderBusiness.GetOrderModel(out_trade_no);
            if (order == null)
            {
                //PayLogHelper.WritePayLog("商户订单号校验失败");
                return false;
            }

            //获得支付总金额total_amount
            string total_amount = Request.Form["total_amount"];
            // 判断total_amount是否确实为该订单的实际金额
            if (order.Price.ToString("0.00") != total_amount)
            {
                //PayLogHelper.WritePayLog("支付金额校验失败");
                return false;
            }

            //获得卖家账号seller_id
            string seller_id = Request.Form["seller_id"].Trim();
            // 验证通知中的seller_email（或者seller_id) 是否为out_trade_no这笔单据的对应的操作方（有的时候，一个商户可能有多个seller_id / seller_email）
            if (AliPayConfig.pid.Trim() != seller_id)
            {
                //PayLogHelper.WritePayLog("seller_id校验失败");
                return false;
            }

            //获得调用方的appid；
            //如果是非授权模式，appid是商户的appid；如果是授权模式（token调用），appid是系统商的appid
            string app_id = Request.Form["app_id"];
            // 验证支付宝小程序app_id是否是调用方的appid
            if (AliPayConfig.miniAppId.Trim() != app_id)
            {
                //PayLogHelper.WritePayLog("app_id校验失败");
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