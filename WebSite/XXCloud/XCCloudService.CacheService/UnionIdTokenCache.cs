using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using XCCloudService.Common;

namespace XCCloudService.CacheService
{
    public class UnionIdTokenCache
    {
        private static Hashtable _unionIdTokenHt = new Hashtable();

        public static void AddToken(string key, string token)
        {
            _unionIdTokenHt[key] = token;
        }

        public static bool ExistToken(string token)
        {
            if (_unionIdTokenHt.ContainsValue(token))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool ExistTokenByKey(string key)
        {
            return _unionIdTokenHt.ContainsKey(key);
        }

        public static bool ExistTokenByValue(string token)
        {
            return _unionIdTokenHt.ContainsValue(token);
        }

        public static void UpdateTokenByKey(string key, string value)
        {
            _unionIdTokenHt[key] = value;
        }

        public static string GetKeyByValue(string value)
        {
            var query = from item in _unionIdTokenHt.Cast<DictionaryEntry>() 
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


    public class UnionIdTokenModel
    {
        public UnionIdTokenModel()
        { 
            
        }

        public UnionIdTokenModel(string unionId)
        {
            this.UnionId = unionId;
        }

        public string UnionId { set; get; }
    }
}