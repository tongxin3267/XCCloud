using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using XCCloudService.Business.Common;

namespace XXCloudService.Test.WorkFlowTest
{
    public partial class StockHandle : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            ViewState["stockhandletoken"] = MobileTokenBusiness.SetMobileToken("link");
        }
    }
}