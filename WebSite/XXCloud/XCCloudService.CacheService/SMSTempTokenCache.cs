using XCCloudService.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XCCloudService.CacheService
{
    /// <summary>
    /// 用户在通过验证码验证后，生成临时token,用户短信验证
    /// </summary>
    public class SMSTempTokenCache
    {
        /// <summary>
        /// 添加短信临时token
        /// </summary>
        /// <param name="key">token值</param>
        /// <param name="value">手机号</param>
        /// <param name="expires">过期时间</param>
        public static void Add(string key, string value, int expires)
        {
            CacheHelper.Insert(CacheType.SMSTempToken + key, value, expires);
        }

        public static object GetValue(string key)
        {
            object obj = CacheHelper.Get(CacheType.SMSTempToken + key);
            return obj;
        }

        public static void Remove(string key)
        {
            CacheHelper.Remove(CacheType.SMSTempToken + key);
        }
    }
}