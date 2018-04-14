using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XCCloudService.Common;

namespace XCCloudService.CacheService.WeiXin
{
    public class RegisterOpenIDCache
    {
        public static void Add(string key, string openId, int expires)
        {
            CacheHelper.Insert(key, openId, expires);
        }

        public static object GetValue(string key)
        {
            object obj = CacheHelper.Get(key);
            return obj;
        }
    }
}
