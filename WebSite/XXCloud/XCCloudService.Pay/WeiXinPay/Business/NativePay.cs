using System;
using System.Collections.Generic;
using System.Web;
using XCCloudService.Model.XCCloud;
using XCCloudService.Pay.WeiXinPay.Lib;

namespace XCCloudService.Pay.WeiXinPay.Business
{
    public class NativePay
    {
        /**
        * 生成扫描支付模式一URL
        * @param productId 商品ID
        * @return 模式一URL
        */
        public string GetPrePayUrl(string productId)
        {
            Log.Info(this.GetType().ToString(), "Native pay mode 1 url is producing...");

            WxPayData data = new WxPayData();
            data.SetValue("appid", WxPayConfig.APPID);//公众帐号id
            data.SetValue("mch_id", WxPayConfig.MCHID);//商户号
            data.SetValue("time_stamp", WxPayApi.GenerateTimeStamp());//时间戳
            data.SetValue("nonce_str", WxPayApi.GenerateNonceStr());//随机字符串
            data.SetValue("product_id", productId);//商品ID
            data.SetValue("sign", data.MakeSign());//签名
            string str = ToUrlParams(data.GetValues());//转换为URL串
            string url = "weixin://wxpay/bizpayurl?" + str;

            Log.Info(this.GetType().ToString(), "Get native pay mode 1 url : " + url);
            return url;
        }

        #region 微信--生成支付二维码URL，模式二
        /**
        * 生成直接支付url，支付url有效期为2小时,模式二
        * @param productId 商品ID
        * @return 模式二URL
        */
        public WxPayData GetPayUrl(Flw_Order order, decimal amount, string subject)
        {
            WxPayData data = new WxPayData();
            data.SetValue("body", subject);//商品描述
            data.SetValue("attach", order.Note);//附加数据
            data.SetValue("out_trade_no", order.OrderID);//订单号
            //data.SetValue("out_trade_no", WxPayApi.GenerateOutTradeNo());//订单号
            data.SetValue("total_fee", Convert.ToInt32(amount * 100));//总金额，单位：分
            data.SetValue("time_start", DateTime.Now.ToString("yyyyMMddHHmmss"));//交易起始时间
            data.SetValue("time_expire", DateTime.Now.AddMinutes(10).ToString("yyyyMMddHHmmss"));//交易结束时间
            data.SetValue("goods_tag", order.ID.ToString());//商品标记
            data.SetValue("trade_type", "NATIVE");//交易类型
            data.SetValue("product_id", order.OrderID);//商品ID，商户自定义

            WxPayData result = WxPayApi.UnifiedOrder(data);//调用统一下单接口
            //qr_url = result.GetValue("code_url").ToString();//获得统一下单接口返回的二维码链接
            return result;
        }
        #endregion

        /**
        * 参数数组转换为url格式
        * @param map 参数名与参数值的映射表
        * @return URL字符串
        */
        private string ToUrlParams(SortedDictionary<string, object> map)
        {
            string buff = "";
            foreach (KeyValuePair<string, object> pair in map)
            {
                buff += pair.Key + "=" + pair.Value + "&";
            }
            buff = buff.Trim('&');
            return buff;
        }
    }
}