using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using XCCloudService.Common;

namespace XCCloudService.SocketService.UDP.Security
{
    public class SignKeyHelper
    {
        /// <summary>
        /// 设置实体签名字段
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="key"></param>
        public static void SetSignKey(object obj,string key)
        { 
            //对象非签名字段，添加到集合，排序
            SortedDictionary<string, string> dict = new SortedDictionary<string, string>();
            PropertyInfo[] propertys = obj.GetType().GetProperties();
            foreach (PropertyInfo p in propertys)
            {
                if (!p.Name.Equals("SignKey"))
                {
                    if (p.GetCustomAttributes().Count() > 0)
                    { 
                        object value = p.GetValue(obj);
                        dict.Add(p.Name.ToLower(), value.ToString());                        
                    }
                }
            }

            StringBuilder parames = new StringBuilder();
            foreach (string name in dict.Keys)
            {
                parames.Append(dict[name]);
            }
            parames.Append(key);

            string md5 = Utils.MD5(parames.ToString());
            PropertyInfo property = obj.GetType().GetProperty("SignKey");
            property.SetValue(obj, md5,null);
        }

        /// <summary>
        /// 验证签名字段
        /// </summary>
        /// <returns></returns>
        public static bool CheckSignKey(object obj, string key)
        {
            //对象非签名字段，添加到集合，排序
            SortedDictionary<string, string> dict = new SortedDictionary<string, string>();
            PropertyInfo[] propertys = obj.GetType().GetProperties();

            foreach (PropertyInfo p in propertys)
            {
                if (!p.Name.Equals("SignKey") )
                {
                    if (!(p.Name.Equals("Result_Data") && p.PropertyType.UnderlyingSystemType.ToString() != "System.String"))
                    { 
                        if (p.GetCustomAttributes().Count() > 0)
                        { 
                            object value = p.GetValue(obj);
                            dict.Add(p.Name, value.ToString());                        
                        }                        
                    }
                }
            }

            StringBuilder parames = new StringBuilder();
            foreach (string name in dict.Keys)
            {
                parames.Append(dict[name]);
            }
            parames.Append(key);

            string md5 = Utils.MD5(parames.ToString());
            PropertyInfo property = obj.GetType().GetProperty("SignKey");
            
            if (property == null)
            { 
                //不需要签名运算
                return true;
            }
            string signKey = property.GetValue(obj, null).ToString();
            if (!signKey.Equals(md5))
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}
