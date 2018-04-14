using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XCCloudService.BLL.Container;
using XCCloudService.Model.XCGameManager;

namespace XCCloudService.Business.XCGameMana
{
    public class DeviceManaBusiness
    {
        private static List<Model.XCGameManager.t_device> deviceList = new List<t_device>();


        public static bool CheckDevice(string mcuId,ref Model.XCGameManager.t_device deviceModel)
        {
            deviceModel = deviceList.Where(p => p.DeviceId.Equals(mcuId)).FirstOrDefault<Model.XCGameManager.t_device>();
            if (deviceModel == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public static void Init()
        {
            BLL.IBLL.XCGameManager.IDeviceService deviceService = BLLContainer.Resolve<BLL.IBLL.XCGameManager.IDeviceService>();
            var deviceModelList = deviceService.GetModels(p => 1==1 ).ToList<Model.XCGameManager.t_device>();
            if (deviceModelList != null && deviceModelList.Count > 0 )
            {
                for (int i = 0; i < deviceModelList.Count; i++)
                { 
                    deviceList.Add(deviceModelList[i]);
                }      
            }
        }

        public static string GetDeviceToken()
        { 
            Random rd = new Random();
            int num = rd.Next(0,int.MaxValue);
            byte[] bData = BitConverter.GetBytes(num);
            return BitConverter.ToString(bData).Replace("-","").ToLower();
        }

        public static bool ExistDevice(string deviceToken, out string storeId)
        {
            storeId = string.Empty;
            BLL.IBLL.XCGameManager.IDeviceService deviceService = BLLContainer.Resolve<BLL.IBLL.XCGameManager.IDeviceService>();
            var deviceModel = deviceService.GetModels(p => p.TerminalNo.Equals(deviceToken, StringComparison.OrdinalIgnoreCase)).FirstOrDefault<Model.XCGameManager.t_device>();
            if (deviceModel == null)
            {
                return false;
            }
            else
            {
                storeId = deviceModel.StoreId;
                return true;
            }
        }

        public static bool ExistDevice(string deviceToken, ref t_device deviceModel,out string errMsg)
        {
            errMsg = string.Empty ;
            BLL.IBLL.XCGameManager.IDeviceService deviceService = BLLContainer.Resolve<BLL.IBLL.XCGameManager.IDeviceService>();
            deviceModel = deviceService.GetModels(p => p.TerminalNo.Equals(deviceToken, StringComparison.OrdinalIgnoreCase)).FirstOrDefault<Model.XCGameManager.t_device>();
            if (deviceModel == null)
            {
                errMsg = "设备信息不存在";
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}
