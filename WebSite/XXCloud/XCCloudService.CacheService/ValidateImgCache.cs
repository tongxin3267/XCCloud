using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using XCCloudService.Common;


namespace XCCloudService.CacheService
{
    public class ValidateImgCache
    {
        /// <summary>
        /// 获取缓存中的验证码
        /// </summary>
        /// <returns></returns>
        public static object GetValue(string key)
        {
            object obj = CacheHelper.Get(CacheType.ImgCode + key);
            return obj;
        }

        public static void Add(string key, string value, int expires)
        {
            CacheHelper.Insert(CacheType.ImgCode + key, value, expires);
        }

        public static bool Exist(string key)
        {
            object obj = CacheHelper.Get(CacheType.ImgCode + key);            
            if (obj == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public static void Remove(string key)
        {
            CacheHelper.Remove(CacheType.ImgCode + key);
        }
    }
}