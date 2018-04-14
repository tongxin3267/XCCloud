using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XCCloudService.CacheService.XCGame;
using XCCloudService.Model.CustomModel.XCGame;

namespace XCCloudService.Business.XCGame
{
   public class UserInfoBusiness
    {
        public static string SetUserToken(string storeId, string mobile)
        {
            //设置会员token
            string newToken = System.Guid.NewGuid().ToString("N");
            string token = string.Empty;
            if (GetUserTokenModel(storeId, mobile, out token))
            {
                UserTokenCaChe.Remove(token);
                UserInfoModel tokenModel = new UserInfoModel(storeId,  mobile);
                UserTokenCaChe.AddToken(newToken, tokenModel);
            }
            else
            {
                UserInfoModel tokenModel = new UserInfoModel(storeId, mobile);
                UserTokenCaChe.AddToken(newToken, tokenModel);
            }

            return newToken;
        }


        public static UserInfoModel GetUserTokenModel(string token)
        {
            if (UserTokenCaChe.UserTokenHTDic.ContainsKey(token))
            {
                return (UserInfoModel)(UserTokenCaChe.UserTokenHTDic[token]);
            }
            else
            {
                return null;
            }
        }


        public static bool GetUserTokenModel(string storeId, string mobile, out string token)
        {
            token = string.Empty;
            var query = from item in UserTokenCaChe.UserTokenHTDic
                        where ((UserInfoModel)(item.Value)).StoreId.Equals(storeId) && ((UserInfoModel)(item.Value)).Mobile == mobile
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

    }
}
