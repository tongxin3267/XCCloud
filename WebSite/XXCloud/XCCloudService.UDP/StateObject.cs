using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;


namespace XCCloudService.SocketService.UDP
{
    public class StateObject
    {
        public const int BUF_SIZE = 1024 * 128;
        public Socket socket;
        public byte[] buffer = new byte[BUF_SIZE];
    }
}
