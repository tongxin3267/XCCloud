using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XCCloudService.BLL.Container;
using XCCloudService.BLL.IBLL.XCGameManagerLog;
using XCCloudService.Model.XCGameManagerLog;

namespace XCCloudService.Business.XCGameManaLog
{
    public class UDPDBBusiness
    {
        public static void SaveRadarRegisterDBLog(t_UDPRadarRegisterLog radarRegisterLog)
        {
            IUDPRadarRegisterLogService service = BLLContainer.Resolve<IUDPRadarRegisterLogService>();
            service.Add(radarRegisterLog);
        }

        public static void SaveRadarHeatDBLog(t_UDPRadarHeatLog radarHeatLog)
        {
            IUDPRadarHeatLogService service = BLLContainer.Resolve<IUDPRadarHeatLogService>();
            service.Add(radarHeatLog);
        }

        public static void SaveDeviceStateChangeDBLog(t_UDPDeviceStateChangeLog deviceStateChangeLog)
        {
            IUDPDeviceStateChangeLogService service = BLLContainer.Resolve<IUDPDeviceStateChangeLogService>();
            service.Add(deviceStateChangeLog);
        }

        public static void SaveUDPSendDeviceControlLog(t_UDPSendDeviceControlLog sendDeviceControlLog)
        {
            IUDPSendDeviceControlLogService service = BLLContainer.Resolve<IUDPSendDeviceControlLogService>();
            service.Add(sendDeviceControlLog);
        }

        public static void SaveUDPRadarNotify(t_UDPRadarNotifyLog radarNotifyLog)
        {
            IUDPRadarNotifyLogService service = BLLContainer.Resolve<IUDPRadarNotifyLogService>();
            service.Add(radarNotifyLog);
        }

        public static void SaveUDPDeviceControlLog(t_UDPDeviceControlLog deviceControlLog)
        {
            IUDPDeviceControlLogService service = BLLContainer.Resolve<IUDPDeviceControlLogService>();
            service.Add(deviceControlLog);
        }
    }
}
