using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using XCCloudService.Base;
using XCCloudService.Business.Common;
using XCCloudService.CacheService;
using XCCloudService.Common;
using XCCloudService.Common.Enum;
using XCCloudService.Model.WeiXin.Session;
using XCCloudService.WeiXin.WeixinOAuth;

namespace XCCloudService.WeiXin.Api
{
    /// <summary>
    /// NumberOfPublic 的摘要说明
    /// </summary>
    public class OAuth : ApiBase
    {
        /// <summary>
        /// 获取小程序用户Session信息
        /// </summary>
        /// <returns></returns>
        [ApiMethodAttribute(SignKeyEnum = SignKeyEnum.MobileToken)]
        public object getSAppUserSession(Dictionary<string, object> dicParas)
        {
            WeiXinSAppSessionModel sessionModel = null;
            string errMsg = string.Empty;
            string serverSessionKey = string.Empty;
            string code = Utils.GetDictionaryValue<string>(dicParas, "code").ToString();
            if (WeiXinUserMana.GetWeiXinSAppUserSession(code, ref sessionModel,out serverSessionKey, out errMsg))
            {
                var dataObj = new {
                    serverSessionKey = serverSessionKey
                };

                //openId写入t_mobileToken
                try
                {
                    MobileTokenModel mobileTokenModel = (MobileTokenModel)(dicParas[Constant.MobileTokenModel]);
                    MobileTokenBusiness.UpdateOpenId(mobileTokenModel.Mobile, sessionModel.OpenId);
                }
                catch
                {

                }
                return ResponseModelFactory.CreateAnonymousSuccessModel(isSignKeyReturn, dataObj);
            }
            else
            {
                return ResponseModelFactory.CreateAnonymousSuccessModel(isSignKeyReturn, errMsg);
            }
        }
    }
}