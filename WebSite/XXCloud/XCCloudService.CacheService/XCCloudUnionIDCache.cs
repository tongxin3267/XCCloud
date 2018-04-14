using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XCCloudService.CacheService
{
    public class XCCloudUnionIDCache
    {
        private static Hashtable _unionIdHt = new Hashtable();

        public static Hashtable UnionIDHt 
        {
            set { _unionIdHt = value; }
            get { return _unionIdHt; }  
        }

        public static void Add(string key, string unionId)
        {
            _unionIdHt.Add(key, unionId);
        }


        public static bool ExistByKey(string key)
        {
            return _unionIdHt.ContainsKey(key);
        }


        public static bool ExistByValue(string unionId)
        {
            return _unionIdHt.ContainsValue(unionId);
        }

        public static void UpdateByKey(string key, string unionId)
        {
            _unionIdHt[key] = unionId;
        }
    }


}