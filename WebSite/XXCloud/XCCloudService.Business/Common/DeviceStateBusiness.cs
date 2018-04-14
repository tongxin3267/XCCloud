using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using XCCloudService.CacheService;


namespace XCCloudService.Business.Common
{
    public class DeviceStateBusiness
    {
        public static string GetStateName(string state)
        {
            switch (state)
            {
                case "0": return "设备离线"; break;
                case "1": return "设备正常"; break;
                case "2": return "出币中"; break;
                case "3": return "设备故障"; break;
                case "4": return "设备锁定"; break;
                default: return "设备离线";
            }
        }

        public static void SetDeviceState(string storeId, string deviceId, string state)
        {
            string key = storeId + "_" + deviceId;
            if (DeviceStateCache.ExistTokenByKey(key))
            {
                DeviceStateCache.UpdateTokenByKey(key, state);
            }
            else
            {
                DeviceStateCache.AddToken(key, state);
            }
        }

        public static bool ExistDeviceState(string storeId, string deviceId)
        {
            string key = storeId + "_" + deviceId;
            return DeviceStateCache.ExistTokenByKey(key);
        }

        public static string GetDeviceState(string storeId, string deviceId)
        {
            string key = storeId + "_" + deviceId;
            object obj = DeviceStateCache.GetValueByKey(key);
            return (obj == null) ? "0" : obj.ToString(); //如果设备缓存不存在，返回离线状态
        }
    }
}