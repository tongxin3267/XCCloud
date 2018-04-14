using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using XCCloudService.Pay.PPosPay;

namespace XXCloudService.Test
{
    public partial class pposTest : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            PPosPayData.WeiXinPubQuery query = new PPosPayData.WeiXinPubQuery();
            PPosPayData.WeiXinPubQueryACK ack = new PPosPayData.WeiXinPubQueryACK();
            string error= string.Empty;

            PPosPayApi ppos = new PPosPayApi();
            bool result = ppos.WeiXinPubQuery(query, ref ack, out error);
            Response.Write(error);
        }

        protected void Button2_Click(object sender, EventArgs e)
        {
            PPosPayData.WeiXinPubPay pay = new PPosPayData.WeiXinPubPay();
            PPosPayData.WeiXinPubPayACK ack = new PPosPayData.WeiXinPubPayACK();
            string error = string.Empty;

            PPosPayApi ppos = new PPosPayApi();
            PPosPayData.WeiXinPubPayACK result = ppos.PubPay(pay, ref ack, out error);
            Response.Write(error);
        }
    }
}