
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using XCCloudService.Common;
using System.Threading;
using XCCloudService.CacheService;
using System.Threading.Tasks;
using XCCloudService.BLL.IBLL.XCGameManager;
using XCCloudService.BLL.Container;
using XCCloudService.Model.XCGameManager;
using XCCloudService.BLL.IBLL.XCCloudRS232;
using XCCloudService.Model.XCCloudRS232;

namespace XCCloudService.Business.Common
{
    public class MobileTokenBusiness
    {
        public static string SetMobileToken(string mobile)
        {
            string token = System.Guid.NewGuid().ToString("N");
            if (MobileTokenCache.ExistTokenByKey(mobile))
            {
                SetDBMobileToken(token, mobile);
                MobileTokenCache.UpdateTokenByKey(mobile, token);
            }
            else
            {
                SetDBMobileToken(token, mobile);
                MobileTokenCache.AddToken(mobile, token);
            }
            return token;
        }

        public static bool ExistToken(string token, out string mobile)
        {
            mobile = string.Empty;
            if (MobileTokenCache.ExistTokenByValue(token))
            {
                mobile = MobileTokenCache.GetKeyByValue(token);
                return true;
            }
            else
            {
                return false;
            }
        }
        public static void Init()
        {
            IMobileTokenService mobileTokenService = BLLContainer.Resolve<IMobileTokenService>();
            var models = mobileTokenService.GetModels(p => 1 == 1).ToList<t_MobileToken>();
            if (models.Count > 0)
            {
                for (int y = 0; y < models.Count; y++)
                {
                    MobileTokenCache.AddToken(models[y].Phone, models[y].Token);
                }
            }           
        }

        public static void SetRS232MobileToken()
        {
            IMerchService mobileTokenService = BLLContainer.Resolve<IMerchService>();
            var models = mobileTokenService.GetModels(p => true).ToList().Where(p=>!string.IsNullOrWhiteSpace(p.Token)).ToList();
            if (models.Count > 0)
            {
                foreach (var item in models)
                {
                    MobileTokenCache.AddToken(CommonConfig.PrefixKey +  item.Mobile, item.Token);
                }
            }
        }

        public static void UpdateOpenId(string phone,string openId)
        {
            IMobileTokenService mobileTokenService = BLLContainer.Resolve<IMobileTokenService>();
            var model = mobileTokenService.GetModels(p => p.Phone.Equals(phone)).FirstOrDefault<t_MobileToken>();
            if (model != null)
            {
                model.OpenId = openId;
                mobileTokenService.Update(model);                
            }
        }

        public static bool GetOpenId(string phone, out string openId,out string errMsg)
        {
            openId = string.Empty;
            errMsg = string.Empty;
            IMobileTokenService mobileTokenService = BLLContainer.Resolve<IMobileTokenService>();
            var model = mobileTokenService.GetModels(p => p.Phone.Equals(phone)).FirstOrDefault<t_MobileToken>();
            if (model != null)
            {
                openId = model.OpenId;
                if (string.IsNullOrEmpty(openId))
                {
                    errMsg = "用户未绑定openId";
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                errMsg = "用户未不存在";
                return false;
            }
        }

        private static void SetDBMobileToken(string Token, string Phone)
        {
            IMobileTokenService mobileTokenService = BLLContainer.Resolve<IMobileTokenService>();
            var model = mobileTokenService.GetModels(p => p.Phone.Equals(Phone)).FirstOrDefault<t_MobileToken>();
            t_MobileToken mtk = new t_MobileToken();
            if (model == null)
            {
                mtk.Token = Token;
                mtk.CreateTime = DateTime.Now;
                mtk.Phone = Phone;
                mtk.OpenId = string.Empty;
                mobileTokenService.Add(mtk);
            }
            else 
            {
                model.Token = Token;
                model.UpdateTime = DateTime.Now;
                mobileTokenService.Update(model);                
            }
        }

        public static bool UpdateAliBuyerId(string phone, string buyerId)
        {
            IMobileTokenService mobileTokenService = BLLContainer.Resolve<IMobileTokenService>();
            var model = mobileTokenService.GetModels(p => p.Phone.Equals(phone)).FirstOrDefault<t_MobileToken>();
            if (model != null)
            {
                model.AliId = buyerId;
                return mobileTokenService.Update(model);
            }
            return false;
        }

        public static bool GetAliId(string phone, out string aliId, out string errMsg)
        {
            aliId = string.Empty;
            errMsg = string.Empty;
            IMobileTokenService mobileTokenService = BLLContainer.Resolve<IMobileTokenService>();
            var model = mobileTokenService.GetModels(p => p.Phone.Equals(phone)).FirstOrDefault<t_MobileToken>();
            if (model != null)
            {
                aliId = model.AliId;
                if (string.IsNullOrEmpty(aliId))
                {
                    errMsg = "用户未绑定aliId";
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                errMsg = "用户未不存在";
                return false;
            }
        }

        public static bool IsHasMobile(string aliId, out string mobile)
        {
            mobile = string.Empty;

            IMobileTokenService mobileTokenService = BLLContainer.Resolve<IMobileTokenService>();
            var model = mobileTokenService.GetModels(p => p.AliId.Equals(aliId)).FirstOrDefault<t_MobileToken>();
            if (model != null && !string.IsNullOrEmpty(model.Phone))
            {
                mobile = model.Phone;
                return true;
            }
            return false;
        }
    }
}