using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XCCloudService.CacheService.XCGame
{
    public class XCGameUserTokenCache
    {
        private static Dictionary<string, object> _userTokenDic = new Dictionary<string, object>();

        public static Dictionary<string, object> UserTokenHTDic
        {
            get { return _userTokenDic; }
        }

        public static void AddToken(string key, object obj)
        {
            _userTokenDic[key] = obj;
        }


        public static bool ExistToken(string key)
        {
            return _userTokenDic.ContainsKey(key);
        }

        public static void Remove(string key)
        {
            _userTokenDic.Remove(key);
        }
    }
}
