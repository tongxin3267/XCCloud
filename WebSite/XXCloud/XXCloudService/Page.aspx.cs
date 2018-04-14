using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace XXCloudService
{
    public partial class Page : System.Web.UI.Page
    {
        public string ID = "";
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Request.QueryString["ID"] == null)
                {
                    return;
                }
                 ID = Request.QueryString["ID"];
            }
        }
    }
}