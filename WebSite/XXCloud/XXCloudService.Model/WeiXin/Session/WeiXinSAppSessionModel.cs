using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XCCloudService.Model.WeiXin.Session
{
    /// <summary>
    /// 微信小程序的登录状态
    /// </summary>
    public class WeiXinSAppSessionModel
    {
        public WeiXinSAppSessionModel()
        { 
        
        }

        public WeiXinSAppSessionModel(string openId,string sessionKey,string unionId)
        {
            this.OpenId = openId;
            this.SessionKey = sessionKey;
            this.UnionId = unionId;
        }

        public string OpenId { set; get; }

        public string SessionKey { set; get; }

        public string UnionId { set; get; }
    }
}
