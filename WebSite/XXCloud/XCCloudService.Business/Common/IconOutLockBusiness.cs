using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XCCloudService.CacheService;
using XCCloudService.Common;

namespace XCCloudService.Business.Common
{
    public class IconOutLockBusiness
    {

        private static List<string> mibileList = new List<string>();

        public static void AddByNoTimeLimit(string key)
        {
            if (!mibileList.Contains(key))
            {
                mibileList.Add(key);
            }
        }

        public static void RemoveByNoTimeList(string key)
        {
            if (mibileList.Contains(key))
            { 
                mibileList.Remove(key);
            }    
        }

        public static void Add(string key ,int coins)
        {
            if (coins == 0)
            {
                IconOutLockCache.Add(key, 36000);   
            }
            else
            { 
                int lockPerSecond = CommonConfig.IconOutLockPerSecond;
                int expires = (coins <= lockPerSecond ? 1 : (coins % lockPerSecond > 0 ? coins / lockPerSecond + 1 : coins / lockPerSecond));
                IconOutLockCache.Add(key, expires);                
            }
        }

        public static bool Exist(string key)
        {
            return IconOutLockCache.Exist(key) || mibileList.Contains(key);
        }
    }
}
