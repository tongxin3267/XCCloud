using Fleck;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using XCCloudService.CacheService;
using XCCloudService.Business.Common;
using XCCloudService.Common;
using XCCloudService.Business.XCGame;
using XCCloudService.Utility;
using XXCloudService.Utility;

namespace XCCloudService
{
    public class Global : System.Web.HttpApplication
    {
        void Application_Start(object sender, EventArgs e)
        {
            ApplicationStart.Init();
            UDPServer.Init();
        }

        void Application_End(object sender, EventArgs e)
        {
            //  在应用程序关闭时运行的代码
            ApplicationEnd.End();
        }

        void Application_Error(object sender, EventArgs e)
        {
            // 在出现未处理的错误时运行的代码
            Exception ex = Server.GetLastError().GetBaseException();
            Server.ClearError();
        }

        void Session_Start(object sender, EventArgs e)
        {
            // 在新会话启动时运行的代码

        }

        void Session_End(object sender, EventArgs e)
        {
            // 在会话结束时运行的代码。 
            // 注意: 只有在 Web.config 文件中的 sessionstate 模式设置为
            // InProc 时，才会引发 Session_End 事件。如果会话模式设置为 StateServer 
            // 或 SQLServer，则不会引发该事件。
            
        }


        protected void Application_BeginRequest(Object sender, EventArgs e)
        {
            string reWriteUrl = string.Empty;
            if (RequestRouteBusiness.IsRewrite(Request.Url.ToString(), Request.Url.Scheme,Request.Url.Authority, out reWriteUrl))
            {
                HttpContext.Current.RewritePath(reWriteUrl);
            } 
        }
    }
}
