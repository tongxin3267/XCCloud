using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XCCloudService.SocketService.UDP.Common
{
    public class UDPConfig
    {
        public static string Host = System.Configuration.ConfigurationManager.AppSettings["UDPSocketServiceHost"].ToString();

        public static int Port = int.Parse(System.Configuration.ConfigurationManager.AppSettings["UDPSocketServicePort"].ToString());
    }
}
