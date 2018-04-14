using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XCCloudService.Business
{
    public class ApiRequestType
    {
        public static string CheckImgCode = "CheckImgCode";//图形验证码请求
        public static string SendSMSCode = "SendSMSCode";//短信验证码请求
    }
}