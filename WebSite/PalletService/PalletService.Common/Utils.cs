using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Reflection;
using System.Runtime.Serialization.Json;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI.WebControls;
using System.Xml;

namespace PalletService.Common
{
    public class Utils
    {
        public static string Number(int Length, bool Sleep)
        {
            if (Sleep)
                System.Threading.Thread.Sleep(3);
            string result = "";
            System.Random random = new Random();
            for (int i = 0; i < Length; i++)
            {
                result += random.Next(10).ToString();
            }
            return result;
        }

        public static string GetException(Exception e)
        {
            string exception = string.Format("{0}{1}{2}", "[", e.Message, "]");
            while (e.InnerException != null)
            {
                e = e.InnerException;
                exception = string.Format("{0}{1}{2}", "[", e.Message, "]");
            }
            return exception;
        }

        #region"数据序列化"
        /// <summary>
        /// 数据协议JSON序列华
        /// </summary>
        /// <param name="resObj"></param>
        /// <returns></returns>
        public static string DataContractJsonSerializer(object resObj)
        {
            //获取响应对象的类名和泛型类型名
            List<Type> list = new List<Type>();
            list.Add(resObj.GetType());
            IEnumerable<Type> iEnum = list;

            DataContractJsonSerializer serializer = new DataContractJsonSerializer(resObj.GetType(), iEnum);
            byte[] byteArr;
            using (MemoryStream ms = new MemoryStream())
            {
                serializer.WriteObject(ms, resObj);
                byteArr = ms.ToArray();
            }
            string jsonStr = Encoding.UTF8.GetString(byteArr);
            return jsonStr;
        }

        public static T DataContractJsonDeserializer<T>(string json)
        {
            Type type = typeof(T);
            string fullName = type.FullName;
            List<Type> list = new List<Type>();
            list.Add(type);
            IEnumerable<Type> iEnum = list;

            DataContractJsonSerializer serializer = new DataContractJsonSerializer(type, iEnum);
            var mStream = new MemoryStream(Encoding.UTF8.GetBytes(json));
            return (T)(serializer.ReadObject(mStream));
        }

        /// <summary>
        /// json解析成object对象
        /// </summary>
        /// <returns></returns>
        public static object DeserializeObject(string json)
        {
            System.Web.Script.Serialization.JavaScriptSerializer jss = new System.Web.Script.Serialization.JavaScriptSerializer();
            return jss.DeserializeObject(json);
        }

        /// <summary>
        /// object序列化为Json
        /// </summary>
        /// <returns></returns>
        public static string SerializeObject(object obj)
        {
            System.Web.Script.Serialization.JavaScriptSerializer jss = new System.Web.Script.Serialization.JavaScriptSerializer();
            return jss.Serialize(obj);
        }


        public static bool CheckApiReturnJson(string json,ref object result_data,out string errMsg)
        {
            errMsg = string.Empty;
            string return_msg = string.Empty;
            string result_msg = string.Empty;
            System.Web.Script.Serialization.JavaScriptSerializer jss = new System.Web.Script.Serialization.JavaScriptSerializer();
            object obj = jss.DeserializeObject(json);
            object return_code = Utils.GetJsonObjectValue(obj, "return_code");
            object result_code = Utils.GetJsonObjectValue(obj, "result_code");
            object return_msgObj = Utils.GetJsonObjectValue(obj, "return_msg");
            object result_msgObj = Utils.GetJsonObjectValue(obj, "result_msg");
            result_data = Utils.GetJsonObjectValue(obj, "result_data");
            if (return_code != null && result_code != null && return_code.ToString().Equals("1") && result_code.ToString().Equals("1"))
            {
                return true;
            }
            else
            {
                return_msg = (return_msgObj == null ? "" : return_msgObj.ToString());
                result_msg = (result_msgObj == null ? "" : result_msgObj.ToString());
                errMsg = (string.IsNullOrEmpty(result_msg) ? (string.IsNullOrEmpty(return_msg) ? "" : return_msg) : result_msg);
                return false;
            }
        }

        /// <summary>
        /// 对象中是否存在属性
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static bool JsonObjectExistProperty(object jsonObj, string key)
        {
            System.Collections.Generic.Dictionary<string, object> dict = (System.Collections.Generic.Dictionary<string, object>)jsonObj;
            if (dict.ContainsKey(key))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static object GetJsonObjectValue(object jsonObj, string key)
        {
            System.Collections.Generic.Dictionary<string, object> dict = (System.Collections.Generic.Dictionary<string, object>)jsonObj;
            if (dict.ContainsKey(key))
            {
                return dict[key];
            }
            else
            {
                return null;
            }
        }

        public static string GetXmlNodeValue(string xmlFilePath,string path)
        {
            //string xmlFilePath = string.Format("{0}//{1}",System.Windows.Forms.Application.StartupPath,"SysConfig.xml");
            XmlDocument xd = new XmlDocument();
            xd.Load(xmlFilePath);
            var node = xd.SelectSingleNode(path);
            if (node == null)
            {
                return string.Empty;
            }
            else
            {
                return node.InnerText.Trim();
            }      
        }

        public static bool SetXmlNodeValue(string xmlFilePath,string path, string value)
        {
            //string xmlFilePath = string.Format("{0}//{1}", System.Windows.Forms.Application.StartupPath, "SysConfig.xml");
            XmlDocument xd = new XmlDocument();
            xd.Load(xmlFilePath);
            var node = xd.SelectSingleNode(path);
            if (node == null)
            {
                return false;
            }
            else
            {
                node.InnerText = value;
                xd.Save(xmlFilePath);
                return true;
            }
        }

        public static object GetPropertyValue(object t, string propertyName)
        {
            Type type = t.GetType();
            PropertyInfo property = t.GetType().GetProperty(propertyName);
            if (property == null)
            {
                switch (t.GetType().FullName)
                {
                    case "System.String": return string.Empty;
                    case "System.Int32": return 0;
                    case "System.Decimal": return 0;
                }
            }

            return property.GetValue(t, null);
        }


        public static object GetFieldAttributeValue(Type classType, string fieldName,Type attributeType ,string attributeName)
        {
            FieldInfo fieldInfo = classType.GetField(fieldName);
            if (fieldInfo == null)
            {
                return null;
            }
            else
            {
                IList<CustomAttributeData> listAttribute = fieldInfo.GetCustomAttributesData();
                for (int i = 0; i < listAttribute.Count; i++)
                {
                    CustomAttributeData data = listAttribute[0];
                    foreach (var name in data.NamedArguments)
                    {
                        if (name.MemberInfo.DeclaringType.Name.Equals(attributeType.Name) && name.MemberInfo.Name.Equals(attributeName))
                        {
                            return name.TypedValue.Value;
                        }
                    }
                }
                return null;
            }
        }

        /// <summary>
        /// DES解密字符串
        /// </summary>
        /// <param name="decryptString">待解密的字符串</param>
        /// <param name="decryptKey">解密密钥,要求为8位,和加密密钥相同</param>
        /// <returns>解密成功返回解密后的字符串，失败返源串</returns>
        public static string DecryptDES(string pToDecrypt, string sKey)
        {
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();

            byte[] inputByteArray = new byte[pToDecrypt.Length / 2];
            for (int x = 0; x < pToDecrypt.Length / 2; x++)
            {
                int i = (Convert.ToInt32(pToDecrypt.Substring(x * 2, 2), 16));
                inputByteArray[x] = (byte)i;
            }

            des.Key = ASCIIEncoding.ASCII.GetBytes(sKey);
            des.IV = ASCIIEncoding.ASCII.GetBytes(sKey);
            MemoryStream ms = new MemoryStream();
            CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(), CryptoStreamMode.Write);
            cs.Write(inputByteArray, 0, inputByteArray.Length);
            cs.FlushFinalBlock();

            StringBuilder ret = new StringBuilder();

            return System.Text.Encoding.Default.GetString(ms.ToArray());
        }

        #endregion


        public static bool IsNumeric(string expression)
        {
            if (expression != null)
            {
                string str = expression;
                if (str.Length > 0 && str.Length <= 11 && Regex.IsMatch(str, @"^[-]?[0-9]*[.]?[0-9]*$"))
                {
                    if ((str.Length < 10) || (str.Length == 10 && str[0] == '1') || (str.Length == 11 && str[0] == '-' && str[1] == '1'))
                        return true;
                }
            }
            return false;
        }

        #region URL请求数据
        /// <summary>
        /// HTTP POST方式请求数据
        /// </summary>
        /// <param name="url">URL.</param>
        /// <param name="param">POST的数据</param>
        /// <returns></returns>
        public static string HttpPost(string url, string param)
        {
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.Accept = "*/*";
            request.Timeout = 15000;
            request.AllowAutoRedirect = false;

            StreamWriter requestStream = null;
            WebResponse response = null;
            string responseStr = null;

            try
            {
                requestStream = new StreamWriter(request.GetRequestStream());
                requestStream.Write(param);
                requestStream.Close();

                response = request.GetResponse();
                if (response != null)
                {
                    StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
                    responseStr = reader.ReadToEnd();
                    reader.Close();
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                request = null;
                requestStream = null;
                response = null;
            }

            return responseStr;
        }

        /// <summary>
        /// HTTP GET方式请求数据.
        /// </summary>
        /// <param name="url">URL.</param>
        /// <returns></returns>
        public static string HttpGet(string url)
        {
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
            request.Method = "GET";
            //request.ContentType = "application/x-www-form-urlencoded";
            request.Accept = "*/*";
            request.Timeout = 15000;
            request.AllowAutoRedirect = false;

            WebResponse response = null;
            string responseStr = null;

            try
            {
                response = request.GetResponse();

                if (response != null)
                {
                    StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
                    responseStr = reader.ReadToEnd();
                    reader.Close();
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                request = null;
                response = null;
            }

            return responseStr;
        }

        /// <summary>
        /// 执行URL获取页面内容
        /// </summary>
        public static string UrlExecute(string urlPath)
        {
            if (string.IsNullOrEmpty(urlPath))
            {
                return "error";
            }
            StringWriter sw = new StringWriter();
            try
            {
                HttpContext.Current.Server.Execute(urlPath, sw);
                return sw.ToString();
            }
            catch (Exception)
            {
                return "error";
            }
            finally
            {
                sw.Close();
                sw.Dispose();
            }
        }

        public static string GetUrlRequestResult(string url)
        {
            WebClient wc = new WebClient();
            wc.Encoding = System.Text.Encoding.UTF8;
            return wc.DownloadString(url);
        }

        /// <summary>
        /// WebClient获取接口结果
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string WebClientDownloadString(string url)
        {
            WebClient wc = new WebClient();
            wc.Encoding = System.Text.Encoding.UTF8;
            return wc.DownloadString(url);
        }


        #endregion
    }
}
