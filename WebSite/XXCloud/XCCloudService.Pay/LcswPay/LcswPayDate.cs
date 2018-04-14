using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace XCCloudService.Pay.LcswPay
{
    /// <summary>
    /// 定义所有交易数据结构
    /// </summary>
    public class LcswPayDate
    {
        /// <summary>
        /// 条码付
        /// </summary>
        public class TradePay
        {
            /// <summary>
            /// 版本号，当前版本100
            /// </summary>
            public string pay_ver { get; set; }
            /// <summary>
            /// 请求类型，010微信，020 支付宝，060qq钱包，080京东钱包，090口碑，110银联二维码
            /// </summary>
            public string pay_type { get; set; }
            /// <summary>
            /// 接口类型，当前类型010
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
            /// 授权码
            /// </summary>
            public string auth_no { get; set; }
            /// <summary>
            /// 金额，单位分
            /// </summary>
            public string total_fee { get; set; }
            /// <summary>
            /// 订单描述
            /// </summary>
            public string order_body { get; set; }
            /// <summary>
            /// 附加数据,原样返回
            /// </summary>
            public string attach { get; set; }
            /// <summary>
            /// 订单包含的商品列表信息，Json格式。pay_type为090时，可选填此字段
            /// </summary>
            public string goods_detail { get; set; }
            /// <summary>
            /// 签名字符串,拼装所有必传参数+令牌，UTF-8编码，32位md5加密转换
            /// </summary>
            public string key_sign { get; set; }

            public string GetSign()
            {
                string s = "";
                s += "pay_ver=" + pay_ver + "&";
                s += "pay_type=" + pay_type + "&";
                s += "service_id=" + service_id + "&";
                s += "merchant_no=" + merchant_no + "&";
                s += "terminal_id=" + terminal_id + "&";
                s += "terminal_trace=" + terminal_trace + "&";
                s += "terminal_time=" + terminal_time + "&";
                s += "auth_no=" + auth_no + "&";
                s += "total_fee=" + total_fee + "&";
                s += "access_token=" + LcswPayConfig.Token;

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

        public class TradePayACK
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
            /// 业务结果：01成功 02失败 ，03支付中
            /// </summary>
            public string result_code { get; set; }
            /// <summary>
            /// 请求类型，010微信，020 支付宝，060qq钱包，080京东钱包，090口碑，110银联二维码
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
            /// 支付完成时间，yyyyMMddHHmmss，全局统一时间格式
            /// </summary>
            public string end_time { get; set; }
            /// <summary>
            /// 利楚唯一订单号
            /// </summary>
            public string out_trade_no { get; set; }
            /// <summary>
            /// 通道订单号，微信订单号、支付宝订单号等，返回时不参与签名
            /// </summary>
            public string channel_trade_no { get; set; }
            /// <summary>
            /// 付款方用户id，“微信openid”、“支付宝账户”、“qq号”等，返回时不参与签名
            /// </summary>
            public string user_id { get; set; }
            /// <summary>
            /// 附加数据,原样返回
            /// </summary>
            public string attach { get; set; }
            /// <summary>
            /// 口碑实收金额，pay_type为090时必填
            /// </summary>
            public string receipt_fee { get; set; }
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

            public string GetSign()
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
                s += "access_token=" + LcswPayConfig.Token;

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
                s += "&access_token=" + LcswPayConfig.Token;

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
        /// <summary>
        /// 查询结算余额
        /// </summary>
        public class Query
        {
            /// <summary>
            /// 机构编号，扫呗分配
            /// </summary>
            public string inst_no { get; set; }
            /// <summary>
            /// 请求流水号
            /// </summary>
            public string trace_no { get; set; }
            /// <summary>
            /// merchant_no
            /// </summary>
            public string merchant_no { get; set; }
            /// <summary>
            /// 签名检验串,拼装所有传递参数加令牌，32位md5加密转换
            /// </summary>
            public string key_sign { get; set; }

            public string GetSign()
            {
                string s = "";
                s += "inst_no=" + inst_no + "&";
                s += "merchant_no=" + merchant_no + "&";
                s += "trace_no=" + trace_no + "&";
                s += "key=" + LcswPayConfig.inst_token;

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
        /// 查询结算余额应答
        /// </summary>
        public class QueryACK
        {
            /// <summary>
            /// 响应码：“01”成功 ，02”失败，请求成功不代表业务处理成功
            /// </summary>
            public string return_code { get; set; }
            /// <summary>
            /// 返回信息提示，“机构号不存在”“请求受限”等
            /// </summary>
            public string return_msg { get; set; }
            /// <summary>
            /// 请求流水号
            /// </summary>
            public string trace_no { get; set; }
            /// <summary>
            /// 业务结果：“01”成功 ，02”失败
            /// </summary>
            public string result_code { get; set; }
            /// <summary>
            /// 商户号
            /// </summary>
            public string merchant_no { get; set; }
            /// <summary>
            /// 已结算金额
            /// </summary>
            public string settled_amt { get; set; }
            /// <summary>
            /// 未结算金额
            /// </summary>
            public string not_settle_amt { get; set; }
            /// <summary>
            /// 签名检验串,拼装传递参数return_code、return_msg、trace_no加令牌，32位md5加密转换
            /// </summary>
            public string key_sign { get; set; }

            public bool CheckSign()
            {
                string s = "";
                s += "return_code=" + return_code;
                s += "&return_msg=" + return_msg;
                s += "&trace_no=" + trace_no;
                s += "&key=" + LcswPayConfig.inst_token;

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
        /// <summary>
        /// 查询手续费
        /// </summary>
        public class Queryfee
        {
            /// <summary>
            /// 机构编号，扫呗分配
            /// </summary>
            public string inst_no { get; set; }
            /// <summary>
            /// 请求流水号
            /// </summary>
            public string trace_no { get; set; }
            /// <summary>
            /// 商户号
            /// </summary>
            public string merchant_no { get; set; }
            /// <summary>
            /// 提现金额，单位分
            /// </summary>
            public string amt { get; set; }
            /// <summary>
            /// 签名检验串,拼装所有传递参数加令牌，32位md5加密转换
            /// </summary>
            public string key_sign { get; set; }

            public string GetSign()
            {
                string s = "";
                s += "amt=" + amt + "&";
                s += "inst_no=" + inst_no + "&";
                s += "merchant_no=" + merchant_no + "&";
                s += "trace_no=" + trace_no + "&";
                s += "key=" + LcswPayConfig.inst_token;

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

        public class QueryfeeACK
        {
            /// <summary>
            /// 响应码：“01”成功 ，02”失败，请求成功不代表业务处理成功
            /// </summary>
            public string return_code { get; set; }
            /// <summary>
            /// 返回信息提示，“查询成功”，“请求受限”等
            /// </summary>
            public string return_msg { get; set; }
            /// <summary>
            /// 请求流水号
            /// </summary>
            public string trace_no { get; set; }
            /// <summary>
            /// 业务结果：“01”成功 ，02”失败
            /// </summary>
            public string result_code { get; set; }
            /// <summary>
            /// 商户号
            /// </summary>
            public string merchant_no { get; set; }
            /// <summary>
            /// 手续费
            /// </summary>
            public string fee_amt { get; set; }
            /// <summary>
            /// 签名检验串,拼装传递参数return_code、return_msg、trace_no加令牌，32位md5加密转换
            /// </summary>
            public string key_sign { get; set; }

            public bool CheckSign()
            {
                string s = "";
                s += "return_code=" + return_code;
                s += "&return_msg=" + return_msg;
                s += "&trace_no=" + trace_no;
                s += "&key=" + LcswPayConfig.inst_token;

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
        public class ApplayReq
        {
            /// <summary>
            /// 机构编号，扫呗分配
            /// </summary>
            public string inst_no { get; set; }
            /// <summary>
            /// 请求流水号
            /// </summary>
            public string trace_no { get; set; }
            /// <summary>
            /// 商户号
            /// </summary>
            public string merchant_no { get; set; }
            /// <summary>
            /// 提现金额，单位分
            /// </summary>
            public string amt { get; set; }
            /// <summary>
            /// 手续费
            /// </summary>
            public string fee_amt { get; set; }
            /// <summary>
            /// 提现类型（1:未结金额提现，2:已结金额提现）
            /// </summary>
            public string apply_type { get; set; }
            /// <summary>
            /// 签名检验串,拼装所有传递参数加令牌，32位md5加密转换
            /// </summary>
            public string key_sign { get; set; }

            public string GetSign()
            {
                string s = "";
                s += "amt=" + amt + "&";
                s += "apply_type=" + apply_type + "&";
                s += "fee_amt=" + fee_amt + "&";
                s += "inst_no=" + inst_no + "&";
                s += "merchant_no=" + merchant_no + "&";
                s += "trace_no=" + trace_no + "&";
                s += "key=" + LcswPayConfig.inst_token;

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

        public class ApplayAck
        {
            /// <summary>
            /// 响应码：“01”成功 ，02”失败，请求成功不代表业务处理成功
            /// </summary>
            public string return_code { get; set; }
            /// <summary>
            /// 返回信息提示，“查询成功”，“请求受限”等
            /// </summary>
            public string return_msg { get; set; }
            /// <summary>
            /// 请求流水号
            /// </summary>
            public string trace_no { get; set; }
            /// <summary>
            /// 业务结果：“01”成功 ，02”失败
            /// </summary>
            public string result_code { get; set; }
            /// <summary>
            /// 商户号
            /// </summary>
            public string merchant_no { get; set; }
            /// <summary>
            /// 签名检验串,拼装传递参数return_code、return_msg、trace_no加令牌，32位md5加密转换
            /// </summary>
            public string key_sign { get; set; }

            public bool CheckSign()
            {
                string s = "";
                s += "return_code=" + return_code;
                s += "&return_msg=" + return_msg;
                s += "&trace_no=" + trace_no;
                s += "&key=" + LcswPayConfig.inst_token;

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
