using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using XCCloudService.Business.Common;
using XCCloudService.Common;
using XCCloudService.Common.Enum;
using XCCloudService.Model.CustomModel.Common;
using XCCloudService.SocketService.TCP.Client;
using XCCloudService.SocketService.TCP.Common;

namespace XCCloudService.SocketService.TCP.Business
{
    public class TCPServiceBusiness
    {
        public static XCCloudService.SocketService.TCP.Server Server { set; get; }

        public static void Send(string mobile, object data)
        {
            if (Server != null)
            {
                Socket socket = null;
                string ip = string.Empty;
                LogHelper.SaveLog(TxtLogType.TCPService, TxtLogContentType.Debug, TxtLogFileType.Day, "出币TCPSend:" + mobile + System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                if (Server.GetSocket(mobile, ref socket, out ip))
                {
                    string dataJson = Utils.SerializeObject(data).ToString();
                    byte[] msgBuffer = Server.PackageServerData(dataJson);
                    socket.BeginSendTo(msgBuffer, 0, msgBuffer.Length, SocketFlags.None, socket.RemoteEndPoint, new AsyncCallback(Server.SendCallBack), socket);
                    LogHelper.SaveLog(TxtLogType.TCPService, TxtLogContentType.Debug, TxtLogFileType.Day, "出币BeginSendTo:" + mobile + System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                }
            }
        }
    }
}
