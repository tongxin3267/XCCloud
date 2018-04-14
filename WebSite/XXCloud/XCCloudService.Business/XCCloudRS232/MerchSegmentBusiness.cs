using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XCCloudService.BLL.Container;

namespace XCCloudService.Business.XCCloudRS232
{
    public class MerchSegmentBusiness
    {
        /// <summary>
        /// 获取已绑定的终端列表
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<Model.XCCloudRS232.Data_MerchSegment> GetMerchSegmentList()
        {
            BLL.IBLL.XCCloudRS232.IMerchSegmentService merchSegmentService = BLLContainer.Resolve<BLL.IBLL.XCCloudRS232.IMerchSegmentService>();
            var merchDeviceList = merchSegmentService.GetModels(d => true);
            return merchDeviceList;
        }

        /// <summary>
        /// 根据分组编号获取终端列表
        /// </summary>
        /// <param name="parentID"></param>
        /// <returns></returns>
        public static IEnumerable<Model.XCCloudRS232.Data_MerchSegment> GetListByGroupId(int groupId)
        {
            return GetMerchSegmentList().Where(m => m.GroupID == groupId);
        }

        /// <summary>
        /// 根据设备编号获取终端绑定实体
        /// </summary>
        /// <param name="deviceID"></param>
        /// <returns></returns>
        public static Model.XCCloudRS232.Data_MerchSegment GetMerchSegmentModel(int deviceID)
        {
            return GetMerchSegmentList().FirstOrDefault(m => m.DeviceID == deviceID);
        }

        /// <summary>
        /// 根据控制器ID和终端短地址获取终端信息
        /// </summary>
        /// <param name="parentId"></param>
        /// <param name="headAddress"></param>
        /// <returns></returns>
        public static Model.XCCloudRS232.Data_MerchSegment GetMerchSegmentModel(int parentId, string headAddress)
        {
            return GetMerchSegmentList().FirstOrDefault(m => m.ParentID == parentId && m.HeadAddress == headAddress);
        }

        /// <summary>
        /// 添加实体
        /// </summary>
        /// <param name="md"></param>
        /// <returns></returns>
        public static bool AddMerchSegment(Model.XCCloudRS232.Data_MerchSegment ms)
        {
            BLL.IBLL.XCCloudRS232.IMerchSegmentService merchSegmentService = BLLContainer.Resolve<BLL.IBLL.XCCloudRS232.IMerchSegmentService>();
            bool ret = merchSegmentService.Add(ms);
            return ret;
        }

        /// <summary>
        /// 删除实体
        /// </summary>
        /// <param name="md"></param>
        /// <returns></returns>
        public static bool DeleteMerchSegment(Model.XCCloudRS232.Data_MerchSegment ms)
        {
            BLL.IBLL.XCCloudRS232.IMerchSegmentService merchSegmentService = BLLContainer.Resolve<BLL.IBLL.XCCloudRS232.IMerchSegmentService>();
            bool ret = merchSegmentService.Delete(ms);
            return ret;
        }
    }
}
