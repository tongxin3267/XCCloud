using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace XCCloudService.Pay.PPosPay
{
    public class PPosSubmit
    {
        /// <summary>
        /// 建立请求，以表单HTML形式构造（默认）
        /// </summary>
        /// <param name="sParaTemp">请求参数数组</param>
        /// <param name="strMethod">提交方式。两个值可选：post、get</param>
        /// <param name="strButtonValue">确认按钮显示文字</param>
        /// <returns>提交表单HTML文本</returns>
        public static string BuildRequest(SortedDictionary<string, string> sParaTemp, string strMethod, string strButtonValue)
        {
            string signValue = CheckSign(sParaTemp);
            sParaTemp.Remove("signvalue");
            sParaTemp.Add("signvalue", signValue);

            //待请求参数数组
            SortedDictionary<string, string> dicPara = sParaTemp;

            StringBuilder sbHtml = new StringBuilder();

            sbHtml.Append("<form id='starpossubmit' name='starpossubmit' action='" + PPosPayConfig.Gateway_Mobile + "' method='" + strMethod.ToLower().Trim() + "'>");

            foreach (KeyValuePair<string, string> temp in dicPara)
            {
                sbHtml.Append("<input type='hidden' name='" + temp.Key + "' value='" + temp.Value + "'/>");
            }

            //submit按钮控件请不要含有name属性
            sbHtml.Append("<input type='submit' value='" + strButtonValue + "' style='display:none;'></form>");

            sbHtml.Append("<script>document.forms['starpossubmit'].submit();</script>");

            return sbHtml.ToString();
        }

        public static string CheckSign(SortedDictionary<string, string> sParaTemp)
        {
            SortedDictionary<string, string> sign = new SortedDictionary<string, string>();

            string SignKey = "";
            foreach (string key in sParaTemp.Keys)
            {
                SignKey += sParaTemp[key];
            }

            SignKey += PPosPayConfig.Token;
            SignKey = SignKey.Replace(" ", "");

            //Console.WriteLine("签名计算：" + SignKey);
            var md5 = MD5.Create();
            var bs = md5.ComputeHash(Encoding.UTF8.GetBytes(SignKey));
            var sb = new StringBuilder();
            foreach (byte b in bs)
            {
                sb.Append(b.ToString("x2"));
            }
            return sb.ToString();
        }
    }
}
