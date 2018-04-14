using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Security.Cryptography;
using System.Text;

namespace XCCloudService.PayChannel.Common
{
    /// <summary>
    ///LCSWPayDate 的摘要说明
    /// </summary>
    public class LCSWPayDate
    {
        public LCSWPayDate()
        {

        }

        /// <summary>
        /// 扫码支付
        /// </summary>
        public class OrderPay
        {
            /// <summary>
            /// 版本号，当前版本100
            /// </summary>
            public string pay_ver { get; set; }
            /// <summary>
            /// 请求类型，010微信，020 支付宝，060qq钱包，080京东钱包，090口碑
            /// </summary>
            public string pay_type { get; set; }
            /// <summary>
            /// 接口类型，当前类型011
            /// </summary>
            public string service_id { get; set; }
            /// <summary>
            /// 商户号
            /// </summary>
            public string merchant_no { get; set; }
            /// <summary>
            /// 终端号
            /// </summary>
            public string terminal_id { get; set; }
            /// <summary>
            /// 终端流水号，填写商户系统的订单号
            /// </summary>
            public string terminal_trace { get; set; }
            /// <summary>
            /// 终端交易时间，yyyyMMddHHmmss，全局统一时间格式
            /// </summary>
            public string terminal_time { get; set; }
            /// <summary>
            /// 金额，单位分
            /// </summary>
            public string total_fee { get; set; }
            /// <summary>
            /// 订单描述
            /// </summary>
            public string order_body { get; set; }
            /// <summary>
            /// 外部系统通知地址
            /// </summary>
            public string notify_url { get; set; }
            /// <summary>
            /// 附加数据，原样返回
            /// </summary>
            public string attach { get; set; }
            /// <summary>
            /// 订单包含的商品列表信息，Json格式。pay_type为090时，可选填此字段
            /// </summary>
            public string goods_detail { get; set; }
            /// <summary>
            /// 	签名字符串,拼装所有必传参数+令牌，UTF-8编码，32位md5加密转换
            /// </summary>
            public string key_sign { get; set; }

            public string GetSign(string Token)
            {
                string s = "";
                s += "pay_ver=" + pay_ver + "&";
                s += "pay_type=" + pay_type + "&";
                s += "service_id=" + service_id + "&";
                s += "merchant_no=" + merchant_no + "&";
                s += "terminal_id=" + terminal_id + "&";
                s += "terminal_trace=" + terminal_trace + "&";
                s += "terminal_time=" + terminal_time + "&";
                s += "total_fee=" + total_fee + "&";
                s += "access_token=" + Token;

                var md5 = MD5.Create();
                var bs = md5.ComputeHash(Encoding.UTF8.GetBytes(s));
                var sb = new StringBuilder();
                foreach (byte b in bs)
                {
                    sb.Append(b.ToString("x2"));
                }
                return sb.ToString().ToLower();
            }
        }

        /// <summary>
        /// 扫码付中，订单包含的商品列表信息，Json格式。pay_type为090时，可选填此字段
        /// </summary>
        public class GoodsDetail
        {
            /// <summary>
            /// 商品编号
            /// </summary>
            public string goods_Id { get; set; }
            /// <summary>
            /// 商品名称
            /// </summary>
            public string goods_name { get; set; }
            /// <summary>
            /// 商品数量
            /// </summary>
            public string quantity { get; set; }
            /// <summary>
            /// 商品单价，单位为分
            /// </summary>
            public string price { get; set; }
        }

        /// <summary>
        /// 扫码支付应答结构
        /// </summary>
        public class OrderPayACK
        {
            /// <summary>
            /// 响应码：01成功 ，02失败，响应码仅代表通信状态，不代表业务结果
            /// </summary>
            public string return_code { get; set; }
            /// <summary>
            /// 返回信息提示，“支付成功”，“支付中”，“请求受限”等
            /// </summary>
            public string return_msg { get; set; }
            /// <summary>
            /// 签名字符串,拼装所有传递参数，UTF-8编码，32位md5加密转换
            /// </summary>
            public string key_sign { get; set; }
            /// <summary>
            /// 业务结果：01成功 ，02失败
            /// </summary>
            public string result_code { get; set; }
            /// <summary>
            /// 请求类型，010微信，020 支付宝，060qq钱包，080京东钱包，090口碑
            /// </summary>
            public string pay_type { get; set; }
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
            /// 终端流水号，商户系统的订单号，扫呗系统原样返回
            /// </summary>
            public string terminal_trace { get; set; }
            /// <summary>
            /// 终端交易时间，yyyyMMddHHmmss，全局统一时间格式
            /// </summary>
            public string terminal_time { get; set; }
            /// <summary>
            /// 金额，单位分
            /// </summary>
            public string total_fee { get; set; }
            /// <summary>
            /// 利楚唯一订单号
            /// </summary>
            public string out_trade_no { get; set; }
            /// <summary>
            /// 二维码码串
            /// </summary>
            public string qr_code { get; set; }

            public bool CheckSign()
            {
                string s = "";
                s += "return_code=" + return_code;
                s += "&return_msg=" + return_msg;
                if (return_code == "01")
                {
                    s += "&result_code=" + result_code;
                    s += "&pay_type=" + pay_type;
                    s += "&merchant_name=" + merchant_name;
                    s += "&merchant_no=" + merchant_no;
                    s += "&terminal_id=" + terminal_id;
                    s += "&terminal_trace=" + terminal_trace;
                    s += "&terminal_time=" + terminal_time;
                    s += "&total_fee=" + total_fee;
                    s += "&out_trade_no=" + out_trade_no;
                    s += "&qr_code=" + qr_code;
                }

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
    }
}