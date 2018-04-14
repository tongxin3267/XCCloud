using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XCCloudService.Common;

namespace XCCloudService.CacheService
{
    public class SocketClientCache
    {
        public static bool IsExist(string key)
        {
            object obj = CacheHelper.Get(CacheType.SMSCode + key);
            if (obj == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// 缓存中是添加验证码
        /// </summary>
        /// <param name="code">验证码</param>
        /// <param name="phone">手机号</param>
        /// <param name="expires">过期时间（分钟）</param>
        /// <returns></returns>
        public static void Add(string key, object socket, int expires)
        {
            CacheHelper.Insert(CacheType.SMSCode + key, socket, expires);
        }

        /// <summary>
        /// 获取缓存中的验证码
        /// </summary>
        /// <returns></returns>
        public static object GetValue(string key)
        {
            object obj = CacheHelper.Get(CacheType.SMSCode + key);
            return obj;
        }

        public static void Remove(string key)
        {
            CacheHelper.Remove(CacheType.SMSCode + key);
        } 
    }
}
