using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XCCloudService.CacheService;
using XCCloudService.Common;

namespace XCCloudService.Business.WeiXin
{
    public class WeiXinAccessTokenBusiness
    {
        public static void AddAccessToken(string accessToken,int expires)
        {
            WeiXinAccessTokenCache.Add(Constant.WeiXinAccessToken,accessToken,expires);
        }

        public static bool GetAccessToken(out string accessToken)
        {
            accessToken = string.Empty;
            object obj = WeiXinAccessTokenCache.GetValue(Constant.WeiXinAccessToken);
            if (obj == null)
            {
                return false;
            }
            else
            {
                accessToken = obj.ToString();
                return true;
            }
        }
    }
}
