using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using XCCloudService.Common.Enum;



namespace XCCloudService.SocketService.TCP
{
    public class Session
    {
        public Session()
        {
            _sendClientId = string.Empty;
            _ip = string.Empty;
        }

        private System.Net.Sockets.Socket _sockeclient;
        private byte[] _buffer;
        private string _ip;
        private SocketClientType _socketClientType;
        private string _sendClientId;

        public System.Net.Sockets.Socket SockeClient
        {
            set { _sockeclient = value; }
            get { return _sockeclient; }
        }

        public byte[] buffer
        {
            set { _buffer = value; }
            get { return _buffer; }
        }

        public string IP
        {
            set { _ip = value; }
            get { return _ip; }
        }

        public SocketClientType SocketClientType
        {
            set { _socketClientType = value; }
            get { return _socketClientType; }
        }

        public string SendClientId
        {
            set { _sendClientId = value; }
            get { return _sendClientId; }
        }
    }
}
