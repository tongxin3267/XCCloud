using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XCCloudService.Common;

namespace XCCloudService.CacheService.WeiXin
{
    public class WeiXinSAppSessionCache
    {
        public static void Add(string sessionkey, object sessionObj, int expires)
        {
            CacheHelper.Insert(CacheType.WeiXinSAppSession + sessionkey, sessionObj, expires);
        }

        public static object GetValue(string sessionkey)
        {
            object obj = CacheHelper.Get(CacheType.WeiXinSAppSession + sessionkey);
            return obj;
        }

        public static void Remove(string sessionkey)
        {
            CacheHelper.Remove(CacheType.WeiXinSAppSession + sessionkey);
        }
    }
}
