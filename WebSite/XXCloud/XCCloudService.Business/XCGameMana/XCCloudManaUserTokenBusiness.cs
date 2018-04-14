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
    public class XCCloudManaUserTokenBusiness
    {
        public static string SetToken(string mobile,string storeId,string storeName,int xcGameUserId)
        {
            //设置会员token
            string newToken = System.Guid.NewGuid().ToString("N");
            string token = string.Empty;
            if (GetUserTokenModel(storeId, mobile, out token))
            {
                SetDBManaUserToken(newToken, mobile, storeId, storeName, xcGameUserId);
                XCCloudManaUserTokenCache.Remove(token);
                XCCloudManaUserTokenModel tokenModel = new XCCloudManaUserTokenModel(storeId, storeName, mobile, xcGameUserId);
                XCCloudManaUserTokenCache.AddToken(newToken, tokenModel);
            }
            else
            {
                SetDBManaUserToken(newToken, mobile, storeId, storeName, xcGameUserId);
                XCCloudManaUserTokenModel tokenModel = new XCCloudManaUserTokenModel(storeId, storeName, mobile, xcGameUserId);
                XCCloudManaUserTokenCache.AddToken(newToken, tokenModel);
            }

            return newToken;
        }


        public static XCCloudManaUserTokenModel GetManaUserTokenModel(string token)
        {
            if (XCCloudManaUserTokenCache.UserTokenHTDic.ContainsKey(token))
            {
                XCCloudManaUserTokenModel tokenModel = (XCCloudManaUserTokenModel)(XCCloudManaUserTokenCache.UserTokenHTDic[token]);
                return tokenModel;
            }
            else
            {
                return null;
            }
        }


        public static bool GetUserTokenModel(string mobile, ref List<XCCloudManaUserTokenResultModel> storeList)
        {
            storeList = new List<XCCloudManaUserTokenResultModel>();
            var query = from item in XCCloudManaUserTokenCache.UserTokenHTDic
                        where ((XCCloudManaUserTokenModel)(item.Value)).Mobile.Equals(mobile)
                        select item.Key.ToString();
            if (query.Count() == 0)
            {
                return false;
            }
            else
            {
                List<string> tmpList = query.ToList<string>();
                string storeName = string.Empty;
                for (int i = 0; i < tmpList.Count; i++)
                {
                    XCCloudManaUserTokenModel userTokenModel = (XCCloudManaUserTokenModel)(XCCloudManaUserTokenCache.UserTokenHTDic[tmpList[i]]);
                    XCCloudService.Model.CustomModel.XCGameManager.StoreCacheModel storeModel = null;
                    string errMsg = string.Empty;
                    StoreBusiness storeBusiness = new StoreBusiness();
                    if (storeBusiness.IsEffectiveStore(userTokenModel.StoreId, ref storeModel, out errMsg))
                    {
                        if (storeModel == null)
                        {
                            storeName = string.Empty;
                        }
                        else
                        {
                            storeName = storeModel.StoreName;
                        }
                    }

                    XCCloudManaUserTokenResultModel userTokenResultModel = new XCCloudManaUserTokenResultModel(userTokenModel.StoreId, storeName, tmpList[i].ToString());
                    storeList.Add(userTokenResultModel);
                }
                return true;
            }
        }

        public static bool GetUserTokenModel(string storeId, string mobile, out string token)
        {
            token = string.Empty;
            var query = from item in XCCloudManaUserTokenCache.UserTokenHTDic
                        where ((XCCloudManaUserTokenModel)(item.Value)).StoreId.Equals(storeId) && ((XCCloudManaUserTokenModel)(item.Value)).Mobile.Equals(mobile)
                        select item.Key.ToString();
            if (query.Count() == 0)
            {
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
