using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XCCloudService.CacheService
{
    public class XCGameRouteDeviceCache
    {
        private static Hashtable _deviceHt = new Hashtable();

        public static Hashtable DeviceHt 
        { 
            set { _deviceHt = value; }
            get { return _deviceHt; }  
        }

        public static void AddToken(string key,string token)
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

        public static void UpdateTokenByKey(string key,string token)
        {
            _deviceHt[key] = token;
        }
    }


}