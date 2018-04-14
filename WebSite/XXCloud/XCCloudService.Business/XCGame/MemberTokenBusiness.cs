using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using XCCloudService.BLL.Container;
using XCCloudService.BLL.IBLL.XCGameManager;
using XCCloudService.CacheService;
using XCCloudService.Common;
using XCCloudService.Model.CustomModel.XCGame;
using XCCloudService.Model.XCGameManager;

namespace XCCloudService.Business.XCGame
{
    public class MemberTokenBusiness
    {

        public static string SetMemberToken(string storeId,string mobile,string MemberLevelName,string storeName,string icCardId,string EndTime)
        {
            //设置会员token
            string newToken = System.Guid.NewGuid().ToString("N");
            string token = string.Empty;
            if (GetMemberTokenModel(storeId, mobile, out token))
            {   
                SetDBMemberToken(newToken, storeId, mobile, MemberLevelName, storeName, icCardId, EndTime);
                XCGameMemberTokenCache.Remove(token);
                XCGameMemberTokenModel tokenModel = new XCGameMemberTokenModel(storeId, mobile, icCardId, MemberLevelName, storeName,EndTime);
                XCGameMemberTokenCache.AddToken(newToken, tokenModel);
            }
            else
            {
                SetDBMemberToken(newToken, storeId, mobile, MemberLevelName, storeName, icCardId, EndTime);
                XCGameMemberTokenModel tokenModel = new XCGameMemberTokenModel(storeId, mobile, icCardId, MemberLevelName, storeName, EndTime);
                XCGameMemberTokenCache.AddToken(newToken, tokenModel);
            }

            return newToken;
        }


        public static XCGameMemberTokenModel GetMemberTokenModel(string token)
        {
            if (XCGameMemberTokenCache.MemberTokenHTDic.ContainsKey(token))
            {
                XCGameMemberTokenModel tokenModel = (XCGameMemberTokenModel)(XCGameMemberTokenCache.MemberTokenHTDic[token]);
                if(Convert.ToDateTime(tokenModel.EndTime + " 23:59:59") > System.DateTime.Now)
                {
                    return tokenModel;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }


        public static bool GetMemberTokenModel(string storeId,string mobile,out string token)
        {
            token = string.Empty;
            var query = from item in XCGameMemberTokenCache.MemberTokenHTDic
                        where ((XCGameMemberTokenModel)(item.Value)).StoreId.Equals(storeId) && ((XCGameMemberTokenModel)(item.Value)).Mobile.Equals(mobile)
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
            IMemberTokenService apirequestlogservice = BLLContainer.Resolve<IMemberTokenService>();
            var model = apirequestlogservice.GetModels(p => 1 == 1).ToList<T_MemberToken>();
            if (model.Count > 0)
            {
                for (int i = 0; i < model.Count; i++)
                {
                    XCGameMemberTokenModel tokenModel = new XCGameMemberTokenModel(model[i].StoreId, model[i].Phone, model[i].ICCardID, model[i].MemberLevelName, model[i].StoreName,model[i].EndTime);
                    XCGameMemberTokenCache.AddToken(model[i].Token, tokenModel);
                }
            }

        }


        private static void SetDBMemberToken(string Token, string StoreId, string Phone, string MemberLevelName, string storeName, string ICCardID, string EndTime)
        {
            IMemberTokenService memberTokenService = BLLContainer.Resolve<IMemberTokenService>();
            var model = memberTokenService.GetModels(p => p.StoreId.Equals(StoreId) & p.Phone.Equals(Phone)).FirstOrDefault<T_MemberToken>();
            T_MemberToken mtk = new T_MemberToken();
            if (model == null)
            {
                mtk.Token = Token;
                mtk.CreateTime = DateTime.Now;
                mtk.StoreId = StoreId;
                mtk.Phone = Phone;
                mtk.ICCardID = ICCardID;
                mtk.StoreName = storeName;
                mtk.MemberLevelName = MemberLevelName;
                mtk.EndTime = EndTime;
                memberTokenService.Add(mtk);
            }
            else
            { 
                model.Token = Token;
                model.UpdateTime = DateTime.Now;
                mtk.ICCardID = ICCardID;
                mtk.StoreName = storeName;
                mtk.MemberLevelName = MemberLevelName;
                model.EndTime = EndTime;
                memberTokenService.Update(model);                
            }
        }
    }
}