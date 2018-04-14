using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Xml.XPath;
using System.Xml.Linq;

namespace XCCloudService.Pay.DinPay
{
    public class DinPayApi
    {
        #region 获取支付二维码
        public string GetQRcode(DinPayData.ScanPay order, out string error)
        {
            error = "";
            order.interface_version = DinPayConfig.InterfaceVersion;
            order.merchant_code = DinPayConfig.MerchantId;
            order.notify_url = DinPayConfig.Notify_url;
            order.client_ip = "222.42.98.187";//客户端IP必填

            string signStr = DinPayHttpHelp.CreateLinkStringUrlencode(order);

            string sign_type = DinPayConfig.SignType;
            //商家私钥
            string merPriKey = DinPayConfig.MerPriKey;
            //私钥转换成C#专用私钥
            merPriKey = DinPayHttpHelp.RSAPrivateKeyJava2DotNet(merPriKey);
            //签名
            string signData = DinPayHttpHelp.RSASign(signStr, merPriKey);
            //将signData进行UrlEncode编码
            signData = HttpUtility.UrlEncode(signData);
            //组装字符串
            string para = signStr + "&sign_type=" + sign_type + "&sign=" + signData;
            //将字符串发送到Dinpay网关
            string _xml = DinPayHttpHelp.HttpPost(DinPayConfig.GatewayURL + "scanpay", para);
            error = _xml;
            //将同步返回的xml中的参数提取出来
            var el = XElement.Load(new StringReader(_xml));
            //将QRcode从XML中提取出来
            var qrcode1 = el.XPathSelectElement("/response/qrcode");
            if (qrcode1 == null)
            {
                error = el.XPathSelectElement("/response/resp_desc").Value;
                return "";
            }
            //去掉首尾的标签并转换成string
            string qrcode = Regex.Match(qrcode1.ToString(), "(?<=>).*?(?=<)").Value;   //二维码链接
            qrcode = HttpUtility.UrlDecode(qrcode);
            return qrcode;
        }
        #endregion

        #region 条码支付
        public string MicroPay(DinPayData.MicroPay order)
        {
            order.input_charset = DinPayConfig.InputCharset;
            order.interface_version = "V3.0"; ;
            order.merchant_code = DinPayConfig.MerchantId;
            order.notify_url = DinPayConfig.Notify_url;
            order.client_ip = "222.42.98.187";//客户端IP必填

            string signStr = DinPayHttpHelp.CreateLinkStringUrlencode(order);

            string sign_type = DinPayConfig.SignType;

            //商家私钥
            string merchant_private_Key = DinPayConfig.MerPriKey;
            //私钥转换成C#专用私钥
            merchant_private_Key = DinPayHttpHelp.RSAPrivateKeyJava2DotNet(merchant_private_Key);
            //签名
            string signData = DinPayHttpHelp.RSASign(signStr, merchant_private_Key);
            //将signData进行UrlEncode编码
            signData = HttpUtility.UrlEncode(signData);
            //组装字符串
            string para = signStr + "&sign_type=" + sign_type + "&sign=" + signData;

            //用HttpPost方式提交
            string _xml = DinPayHttpHelp.HttpPost(DinPayConfig.GatewayURL + "micropay", para);
            return _xml;
        } 
        #endregion

        #region H5支付
        public string WebPagePay(DinPayData.WxCommonPay order, string payType)
        {
            order.input_charset = DinPayConfig.InputCharset;
            order.interface_version = "V3.0";
            order.merchant_code = DinPayConfig.MerchantId;
            order.notify_url = DinPayConfig.Notify_url;
            order.sign = "";
            order.sign_type = "";

            string gatewayURL = "";

            if (payType == "1")
            {
                order.service_type = "alipay_h5";
                gatewayURL = DinPayConfig.AliH5URL;
            }
            else
            {
                order.service_type = "wxpub_pay";
                gatewayURL = DinPayConfig.WxPubURL;
            }
            string signStr = DinPayHttpHelp.CreateLinkStringUrlencode(order);

            //商家私钥
            string merchant_private_key = DinPayConfig.MerPriKey;
            //私钥转换成C#专用私钥
            merchant_private_key = DinPayHttpHelp.RSAPrivateKeyJava2DotNet(merchant_private_key);
            //签名
            string signData = DinPayHttpHelp.RSASign(signStr, merchant_private_key);

            order.sign_type = "RSA-S";
            order.sign = signData;

            SortedDictionary<string, string> dic = DinPayHttpHelp.GetDataArray(order);

            //构造请求
            string sHtmlText = DinPayHttpHelp.BuildRequest(dic, "post", "确认", gatewayURL);
            return sHtmlText;
        }
        #endregion

        #region 转账
        /// <summary>
        /// 转账
        /// </summary>
        /// <param name="order"></param>
        /// <param name="payType"></param>
        /// <returns></returns>
        public string TransferPay(DinPayData.TransferData order)
        {
            order.interface_version = "V3.1.0";
            order.merchant_no = DinPayConfig.MerchantId;
            order.tran_code = "DMTI";

            string signStr = DinPayHttpHelp.CreateLinkStringUrlencode(order);

            string sign_type = DinPayConfig.SignType;

            //商家私钥
            string merchant_private_key = DinPayConfig.MerPriKey;
            //私钥转换成C#专用私钥
            merchant_private_key = DinPayHttpHelp.RSAPrivateKeyJava2DotNet(merchant_private_key);
            //签名
            string signData = DinPayHttpHelp.RSASign(signStr, merchant_private_key);
            //将signData进行UrlEncode编码
            signData = HttpUtility.UrlEncode(signData);
            //组装字符串
            string para = signStr + "&sign_type=" + sign_type + "&sign_info=" + signData;
            //将字符串发送到Dinpay网关
            string _xml = DinPayHttpHelp.HttpPost("https://transfer.dinpay.com/transfer", para);
            _xml = HttpUtility.HtmlEncode(_xml);
            return _xml;
        }
        #endregion
    }
}
