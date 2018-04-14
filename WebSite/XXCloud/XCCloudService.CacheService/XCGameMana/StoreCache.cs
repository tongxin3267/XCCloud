using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XCCloudService.Model.CustomModel.XCGameManager;

namespace XCCloudService.CacheService.XCGameMana
{
    public class StoreCache
    {
        private static List<StoreCacheModel> storeList = null;

        public static void Add(List<StoreCacheModel> list)
        {
            storeList = list;
        }

        public static List<StoreCacheModel> GetStore()
        {
            return storeList;
        }
    }

    public class StoreDogCache
    {
        private static List<StoreDogCacheModel> storeDogList = null;

        public static void Add(List<StoreDogCacheModel> list)
        {
            storeDogList = list;
        }

        public static List<StoreDogCacheModel> GetStore()
        {
            return storeDogList;
        }
      
        public static bool ExistDogId(string storeId,string dogId)
        {
            storeDogList = storeDogList.Where(p => p.StoreID == storeId && p.DogId == dogId).ToList() ;
            if (storeDogList.Count > 0)
            {
                return true;
            }
            //int count = storeDogList.Where<StoreDogCacheModel>(p=>p.StoreID==storeId && p.DogId==dogId).Count<StoreDogCacheModel>();
            return false;
        }
    }
}
