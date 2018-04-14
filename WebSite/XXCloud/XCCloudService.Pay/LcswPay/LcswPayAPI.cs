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

namespace XCCloudService.Pay.LcswPay
{
    public class LcswPayAPI
    {
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
                    X509Certificate2 cert = new X509Certificate2(path + LcswPayConfig.SSLCERT_PATH, LcswPayConfig.SSLCERT_PASSWORD);
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
        string CheckSign(object o)
        {
            string s = "";
            Type t = o.GetType();
            foreach (PropertyInfo pi in t.GetProperties())
            {
                if (pi.GetValue(o, null) != null)
                {
                    string v = pi.GetValue(o, null).ToString();
                    if (pi.Name != "key_sign")
                    {
                        if (v != "")
                            s += string.Format("{0}={1}&", pi.Name, v);
                    }
                }
            }
            s += "access_token=" + LcswPayConfig.Token;

            var md5 = MD5.Create();
            var bs = md5.ComputeHash(Encoding.UTF8.GetBytes(s));
            var sb = new StringBuilder();
            foreach (byte b in bs)
            {
                sb.Append(b.ToString("x2"));
            }
            return sb.ToString().ToLower();
        }
        /// <summary>
        /// 请求查询账户余额
        /// </summary>
        /// <param name="Unsettle">未结算金额</param>
        /// <param name="Settled">已结算金额</param>
        /// <returns>成功返回TRUE，失败返回FALSE</returns>
        public bool AccountQuery(out string Unsettle, out string Settled)
        {
            Unsettle = "0"; Settled = "0";
            LcswPayDate.Query q = new LcswPayDate.Query();
            q.inst_no = LcswPayConfig.inst_no;
            q.merchant_no = LcswPayConfig.StoreID;
            q.trace_no = Guid.NewGuid().ToString().Replace("-", "");
            q.key_sign = q.GetSign();

            JavaScriptSerializer jsonSerialize = new JavaScriptSerializer();
            string data = jsonSerialize.Serialize(q);

            string response = Post(data, LcswPayConfig.GatewayURL + "merchant/100/withdraw/query", false, 20);
            Console.WriteLine(response);

            LcswPayDate.QueryACK ack = jsonSerialize.Deserialize<LcswPayDate.QueryACK>(response);
            if (ack.return_code == "01")
            {
                if (ack.CheckSign())
                {
                    if (ack.result_code == "01")
                    {
                        Unsettle = ack.not_settle_amt;
                        Settled = ack.settled_amt;
                        return true;
                    }
                }
            }
            return false;
        }
        /// <summary>
        /// 请求提现手续费
        /// </summary>
        /// <param name="Money"></param>
        /// <param name="Fee"></param>
        /// <returns></returns>
        public bool QueryFee(string Money, out string Fee)
        {
            Fee = "0";
            LcswPayDate.Queryfee q = new LcswPayDate.Queryfee();
            q.inst_no = LcswPayConfig.inst_no;
            q.merchant_no = LcswPayConfig.StoreID;
            q.trace_no = Guid.NewGuid().ToString().Replace("-", "");
            q.amt = Money;
            q.key_sign = q.GetSign();

            JavaScriptSerializer jsonSerialize = new JavaScriptSerializer();
            string data = jsonSerialize.Serialize(q);

            string response = Post(data, LcswPayConfig.GatewayURL + "merchant/100/withdraw/queryfee", false, 20);
            Console.WriteLine(response);

            LcswPayDate.QueryfeeACK ack = jsonSerialize.Deserialize<LcswPayDate.QueryfeeACK>(response);
            if (ack.return_code == "01")
            {
                if (ack.CheckSign())
                {
                    if (ack.result_code == "01")
                    {
                        Fee = ack.fee_amt;
                        return true;
                    }
                }
            }
            return false;
        }
        /// <summary>
        /// 发起提现
        /// </summary>
        /// <param name="Money"></param>
        /// <param name="Fee"></param>
        /// <returns></returns>
        public bool Applay(string Money, string Fee, string apply_type)
        {
            LcswPayDate.ApplayReq q = new LcswPayDate.ApplayReq();
            q.inst_no = LcswPayConfig.inst_no;
            q.merchant_no = LcswPayConfig.StoreID;
            q.trace_no = Guid.NewGuid().ToString().Replace("-", "");
            q.amt = Money;
            q.fee_amt = Fee;
            q.apply_type = apply_type;
            q.key_sign = q.GetSign();

            JavaScriptSerializer jsonSerialize = new JavaScriptSerializer();
            string data = jsonSerialize.Serialize(q);

            string response = Post(data, LcswPayConfig.GatewayURL + "merchant/100/withdraw/apply", false, 20);
            Console.WriteLine(response);

            LcswPayDate.ApplayAck ack = jsonSerialize.Deserialize<LcswPayDate.ApplayAck>(response);
            if (ack.return_code == "01")
            {
                if (ack.CheckSign())
                {
                    if (ack.result_code == "01")
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        /// <summary>
        /// 请求扫码支付条码
        /// </summary>
        /// <param name="order">扫码支付对象</param>
        /// <returns>二维码字符串</returns>
        public bool OrderPay(LcswPayDate.OrderPay order, out string qrCode)
        {
            qrCode = "";
            order.pay_ver = LcswPayConfig.Ver;
            order.merchant_no = LcswPayConfig.StoreID;
            order.terminal_id = LcswPayConfig.DeviceID;
            order.terminal_time = DateTime.Now.ToString("yyyyMMddHHmmss");
            order.service_id = "011";
            order.notify_url = LcswPayConfig.NotifyURL;
            order.key_sign = order.GetSign();

            JavaScriptSerializer jsonSerialize = new JavaScriptSerializer();
            string data = jsonSerialize.Serialize(order);

            string response = Post(data, LcswPayConfig.GatewayURL + "pay/100/prepay", false, 20);
            Console.WriteLine(response);

            LcswPayDate.OrderPayACK ack = jsonSerialize.Deserialize<LcswPayDate.OrderPayACK>(response);
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

        /// <summary>
        /// 刷卡（条码）支付
        /// </summary>
        /// <param name="order">扫码支付对象</param>
        /// <returns>二维码字符串</returns>
        public LcswPayDate.OrderPayACK BarcodePay(LcswPayDate.TradePay order)
        {
            order.pay_ver = LcswPayConfig.Ver;
            order.merchant_no = LcswPayConfig.StoreID;
            order.terminal_id = LcswPayConfig.DeviceID;
            order.terminal_time = DateTime.Now.ToString("yyyyMMddHHmmss");
            order.service_id = "010";
            order.key_sign = order.GetSign();

            JavaScriptSerializer jsonSerialize = new JavaScriptSerializer();
            string data = jsonSerialize.Serialize(order);

            string response = Post(data, LcswPayConfig.GatewayURL + "pay/100/barcodepay", false, 20);
            Console.WriteLine(response);

            LcswPayDate.OrderPayACK ack = jsonSerialize.Deserialize<LcswPayDate.OrderPayACK>(response);
            if (ack.CheckSign())
            {
                //验签成功
                if (ack.return_code == "01")
                {
                    //响应成功
                    if (ack.result_code == "01")
                    {
                        //qrCode = ack.qr_code;
                        //return true;
                        return ack;
                    }
                }
            }

            return null;
        }
    }
}
