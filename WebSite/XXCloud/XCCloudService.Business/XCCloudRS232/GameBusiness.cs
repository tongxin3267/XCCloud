using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XCCloudService.BLL.Container;

namespace XCCloudService.Business.XCCloudRS232
{
    public class GameBusiness
    {
        /// <summary>
        /// 获取分组列表
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<Model.XCCloudRS232.Data_GameInfo> GetGameList()
        {
            BLL.IBLL.XCCloudRS232.IDataGameInfoService deviceService = BLLContainer.Resolve<BLL.IBLL.XCCloudRS232.IDataGameInfoService>();
            var deviceList = deviceService.GetModels(d => true);
            return deviceList;
        }

        /// <summary>
        /// 根据分组ID获取分组实体
        /// </summary>
        /// <param name="groupId"></param>
        /// <returns></returns>
        public static Model.XCCloudRS232.Data_GameInfo GetGameInfoModel(int groupId)
        {
            return GetGameList().FirstOrDefault(m => m.GroupID == groupId);
        }
    }
}
