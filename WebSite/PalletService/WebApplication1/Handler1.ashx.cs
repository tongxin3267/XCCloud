

using PalletService.Business.SysConfig;
using PalletService.Common;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace WebApplication1
{
    /// <summary>
    /// Handler1 的摘要说明
    /// </summary>
    public class Handler1 : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            string action = context.Request["action"] ?? "";
            switch (action)
            {
                case "userLogin": userLogin(context); break;
            }
        }

        /// <summary>
        /// 从请求流中获取JSON格式字符串
        /// </summary>
        /// <returns>JSON字符串</returns>
        private string GetJsonByRequestStream(HttpContext context)
        {
            int count = 0;
            System.IO.Stream s = context.Request.InputStream;
            byte[] buffer = new byte[1024];
            StringBuilder builder = new StringBuilder();
            while ((count = s.Read(buffer, 0, 1024)) > 0)
            {
                builder.Append(Encoding.UTF8.GetString(buffer, 0, count));
            }
            string postJson = HttpUtility.UrlDecode(builder.ToString());
            string regex = @"{(.*)}";
            Match m = Regex.Match(postJson, regex);
            if (m != null)
            {
                return m.ToString();
            }
            else
            {
                return string.Empty;
            }
        }

        private void userLogin(HttpContext context)
        {
            string url = SysConfigBusiness.XCCloudHost + "/xccloud/UserInfo?action=barLogin";
            string parasJson = GetJsonByRequestStream(context);
            string json = Utils.HttpPost(url, parasJson);
            object obj = Utils.DeserializeObject(json);
            string return_code = Utils.GetJsonObjectValue(obj, "return_code").ToString();
            string result_code = Utils.GetJsonObjectValue(obj, "result_code").ToString();
            if (return_code == "1" && result_code == "1")
            {
                object result_data = Utils.GetJsonObjectValue(obj, "result_data");
                string userToken = Utils.GetJsonObjectValue(result_data, "userToken").ToString();
                var newobj = new
                {
                    result_code = "1",
                    userToken = userToken
                };
                context.Response.Write(Utils.SerializeObject(newobj));
            }
            else
            {
                var newobj = new
                {
                    result_code = "0"
                };
                context.Response.Write(Utils.SerializeObject(newobj));
            }
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