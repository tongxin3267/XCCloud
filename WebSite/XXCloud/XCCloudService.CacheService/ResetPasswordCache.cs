using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using XCCloudService.Common;


namespace XCCloudService.CacheService
{

    public class ResetPasswordCache
    {

        /// <summary>
        /// 缓存中是否存在密码
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static bool IsExist(string key)
        {
            object obj = CacheHelper.Get(CacheType.ResetPassword + key);
            if(obj == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// 缓存中是添加新密码
        /// </summary>
        /// <param name="key">关键字</param>
        /// <param name="pwd">新密码</param>
        /// <param name="expires">过期时间（分钟）</param>
        /// <returns></returns>
        public static void Add(string key, string pwd, int expires)
        {
            CacheHelper.Insert(CacheType.ResetPassword + key, pwd, expires);
        }

        /// <summary>
        /// 获取缓存中的密码
        /// </summary>
        /// <returns></returns>
        public static object GetValue(string key)
        {
            object obj = CacheHelper.Get(CacheType.ResetPassword + key);
            return obj;
        }

        public static void Remove(string key)
        {
            CacheHelper.Remove(CacheType.ResetPassword + key);
        }
    }

}