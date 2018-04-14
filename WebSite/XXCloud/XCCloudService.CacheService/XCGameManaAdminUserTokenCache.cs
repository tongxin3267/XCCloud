using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using XCCloudService.Common;

namespace XCCloudService.CacheService
{
    public class XCGameManaAdminUserTokenCache
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


    public class XCGameManaAdminUserTokenModel
    {
        public XCGameManaAdminUserTokenModel(string mobile,int xcGameManaUserId)
        {
            this.XCGameManaUserId = xcGameManaUserId;
            this.Mobile = mobile;
        }

        public int XCGameManaUserId { set; get; }

        public string Mobile { set; get; }
    }
}