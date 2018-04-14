using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XCCloudService.Common;

namespace XCCloudService.CacheService
{
    /// <summary>
    /// 出币锁定缓存
    /// </summary>
    public class IconOutLockCache
    {
        public static void Add(string key,int expires)
        {
            if (CacheHelper.Get(CacheType.IconOutLock + key) != null)
            {
                CacheHelper.Remove(CacheType.IconOutLock + key);
            }
            CacheHelper.Insert(CacheType.IconOutLock + key, key, expires);
        }

        public static bool Exist(string key)
        {
            object obj = CacheHelper.Get(CacheType.IconOutLock + key);
            if (obj != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
