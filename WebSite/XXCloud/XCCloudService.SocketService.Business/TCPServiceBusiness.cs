using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace XCCloudService.SocketService.Business
{
    public class TCPServiceBusiness
    {
        public static XCCloudService.SocketService.TCP.Server Server { set; get; }

        public static void Send(string mobile, string data)
        {
            if (Server != null)
            {
                Socket socket = null;
                string ip = string.Empty;
                if (Server.GetEndPoint(mobile, ref socket, out ip))
                {
                    //var msg = new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(data);
                    byte[] msgBuffer = Server.PackageServerData(data);
                    //Server.SockeServer.BeginSendTo(msgBuffer, 0, msgBuffer.Length, SocketFlags.None, endPoint, new AsyncCallback(Server.SendCallBack), Server.SockeServer);
                    //socket.Send(msgBuffer);
                    //socket.BeginSendTo(msgBuffer, 0, msgBuffer.Length, SocketFlags.None, socket.RemoteEndPoint, new AsyncCallback(Server.SendCallBack), socket);
                    socket.SendTo(msgBuffer, socket.RemoteEndPoint);
                }
            }
        }
    }
}
