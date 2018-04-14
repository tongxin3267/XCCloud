using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XCCloudService.Common.Enum;
using XCCloudService.SocketService.UDP.Common;

namespace XCCloudService.SocketService.UDP
{
    public class ClientService
    {
        public void Connection()
        {
            Client.Init(UDPConfig.Host, UDPConfig.Port, System.Guid.NewGuid(), UDPSocketClientType.串口通讯服务);

        }

        public void Send(byte[] data)
        {
            Client.Send(data);
        }
    }
}
