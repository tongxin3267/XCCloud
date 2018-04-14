using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using XCCloudService.BLL.Container;
using XCCloudService.BLL.IBLL.XCGame;
using XCCloudService.Model.XCGame;

namespace XCCloudService.Business.XCGame
{
    public class DeviceBusiness
    {
        public bool IsEffectiveStoreDevice(string xcGameDBName, string deviceMCUID, out string errMsg)
        {
            errMsg = string.Empty;

            IDeviceService deviceService = BLLContainer.Resolve<IDeviceService>(xcGameDBName);
            var deviceModel = deviceService.GetModels(p => p.MCUID.ToString().Equals(deviceMCUID, StringComparison.OrdinalIgnoreCase)).FirstOrDefault<t_device>();
            if (deviceModel == null)
            {
                errMsg = "设备不存在";
                return false;
            }

            return true;
        }

        public bool IsEffectiveStoreSegment(string dbName, string segment, out string errMsg)
        {
            errMsg = string.Empty;

            IDeviceService deviceService = BLLContainer.Resolve<IDeviceService>(dbName);
            int count = deviceService.GetModels(p => p.segment.Equals(segment, StringComparison.OrdinalIgnoreCase)).Count<t_device>();
            if (count == 0)
            {
                errMsg = "路由器段地址无效";
                return false;
            }

            return true;
        }
    }
}