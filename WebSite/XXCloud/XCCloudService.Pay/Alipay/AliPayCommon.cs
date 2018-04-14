using Com.Alipay.Domain;
using Com.Alipay.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XCCloudService.Model.XCCloud;

namespace XCCloudService.Pay.Alipay
{
    public class AliPayCommon
    {
        #region 构造支付宝二维码请求数据
        /// <summary>
        /// 构造支付请求数据
        /// </summary>
        /// <returns>请求数据集</returns>
        public AlipayTradePrecreateContentBuilder BuildPrecreateContent(Flw_Order order, decimal amount, string subject)
        {
            AlipayTradePrecreateContentBuilder builder = new AlipayTradePrecreateContentBuilder();
            //收款账号
            builder.seller_id = AliPayConfig.pid;
            //订单编号
            builder.out_trade_no = order.OrderID;
            //订单总金额
            builder.total_amount = amount.ToString("0.00");
            //参与优惠计算的金额
            //builder.discountable_amount = "";
            //不参与优惠计算的金额
            //builder.undiscountable_amount = "";
            //订单名称
            builder.subject = subject;
            //自定义超时时间
            //builder.timeout_express = "5m";
            //订单描述
            builder.body = order.Note;
            //门店编号，很重要的参数，可以用作之后的营销
            builder.store_id = order.StoreID;
            //操作员编号，很重要的参数，可以用作之后的营销
            builder.operator_id = order.AuthorID == null ? "" : order.AuthorID.Value.ToString();

            ////传入商品信息详情
            //List<GoodsInfo> gList = new List<GoodsInfo>();
            //GoodsInfo goods = new GoodsInfo();
            //goods.goods_id = "goods id";
            //goods.goods_name = "goods name";
            //goods.price = "0.01";
            //goods.quantity = "1";
            //gList.Add(goods);
            //builder.goods_detail = gList;

            //系统商接入可以填此参数用作返佣
            //ExtendParams exParam = new ExtendParams();
            //exParam.sysServiceProviderId = "20880000000000";
            //builder.extendParams = exParam;

            return builder;

        }
        #endregion

        #region 构造支付宝条码支付请求数据
        /// <summary>
        /// 构造支付宝条码支付请求数据
        /// </summary>
        /// <returns>请求数据集</returns>
        public AlipayTradePayContentBuilder BuildPayContent(Flw_Order order, decimal amount, string subject, string authcode)
        {
            //扫码枪扫描到的用户手机钱包中的付款条码
            AlipayTradePayContentBuilder builder = new AlipayTradePayContentBuilder();

            //收款账号
            builder.seller_id = AliPayConfig.pid;
            //订单编号
            builder.out_trade_no = order.OrderID;
            //支付场景，无需修改
            builder.scene = "bar_code";
            //支付授权码,付款码
            builder.auth_code = authcode;
            //订单总金额
            builder.total_amount = amount.ToString("0.00");
            //参与优惠计算的金额
            //builder.discountable_amount = "";
            //不参与优惠计算的金额
            //builder.undiscountable_amount = "";
            //订单名称
            builder.subject = subject;
            //自定义超时时间
            builder.timeout_express = "2m";
            //订单描述
            builder.body = order.Note;
            //门店编号，很重要的参数，可以用作之后的营销
            builder.store_id = order.StoreID;
            //操作员编号，很重要的参数，可以用作之后的营销
            builder.operator_id = order.AuthorID == null ? "" : order.AuthorID.Value.ToString();


            ////传入商品信息详情
            //List<GoodsInfo> gList = new List<GoodsInfo>();

            //GoodsInfo goods = new GoodsInfo();
            //goods.goods_id = "304";
            //goods.goods_name = "goods#name";
            //goods.price = "0.01";
            //goods.quantity = "1";
            //gList.Add(goods);
            //builder.goods_detail = gList;

            //系统商接入可以填此参数用作返佣
            //ExtendParams exParam = new ExtendParams();
            //exParam.sysServiceProviderId = "20880000000000";
            //builder.extendParams = exParam;

            return builder;

        }
        #endregion
    }
}
