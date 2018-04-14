using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XCCloudService.CacheService;
using XCCloudService.Common;
using XCCloudService.Model.CustomModel.XCCloud;

namespace XCCloudService.Business.XCGameMana
{
    public class XCCloudUserTokenBusiness
    {
        private static object syncRoot = new Object();  

        public static string SetUserToken(string logId, int logType, TokenDataModel dataModel = null)
        {
            //设置用户token      
            string newToken = System.Guid.NewGuid().ToString("N");
            string token = string.Empty;
            if (GetUserTokenModel(logId, logType, out token))
            {
                XCCloudUserTokenCache.Remove(token);
            }

            XCCloudUserTokenModel tokenModel = new XCCloudUserTokenModel(logId, Utils.ConvertDateTimeToLong(DateTime.Now), logType, dataModel);
            XCCloudUserTokenCache.AddToken(newToken, tokenModel);

            return newToken;
        }

        public static void RemoveToken(string token)
        {
            XCCloudUserTokenCache.Remove(token);
        }

        public static XCCloudUserTokenModel GetUserTokenModel(string token)
        {
            if (XCCloudUserTokenCache.ExistToken(token))
            {
                lock (syncRoot)
                {
                    if (XCCloudUserTokenCache.ExistToken(token))
                    {
                        var userTokenModel = (XCCloudUserTokenModel)(XCCloudUserTokenCache.UserTokenHTDic[token]);
                        long lastimestamp = userTokenModel.EndTime;
                        long newtimestamp = Utils.ConvertDateTimeToLong(DateTime.Now);
                        userTokenModel.EndTime = newtimestamp;
                        if ((newtimestamp - lastimestamp) > CacheExpires.CommonPageQueryDataCacheTime)
                        {
                            XCCloudUserTokenCache.Remove(token);
                            userTokenModel = null;
                        }
                        else
                        {
                            XCCloudUserTokenCache.AddToken(token, userTokenModel);
                        }

                        return userTokenModel;
                    }
                }                                                
            }

            return null;
        }

        public static bool GetUserTokenModel(string logId, int logType, out string token)
        {
            token = string.Empty;
            var query = from item in XCCloudUserTokenCache.UserTokenHTDic
                        where ((XCCloudUserTokenModel)(item.Value)).LogId.Equals(logId) && ((XCCloudUserTokenModel)(item.Value)).LogType == logType
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

        /// <summary>
        /// 移除在同一工作站登录的门店用户
        /// </summary>
        /// <param name="logId"></param>
        /// <param name="logType"></param>
        public static void RemoveStoreUserTokenByWorkStaion(string logId, int logType, string workStation)
        {
            var query = from item in XCCloudUserTokenCache.UserTokenHTDic
                        where ((XCCloudUserTokenModel)(item.Value)).LogId.Equals(logId) && ((XCCloudUserTokenModel)(item.Value)).LogType == logType &&
                        ((StoreIDDataModel)((XCCloudUserTokenModel)(item.Value)).DataModel).WorkStation.Equals(workStation)
                        select item.Key.ToString();
            if (query.Count() > 0)
            {
                int i = 0;
                foreach (var q in query)
                {
                    q.Remove(i);
                    i++;
                }
            }
        }

    }
}
