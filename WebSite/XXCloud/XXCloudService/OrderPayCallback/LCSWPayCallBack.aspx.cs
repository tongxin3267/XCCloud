using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;
using XCCloudService.Business.XCCloud;
using XCCloudService.Common;
using XCCloudService.Common.Enum;
using XCCloudService.PayChannel.Common;

namespace XXCloudService.PayChannel
{
    public partial class LCSWPayCallBack : System.Web.UI.Page
    {
        /// <summary>
        /// 通知应答结构
        /// </summary>
        public class NotifyResponse
        {
            /// <summary>
            /// 响应码：01成功 ，02失败，响应码仅代表通信状态，不代表业务结果
            /// </summary>
            public string return_code { get; set; }
            /// <summary>
            /// 返回信息提示，“签名失败”，“参数格式校验错误"等
            /// </summary>
            public string return_msg { get; set; }
            /// <summary>
            /// 业务结果：01成功 ，02失败
            /// </summary>
            public string result_code { get; set; }
            /// <summary>
            /// 请求类型，010微信，020 支付宝，060qq钱包，080京东钱包，090口碑
            /// </summary>
            public string pay_type { get; set; }
            /// <summary>
            /// 付款方用户id，“微信openid”、“支付宝账户”、“qq号”等
            /// </summary>
            public string user_id { get; set; }
            /// <summary>
            /// 商户名称
            /// </summary>
            public string merchant_name { get; set; }
            /// <summary>
            /// 商户号
            /// </summary>
            public string merchant_no { get; set; }
            /// <summary>
            /// 终端号
            /// </summary>
            public string terminal_id { get; set; }
            /// <summary>
            /// 终端流水号，此处传商户发起预支付或公众号支付时所传入的交易流水号
            /// </summary>
            public string terminal_trace { get; set; }
            /// <summary>
            /// 终端交易时间，yyyyMMddHHmmss，全局统一时间格式（01时参与拼接）
            /// </summary>
            public string terminal_time { get; set; }
            /// <summary>
            /// 当前支付终端流水号，与pay_time同时传递，返回时不参与签名
            /// </summary>
            public string pay_trace { get; set; }
            /// <summary>
            /// 当前支付终端交易时间，yyyyMMddHHmmss，全局统一时间格式，与pay_trace同时传递
            /// </summary>
            public string pay_time { get; set; }
            /// <summary>
            /// 金额，单位分
            /// </summary>
            public string total_fee { get; set; }
            /// <summary>
            /// 支付完成时间，yyyyMMddHHmmss，全局统一时间格式
            /// </summary>
            public string end_time { get; set; }
            /// <summary>
            /// 利楚唯一订单号
            /// </summary>
            public string out_trade_no { get; set; }
            /// <summary>
            /// 通道订单号，微信订单号、支付宝订单号等
            /// </summary>
            public string channel_trade_no { get; set; }
            /// <summary>
            /// 附加数据，原样返回
            /// </summary>
            public string attach { get; set; }
            /// <summary>
            /// 口碑实收金额，pay_type为090时必填
            /// </summary>
            public string receipt_fee { get; set; }
            /// <summary>
            /// 签名字符串,拼装所有必传参数+令牌，32位md5加密转换
            /// </summary>
            public string key_sign { get; set; }

            public bool CheckSign()
            {
                string s = "";
                s += "return_code=" + return_code;
                s += "&return_msg=" + return_msg;
                if (return_code == "01")
                {
                    s += "&result_code=" + result_code;
                    s += "&pay_type=" + pay_type;
                    s += "&user_id=" + user_id;
                    s += "&merchant_name=" + merchant_name;
                    s += "&merchant_no=" + merchant_no;
                    s += "&terminal_id=" + terminal_id;
                    s += "&terminal_trace=" + terminal_trace;
                    s += "&terminal_time=" + terminal_time;
                    s += "&total_fee=" + total_fee;
                    s += "&end_time=" + end_time;
                    s += "&out_trade_no=" + out_trade_no;
                    s += "&channel_trade_no=" + channel_trade_no;
                    s += "&attach=" + attach;
                }
                //s += "&access_token=" + PayConfig.LCToken;

                var md5 = MD5.Create();
                var bs = md5.ComputeHash(Encoding.UTF8.GetBytes(s));
                var sb = new StringBuilder();
                foreach (byte b in bs)
                {
                    sb.Append(b.ToString("x2"));
                }
                return (sb.ToString().ToLower() == key_sign);
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            System.IO.Stream s = Request.InputStream;
            int count = 0;
            byte[] buffer = new byte[1024];
            StringBuilder builder = new StringBuilder();
            while ((count = s.Read(buffer, 0, 1024)) > 0)
            {
                builder.Append(Encoding.UTF8.GetString(buffer, 0, count));
            }
            s.Flush();
            s.Close();
            s.Dispose();

            PayLogHelper.WriteEvent(builder.ToString(), "扫呗支付");

            //string testresponse = "{\"attach\":\"\",\"channel_trade_no\":\"4009262001201708186903067327\",\"end_time\":\"20170818160418\",\"key_sign\":\"b503c7c76e4df6379c6d43c5103595b9\",\"merchant_name\":\"45对接专用1号\",\"merchant_no\":\"852100210000005\",\"out_trade_no\":\"300508950021217081816035700005\",\"pay_type\":\"010\",\"receipt_fee\":\"10\",\"result_code\":\"01\",\"return_code\":\"01\",\"return_msg\":\"支付成功\",\"terminal_id\":\"30050895\",\"terminal_time\":\"20170818160255\",\"terminal_trace\":\"b609bfed0a8f4badaa5373d20b30a52c\",\"total_fee\":\"10\",\"user_id\":\"on9DrwJ7GgmOaxHBN_yIkSCeBZVo\"}";

            JavaScriptSerializer jsonSerialize = new JavaScriptSerializer();
            NotifyResponse ack = jsonSerialize.Deserialize<NotifyResponse>(builder.ToString());
            //NotifyResponse ack = jsonSerialize.Deserialize<NotifyResponse>(testresponse);       
            Response.ContentType = "application/json";
            Response.HeaderEncoding = Encoding.UTF8;

            try
            {
                if (ack.CheckSign())
                {
                    if (ack.return_code == "01")
                    {
                        if (ack.result_code == "01")
                        {

                            string out_trade_no = ack.out_trade_no;
                            decimal total_fee = Convert.ToDecimal(ack.total_fee);
                            decimal payAmount = total_fee / 100;

                            Flw_OrderBusiness.OrderPay(out_trade_no, payAmount, SelttleType.LcswPay);

                        }
                    }

                    Response.Write("{\"return_code\":\"01\",\"return_msg\":\"success\"}");
                }
                else
                    Response.Write("{\"return_code\":\"02\",\"return_msg\":\"签名失败\"}");
            }
            catch (Exception ex)
            {
                Response.Write("{\"return_code\":\"02\",\"return_msg\":\"签名失败\"}");
                PayLogHelper.WriteError(ex);
            }
        }
    }
}