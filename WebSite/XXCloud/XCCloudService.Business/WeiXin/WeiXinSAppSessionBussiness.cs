using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using XCCloudService.CacheService.WeiXin;
using XCCloudService.Model.WeiXin.Session;

namespace XCCloudService.Business.WeiXin
{
    public class WeiXinSAppSessionBussiness
    {
        public static bool Add(WeiXinSAppSessionModel sessionModel,int expires,out string serverSessionKey)
        {
            serverSessionKey = System.Guid.NewGuid().ToString("N");
            WeiXinSAppSessionCache.Add(serverSessionKey, sessionModel, expires);
            return true;
        }

        public static bool Exist(string localSessionKey)
        {
            if (WeiXinSAppSessionCache.GetValue(localSessionKey) != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool GetSession(string localSessionKey, ref WeiXinSAppSessionModel sessionModel)
        {
            sessionModel = WeiXinSAppSessionCache.GetValue(localSessionKey) as WeiXinSAppSessionModel;
            if (sessionModel != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
