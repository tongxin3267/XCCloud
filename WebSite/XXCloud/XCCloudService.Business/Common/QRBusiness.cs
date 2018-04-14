using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace XCCloudService.Business.Common
{
    public class QRBusiness
    {
        public static bool GetRequestUrl(string url,string mailUrl,out string rewriteUrl)
        {
            rewriteUrl = string.Empty;
            string requestQRType = string.Empty;
            string[] requestParas = null;
            if (!GetRequestQRType(url, mailUrl, out requestQRType, out requestParas))
            {
                return false;
            }

            StringBuilder sb = new StringBuilder();
            sb.Append(string.Format("{0}api/common/qr.ashx?action=getQR&type={1}", mailUrl.Replace("qr/",""),requestQRType));
            for (int i = 0; i < requestParas.Length; i++)
            {
                sb.Append(string.Format("&para{0}={1}", (i + 1), requestParas[i]));
            }
            rewriteUrl = sb.ToString();
            return true;
        }

        /// <summary>
        /// 获取请求的QR类型
        /// </summary>
        /// <param name="requestQRType"></param>
        /// <param name="paras"></param>
        /// <returns></returns>
        private static bool GetRequestQRType(string url,string mailUrl, out string requestQRType,out string[] requestParas)
        {
            requestQRType = string.Empty;
            requestParas = null;
            var parasUrl = url.Replace(mailUrl,"");
            string[] strArr = parasUrl.Split('/');
            requestQRType = strArr[0];
            requestParas = new string[strArr.Length - 1];
            for(int i = 0;i < requestParas.Length;i++)
            {
                requestParas[i] = strArr[i + 1];
            }
            return true;
        }
    }
}
