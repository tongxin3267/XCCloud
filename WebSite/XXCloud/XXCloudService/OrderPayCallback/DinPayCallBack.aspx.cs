using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using XCCloudService.Business.XCCloud;
using XCCloudService.Common.Enum;
using XCCloudService.Pay.DinPay;

namespace XXCloudService.OrderPayCallback
{
    public partial class DinPayCallBack : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                SortedDictionary<string, string> arrParams = GetRequestPost();
                string sign_type = arrParams.FirstOrDefault(t => t.Key == "sign_type").Value;
                string dinpaysign = arrParams.FirstOrDefault(t => t.Key == "sign").Value;

                arrParams.Remove("sign_type");
                arrParams.Remove("sign");

                string signStr = DinPayHttpHelp.GetSignString(arrParams);

                if (sign_type == "RSA-S") //RSA-S的验签方法
                {
                    //使用智付公钥对返回的数据验签
                    string dinpayPubKey = DinPayConfig.MerPubKey;

                    //将智付公钥转换成C#专用格式
                    dinpayPubKey = DinPayHttpHelp.RSAPublicKeyJava2DotNet(dinpayPubKey);
                    //验签
                    bool result = DinPayHttpHelp.ValidateRsaSign(signStr, dinpayPubKey, dinpaysign);
                    if (result == true)
                    {
                        //如果验签结果为true，则对订单进行更新
                        string out_trade_no = arrParams.FirstOrDefault(t => t.Key == "order_no").Value;
                        string order_amount = arrParams.FirstOrDefault(t => t.Key == "order_amount").Value;
                        decimal amount = Convert.ToDecimal(order_amount);

                        Flw_OrderBusiness.OrderPay(out_trade_no, amount, SelttleType.DinPay);

                        //订单更新完之后打印SUCCESS
                        Response.Write("SUCCESS");
                    }
                    else
                    {
                        //验签失败
                        Response.Write("验签失败");
                    }

                }
            }
            catch
            { }
        }

        /// <summary>
        /// 获取POST过来通知消息，并以“参数名=参数值”的形式组成数组
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