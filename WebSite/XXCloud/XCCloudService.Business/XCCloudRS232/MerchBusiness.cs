using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XCCloudService.BLL.Container;
using XCCloudService.Model.XCCloudRS232;

namespace XCCloudService.Business.XCCloudRS232
{
    public class MerchBusiness
    {
        public static List<Base_MerchInfo> merchList = new List<Base_MerchInfo>();
        public static void Init()
        {
            BLL.IBLL.XCCloudRS232.IMerchService deviceService = BLLContainer.Resolve<BLL.IBLL.XCCloudRS232.IMerchService>();
            var merch = deviceService.GetModels(d => true).ToList<Base_MerchInfo>();
            merchList.AddRange(merch);
        }
        /// <summary>
        /// 获取商户列表
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<Model.XCCloudRS232.Base_MerchInfo> GetMerchList()
        {
            BLL.IBLL.XCCloudRS232.IMerchService deviceService = BLLContainer.Resolve<BLL.IBLL.XCCloudRS232.IMerchService>();
            var merchList = deviceService.GetModels(d => true);
            return merchList;
        }

        /// <summary>
        /// 根据token获取商户实体
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public static Model.XCCloudRS232.Base_MerchInfo GetMerchModel(string token)
        {
            return GetMerchList().FirstOrDefault(m => m.Token.Equals(token));
        }

        public static bool IsEffectiveMerch(string storeId, ref Base_MerchInfo merchInfo, out string errMsg)
        {
            errMsg = string.Empty;
            merchInfo = merchList.Where<Base_MerchInfo>(p => p.ID.ToString().Equals(storeId)).FirstOrDefault<Base_MerchInfo>();
            if (merchInfo == null)
            {
                errMsg = "门店信息不存在";
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}
