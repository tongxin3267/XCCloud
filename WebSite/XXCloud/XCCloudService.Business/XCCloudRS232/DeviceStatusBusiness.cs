using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XCCloudService.CacheService;
using XCCloudService.Common.Enum;
using XCCloudService.Common.Extensions;
using XCCloudService.Model.XCCloudRS232;

namespace XCCloudService.Business.XCCloudRS232
{
    public class DeviceStatusBusiness
    {
        public static void SetDeviceState(string token, string state)
        {
            if (DeviceStateCache.ExistTokenByKey(token))
            {
                DeviceStateCache.UpdateTokenByKey(token, state);
            }
            else
            {
                DeviceStateCache.AddToken(token, state);
            }
        }

        public static bool ExistDeviceState(string token)
        {
            return DeviceStateCache.ExistTokenByKey(token);
        }

        public static string GetDeviceState(string token)
        {
            Base_DeviceInfo device = DeviceBusiness.GetDeviceModel(token);

            if (!ExistDeviceState(token))
            {
                if (device.DeviceType == 0)
                {
                    return DeviceStatusEnum.在线.ToDescription();
                }
                return DeviceStatusEnum.离线.ToDescription();
            }

            object obj = DeviceStateCache.GetValueByKey(token);
            string status = obj.ToString();

            //如果设备缓存不存在或等于启用,返回离线状态
            if (obj == null || obj.ToString() == DeviceStatusEnum.启用.ToDescription())
            {
                status = DeviceStatusEnum.离线.ToDescription();
            }
            return status; 
        }
    }
}
