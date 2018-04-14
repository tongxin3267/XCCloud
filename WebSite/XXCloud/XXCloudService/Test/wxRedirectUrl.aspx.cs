using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using XCCloudService.PayChannel.Common;

namespace XXCloudService.Test
{
    public partial class wxRedirectUrl : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string code = Request["code"] ?? "";
            string state = Request["state"] ?? "";

            PayLogHelper.WritePayLog(code + " ==== " + state);
        }
    }
}