using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
using System.Net;
using System.IO;
using System.Text;
using System.Threading;
using System.Reflection;
using System.Web.Script.Serialization;

namespace XCCloudService.PayChannel.Common
{
    /// <summary>
    ///PayAPI 的摘要说明
    /// </summary>
    public class LCSWPayAPI
    {
        const string SSLCERT_PATH = "";
        const string SSLCERT_PASSWORD = "";
        const string Ver = "100";
        const string NotifyURL = "http://123.56.111.145:789/LCSWPayCallBack.aspx";
        const string GatewayURL = "http://pay.lcsw.cn/lcsw/";

        public LCSWPayAPI()
        {

        }

        bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
        {
            //直接确认，否则打不开    
            return true;

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
                byte[] data = System.Text.Encoding.UTF8.GetBytes(str);
                request.ContentLength = data.Length;

                //是否使用证书
                if (isUseCert)
                {
                    string path = HttpContext.Current.Request.PhysicalApplicationPath;
                    X509Certificate2 cert = new X509Certificate2(path + SSLCERT_PATH, SSLCERT_PASSWORD);
                    request.ClientCertificates.Add(cert);
                }

                //往服务器写入数据
                reqStream = request.GetRequestStream();
                reqStream.Write(data, 0, data.Length);
                reqStream.Close();

                //获取服务端返回
                response = (HttpWebResponse)request.GetResponse();

                //获取服务端返回数据
                StreamReader sr = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
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
        /// 请求扫码支付条码
        /// </summary>
        /// <param name="order">扫码支付对象</param>
        /// <returns>二维码字符串</returns>
        public bool OrderPay(LCSWPayDate.OrderPay order, string token, out string qrCode)
        {
            qrCode = "";
            order.pay_ver = Ver;
            order.terminal_time = DateTime.Now.ToString("yyyyMMddHHmmss");
            order.terminal_trace = Guid.NewGuid().ToString().Replace("-", "");
            order.service_id = "011";
            order.notify_url = NotifyURL;
            order.key_sign = order.GetSign(token);

            JavaScriptSerializer jsonSerialize = new JavaScriptSerializer();
            string data = jsonSerialize.Serialize(order);

            string response = Post(data, GatewayURL + "pay/100/prepay", false, 20);
            Console.WriteLine(response);

            LCSWPayDate.OrderPayACK ack = jsonSerialize.Deserialize<LCSWPayDate.OrderPayACK>(response);
            if (ack.CheckSign())
            {
                //验签成功
                if (ack.return_code == "01")
                {
                    //响应成功
                    if (ack.result_code == "01")
                    {
                        //条码请求成功
                        qrCode = ack.qr_code;
                        return true;
                    }
                }
            }

            return false;
        }
    }
}