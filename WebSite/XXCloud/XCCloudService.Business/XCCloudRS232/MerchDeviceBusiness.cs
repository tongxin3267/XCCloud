using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XCCloudService.BLL.Container;

namespace XCCloudService.Business.XCCloudRS232
{
    public class MerchDeviceBusiness
    {
        /// <summary>
        /// 获取已绑定的外设列表
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<Model.XCCloudRS232.Data_MerchDevice> GetMerchDeviceList()
        {
            BLL.IBLL.XCCloudRS232.IMerchDeviceService merchDeviceService = BLLContainer.Resolve<BLL.IBLL.XCCloudRS232.IMerchDeviceService>();
            var merchDeviceList = merchDeviceService.GetModels(d => true);
            return merchDeviceList;
        }

        /// <summary>
        /// 根据设备编号获取实体
        /// </summary>
        /// <param name="deviceID"></param>
        /// <returns></returns>
        public static Model.XCCloudRS232.Data_MerchDevice GetMerchDeviceModel(int deviceID)
        {
            return GetMerchDeviceList().FirstOrDefault(m => m.DeviceID == deviceID);
        }

        /// <summary>
        /// 根据控制器编号获取外设列表
        /// </summary>
        /// <param name="parentID"></param>
        /// <returns></returns>
        public static IEnumerable<Model.XCCloudRS232.Data_MerchDevice> GetListByParentId(int parentId)
        {
            return GetMerchDeviceList().Where(m => m.ParentID == parentId);
        }

        /// <summary>
        /// 根据控制器和外设短地址获取外设绑定信息
        /// </summary>
        /// <param name="parentId"></param>
        /// <param name="headAddress"></param>
        /// <returns></returns>
        public static Model.XCCloudRS232.Data_MerchDevice GetMerchModel(int parentId, string headAddress)
        {
            return GetMerchDeviceList().FirstOrDefault(m => m.ParentID == parentId && m.HeadAddress == headAddress);
        }

        /// <summary>
        /// 添加实体
        /// </summary>
        /// <param name="md"></param>
        /// <returns></returns>
        public static bool AddMerchDevice(Model.XCCloudRS232.Data_MerchDevice md)
        {
            BLL.IBLL.XCCloudRS232.IMerchDeviceService deviceService = BLLContainer.Resolve<BLL.IBLL.XCCloudRS232.IMerchDeviceService>();
            bool ret = deviceService.Add(md);
            return ret;
        }

        /// <summary>
        /// 删除实体
        /// </summary>
        /// <param name="md"></param>
        /// <returns></returns>
        public static bool DeleteMerchDevice(Model.XCCloudRS232.Data_MerchDevice md)
        {
            BLL.IBLL.XCCloudRS232.IMerchDeviceService deviceService = BLLContainer.Resolve<BLL.IBLL.XCCloudRS232.IMerchDeviceService>();
            bool ret = deviceService.Delete(md);
            return ret;
        }
    }
}
