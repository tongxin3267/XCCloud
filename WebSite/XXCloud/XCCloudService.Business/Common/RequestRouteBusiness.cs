using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XCCloudService.Business.Common;

namespace XCCloudService.Common
{
    public class RequestRouteBusiness
    {
        public static bool IsRewrite(string requestUrl, string scheme,string authority,out string rewriteUrl)
        {
            string hostUrl = string.Format("{0}://{1}", scheme, authority);
            rewriteUrl = string.Empty;
            bool isRewrite = false;
            string url = requestUrl;
            string noRewriteUrl = string.Format("{0}://{1}/{2}/", scheme, authority, "api");

            if (url.IndexOf(noRewriteUrl) < 0)
            {
                if (url.LastIndexOf("?action=") >= 0)
                {
                    if (url.ToLower().IndexOf(string.Format("{0}/{1}/", hostUrl, "wx")) >= 0)
                    {
                        url = url.Replace(string.Format("{0}/{1}/", hostUrl, "wx"), string.Format("{0}/{1}/", hostUrl, "weixin/api"));
                        url = url.Insert(url.LastIndexOf("?action="), ".ashx");
                        isRewrite = true;
                        rewriteUrl = url.Replace(hostUrl, "");
                    }
                    else if (url.ToLower().IndexOf(string.Format("{0}/{1}/", hostUrl, "pay")) >= 0)
                    {
                        url = url.Insert(url.LastIndexOf("?action="), ".ashx");
                        isRewrite = true;
                        rewriteUrl = url.Replace(hostUrl, "");
                    }
                    else
                    {
                        url = url.Replace(string.Format("{0}/", hostUrl), string.Format("{0}/{1}/", hostUrl, "api"));
                        url = url.Insert(url.LastIndexOf("?action="), ".ashx");
                        isRewrite = true;
                        rewriteUrl = url.Replace(hostUrl, "");
                    }
                }
                else if (url.ToLower().IndexOf(string.Format("{0}://{1}/{2}/", scheme, authority, "qr")) >= 0)
                {
                    //请求qr码图
                    url = url.ToLower();
                    string mailUrl = string.Format("{0}://{1}/{2}/", scheme, authority, "qr");
                    isRewrite = QRBusiness.GetRequestUrl(url, mailUrl, out rewriteUrl);
                    rewriteUrl = rewriteUrl.Replace(hostUrl, "");
                }
            }
            return isRewrite;
        }
    }
}
