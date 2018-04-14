using PalletService.SocketService;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Net.Sockets;
using System.Text;

namespace PalletService.TCPService.Common
{
    public class TcpHelper
    {
        private static byte[] PackageServerData(string msg)
        {
            byte[] content = null;
            byte[] temp = Encoding.UTF8.GetBytes(msg);
            if (temp.Length < 126)
            {
                content = new byte[temp.Length + 2];
                content[0] = 0x81;
                content[1] = (byte)temp.Length;
                Buffer.BlockCopy(temp, 0, content, 2, temp.Length);
            }
            else if (temp.Length < 0xFFFF)
            {
                content = new byte[temp.Length + 4];
                content[0] = 0x81;
                content[1] = 126;
                content[2] = (byte)(temp.Length & 0xFF);
                content[3] = (byte)(temp.Length >> 8 & 0xFF);
                Buffer.BlockCopy(temp, 0, content, 4, temp.Length);
            }
            return content;
        }

        /// <summary>
        /// 重发消息
        /// </summary>
        /// <param name="sendStoreId"></param>
        /// <param name="Mobile"></param>
        /// <param name="SessionPool"></param>
        /// <param name="IP"></param>
        public static void Retransmission(string sendStoreId, string Mobile, Dictionary<string, Session> SessionPool, string IP)
        {

        }
    }
}
