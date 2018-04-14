using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using XCCloudService.Base;
using XCCloudService.Business.Common;
using XCCloudService.Business.WeiXin;
using XCCloudService.CacheService;
using XCCloudService.Common;
using XCCloudService.Common.Enum;
using XCCloudService.Model.WeiXin.Session;
using XCCloudService.Pay;
using XCCloudService.Pay.WeiXinPay.Business;
using XCCloudService.Pay.WeiXinPay.Lib;
using XCCloudService.WeiXin.Common;
using XCCloudService.WeiXin.WeixinOAuth;

namespace XCCloudService.Pay
{
    /// <summary>
    /// NumberOfPublic 的摘要说明
    /// </summary>
    public class WeiXinPayment : ApiBase
    {
        /// <summary>
        /// 获取小程序用户Session信息
        /// </summary>
        /// <returns></returns>
        [ApiMethodAttribute(SignKeyEnum = SignKeyEnum.MobileToken)]
        public object getWXRepareId(Dictionary<string, object> dicParas)
        {
            try
            {
                int coins = 0;
                string orderNo = string.Empty;
                string errMsg = string.Empty;
                string mobileToken = dicParas.ContainsKey("mobileToken") ? dicParas["mobileToken"].ToString() : string.Empty;
                string storeId = dicParas.ContainsKey("storeId") ? dicParas["storeId"].ToString() : string.Empty;
                string productName = dicParas.ContainsKey("productName") ? dicParas["productName"].ToString() : string.Empty;
                string payPriceStr = dicParas.ContainsKey("payPrice") ? dicParas["payPrice"].ToString() : string.Empty;
                string buyType = dicParas.ContainsKey("buyType") ? dicParas["buyType"].ToString() : string.Empty;
                string coinsStr = dicParas.ContainsKey("coins") ? dicParas["coins"].ToString() : string.Empty;
                string serverSessionKey = dicParas.ContainsKey("serverSessionKey") ? dicParas["serverSessionKey"].ToString() : string.Empty;

                decimal payPrice = 0;
                if (!decimal.TryParse(payPriceStr,out payPrice))
                {
                    return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "支付金额不正确");
                }

                if (!int.TryParse(coinsStr, out coins))
                {
                    return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "购买币数不正确");
                }
                MobileTokenModel mobileTokenModel = (MobileTokenModel)(dicParas[Constant.MobileTokenModel]);

                //验证用户session,获取openId
                if (!WeiXinSAppSessionBussiness.Exist(serverSessionKey))
                {
                    return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "用户Session失效");
                }

                WeiXinSAppSessionModel sessionModel = null;
                WeiXinSAppSessionBussiness.GetSession(serverSessionKey, ref sessionModel);

                //生成服务器订单号
                orderNo = PayOrderHelper.CreateXCGameOrderNo(storeId, payPrice, 0, (int)(OrderType.WeiXin), productName, mobileTokenModel.Mobile, buyType, coins);

                JsApiPay jsApiPay = new JsApiPay();
                jsApiPay.openid = sessionModel.OpenId;
                jsApiPay.total_fee = (int)(payPrice * 100);
                jsApiPay.body = productName;
                jsApiPay.out_trade_no = orderNo;
                jsApiPay.device_info = storeId;


                //微信下单接口
                WxPayData wxPayData = null;
                try
                {
                    wxPayData = jsApiPay.GetUnifiedOrderResult();
                }
                catch(Exception e)
                {
                    LogHelper.SaveLog(TxtLogType.WeiXinPay, TxtLogContentType.Exception, TxtLogFileType.Day, e.Message);
                    return ResponseModelFactory.CreateAnonymousModelByFail(isSignKeyReturn, "小程序下单失败");
                }
                
                string result_code = wxPayData.GetValue("result_code") != null ? wxPayData.GetValue("result_code").ToString(): "";
                string return_code = wxPayData.GetValue("return_code") != null ? wxPayData.GetValue("return_code").ToString() : "";

                //根据微信下单结果判断
                if (result_code.Equals("SUCCESS") && return_code.Equals("SUCCESS"))
                {
                    //返回微信预处理订单信息
                    string prepay_id = wxPayData.GetValue("prepay_id").ToString();
                    string timeStamp = Utils.ConvertDateTimeToLong(System.DateTime.Now,1).ToString();
                    string nonceStr = System.Guid.NewGuid().ToString("N");
                
                    var dataObj = new
                    {
                        prepay_id = prepay_id,
                        timeStamp = timeStamp,
                        nonceStr = nonceStr,
                        signType = "MD5",
                        paySign = WeiXinPaySignHelper.GetSAppPaySignKey(nonceStr, prepay_id,"MD5",timeStamp),
                        orderNo = orderNo
                    };

                    return ResponseModelFactory.CreateAnonymousSuccessModel(isSignKeyReturn, dataObj);
                }
                else
                {
                    return ResponseModelFactory.CreateAnonymousModelByFail(isSignKeyReturn, "微信端响应出错");
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }

        }
    }
}