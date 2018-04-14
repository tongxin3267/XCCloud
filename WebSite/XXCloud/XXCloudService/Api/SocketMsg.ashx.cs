using Fleck;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using XCCloudService.Common;

namespace XCCloudService.Api
{
    /// <summary>
    /// SocketMsg 的摘要说明
    /// </summary>
    public class SocketMsg : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            string str = "fewfewli测试数据";
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(str);
            //XCCloudWebSocketService.Server.Send("192.168.1.110", 12882, bytes);
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}