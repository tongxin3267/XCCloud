using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;
using XCCloudService.BLL.CommonBLL;
using XCCloudService.Business.XCCloud;
using XCCloudService.CacheService;
using XCCloudService.CacheService.XCCloud;
using XCCloudService.Common.Enum;
using XCCloudService.Model.CustomModel.XCCloud;
using XCCloudService.Model.XCCloud;
using XCCloudService.PayChannel.Common;

namespace XXCloudService.PayChannel
{
    public partial class POSPayCallBack : System.Web.UI.Page
    {
        class PosStarCallback
        {
            public string logNo { get; set; }
            public string TxnLogId { get; set; }
            public string BusinessId { get; set; }
            public string TxnCode { get; set; }
            public string TxnStatus { get; set; }
            public string TxnAmt { get; set; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            System.IO.Stream s = Request.InputStream;
            int count = 0;
            byte[] buffer = new byte[1024];
            StringBuilder builder = new StringBuilder();
            while ((count = s.Read(buffer, 0, 1024)) > 0)
            {
                builder.Append(Encoding.GetEncoding("GBK").GetString(buffer, 0, count));
            }
            s.Flush();
            s.Close();
            s.Dispose();

            PayLogHelper.WriteEvent(builder.ToString(), "新大陆支付");

            JavaScriptSerializer jsonSerialize = new JavaScriptSerializer();
            PosStarCallback callback = jsonSerialize.Deserialize<PosStarCallback>(builder.ToString());

            if (callback != null)
            {
                string out_trade_no = callback.TxnLogId;
                decimal total_fee = Convert.ToDecimal(callback.TxnAmt);
                decimal payAmount = total_fee / 100;

                Flw_OrderBusiness.OrderPay(out_trade_no, payAmount, SelttleType.StarPos);

                #region MyRegion
//                Flw_Order order = Flw_OrderBusiness.GetOrderModel(out_trade_no);
//                if (order != null)
//                {
//                    decimal PayCount = order.PayCount != null ? (decimal)order.PayCount * 100 : 0; //应付金额
//                    decimal FreePay = order.FreePay != null ? (decimal)order.FreePay * 100 : 0;   //减免金额

//                    if (total_fee == PayCount - FreePay)
//                    {
//                        string payTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
//                        string sql = string.Format("update Flw_Order set OrderStatus=2, RealPay='{0}', PayTime='{1}' where OrderID='{2}'", payAmount, payTime, out_trade_no);
//                        XCCloudBLL.ExecuteSql(sql);

//                        OrderPayNotify.AddOrderPayCache(out_trade_no, payAmount, payTime, 2);

//                        sql = @"SELECT a.StoreID, SettleFee FROM Base_StoreInfo a
//                                INNER JOIN Flw_Order b ON b.StoreID = a.StoreID
//                                INNER JOIN Base_SettlePPOS c ON c.ID = a.SettleID
//                                WHERE b.OrderID = '" + out_trade_no + "'";

//                        DataTable dt = XCCloudBLL.ExecuterSqlToTable(sql);
//                        if (dt.Rows.Count > 0)
//                        {
//                            //获取当前结算费率，计算手续费
//                            double fee = Convert.ToDouble(dt.Rows[0]["SettleFee"]);
//                            double d = Math.Round(Convert.ToDouble(payAmount) * fee, 2, MidpointRounding.AwayFromZero);   //最小单位为0.01元
//                            if (d < 0.01) d = 0.01;
//                            sql = "update Flw_Order set PayFee='" + d + "' where OrderID='" + out_trade_no + "'";
//                            XCCloudBLL.ExecuteSql(sql);
//                        }
//                    }
//                    else
//                    {
//                        //支付异常
//                        //PayList.AddNewItem(out_trade_no, amount);
//                        string sql = string.Format("update Flw_Order set OrderStatus=3, RealPay='{0}', PayTime=GETDATE() where OrderID='{1}'", payAmount, out_trade_no);
//                        XCCloudBLL.ExecuteSql(sql);
//                    }
//                } 
                #endregion
            }

            Response.ContentType = "application/json";
            Response.HeaderEncoding = Encoding.GetEncoding("GBK");
            //string blank = "";
            //blank = blank.PadLeft(64, ' ');
            //string responseWrite=
            string r = "{\"RspCode\":\"000000\",\"RspDes\":\"\"}";
            //Response.Write(string.Format("{\"RspCode\":\"000000\",\"RspDes\":\"{0}\"}", blank));
            Response.Write(r);
        }
    }
}