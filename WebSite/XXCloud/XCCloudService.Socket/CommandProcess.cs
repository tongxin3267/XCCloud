using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using XCCloudService.Common;
using XCCloudService.SocketService.TCP.Model;
using XCCloudService.Business.XCGameMana;
using XCCloudService.SocketService.TCP.Common;
using XCCloudService.Business.XCGame;
using XCCloudService.Common.Enum;
using XCCloudService.Business.Common;
using XCCloudService.Model.Socket;
using XCCloudService.Model.XCGameManager;

namespace XCCloudService.SocketService.TCP
{
    public class CommandProcessHandler
    {
        #region 发送数据
        /// <summary>
        /// 把发送给客户端消息打包处理（拼接上谁什么时候发的什么消息）
        /// </summary>
        /// <returns>The data.</returns>
        /// <param name="message">Message.</param>
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
        #endregion

        /// <summary>
        /// 用户注册
        /// </summary>
        /// <param name="commonObject"></param>
        /// <param name="SessionPool"></param>
        /// <param name="IP"></param>
        public static void Register(Socket socketClient,string msg, Dictionary<string, Session> sessionPool, string IP)
        {
            string mobile = string.Empty;
            string errMsg = string.Empty;
            //解析消息对象
            SocketDataModel<MemberSendObject, MemberReceiveObject> socketDataModel = Utils.DataContractJsonDeserializer<SocketDataModel<MemberSendObject, MemberReceiveObject>>(msg);
            MemberSendObject sendObj = socketDataModel.SendObject;

            //验证解析token
            if (MobileTokenBusiness.ExistToken(sendObj.Token, out mobile))
            {
                //设置客户端对象
                List<string> repeatIPList = new List<string>();
                var pair = sessionPool.Where(p => p.Value.SendClientId.Equals(mobile));
                foreach (var p in pair)
                {
                    if (!p.Value.IP.Equals(IP))
                    {
                        repeatIPList.Add(p.Value.IP);
                    }
                }

                foreach(var v in repeatIPList)
                {
                    sessionPool.Remove(v);
                }

                sessionPool[IP].SendClientId = mobile;
                sessionPool[IP].SockeClient = socketClient;
                //应答数据
                var msgObj = new { result_code = 1,answerMsgType = "0",answerMsg = "注册成功" };
                                                                
                SendData(msgObj, sessionPool, IP);
            }
            else
            {
                //应答数据
                var msgObj = new { result_code = 0, result_msg = "手机token无效" };
                SendData(msgObj, sessionPool, IP);
            }
        }

        /// <summary>
        /// 消息推送
        /// </summary>
        /// <param name="socketDataModel"></param>
        /// <param name="SessionPool"></param>
        /// <param name="IP"></param>
        public static void Push(Socket sockeClient, string msg, Dictionary<string, Session> sessionPool, string sendIP)
        {
            string receiveIP = string.Empty;
            string sendMobile = string.Empty;
            string receiveMobile = string.Empty;
            string errMsg = string.Empty;
            //解析消息对象
            SocketDataModel<MemberSendObject, MemberReceiveObject> socketDataModel = Utils.DataContractJsonDeserializer<SocketDataModel<MemberSendObject, MemberReceiveObject>>(msg);
            MemberSendObject sendObj = socketDataModel.SendObject;
            MemberReceiveObject receiveObj = socketDataModel.ReceiveObject;

            if (!MobileTokenBusiness.ExistToken(sendObj.Token, out sendMobile))
            {
                //如果发送方token不正确
                var msgObj = new { result_code = 0, errMsg = "手机token无效" };
                //TcpHelper.SaveDBLog(socketDataModel.MessageType, "", sendObj.Token, receiveObj.StoreId, receiveObj.Mobile, socketDataModel.Data.ToString(), "0");
                SendData(msgObj, sessionPool, sendIP);
            }

            //接受信息方是否存在
            if (!GetIP(sessionPool, receiveObj.Mobile, out receiveIP))
            {
                //如果发送方token不正确
                var msgObj = new { result_code = 0, errMsg = "对方不在线" };
                //tb.show(socketDataModel.MessageType, sendObj.StoreId, sendObj.Token, receiveObj.StoreId, receiveObj.Mobile, socketDataModel.Data.ToString(), "0");
                SendData(msgObj, sessionPool, sendIP);
            }
            else
            {
                TcpHelper tb = new TcpHelper();

                //发送消息给接受方
                var msgObj1 = new { result_code = 1, content = socketDataModel.Data };
                SendData(msgObj1, sessionPool, receiveIP);

                //提示发送方消息发送成功
                var msgObj2 = new { result_code = 1 };
                SendData(msgObj2, sessionPool, sendIP);

                //记录TCP日志
                //DataMessageModel logModel = new DataMessageModel();
                //logModel.ManageType = socketDataModel.MessageType;
                //TcpHelper.SaveDBLog(socketDataModel.MessageType, "", sendMobile, "", receiveObj.Mobile, socketDataModel.Data.ToString(), "1");
            }
        }


        public static void RadarAnswer(Socket sockeClient, string msg, Dictionary<string, Session> sessionPool, string sendIP)
        {
            string receiveIP = string.Empty;
            string receiveMobile = string.Empty;
            string errMsg = string.Empty;
            //解析消息对象
            SocketDataModel<RadarSendObject, MemberReceiveObject, RadarAnswerObj> socketDataModel = Utils.DataContractJsonDeserializer<SocketDataModel<RadarSendObject, MemberReceiveObject, RadarAnswerObj>>(msg);
            RadarSendObject sendObj = socketDataModel.SendObject;
            MemberReceiveObject receiveObj = socketDataModel.ReceiveObject;
            RadarAnswerObj radarAnswerObj = socketDataModel.Data;
            //接受信息方是否存在
            if (!GetIP(sessionPool, receiveObj.Mobile, out receiveIP))
            {
                //tb.show(socketDataModel.MessageType, sendObj.StoreId, sendObj.Token, receiveObj.StoreId, receiveObj.Mobile, socketDataModel.Data.ToString(), "0");
                return;
            }

            TcpHelper tb = new TcpHelper();

            //发送消息给接受方
            //var msgObj = Utils.DeserializeObject(socketDataModel.Data.ToString());            
            SendData(radarAnswerObj, sessionPool, receiveIP);

            //tb.show(socketDataModel.MessageType, sendObj.StoreId, sendMobile, "", receiveObj.Mobile, socketDataModel.Data.ToString(), "1");

        }


        /// <summary>
        /// 向客户端发送消息
        /// </summary>
        /// <param name="data"></param>
        /// <param name="sessionPool"></param>
        /// <param name="ip"></param>
        private static void SendData(object data, Dictionary<string, Session> sessionPool,string ip)
        {
            var msg = new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(data);
            byte[] msgBuffer = PackageServerData(msg);
            sessionPool[ip].SockeClient.BeginSendTo(msgBuffer, 0, msgBuffer.Length, SocketFlags.None, sessionPool[ip].SockeClient.RemoteEndPoint, new AsyncCallback(SendCallBack), sessionPool[ip].SockeClient);
        }

        public static bool GetIP(Dictionary<string, Session> sessionPool,string mobile, out string ip)
        {
            ip = string.Empty;
            string receiveClientID = mobile;
            KeyValuePair<string, Session> pair = sessionPool.FirstOrDefault(p => p.Value.SendClientId.Equals(receiveClientID));
            if (pair.Key == null)
            {
                return false;
            }
            else
            {
                ip = pair.Key;
                return true;
            }
        }

        public static void SendCallBack(IAsyncResult ar)
        {
            try
            {
                Socket Client = (Socket)ar.AsyncState;
                Client.EndSend(ar);
            }
            catch (Exception e)
            {
                //LogHelper.WriteLog(e);
            }
        }

    }
}
