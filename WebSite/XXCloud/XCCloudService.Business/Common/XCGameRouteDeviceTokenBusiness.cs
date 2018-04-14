using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using XCCloudService.CacheService;
using XCCloudService.Common;
using XCCloudService.Model.CustomModel.XCGame;



namespace XCCloudService.Business.Common
{
    public class XCGameRadarDeviceTokenBusiness
    {
        public static string SetRadarDeviceToken(string storeId, string segment)
        {
            //设置会员token
            string key = storeId + "_" + segment;
            string token = System.Guid.NewGuid().ToString("N");
            if (XCGameRouteDeviceCache.ExistTokenByKey(key))
            {
                XCGameRouteDeviceCache.AddToken(key, token);
            }
            else
            {
                XCGameRouteDeviceCache.UpdateTokenByKey(key, token);
            }
            return token;
        }


        /// <summary>
        /// 获取token对应的Key实体
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public static XCGameRadarDeviceTokenModel GetRadarDeviceTokenModel(string token)
        {
            var query = from item in XCGameRouteDeviceCache.DeviceHt.Cast<DictionaryEntry>()
                        where item.Value.ToString().Equals(token)
                        select item.Key.ToString();
            if (query.Count() == 0)
            {
                return null;
            }

            string key = query.First();
            string[] keyArr = key.Split('_');
            if (keyArr.Length != 2)
            {
                return null;
            }

            XCGameRadarDeviceTokenModel model = new XCGameRadarDeviceTokenModel(keyArr[0], keyArr[1]);
            return model;
        }

        /// <summary>
        /// 获取雷达token值
        /// </summary>
        /// <param name="storeId"></param>
        /// <param name="segment"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static bool GetRouteDeviceToken(string storeId, string segment,out string token)
        {
            token = string.Empty;
            string key = storeId + "_" + segment;
            var query = from item in XCGameRouteDeviceCache.DeviceHt.Cast<DictionaryEntry>()
                        where item.Key.ToString().Equals(key)
                        select item.Value.ToString();
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


        public static bool GetRouteDeviceToken(string storeId,out string token)
        {
            token = string.Empty;
            string key = storeId + "_";
            var query = from item in XCGameRouteDeviceCache.DeviceHt.Cast<DictionaryEntry>()
                        where item.Key.ToString().Contains(key)
                        select item.Value.ToString();
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