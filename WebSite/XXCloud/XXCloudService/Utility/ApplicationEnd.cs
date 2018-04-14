using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XCCloudService.Common;
using XCCloudService.Common.Enum;
using XCCloudService.SocketService.TCP.Business;

namespace XCCloudService.Utility
{
    public class ApplicationEnd
    {
        public static void End()
        {
            LogHelper.SaveLog(TxtLogType.SystemInit, "********************************Application End********************************");
            //TCPSocketEnd();
            //UDPSocketEnd();
        }

        private static void TCPSocketEnd()
        {
            try
            { 
                TCPServiceBusiness.Server.End();
                LogHelper.SaveLog(TxtLogType.SystemInit, "TCP End Success...");
            }
            catch (Exception ex)
            {
                LogHelper.SaveLog(TxtLogType.SystemInit, "TCP End Fail..." + Utils.GetException(ex));
            }      
        }

        private static void UDPSocketEnd()
        {
            try
            {
                XCCloudService.SocketService.UDP.Server.End();
                LogHelper.SaveLog(TxtLogType.SystemInit, "UDP End Success...");
            }
            catch (Exception ex)
            {
                LogHelper.SaveLog(TxtLogType.SystemInit, "UDP End Fail..." + Utils.GetException(ex));
            }
        }
    }
}
