using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using XCCloudService.BLL.CommonBLL;
using XCCloudService.Business.XCCloud;
using XCCloudService.CacheService;
using XCCloudService.CacheService.XCCloud;
using XCCloudService.Common.Enum;
using XCCloudService.Model.CustomModel.XCCloud;
using XCCloudService.Model.XCCloud;
using XCCloudService.Pay.WeiXinPay.Lib;
using XCCloudService.PayChannel.Common;

namespace XXCloudService.PayChannel
{
    public partial class WxPayCallBack : System.Web.UI.Page
    {
        public WxPayData GetNotifyData()
        {
            //接收从微信后台POST过来的数据
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

            //        string temp = @"<xml><appid><![CDATA[wx359e3843fe4c20e6]]></appid>
            //<bank_type><![CDATA[ABC_DEBIT]]></bank_type>
            //<cash_fee><![CDATA[1]]></cash_fee>
            //<device_info><![CDATA[100010_SC-201609020843]]></device_info>
            //<fee_type><![CDATA[CNY]]></fee_type>
            //<is_subscribe><![CDATA[Y]]></is_subscribe>
            //<mch_id><![CDATA[1264100601]]></mch_id>
            //<nonce_str><![CDATA[9551f570dc8b434a98c7fb5e95543664]]></nonce_str>
            //<openid><![CDATA[oaxaDv5ATDpyyTJjh03vl_x55a9I]]></openid>
            //<out_trade_no><![CDATA[03ad8b23b8554450b272b543bd5f9dbd]]></out_trade_no>
            //<result_code><![CDATA[SUCCESS]]></result_code>
            //<return_code><![CDATA[SUCCESS]]></return_code>
            //<sign><![CDATA[774AB611BAEF0D61B94AD5FD5DF3097D]]></sign>
            //<time_end><![CDATA[20170301141219]]></time_end>
            //<total_fee>1</total_fee>
            //<trade_type><![CDATA[NATIVE]]></trade_type>
            //<transaction_id><![CDATA[4009262001201703011791419767]]></transaction_id>
            //</xml>";

            //转换数据格式并验证签名
            WxPayData data = new WxPayData();
            try
            {
                PayLogHelper.WriteEvent(builder.ToString(), "微信支付");
                data.FromXml(builder.ToString());

                string resule = data.GetValue("result_code").ToString();
                if (resule == "SUCCESS")
                {
                    string out_trade_no = data.GetValue("out_trade_no").ToString();
                    decimal total_fee = Convert.ToDecimal(data.GetValue("total_fee"));
                    decimal payAmount = total_fee / 100;

                    Flw_OrderBusiness.OrderPay(out_trade_no, payAmount, SelttleType.AliWxPay);

                    #region MyRegion
                    //Flw_Order order = Flw_OrderBusiness.GetOrderModel(out_trade_no);
                    //if (order != null)
                    //{
                    //    decimal PayCount = order.PayCount != null ? (decimal)order.PayCount * 100 : 0; //应付金额
                    //    decimal FreePay = order.FreePay != null ? (decimal)order.FreePay * 100 : 0;   //减免金额

                    //    if (total_fee == PayCount - FreePay)
                    //    {
                    //        string payTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    //        string sql = string.Format("update Flw_Order set OrderStatus=2, RealPay='{0}', PayTime='{1}' where OrderID='{2}'", payAmount, payTime, out_trade_no);
                    //        XCCloudBLL.ExecuteSql(sql);

                    //        OrderPayNotify.AddOrderPayCache(out_trade_no, payAmount, payTime, 2);

                    //        sql = "SELECT SettleFee FROM Base_StoreInfo a, Base_SettleOrg b WHERE a.SettleID = b.ID AND a.StoreID = '" + order.StoreID + "'";
                    //        DataTable dt = XCCloudBLL.ExecuterSqlToTable(sql);
                    //        if (dt.Rows.Count > 0)
                    //        {
                    //            //获取当前结算费率，计算手续费
                    //            double fee = Convert.ToDouble(dt.Rows[0]["SettleFee"]);
                    //            double d = Math.Round(Convert.ToDouble(payAmount) * fee, 2, MidpointRounding.AwayFromZero);   //最小单位为0.01元
                    //            if (d < 0.01) d = 0.01;
                    //            sql = "update Flw_Order set PayFee='" + d + "' where OrderID='" + out_trade_no + "'";
                    //            XCCloudBLL.ExecuteSql(sql);
                    //        }
                    //    }
                    //    else
                    //    {
                    //        //支付异常
                    //        //PayList.AddNewItem(out_trade_no, amount);
                    //        string sql = string.Format("update Flw_Order set OrderStatus=3, RealPay='{0}', PayTime=GETDATE() where OrderID='{1}'", payAmount, out_trade_no);
                    //        XCCloudBLL.ExecuteSql(sql);
                    //    }
                    //} 
                    #endregion
                }
                //data.FromXml(temp);
            }
            catch (WxPayException ex)
            {
                //若签名错误，则立即返回结果给微信支付后台
                WxPayData res = new WxPayData();
                res.SetValue("return_code", "FAIL");
                res.SetValue("return_msg", ex.Message);
                Response.Write(res.ToXml());
                Response.End();
            }
            return data;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            WxPayData data = GetNotifyData();
            string tradeNum = data.GetValue("out_trade_no").ToString();
            //PayList.AddNewItem(tradeNum, data.GetValue("cash_fee").ToString());
        }
    }
}