using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XCCloudService.BLL.Container;
using XCCloudService.BLL.IBLL.XCCloud;
using XCCloudService.Common;
using XCCloudService.Model.CustomModel.XCCloud.Store;
using XCCloudService.Model.XCCloud;

namespace XCCloudService.Business.XCCloud
{
    public class XCCloudStoreBusiness
    {
        private static List<StoreInfoCacheModel> listStore = null;

        public static List<StoreInfoCacheModel> StoreInfoList 
        {
            get 
            {
                if (listStore == null) Init();
                return listStore; 
            }
        }

        public static void Init()
        {
            IBase_StoreInfoService storeInfoService = BLLContainer.Resolve<IBase_StoreInfoService>();
            List<Base_StoreInfo> list = storeInfoService.GetModels().ToList<Base_StoreInfo>();
            listStore = Utils.GetCopyList<StoreInfoCacheModel, Base_StoreInfo>(list);
        }

        public static bool IsEffectiveStore(string storeId)
        {
            return StoreInfoList.Where<StoreInfoCacheModel>(p => p.StoreID == storeId).Count() > 0; 
        }

        public static bool IsEffectiveStore(string storeId,out string storeName,out string password)
        {
            storeName = string.Empty;
            password = string.Empty;
            StoreInfoCacheModel storeInfo = StoreInfoList.Where<StoreInfoCacheModel>(p => p.StoreID.Equals(storeId, StringComparison.OrdinalIgnoreCase)).FirstOrDefault<StoreInfoCacheModel>();
            if (storeInfo != null)
            {
                storeName = storeInfo.StoreName;
                password = storeInfo.Password;
                return true;
            }
            else
            {
                return false;
            }
        }


        public static bool IsEffectiveStore(string storeId, ref StoreInfoCacheModel storeInfo)
        {
            storeInfo = StoreInfoList.Where<StoreInfoCacheModel>(p => p.StoreID.Equals(storeId, StringComparison.OrdinalIgnoreCase)).FirstOrDefault<StoreInfoCacheModel>();
            if (storeInfo != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 门店列表
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<Model.XCCloud.Base_StoreInfo> GetStoreList()
        {
            BLL.IBLL.XCCloud.IBase_StoreInfoService storeInfoService = BLLContainer.Resolve<BLL.IBLL.XCCloud.IBase_StoreInfoService>();
            var storeList = storeInfoService.GetModels(d => true);
            return storeList;
        }

        /// <summary>
        /// 根据订单号获取订单实体
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public static Model.XCCloud.Base_StoreInfo GetStoreModel(string storeId)
        {
            return GetStoreList().FirstOrDefault(m => m.StoreID.Equals(storeId));
        }
    }
}
