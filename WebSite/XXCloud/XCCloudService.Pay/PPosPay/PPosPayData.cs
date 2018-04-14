using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XCCloudService.Pay.PPosPay
{
    /// <summary>
    /// 系统数据结构
    /// </summary>
    public class PPosPayData
    {

        #region "公众号支付"

        public class WeiXinPubPay
        {
            /// <summary>
            /// 机构号
            /// </summary>
            public string orgNo { get; set; }

            /// <summary>
            /// 商户号
            /// </summary>
            public string mercId { get; set; }

            /// <summary>
            /// 设备号
            /// </summary>
            public string trmNo { get; set; }

            /// <summary>
            /// 设备端交易时间
            /// </summary>
            public string txnTime { get; set; }

            /// <summary>
            /// 版本号
            /// </summary>
            public string version { get; set; }

            /// <summary>
            /// 在授权回调页面中获取到的授权code或者openid
            /// </summary>
            public string openid { get; set; }

            /// <summary>
            /// 实付金额
            /// </summary>
            public string amount { get; set; }

            /// <summary>
            /// 订单总金额
            /// </summary>
            public string total_amount { get; set; }

            /// <summary>
            /// 订单标题
            /// </summary>
            public string subject { get; set; }

            /// <summary>
            /// 订单号
            /// </summary>
            public string selOrderNo { get; set; }

            /// <summary>
            /// 订单优惠说明
            /// </summary>
            public string goods_tag { get; set; }

        }

        public class WeiXinPubPayACK
        {
            /// <summary>
            /// 返回码
            /// </summary>
            public string returnCode { get; set; }

            /// <summary>
            /// 系统交易时间
            /// </summary>
            public string sysTime { get; set; }

            /// <summary>
            /// 返回信息
            /// </summary>
            public string message { get; set; }

            /// <summary>
            /// 商户号
            /// </summary>
            public string mercId { get; set; }

            /// <summary>
            /// 系统流水号
            /// </summary>
            public string logNo { get; set; }

            /// <summary>
            /// 交易结查
            /// </summary>
            public string result { get; set; }

            /// <summary>
            /// 支付渠道订单号
            /// </summary>
            public string orderNo { get; set; }

            /// <summary>
            /// 实付金额
            /// </summary>
            public string amount { get; set; }

            /// <summary>
            /// 订单总金额
            /// </summary>
            public string total_amount { get; set; }

            /// <summary>
            /// 预支付 ID
            /// </summary>
            public string prepayId { get; set; }

            /// <summary>
            /// 支付公众号 ID
            /// </summary>
            public string apiAppid { get; set; }

            /// <summary>
            /// 支付时间戳
            /// </summary>
            public string apiTimestamp { get; set; }

            /// <summary>
            /// 支付随机字符串
            /// </summary>
            public string apiNoncestr { get; set; }

            /// <summary>
            /// 订单详情扩展字符串
            /// </summary>
            public string apiPackage { get; set; }

            /// <summary>
            /// 签名方式
            /// </summary>
            public string apiSigntype { get; set; }

            /// <summary>
            /// 签名
            /// </summary>
            public string apiPaysign { get; set; }

            /// <summary>
            /// 订单标题
            /// </summary>
            public string subject { get; set; }

            /// <summary>
            /// 订单号
            /// </summary>
            public string selOrderNo { get; set; }

            /// <summary>
            /// 订单优惠说明
            /// </summary>
            public string goodsTag { get; set; }

        }

        #endregion

        #region "公众号查询"

        public class WeiXinPubQuery
        {
            /// <summary>
            /// 机构号
            /// </summary>
            public string orgNo { get; set; }

            /// <summary>
            /// 商户号
            /// </summary>
            public string mercId { get; set; }

            /// <summary>
            /// 设备号
            /// </summary>
            public string trmNo { get; set; }

            /// <summary>
            /// 设备端交易时间
            /// </summary>
            public string txnTime { get; set; }

            /// <summary>
            /// 签名方式
            /// </summary>
            public string signType { get; set; }

            /// <summary>
            /// 签名域
            /// </summary>
            public string signValue { get; set; }

            /// <summary>
            /// 版本号
            /// </summary>
            public string version { get; set; }
        }

        public class WeiXinPubQueryACK
        {
            /// <summary>
            /// 返回码
            /// </summary>
            public string returnCode { get; set; }

            /// <summary>
            /// 系统交易时间
            /// </summary>
            public string sysTime { get; set; }

            /// <summary>
            /// 返回信息
            /// </summary>
            public string message { get; set; }

            /// <summary>
            /// 商户号
            /// </summary>
            public string mercId { get; set; }

            /// <summary>
            /// 签名域
            /// </summary>
            public string signValue { get; set; }

            /// <summary>
            /// 微信公众号ID
            /// </summary>
            public string appId { get; set; }

            /// <summary>
            /// 微信公众号密钥
            /// </summary>
            public string appIdKey { get; set; }
        }

        #endregion

        #region 扫码支付
        /// <summary>
        /// 扫码支付
        /// </summary>
        public class OrderPay
        {
            /// <summary>
            /// 操作系统
            /// 0：ANDROID sdk 1：IOS sdk 2：windows sdk 3:直连
            /// </summary>
            public string opSys { get; set; }
            /// <summary>
            /// 字符集
            /// 默认00    GBK
            /// </summary>
            public string characterSet { get; set; }
            /// <summary>
            /// 机构号
            /// </summary>
            public string orgNo { get; set; }
            /// <summary>
            /// 商户号
            /// </summary>
            public string mercId { get; set; }
            /// <summary>
            /// 终端号
            /// </summary>
            public string trmNo { get; set; }
            /// <summary>
            /// 订单编号
            /// </summary>
            public string tradeNo { get; set; }
            /// <summary>
            /// 交易时间
            /// </summary>
            public string txnTime { get; set; }
            /// <summary>
            /// 签名方式
            /// MD5
            /// </summary>
            public string signType { get; set; }
            /// <summary>
            /// 签名域
            /// </summary>
            public string signValue { get; set; }
            /// <summary>
            /// 版本号
            /// 默认V1.0.0
            /// </summary>
            public string version { get; set; }
            /// <summary>
            /// 实付金额，单位分
            /// </summary>
            public string amount { get; set; }
            /// <summary>
            /// 订单总金额
            /// </summary>
            public string total_amount { get; set; }
            /// <summary>
            /// 支付渠道
            /// ALIPAY  支付宝
            /// WXPAY   微信
            /// YLPAY   银联
            /// </summary>
            public string payChannel { get; set; }
            /// <summary>
            /// 订单标题
            /// </summary>
            public string subject { get; set; }
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

        #region 条码支付
        /// <summary>
        /// 条码支付
        /// </summary>
        public class MicroPay
        {
            /// <summary>
            /// 操作系统
            /// 0：ANDROID sdk 1：IOS sdk 2：windows sdk 3:直连
            /// </summary>
            public string opSys { get; set; }
            /// <summary>
            /// 字符集
            /// 默认00    GBK
            /// </summary>
            public string characterSet { get; set; }
            /// <summary>
            /// 机构号
            /// </summary>
            public string orgNo { get; set; }
            /// <summary>
            /// 商户号
            /// </summary>
            public string mercId { get; set; }
            /// <summary>
            /// 终端号
            /// </summary>
            public string trmNo { get; set; }
            /// <summary>
            /// 订单编号
            /// </summary>
            public string tradeNo { get; set; }
            /// <summary>
            /// 交易时间
            /// </summary>
            public string txnTime { get; set; }
            /// <summary>
            /// 签名方式
            /// MD5
            /// </summary>
            public string signType { get; set; }
            /// <summary>
            /// 签名域
            /// </summary>
            public string signValue { get; set; }
            /// <summary>
            /// 版本号
            /// 默认V1.0.0
            /// </summary>
            public string version { get; set; }
            /// <summary>
            /// 实付金额
            /// </summary>
            public string amount { get; set; }
            /// <summary>
            /// 应付金额
            /// </summary>
            public string total_amount { get; set; }
            /// <summary>
            /// 授权条码
            /// </summary>
            public string authCode { get; set; }
            /// <summary>
            /// 支付渠道
            /// 支付宝     ALIPAY
            /// 微信       WXPAY
            /// </summary>
            public string payChannel { get; set; }
            /// <summary>
            /// 订单描述
            /// </summary>
            public string subject { get; set; }
            /// <summary>
            /// 订单号
            /// </summary>
            public string selOrderNo { get; set; }
        }

        /// <summary>
        /// 条码支付应答
        /// </summary>
        public class MicroPayACK
        {
            /// <summary>
            /// 返回代码
            /// </summary>
            public string returnCode { get; set; }
            /// <summary>
            /// 返回消息
            /// </summary>
            public string message { get; set; }
            /// <summary>
            /// 订单编号
            /// </summary>
            public string tradeNo { get; set; }
            /// <summary>
            /// 系统流水号
            /// </summary>
            public string logNo { get; set; }
            /// <summary>
            /// 交易结果
            /// </summary>
            public string result { get; set; }
            /// <summary>
            /// 系统单号
            /// </summary>
            public string orderNo { get; set; }
            /// <summary>
            /// 实付金额
            /// </summary>
            public string amount { get; set; }
            /// <summary>
            /// 应付金额
            /// </summary>
            public string total_amount { get; set; }
            /// <summary>
            /// 订单号
            /// </summary>
            public string selOrderNo { get; set; }
        } 
        #endregion

        #region 订单查询
        /// <summary>
        /// 订单查询
        /// </summary>
        public class QueryOrder
        {
            /// <summary>
            /// 操作系统
            /// 0：ANDROID sdk 1：IOS sdk 2：windows sdk 3:直连
            /// </summary>
            public string opSys { get; set; }
            /// <summary>
            /// 字符集
            /// 默认00    GBK
            /// </summary>
            public string characterSet { get; set; }
            /// <summary>
            /// 机构号
            /// </summary>
            public string orgNo { get; set; }
            /// <summary>
            /// 商户号
            /// </summary>
            public string mercId { get; set; }
            /// <summary>
            /// 终端号
            /// </summary>
            public string trmNo { get; set; }
            /// <summary>
            /// 订单编号
            /// </summary>
            public string tradeNo { get; set; }
            /// <summary>
            /// 交易时间
            /// </summary>
            public string txnTime { get; set; }
            /// <summary>
            /// 签名方式
            /// MD5
            /// </summary>
            public string signType { get; set; }
            /// <summary>
            /// 签名域
            /// </summary>
            public string signValue { get; set; }
            /// <summary>
            /// 版本号
            /// 默认V1.0.0
            /// </summary>
            public string version { get; set; }
            /// <summary>
            /// 查询号码
            /// 可以是logNo、orderNo、tradeNo
            /// </summary>
            public string qryNo { get; set; }
        }

        /// <summary>
        /// 订单查询结果
        /// </summary>
        public class QueryOrderACK
        {
            public string returnCode { get; set; }
            public string message { get; set; }
            public string logNo { get; set; }
            public string result { get; set; }
            public string payChannel { get; set; }
            public string orderNo { get; set; }
            public string amount { get; set; }
            public string total_amount { get; set; }
            public string subject { get; set; }
            public string selOrderNo { get; set; }
        } 
        #endregion
    }
}