using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;

namespace XCCloudService.CacheService
{
    public class RequestTotalCache
    {
        private static Hashtable _mainHt = new Hashtable();
        private static Hashtable _requestConfigHt = new Hashtable();

        static RequestTotalCache()
        {
            string xmlFilePath = HttpContext.Current.Server.MapPath("/Config/ApiRequest.xml");
            XmlDocument xd = new XmlDocument();
            xd.Load(xmlFilePath);
            XmlNode nodes = xd.SelectSingleNode("/reqeust");
            foreach (XmlNode node in nodes.ChildNodes)
            {
                _requestConfigHt[node.Name] = int.Parse(node.InnerXml.Trim());
            }
        }

        /// <summary>
        /// 添加请求记录
        /// </summary>
        /// <param name="mobile"></param>
        /// <param name="apiRequestType"></param>
        public static void Add(string mobile, string apiRequestType)
        {
            //如果是新的一天的头条数据，添加对应的子明细表
            string dateKey = System.DateTime.Now.ToString("yyyy-MM-dd");
            if (_mainHt[dateKey] == null)
            { 
                _mainHt[dateKey] = new Hashtable();
            }

            //移除历史记录
            var query = from item in _mainHt.Cast<DictionaryEntry>()
                        where item.Key.ToString() != dateKey
                        select item.Key.ToString();

            foreach (var q in query)
            {
                _mainHt.Remove(q);
            }

            string key = mobile + "_" + apiRequestType;
            Hashtable _detailHt = (Hashtable)_mainHt[dateKey];
            //保存手机号和请求类别添加到明细表
            if (_detailHt.ContainsKey(key))
            {
                int count = int.Parse(_detailHt[key].ToString());
                _detailHt[key] = count + 1;
            }
            else
            {
                _detailHt[key] = 1;
            }
        }

        /// <summary>
        /// 是否可以请求接口
        /// </summary>
        /// <returns></returns>
        public static bool CanRequest(string mobile, string apiRequestType)
        {
            string dateKey = System.DateTime.Now.ToString("yyyy-MM-dd");
            if (_mainHt[dateKey] == null)
            {
                return true;
            }

            //获取当日已请求数量
            string key = mobile + "_" + apiRequestType;
            Hashtable _detailHt = (Hashtable)_mainHt[dateKey];
            int currentCount = 0;
            int maxCount = 0;

            if (_detailHt.ContainsKey(key))
            {
                currentCount = int.Parse(_detailHt[key].ToString());
            }
             
            //获取单日最大请求次数
            if (_requestConfigHt.ContainsKey(apiRequestType))
            {
                maxCount = int.Parse(_requestConfigHt[apiRequestType].ToString());
            }
            else
            {
                maxCount = 100;
            }

            if (maxCount > currentCount)
            {
                return true;
            }

            return false;
        }
    }
}