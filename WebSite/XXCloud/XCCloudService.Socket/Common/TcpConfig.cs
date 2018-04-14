using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XCCloudService.SocketService.TCP.Common
{
    public class TcpConfig
    {
        public static string Host = System.Configuration.ConfigurationManager.AppSettings["TCPSocketServiceHost"].ToString();

        public static int Port = int.Parse(System.Configuration.ConfigurationManager.AppSettings["TCPSocketServicePort"].ToString());
    }
}
