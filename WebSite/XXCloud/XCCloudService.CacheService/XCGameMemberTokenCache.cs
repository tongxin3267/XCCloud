using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XCCloudService.CacheService
{
    public class XCGameMemberTokenCache
    {
        private static Dictionary<string, object> _memberTokenDic = new Dictionary<string, object>();

        public static Dictionary<string, object> MemberTokenHTDic
        {
            get { return _memberTokenDic; }
        }

        public static void AddToken(string key, object obj)
        {
            _memberTokenDic[key] = obj;
        }


        public static bool ExistToken(string key)
        {
            return _memberTokenDic.ContainsKey(key);
        }

        public static void Remove(string key)
        {
            _memberTokenDic.Remove(key);
        }
    }
}