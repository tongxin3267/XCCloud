﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Net.Sockets;
using System.Net;
using System.Text.RegularExpressions;
using System.Security.Cryptography;
using PalletService.TCPService;
using PalletService.Common;
using PalletService.TCPService.Common;




namespace PalletService.SocketService
{
    public class Server
    {
        private Dictionary<string, Session> SessionPool = new Dictionary<string, Session>();
        private Dictionary<string, string> MsgPool = new Dictionary<string, string>();
        private Socket sockeServer;

        public Socket SockeServer
        {
            set { sockeServer = value; }
            get { return sockeServer; }
        }

        public bool GetSocket(string mobile,ref Socket socket,out string ip)
        {
            ip = string.Empty;
            return false;
            
            //socket = null;
            //string receiveClientID = mobile;
            //KeyValuePair<string, Session> pair = SessionPool.FirstOrDefault(p => p.Value.SendClientId.Equals(receiveClientID));
            //if (pair.Key == null)
            //{
            //    return false;
            //}
            //else
            //{
            //    ip = pair.Key;
            //}

            //if (SessionPool[ip] != null)
            //{
            //    socket = SessionPool[ip].SockeClient;
            //    return true;
            //}
            //else
            //{
            //    return false;
            //}
        }

        #region 启动WebSocket服务
        /// <summary>
        /// 启动WebSocket服务
        /// </summary>
        public void Start(int port)
        {
            sockeServer = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            sockeServer.Bind(new IPEndPoint(IPAddress.Any, port));
            sockeServer.Listen(2000);
            sockeServer.BeginAccept(new AsyncCallback(Accept), sockeServer);
        }

        #endregion

        public void End()
        {
            sockeServer.Close();
        }


        #region 处理客户端连接请求
        /// <summary>
        /// 处理客户端连接请求
        /// </summary>
        /// <param name="result"></param>
        private void Accept(IAsyncResult socket)
        {
            // 还原传入的原始套接字
            Socket SockeServer = (Socket)socket.AsyncState;
            // 在原始套接字上调用EndAccept方法，返回新的套接字
            Socket SockeClient = SockeServer.EndAccept(socket);
            byte[] buffer = new byte[4096];
            try
            {
                //接收客户端的数据
                SockeClient.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(Recieve), SockeClient);
                //保存登录的客户端
                Session session = new Session();
                session.SockeClient = SockeClient;
                session.IP = SockeClient.RemoteEndPoint.ToString();
                session.buffer = buffer;
                lock (SessionPool)
                {
                    if (SessionPool.ContainsKey(session.IP))
                    {
                        SessionPool.Remove(session.IP);
                    }
                    SessionPool.Add(session.IP, session);
                    session.SocketClientType = SocketClientType.Client;
                }
                //准备接受下一个客户端
                SockeServer.BeginAccept(new AsyncCallback(Accept), SockeServer);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error : " + ex.ToString());
                LogHelper.SaveLog(TxtLogType.TCPService, TxtLogContentType.Exception, TxtLogFileType.Time, "Accept:" + ex.Message);
            }
        }
        #endregion


        #region 处理接收的数据
        /// <summary>
        /// 处理接受的数据
        /// </summary>
        /// <param name="socket"></param>
        private void Recieve(IAsyncResult socket)
        {
            Socket SockeClient = (Socket)socket.AsyncState;
            string IP = SockeClient.RemoteEndPoint.ToString();
            if (SockeClient == null || !SessionPool.ContainsKey(IP))
            {
                return;
            }
            try
            {
                int length = SockeClient.EndReceive(socket);
                if (length > 0)
                {
                    byte[] buffer = SessionPool[IP].buffer;
                    SockeClient.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(Recieve), SockeClient);
                    string msg = Encoding.UTF8.GetString(buffer, 0, length);

                    //  websocket建立连接的时候，除了TCP连接的三次握手，websocket协议中客户端与服务器想建立连接需要一次额外的握手动作
                    SocketClientType socketClientType = SocketClientType.UnknownClient;
                    if (TCPClientHelper.IsClientConnctionRequest(msg, out socketClientType))
                    {
                        SockeClient.Send(PackageHandShakeData(buffer, length));
                        SessionPool[IP].SocketClientType = socketClientType;
                        return;
                    }

                    if (TCPClientHelper.IsConnected(SessionPool, IP))
                    {
                        if (SessionPool[IP].SocketClientType == SocketClientType.WebClient)
                        {
                            msg = AnalyzeClientData(buffer, length);
                        }
                        CommandProcess(SockeClient, msg, IP);
                    }
                }
            }
            catch
            {
                try
                {
                    //SockeClient.Disconnect(true);
                    CommandProcessHandler.ExceptionAnswer(SockeClient);
                }
                catch
                {

                }
                finally
                { 
                    //SessionPool.Remove(IP);
                }
            }
        }
        #endregion

        public void SendCallBack(IAsyncResult ar)
        {
            try
            {
                Socket Client = (Socket)ar.AsyncState;
                Client.EndSend(ar);
            }
            catch (Exception e)
            {
                LogHelper.SaveLog("few");
            }
        }


        private void CommandProcess(Socket sockeClient, string msg, string IP)
        {
            int messageType = 0;
            try
            {
                var msgObj = Utils.DeserializeObject(msg);
                if (Utils.JsonObjectExistProperty(msgObj, "msgType"))
                {
                    messageType = int.Parse(Utils.GetJsonObjectValue(msgObj, "msgType").ToString());
                }
                else
                {
                    return;
                }
            }
            catch
            {
                return;
            }

            switch (messageType)
            {
                case (int)(TCPMessageType.用户客户端注册): CommandProcessHandler.Register(sockeClient, msg, SessionPool, IP); break;
                case (int)(TCPMessageType.读取机器名): CommandProcessHandler.GetMachineName(msg, SessionPool, IP); break;
                case (int)(TCPMessageType.读取加密狗): CommandProcessHandler.GetDogId(msg,SessionPool, IP); break;
                case (int)(TCPMessageType.读取身份证): CommandProcessHandler.GetPeopleCard(msg, SessionPool, IP); break;
                case (int)(TCPMessageType.读取IC卡): CommandProcessHandler.GetICCard(msg, SessionPool, IP); break;
                case (int)(TCPMessageType.办理新IC卡): CommandProcessHandler.CreateICCard(msg, SessionPool, IP); break;
                case (int)(TCPMessageType.退卡): CommandProcessHandler.ExitICCard(msg, SessionPool, IP); break;
                case (int)(TCPMessageType.打印小票): CommandProcessHandler.Print(msg, SessionPool, IP); break;
                case (int)(TCPMessageType.出币): CommandProcessHandler.Coin(msg, SessionPool, IP); break;
                case (int)(TCPMessageType.读取加密狗和机器名): CommandProcessHandler.GetDogIdAndMachineName(msg, SessionPool, IP); break;
            }
        }

        #region 打包请求连接数据
        /// <summary>
        /// 打包请求连接数据
        /// </summary>
        /// <param name="handShakeBytes"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public byte[] PackageHandShakeData(byte[] handShakeBytes, int length)
        {
            string handShakeText = Encoding.UTF8.GetString(handShakeBytes, 0, length);
            string key = string.Empty;
            Regex reg = new Regex(@"Sec\-WebSocket\-Key:(.*?)\r\n");
            Match m = reg.Match(handShakeText);
            if (m.Value != "")
            {
                key = Regex.Replace(m.Value, @"Sec\-WebSocket\-Key:(.*?)\r\n", "$1").Trim();
            }
            byte[] secKeyBytes = SHA1.Create().ComputeHash(Encoding.ASCII.GetBytes(key + "258EAFA5-E914-47DA-95CA-C5AB0DC85B11"));
            string secKey = Convert.ToBase64String(secKeyBytes);
            var responseBuilder = new StringBuilder();
            responseBuilder.Append("HTTP/1.1 101 Switching Protocols" + "\r\n");
            responseBuilder.Append("Upgrade: websocket" + "\r\n");
            responseBuilder.Append("Connection: Upgrade" + "\r\n");
            responseBuilder.Append("Sec-WebSocket-Accept: " + secKey + "\r\n\r\n");
            return Encoding.UTF8.GetBytes(responseBuilder.ToString());
        }
        #endregion

        #region 处理接收的数据
        /// <summary>
        /// 处理接收的数据
        /// 参考 http://www.cnblogs.com/smark/archive/2012/11/26/2789812.html
        /// </summary>
        /// <param name="recBytes"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public string AnalyzeClientData(byte[] recBytes, int length)
        {
            int start = 0;
            // 如果有数据则至少包括3位
            if (length < 2) return "";
            // 判断是否为结束针
            bool IsEof = (recBytes[start] >> 7) > 0;
            // 暂不处理超过一帧的数据
            if (!IsEof) return "";
            start++;
            // 是否包含掩码
            bool hasMask = (recBytes[start] >> 7) > 0;
            // 不包含掩码的暂不处理
            if (!hasMask) return "";
            // 获取数据长度
            UInt64 mPackageLength = (UInt64)recBytes[start] & 0x7F;
            start++;
            // 存储4位掩码值
            byte[] Masking_key = new byte[4];
            // 存储数据
            byte[] mDataPackage;
            if (mPackageLength == 126)
            {
                // 等于126 随后的两个字节16位表示数据长度
                mPackageLength = (UInt64)(recBytes[start] << 8 | recBytes[start + 1]);
                start += 2;
            }
            if (mPackageLength == 127)
            {
                // 等于127 随后的八个字节64位表示数据长度
                mPackageLength = (UInt64)(recBytes[start] << (8 * 7) | recBytes[start] << (8 * 6) | recBytes[start] << (8 * 5) | recBytes[start] << (8 * 4) | recBytes[start] << (8 * 3) | recBytes[start] << (8 * 2) | recBytes[start] << 8 | recBytes[start + 1]);
                start += 8;
            }
            mDataPackage = new byte[mPackageLength];
            for (UInt64 i = 0; i < mPackageLength; i++)
            {
                mDataPackage[i] = recBytes[i + (UInt64)start + 4];
            }
            Buffer.BlockCopy(recBytes, start, Masking_key, 0, 4);
            for (UInt64 i = 0; i < mPackageLength; i++)
            {
                mDataPackage[i] = (byte)(mDataPackage[i] ^ Masking_key[i % 4]);
            }
            return Encoding.UTF8.GetString(mDataPackage);
        }

        public byte[] PackageServerData(string msg)
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

    }
}
