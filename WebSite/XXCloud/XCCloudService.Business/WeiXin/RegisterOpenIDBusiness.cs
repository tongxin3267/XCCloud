using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XCCloudService.CacheService;
using XCCloudService.CacheService.WeiXin;
using XCCloudService.Common;

namespace XCCloudService.Business.WeiXin
{
    public class RegisterOpenIDBusiness
    {
        public static void AddOpenId(string key, string openId, int expires)
        {
            RegisterOpenIDCache.Add(key, openId, expires);
        }

        public static bool GetOpenId(string key, out string openId)
        {
            openId = string.Empty;
            object obj = RegisterOpenIDCache.GetValue(key);
            if (obj == null)
            {
                return false;
            }
            else
            {
                openId = obj.ToString();
                return true;
            }
        }        
    }
}
