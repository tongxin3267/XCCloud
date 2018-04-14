using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Reflection;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Script.Serialization;
using XCCloudService.Common;
using XCCloudService.Model.XCCloud;

namespace XCCloudService.Pay.PPosPay
{
    public class PPosPayApi
    {
        bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
        {
            //直接确认，否则打不开    
            return true;
        }

        string CheckSign(object o)
        {
            SortedDictionary<string, string> sign = new SortedDictionary<string, string>();
            Type t = o.GetType();
            foreach (PropertyInfo pi in t.GetProperties())
            {
                if (pi.GetValue(o, null) != null)
                {
                    string v = pi.GetValue(o, null).ToString();
                    if (v != "")
                    {
                        sign.Add(pi.Name, v);
                    }
                }
            }

            string SignKey = "";
            foreach (string key in sign.Keys)
            {
                SignKey += sign[key];
            }

            SignKey += PPosPayConfig.Token;
            SignKey = SignKey.Replace(" ", "");

            Console.WriteLine("签名计算：" + SignKey);
            var md5 = MD5.Create();
            var bs = md5.ComputeHash(Encoding.UTF8.GetBytes(SignKey));
            var sb = new StringBuilder();
            foreach (byte b in bs)
            {
                sb.Append(b.ToString("x2"));
            }
            return sb.ToString();
        }

        string Post(string str, string url, bool isUseCert, int timeout)
        {
            System.GC.Collect();//垃圾回收，回收没有正常关闭的http连接

            string result = "";//返回结果

            HttpWebRequest request = null;
            HttpWebResponse response = null;
            Stream reqStream = null;

            try
            {
                //设置最大连接数
                ServicePointManager.DefaultConnectionLimit = 200;
                //设置https验证方式
                if (url.StartsWith("https", StringComparison.OrdinalIgnoreCase))
                {
                    ServicePointManager.ServerCertificateValidationCallback =
                            new RemoteCertificateValidationCallback(CheckValidationResult);
                }

                /***************************************************************
                * 下面设置HttpWebRequest的相关属性
                * ************************************************************/
                request = (HttpWebRequest)WebRequest.Create(url);

                request.Method = "POST";
                request.Timeout = timeout * 1000;

                //设置代理服务器
                //WebProxy proxy = new WebProxy();                          //定义一个网关对象
                //proxy.Address = new Uri(WxPayConfig.PROXY_URL);              //网关服务器端口:端口
                //request.Proxy = proxy;

                //设置POST的数据类型和长度
                request.ContentType = "application/json";
                byte[] data = System.Text.Encoding.GetEncoding("GBK").GetBytes(str);
                request.ContentLength = data.Length;

                //是否使用证书
                if (isUseCert)
                {
                    string path = HttpContext.Current.Request.PhysicalApplicationPath;
                    X509Certificate2 cert = new X509Certificate2(path + PPosPayConfig.SSLCERT_PATH, PPosPayConfig.SSLCERT_PASSWORD);
                    request.ClientCertificates.Add(cert);
                }

                //往服务器写入数据
                reqStream = request.GetRequestStream();
                reqStream.Write(data, 0, data.Length);
                reqStream.Close();

                //获取服务端返回
                response = (HttpWebResponse)request.GetResponse();

                //获取服务端返回数据
                StreamReader sr = new StreamReader(response.GetResponseStream(), Encoding.GetEncoding("GBK"));
                result = sr.ReadToEnd().Trim();
                sr.Close();
            }
            catch (System.Threading.ThreadAbortException e)
            {
                Thread.ResetAbort();
            }
            catch (WebException e)
            {
                Console.WriteLine(e);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            finally
            {
                //关闭连接和流
                if (response != null)
                {
                    response.Close();
                }
                if (request != null)
                {
                    request.Abort();
                }
            }
            return result;
        }

        string GetObjectJSON(object o)
        {
            string s = "";
            Type t = o.GetType();
            s += "{";
            foreach (PropertyInfo pi in t.GetProperties())
            {
                s += string.Format("\"{0}\":\"{1}\",", pi.Name, pi.GetValue(o, null));
            }
            s = s.Substring(0, s.Length - 1);
            s += "}";
            return s;
        }

        /// <summary>
        /// 扫码付请求条码
        /// </summary>
        /// <returns></returns>
        public PPosPayData.OrderPayACK OrderPay(PPosPayData.OrderPay order, out string error)
        {
            error = "";
            order.characterSet = PPosPayConfig.Character;
            order.mercId = PPosPayConfig.MerchNo;
            order.orgNo = PPosPayConfig.InstNo;
            order.signType = PPosPayConfig.SignType;
            order.signValue = "";
            order.trmNo = PPosPayConfig.TerminalNo;
            order.version = PPosPayConfig.Version;
            order.opSys = Convert.ToInt32(OSType.WINDOWS).ToString();
            order.signValue = CheckSign(order);

            JavaScriptSerializer jsonSerialize = new JavaScriptSerializer();
            string data = jsonSerialize.Serialize(order);

            string response = Post(data, PPosPayConfig.GatewayURL + "sdkBarcodePosPay.json", false, 20);
            Console.WriteLine(response);

            PPosPayData.OrderPayACK ack = jsonSerialize.Deserialize<PPosPayData.OrderPayACK>(response);
            if (ack.returnCode != "000000")
            {
                error = System.Web.HttpUtility.UrlDecode(ack.message, Encoding.UTF8);
                return null;
            }
            else
            {
                //return ack.payCode;
                return ack;
            }
        }

        public PPosPayData.MicroPayACK ScanPay(PPosPayData.MicroPay order, out string error)
        {
            error = "";
            order.characterSet = PPosPayConfig.Character;
            order.mercId = PPosPayConfig.MerchNo;
            order.orgNo = PPosPayConfig.InstNo;
            order.signType = PPosPayConfig.SignType;
            order.trmNo = PPosPayConfig.TerminalNo;
            order.version = PPosPayConfig.Version;
            order.opSys = Convert.ToInt32(OSType.WINDOWS).ToString();
            order.signValue = CheckSign(order);

            JavaScriptSerializer jsonSerialize = new JavaScriptSerializer();
            string data = jsonSerialize.Serialize(order);
            string response = Post(data, PPosPayConfig.GatewayURL + "sdkBarcodePay.json", false, 20);

            PPosPayData.MicroPayACK ack = jsonSerialize.Deserialize<PPosPayData.MicroPayACK>(response);
            if (ack.returnCode != "000000")
            {
                error = System.Web.HttpUtility.UrlDecode(ack.message, Encoding.UTF8);
                return null;
            }
            else
            {
                return ack;
            }
        }

        public PPosPayData.WeiXinPubPayACK PubPay(PPosPayData.WeiXinPubPay pay, ref PPosPayData.WeiXinPubPayACK ack, out string errMsg)
        {
            errMsg = string.Empty;
            pay.orgNo = PPosPayConfig.InstNo;
            pay.mercId = PPosPayConfig.MerchNo;
            pay.trmNo = PPosPayConfig.TerminalNo;
            pay.txnTime = System.DateTime.Now.ToString("yyyyMMddHHmmss");
            pay.version = PPosPayConfig.Version;

            //pay.code = "011aBksI0mjBXi2ZDIqI0uVfsI0aBks4";//在授权回调页面中获取到的授权code
            
            //pay.amount = "1";//实际付款
            //pay.total_amount = "1";//订单总金额
            //pay.subject = "cesgu";
            //pay.selOrderNo = System.DateTime.Now.ToString("yyyyMMddHHmmss");
            //pay.goods_tag = "ceshi";

            JavaScriptSerializer jsonSerialize = new JavaScriptSerializer();
            string data = jsonSerialize.Serialize(pay);
            string response = Post(data, PPosPayConfig.GatewayURL + "pubSigPay.json", false, 20);
            ack = jsonSerialize.Deserialize<PPosPayData.WeiXinPubPayACK>(response);
            if (ack.returnCode != "000000")
            {
                errMsg = HttpUtility.UrlDecode(ack.message);
                //return null;
            }
            return ack;
        }

        /// <summary>
        /// 微信公众号查询
        /// </summary>
        /// <returns></returns>
        public bool WeiXinPubQuery(PPosPayData.WeiXinPubQuery query, ref PPosPayData.WeiXinPubQueryACK ack, out string errMsg)
        {
            errMsg = "";
            query.orgNo = PPosPayConfig.InstNo;
            query.mercId = PPosPayConfig.MerchNo;
            query.trmNo = PPosPayConfig.TerminalNo;
            query.txnTime = DateTime.Now.ToString("yyyyMMddHHmmss");
            query.signType = PPosPayConfig.SignType;            
            query.version = PPosPayConfig.Version;
            query.signValue = CheckSign(query);

            JavaScriptSerializer jsonSerialize = new JavaScriptSerializer();
            string data = jsonSerialize.Serialize(query);

            string response = Post(data, PPosPayConfig.GatewayURL + "pubSigQry.json", false, 20);
            ack = jsonSerialize.Deserialize<PPosPayData.WeiXinPubQueryACK>(response);
            if (ack.returnCode != "000000")
            {
                errMsg = HttpUtility.UrlDecode(ack.message, Encoding.UTF8);
                return false;
            }
            errMsg = response;
            return true;
        }
    }
}
