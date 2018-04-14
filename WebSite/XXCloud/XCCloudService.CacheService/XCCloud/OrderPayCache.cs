using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XCCloudService.Common;
using XCCloudService.Model.CustomModel.XCCloud;

namespace XCCloudService.CacheService.XCCloud
{
    public class OrderPayCache
    {
        /// <summary>
        /// 缓存中是否存在数据
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public static bool IsExist(string key)
        {
            object obj = CacheHelper.Get(CacheType.OrderPayCache + key);
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
        /// 缓存回调的订单支付信息
        /// </summary>
        /// <param name="key"></param>
        /// <param name="list"></param>
        /// <param name="expires"></param>
        public static void Add(string key, OrderPayCacheModel model, int expires)
        {
            if (IsExist(CacheType.OrderPayCache + key))
            {
                CacheHelper.Remove(CacheType.OrderPayCache + key);
            }
            CacheHelper.Insert(CacheType.OrderPayCache + key, model, expires);
        }

        /// <summary>
        /// 获取缓存中的信息
        /// </summary>
        /// <returns></returns>
        public static object GetValue(string key)
        {
            object obj = CacheHelper.Get(CacheType.MemberCardQuery + key);
            return obj;
        }

        public static void Remove(string key)
        {
            CacheHelper.Remove(CacheType.OrderPayCache + key);
        }
    }
}
