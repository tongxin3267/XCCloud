using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using XCCloudService.Common;

namespace XCCloudService.CacheService
{
    public class MobileTokenCache
    {
        private static Hashtable _mobileTokenHt = new Hashtable();

        public static void AddToken(string key, string token)
        {
            _mobileTokenHt[key] = token;
        }

        public static bool ExistToken(string token)
        {
            if (_mobileTokenHt.ContainsValue(token))
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
            return _mobileTokenHt.ContainsKey(key);
        }

        public static bool ExistTokenByValue(string token)
        {
            return _mobileTokenHt.ContainsValue(token);
        }

        public static void UpdateTokenByKey(string key, string value)
        {
            _mobileTokenHt[key] = value;
        }

        public static string GetKeyByValue(string value)
        {
            var query = from item in _mobileTokenHt.Cast<DictionaryEntry>() 
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


    public class MobileTokenModel
    {
        public MobileTokenModel()
        { 
            
        }

        public MobileTokenModel(string mobile)
        {
            this.Mobile = mobile;
        }

        public string StoreId { set; get; }

        public string Mobile { set; get; }
    }
}