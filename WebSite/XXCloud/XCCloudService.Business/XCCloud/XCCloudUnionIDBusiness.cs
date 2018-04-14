using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using XCCloudService.CacheService;
using XCCloudService.Common;
using XCCloudService.Model.CustomModel.XCGame;



namespace XCCloudService.Business.XCCloud
{
    public class XCCloudUnionIDBusiness
    {
        public static void SetUnionID(string openId, string unionId)
        {
            string key = Constant.WeiXinUnionId + "_" + openId;
            if (!XCCloudUnionIDCache.ExistByKey(key))
            {
                XCCloudUnionIDCache.Add(key, unionId);
            }
            else
            {
                XCCloudUnionIDCache.UpdateByKey(key, unionId);
            }
        }


        /// <summary>
        /// 获取unionId对应的Key实体
        /// </summary>
        /// <param name="unionId"></param>
        /// <param name="openId"></param>
        /// <returns></returns>
        public static bool GetOpenID(string unionId, out string openId)
        {
            openId = string.Empty;
            var query = from item in XCCloudUnionIDCache.UnionIDHt.Cast<DictionaryEntry>()
                        where item.Value.ToString().Equals(unionId)
                        select item.Key.ToString();
            if (query.Count() == 0)
            {
                return false;
            }

            string key = query.First();
            string[] keyArr = key.Split('_');
            if (keyArr.Length != 2)
            {
                return false;
            }

            openId = keyArr[1];
            return true;
        }

        /// <summary>
        /// 获取unionId值
        /// </summary>
        /// <param name="storeId"></param>
        /// <param name="segment"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static bool GetUnionID(string openId, out string unionId)
        {
            unionId = string.Empty;
            string key = Constant.WeiXinUnionId + "_" + unionId;
            var query = from item in XCCloudUnionIDCache.UnionIDHt.Cast<DictionaryEntry>()
                        where item.Key.ToString().Equals(key)
                        select item.Value.ToString();
            if (query.Count() == 0)
            {
                return false;
            }
            else
            {
                unionId = query.First();
                return true;
            }
        }
    }
}