using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using XCCloudService.BLL.Container;
using XCCloudService.BLL.IBLL.XCGameManager;
using XCCloudService.Model.Socket;
using XCCloudService.Model.XCGameManager;

namespace XCCloudService.SocketService.TCP.Common
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
        /// 添加消息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static bool SaveDBLog(t_datamessage model)
        {
            IDataMessageService tcpService = BLLContainer.Resolve<IDataMessageService>();
            tcpService.Add(model);
            return true;
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

            //string xcGameDBName = "XCGameManagerDB";
            //ITCPService tcpService = BLLContainer.Resolve<ITCPService>();
            //DateTime time = DateTime.Now.AddDays(-3);
            //var menulist = tcpService.GetModels(p => p.CreateTime > time && p.ReceiveStoreId == sendStoreId && p.ReceiveMobile == Mobile && p.State == 0).ToList();
            //if (menulist.Count > 0)
            //{
            //    for (int i = 0; i < menulist.Count; i++)
            //    {
            //        string receiveClientID = menulist[i].ReceiveStoreId + "_" + menulist[i].ReceiveMobile;
            //        KeyValuePair<string, Session> pair = SessionPool.FirstOrDefault(p => p.Value.SendClientId.Equals(receiveClientID));
            //        if (pair.Key != null)
            //        {                        
            //            var msgObj1 = new { result_code = 1, content = menulist[i].TextData };
            //            var msg1 = new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(msgObj1);
            //            byte[] msgBuffer1 = PackageServerData(msg1);
            //            SessionPool[pair.Key].SockeClient.Send(msgBuffer1, msgBuffer1.Length, SocketFlags.None);                                              
            //            menulist[i].State = 1;
            //            tcpService.Update(menulist[i], xcGameDBName);
            //        }
            //    }
            //}
        }
    }
}
