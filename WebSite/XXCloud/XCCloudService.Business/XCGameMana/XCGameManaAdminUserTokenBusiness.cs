using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XCCloudService.BLL.Container;
using XCCloudService.BLL.IBLL.XCGameManager;
using XCCloudService.CacheService;
using XCCloudService.Common;
using XCCloudService.Model.CustomModel.XCCloud;
using XCCloudService.Model.XCGameManager;

namespace XCCloudService.Business.XCGameMana
{
    public class XCGameManaAdminUserTokenBusiness
    {
        public static string SetToken(string mobile,int xcGameManaUserId)
        {
            //设置会员token
            string newToken = System.Guid.NewGuid().ToString("N");
            string token = string.Empty;
            if (GetUserToken(mobile, out token))
            {
                XCGameManaAdminUserTokenCache.Remove(token);
                XCGameManaAdminUserTokenModel tokenModel = new XCGameManaAdminUserTokenModel(mobile, xcGameManaUserId);
                XCGameManaAdminUserTokenCache.AddToken(newToken, tokenModel);
            }
            else
            {
                XCGameManaAdminUserTokenModel tokenModel = new XCGameManaAdminUserTokenModel(mobile, xcGameManaUserId);
                XCGameManaAdminUserTokenCache.AddToken(newToken, tokenModel);
            }

            return newToken;
        }


        public static XCGameManaAdminUserTokenModel GetTokenModel(string token)
        {
            if (XCGameManaAdminUserTokenCache.UserTokenHTDic.ContainsKey(token))
            {
                XCGameManaAdminUserTokenModel tokenModel = (XCGameManaAdminUserTokenModel)(XCGameManaAdminUserTokenCache.UserTokenHTDic[token]);
                return tokenModel;
            }
            else
            {
                return null;
            }
        }


        public static bool GetUserToken(string mobile, out string token)
        {
            var query = from item in XCGameManaAdminUserTokenCache.UserTokenHTDic
                        where ((XCGameManaAdminUserTokenModel)(item.Value)).Mobile.Equals(mobile)
                        select item.Key.ToString();
            if (query.Count() == 0)
            {
                token = string.Empty;
                return false;
            }
            else
            {
                token = query.First();
                return true;
            }
        }



        public static void Init()
        {
            IUserTokenService userTokenService = BLLContainer.Resolve<IUserTokenService>();
            var list = userTokenService.GetModels(p => 1 == 1).ToList<t_usertoken>();
            if (list.Count > 0)
            {
                for (int i = 0; i < list.Count; i++)
                {
                    XCCloudManaUserTokenModel tokenModel = new XCCloudManaUserTokenModel(list[i].StoreId, list[i].StoreName, list[i].Mobile, Convert.ToInt32(list[i].XCGameUserId));
                    XCCloudManaUserTokenCache.AddToken(list[i].Token, tokenModel);
                }
            }
        }


        private static void SetDBManaUserToken(string token,string mobile , string storeId, string storeName,int xcGameUserId)
        {
            IUserTokenService userTokenService = BLLContainer.Resolve<IUserTokenService>();
            var model = userTokenService.GetModels(p => p.StoreId.Equals(storeId) & p.Mobile.Equals(mobile)).FirstOrDefault<t_usertoken>();
            
            if (model == null)
            {
                t_usertoken userToken = new t_usertoken();
                userToken.Token = token;
                userToken.Mobile = mobile;
                userToken.StoreId = storeId;
                userToken.StoreName = storeName;
                userToken.CreateTime = DateTime.Now;
                userToken.XCGameUserId = xcGameUserId;
                userTokenService.Add(userToken);
            }
            else
            {
                model.Token = token;
                model.Mobile = mobile;
                model.StoreId = storeId;
                model.StoreName = storeName;
                model.UpdateTime = DateTime.Now;
                model.XCGameUserId = xcGameUserId;
                userTokenService.Update(model);
            }
        }

    }
}
