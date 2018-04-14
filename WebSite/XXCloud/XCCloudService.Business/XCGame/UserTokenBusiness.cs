using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using XCCloudService.CacheService.XCGame;
using XCCloudService.Model.CustomModel.XCGame;


namespace XCCloudService.Business.XCGame
{
    public class UserTokenBusiness
    {
        public static string SetUserToken(string storeId,int userID,string userName,string realName,string mobile)
        {
            //设置会员token
            string newToken = System.Guid.NewGuid().ToString("N");
            string token = string.Empty;
            if (GetUserTokenModel(storeId, userID, out token))
            {   
                //SetDBMemberToken(newToken, storeId, mobile, MemberLevelName, storeName, icCardId, EndTime);
                XCGameUserTokenCache.Remove(token);
                XCGameUserTokenModel tokenModel = new XCGameUserTokenModel(storeId,userID,userName,realName,mobile);
                XCGameUserTokenCache.AddToken(newToken, tokenModel);
            }
            else
            {
                //SetDBMemberToken(newToken, storeId, mobile, MemberLevelName, storeName, icCardId, EndTime);
                XCGameUserTokenModel tokenModel = new XCGameUserTokenModel(storeId, userID, userName, realName, mobile);
                XCGameUserTokenCache.AddToken(newToken, tokenModel);
            }

            return newToken;
        }


        public static XCGameUserTokenModel GetUserTokenModel(string token)
        {
            if (XCGameUserTokenCache.UserTokenHTDic.ContainsKey(token))
            {
                return (XCGameUserTokenModel)(XCGameUserTokenCache.UserTokenHTDic[token]);
            }
            else
            {
                return null;
            }
        }


        public static bool GetUserTokenModel(string storeId, int userID, out string token)
        {
            token = string.Empty;
            var query = from item in XCGameUserTokenCache.UserTokenHTDic
                        where ((XCGameUserTokenModel)(item.Value)).StoreId.Equals(storeId) && ((XCGameUserTokenModel)(item.Value)).UserId == userID
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


        //public static void Init()
        //{
        //    IMemberTokenService apirequestlogservice = BLLContainer.Resolve<IMemberTokenService>();
        //    var model = apirequestlogservice.GetModels(p => 1 == 1).ToList<T_MemberToken>();
        //    if (model.Count > 0)
        //    {
        //        for (int i = 0; i < model.Count; i++)
        //        {
        //            XCGameMemberTokenModel tokenModel = new XCGameMemberTokenModel(model[i].StoreId, model[i].Phone, model[i].ICCardID, model[i].MemberLevelName, model[i].StoreName,model[i].EndTime);
        //            XCGameMemberTokenCache.AddToken(model[i].Token, tokenModel);
        //        }
        //    }

        //}


        //private static void SetDBMemberToken(string Token, string StoreId, string Phone, string MemberLevelName, string storeName, string ICCardID, string EndTime)
        //{
        //    IMemberTokenService memberTokenService = BLLContainer.Resolve<IMemberTokenService>();
        //    var model = memberTokenService.GetModels(p => p.StoreId.Equals(StoreId) & p.Phone.Equals(Phone)).FirstOrDefault<T_MemberToken>();
        //    T_MemberToken mtk = new T_MemberToken();
        //    if (model == null)
        //    {
        //        mtk.Token = Token;
        //        mtk.CreateTime = DateTime.Now;
        //        mtk.StoreId = StoreId;
        //        mtk.Phone = Phone;
        //        mtk.ICCardID = ICCardID;
        //        mtk.StoreName = storeName;
        //        mtk.MemberLevelName = MemberLevelName;
        //        mtk.EndTime = EndTime;
        //        memberTokenService.Add(mtk);
        //    }
        //    else
        //    { 
        //        model.Token = Token;
        //        model.UpdateTime = DateTime.Now;
        //        mtk.ICCardID = ICCardID;
        //        mtk.StoreName = storeName;
        //        mtk.MemberLevelName = MemberLevelName;
        //        model.EndTime = EndTime;
        //        memberTokenService.Update(model);                
        //    }
        //}
    }
}