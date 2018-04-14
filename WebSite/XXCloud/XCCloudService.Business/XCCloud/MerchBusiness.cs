using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XCCloudService.BLL.Container;
using XCCloudService.BLL.IBLL.XCCloud;
using XCCloudService.Common;
using XCCloudService.Common.Enum;
using XCCloudService.Model.CustomModel.XCCloud.Merch;
using XCCloudService.Model.XCCloud;

namespace XCCloudService.Business.XCCloud
{
    public class MerchBusiness
    {
        private static List<MerchInfoCacheModel> listMerch = null;

        public static List<MerchInfoCacheModel> MerchInfoList 
        {
            get 
            {
                if (listMerch == null) Init();
                return listMerch; 
            }
        }

        public static void Init()
        {
            IBase_MerchantInfoService base_MerchantInfoService = BLLContainer.Resolve<IBase_MerchantInfoService>();
            List<Base_MerchantInfo> list = base_MerchantInfoService.GetModels().ToList();
            listMerch = Utils.GetCopyList<MerchInfoCacheModel, Base_MerchantInfo>(list);
        }

        public static bool IsEffectiveMerch(string openId, out MerchInfoCacheModel merchInfoCacheModel)
        {
            merchInfoCacheModel = null;
            if (MerchInfoList.Any<MerchInfoCacheModel>(p => p.WxOpenID.Equals(openId, StringComparison.OrdinalIgnoreCase)))
            {
                merchInfoCacheModel = MerchInfoList.Where(p => p.WxOpenID.Equals(openId, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
                return true;
            }

            return false;
        }        
    }
}
