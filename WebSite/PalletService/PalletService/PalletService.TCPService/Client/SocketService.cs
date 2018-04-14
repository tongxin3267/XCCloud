

using PalletService.Business.SysConfig;
using PalletService.Common;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;


namespace PalletService.TCPService.Client
{
    public class ClientSerivce
    {
        private System.Net.Sockets.Socket clientSocket = null;

        public ClientSerivce()
        { 
            
        }

        public ClientSerivce(System.Net.Sockets.Socket clientSocket)
        {
            this.clientSocket = clientSocket;
        }

        public System.Net.Sockets.Socket ClientSocket
        {
            get { return this.clientSocket; }
        }


        //链接socket服务
        public bool ConnectSocket()
        {
            try
            {
                IPAddress ip = IPAddress.Parse(SysConfigBusiness.TCPHost);
                IPEndPoint ipe = new IPEndPoint(ip, SysConfigBusiness.TCPPort);
                clientSocket = new System.Net.Sockets.Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                clientSocket.Connect(ipe);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool ConnectSocket(ref System.Net.Sockets.Socket clientSocket)
        {
            try
            {
                IPAddress ip = IPAddress.Parse(SysConfigBusiness.TCPHost);
                IPEndPoint ipe = new IPEndPoint(ip, SysConfigBusiness.TCPPort);
                clientSocket = new System.Net.Sockets.Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                clientSocket.Connect(ipe);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 发送文本内容
        /// </summary>
        /// <param name="data"></param>
        public void Send(string data)
        {
            clientSocket.Send(System.Text.Encoding.UTF8.GetBytes(data));
        }
    }
}
