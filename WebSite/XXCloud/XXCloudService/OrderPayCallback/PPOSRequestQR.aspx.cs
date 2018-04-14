using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using XCCloudService.PayChannel.Common;

namespace XXCloudService.PayChannel
{
    public partial class PPOSRequestQR : System.Web.UI.Page
    {
        /// <summary>
        /// 通用扫码请求接口
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            string order_id = Request.QueryString["OrderID"];           //订单编号
            string store_id = Request.QueryString["StoreID"];           //商户编号
            string price = Request.QueryString["Price"];                //支付金额单位为 分
            string workstation = Request.QueryString["WorkStation"];    //支付工作站
            string order_type = Request.QueryString["OrderType"];       //支付方式 01 微信 02 支付宝
            string descript = Request.QueryString["Descript"];          //支付标题，展示给客户
            string timestamp = Request.QueryString["Stamp"];            //时间戳

            //DataAccess ac = new DataAccess();
            string sql = "";
        }
    }
}