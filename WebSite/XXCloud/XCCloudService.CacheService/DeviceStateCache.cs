using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using XCCloudService.Common;

namespace XCCloudService.CacheService
{
    public class DeviceStateCache
    {
        private static Hashtable _deviceHt = new Hashtable();

        public static void AddToken(string key, string token)
        {
            _deviceHt[key] = token;
        }


        public static bool ExistTokenByKey(string key)
        {
            return _deviceHt.ContainsKey(key);
        }

        public static bool ExistTokenByValue(string token)
        {
            return _deviceHt.ContainsValue(token);
        }

        public static object GetValueByKey(string key)
        {
            return _deviceHt[key];
        }

        public static void UpdateTokenByKey(string key, string value)
        {
            _deviceHt[key] = value;
        }

        public static string GetKeyByValue(string value)
        {
            var query = from item in _deviceHt.Cast<DictionaryEntry>()
                        where item.Value.ToString().Equals(value)
                        select item.Key.ToString();
            if (query.Count() == 0)
            {
                return null;
            }
            else
            {
                return query.First();
            }
        }
    }
}