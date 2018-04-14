using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XCCloudService.Pay.DinPay
{
    public class DinPayData
    {
        #region 扫码支付
        /// <summary>
        /// 扫码支付
        /// </summary>
        public class ScanPay
        {
            /// <summary>
            /// 接口版本
            /// </summary>
            public string interface_version { get; set; }

            /// <summary>
            /// 服务类型
            /// </summary>
            public string service_type { get; set; }

            /// <summary>
            /// 商户号
            /// </summary>
            public string merchant_code { get; set; }

            /// <summary>
            /// 商户订单号
            /// </summary>
            public string order_no { get; set; }

            /// <summary>
            /// 商户订单时间
            /// </summary>
            public string order_time { get; set; }

            /// <summary>
            /// 订单金额，单位：元
            /// </summary>
            public string order_amount { get; set; }

            /// <summary>
            /// 商品名称
            /// </summary>
            public string product_name { get; set; }

            ///// <summary>
            ///// 商品编号
            ///// </summary>
            //public string product_code { get; set; }

            ///// <summary>
            ///// 商品数量
            ///// </summary>
            //public string product_num { get; set; }

            /// <summary>
            /// 商品描述
            /// </summary>
            public string product_desc { get; set; }

            ///// <summary>
            ///// 公用回传参数
            ///// </summary>
            //public string extra_return_param { get; set; }

            ///// <summary>
            ///// 公用业务扩展参数
            ///// </summary>
            //public string extend_param { get; set; }

            /// <summary>
            /// 异步通知地址（Notify URL）
            /// </summary>
            public string notify_url { get; set; }

            /// <summary>
            /// 客户端IP（Client IP）
            /// </summary>
            public string client_ip { get; set; }
        }

        /// <summary>
        /// 扫码付应答
        /// </summary>
        public class OrderPayACK
        {
            /// <summary>
            /// 实付金额
            /// </summary>
            public string amount { get; set; }
            /// <summary>
            /// 返回消息
            /// </summary>
            public string message { get; set; }
            /// <summary>
            /// 系统时间
            /// </summary>
            public string sysTime { get; set; }
            /// <summary>
            /// 返回代码
            /// </summary>
            public string returnCode { get; set; }
            /// <summary>
            /// 订单编号
            /// </summary>
            public string tradeNo { get; set; }
            /// <summary>
            /// 商品描述
            /// </summary>
            public string subject { get; set; }
            /// <summary>
            /// 商户号
            /// </summary>
            public string mercId { get; set; }
            /// <summary>
            /// 条码内容
            /// </summary>
            public string payCode { get; set; }
            /// <summary>
            /// 签名值
            /// </summary>
            public string signValue { get; set; }
        }
        #endregion

        #region 刷卡(条码)支付
        /// <summary>
        /// 刷卡(条码)支付
        /// </summary>
        public class MicroPay
        {
            /// <summary>
            /// 参数编码字符集
            /// </summary>
            public string input_charset { get; set; }
            /// <summary>
            /// 接口版本
            /// </summary>
            public string interface_version { get; set; }
            /// <summary>
            /// 商户号
            /// </summary>
            public string merchant_code { get; set; }
            /// <summary>
            /// 服务类型
            /// </summary>
            public string service_type { get; set; }
            /// <summary>
            /// 结果回调地址
            /// </summary>
            public string notify_url { get; set; }
            /// <summary>
            /// 订单号
            /// </summary>
            public string order_no { get; set; }
            /// <summary>
            /// 订单时间
            /// </summary>
            public string order_time { get; set; }
            /// <summary>
            /// 订单金额
            /// </summary>
            public string order_amount { get; set; }
            /// <summary>
            /// 产品名称
            /// </summary>
            public string product_name { get; set; }
            /// <summary>
            /// 授权码
            /// </summary>
            public string auth_code { get; set; }
            /// <summary>
            /// 客户端IP
            /// </summary>
            public string client_ip { get; set; }
            /// <summary>
            /// 页面跳转同步通知地址
            /// </summary>
            //public string return_url { get; set; }
            ///// <summary>
            ///// 是否允许重复订单
            ///// </summary>
            //public string redo_flag { get; set; }
        }                    
        #endregion

        #region 微信公众号支付
        /// <summary>
        /// 微信公众号支付
        /// </summary>
        public class WxCommonPay
        {
            /// <summary>
            /// 参数编码字符集
            /// </summary>
            public string input_charset { get; set; }
            /// <summary>
            /// 接口版本
            /// </summary>
            public string interface_version { get; set; }
            /// <summary>
            /// 商户号
            /// </summary>
            public string merchant_code { get; set; }
            /// <summary>
            /// 服务类型
            /// </summary>
            public string service_type { get; set; }
            /// <summary>
            /// 结果回调地址
            /// </summary>
            public string notify_url { get; set; }
            /// <summary>
            /// 订单号
            /// </summary>
            public string order_no { get; set; }
            /// <summary>
            /// 订单时间
            /// </summary>
            public string order_time { get; set; }
            /// <summary>
            /// 订单金额
            /// </summary>
            public string order_amount { get; set; }
            /// <summary>
            /// 产品名称
            /// </summary>
            public string product_name { get; set; }

            /// <summary>
            /// 签名类型
            /// </summary>
            public string sign_type { get; set; }

            /// <summary>
            /// 签名
            /// </summary>
            public string sign { get; set; }

            ///// <summary>
            ///// 页面跳转同步通知地址
            ///// </summary>
            //public string return_url { get; set; }
            ///// <summary>
            ///// 是否允许重复订单
            ///// </summary>
            //public string redo_flag { get; set; }
        }
        #endregion

        #region 转账
        /// <summary>
        /// 转账
        /// </summary>
        public class TransferData
        {
            /// <summary>
            /// 接口版本号（V3.1.0）
            /// </summary>
            public string interface_version { get; set; }

            /// <summary>
            /// 商家网站生成的转账编号
            /// </summary>
            public string mer_transfer_no { get; set; }

            /// <summary>
            /// 商户号
            /// </summary>
            public string merchant_no { get; set; }

            /// <summary>
            /// 请求码（转账：DMTI）
            /// </summary>
            public string tran_code { get; set; }

            /// <summary>
            /// 银行代码
            /// </summary>
            public string recv_bank_code { get; set; }

            /// <summary>
            /// 收款人的银行账号
            /// </summary>
            public string recv_accno { get; set; }

            /// <summary>
            /// 收款人的姓名
            /// </summary>
            public string recv_name { get; set; }

            /// <summary>
            /// 省份代码
            /// </summary>
            public string recv_province { get; set; }

            /// <summary>
            /// 城市代码
            /// </summary>
            public string recv_city { get; set; }

            /// <summary>
            /// 转账金额（单位：元，如100.23）
            /// </summary>
            public string tran_amount { get; set; }

            /// <summary>
            /// 扣除手续费的方式：
            ///   0：从转账金额中扣除
            ///   1：从账户余额中扣除
            /// </summary>
            public string tran_fee_type { get; set; }

            /// <summary>
            /// 转账的方式（0：普通  1：加急）
            /// </summary>
            public string tran_type { get; set; }
        } 
        #endregion
    }
}
