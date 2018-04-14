using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace XCCloudService.Test
{
    public partial class api : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string interfaceUrl = "https://mp.4000051530.com/WeiXin/Index.aspx";
            //string url = string.Format("https://open.weixin.qq.com/connect/qrconnect?appid={0}&redirect_uri={1}&response_type=code&scope=snsapi_login&state={2}#wechat_redirect",
            //    "wx86275e2035a8089d", HttpUtility.UrlEncode(interfaceUrl),"test123");
            string url = string.Format("https://open.weixin.qq.com/connect/oauth2/authorize?appid=wx86275e2035a8089d&redirect_uri={0}&response_type=code&scope=snsapi_userinfo&state=123#wechat_redirect", HttpUtility.UrlEncode(interfaceUrl));
        }
    }
}