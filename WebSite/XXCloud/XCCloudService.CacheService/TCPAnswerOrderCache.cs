using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XCCloudService.Common;

namespace XCCloudService.CacheService
{
    public class TCPAnswerOrderCache
    {
        private static Hashtable _ht = new Hashtable();

        public static void Add(string orderNo, object obj)
        {
            _ht[orderNo] = obj;
        }

        public static bool ExistToken(string orderNo)
        {
            if (_ht.ContainsValue(orderNo))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static object GetValue(string key)
        {
            return _ht[key];
        }

        public static bool ExistTokenByKey(string key)
        {
            return _ht.ContainsKey(key);
        }


        public static void Remove(string key)
        {
            _ht.Remove(key);
        }

        public static void UpdateTokenByKey(string key, string value)
        {
            _ht[key] = value;
        }

        public static string GetKeyByValue(string value)
        {
            var query = from item in _ht.Cast<DictionaryEntry>()
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
