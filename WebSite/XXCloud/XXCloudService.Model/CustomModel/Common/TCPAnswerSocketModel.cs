using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;

namespace XCCloudService.Model.CustomModel.Common
{
    public class TCPAnswerSocketModel
    {
        public TCPAnswerSocketModel(System.Net.Sockets.Socket socketClient, string clientId)
        {
            this.SocketClient = socketClient;
            this.ClientId = clientId;
            this.CreateTime = System.DateTime.Now;
        }

        public System.Net.Sockets.Socket SocketClient { set; get; }

        public string ClientId { set; get; }

        public DateTime CreateTime { set; get; }
    }
}
