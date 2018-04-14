using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XCCloudService.Common;

namespace XCCloudService.CacheService
{
    public class WeiXinAccessTokenCache
    {
        public static void Add(string key,string accessToken,int expires)
        {
            CacheHelper.Insert(key, accessToken, expires);
        }

        public static object GetValue(string key)
        {
            object obj = CacheHelper.Get(key);
            return obj;
        }
    }
}
