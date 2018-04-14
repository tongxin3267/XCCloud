using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Threading;
using System.Net;
using System.Data;
using CloudRS232Server.Command.Recv;

namespace CloudRS232Server
{
    public class UDPClientHelper
    {
        Socket client;
        ManualResetEvent allDone = new ManualResetEvent(false);

        string ServerIP = "";
        int ServerPort = 0;

        public class StateObject
        {
            public const int BUF_SIZE = 1024 * 8;
            public Socket socket;
            public byte[] buffer = new byte[BUF_SIZE];
        }
        
        public bool Init(string IP, int Port)
        {
            try
            {
                ServerIP = IP;
                ServerPort = Port;

                client = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                byte[] optionInValue = { Convert.ToByte(false) };
                byte[] optionOutValue = new byte[4];
                uint IOC_IN = 0x80000000;
                uint IOC_VENDOR = 0x18000000;
                uint SIO_UDP_CONNRESET = IOC_IN | IOC_VENDOR | 12;
                client.IOControl((int)SIO_UDP_CONNRESET, optionInValue, optionOutValue);
                IPEndPoint serverPoint = new IPEndPoint(IPAddress.Parse(ServerIP), ServerPort);
                client.Connect((EndPoint)serverPoint);

                IPEndPoint sendP = new IPEndPoint(IPAddress.Any, 0);
                EndPoint tempRemoteEP = (EndPoint)sendP;

                StateObject so = new StateObject();
                so.socket = client;
                client.BeginReceiveFrom(so.buffer, 0, StateObject.BUF_SIZE, SocketFlags.None, ref tempRemoteEP, new AsyncCallback(RecvCallBack), so);

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return false;
        }

        /// <summary>
        /// socket数据接收异步回调
        /// </summary>
        /// <param name="ar"></param>
        void RecvCallBack(IAsyncResult ar)
        {
            allDone.Set();

            StateObject so = (StateObject)ar.AsyncState;
            IPEndPoint sender = new IPEndPoint(IPAddress.Any, 0);
            EndPoint tempRemoteEP = (EndPoint)sender;

            int readBytes = 0;
            try
            {
                readBytes = so.socket.EndReceiveFrom(ar, ref tempRemoteEP);
            }
            catch (ObjectDisposedException oe)
            {
                Console.WriteLine(oe);
                throw oe;
            }
            catch (SocketException se)
            {
                Console.WriteLine(se);
                throw se;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                // 获得接收失败信息 
                throw e;
            }

            if (readBytes > 0)
            {
                byte[] mybytes = new byte[readBytes];
                Array.Copy(so.buffer, mybytes, readBytes);
                //获取DTU设备的定义码
                byte[] code = mybytes.Take(4).ToArray();
                string RouteCode = PubLib.Hex2String(code[0]) + PubLib.Hex2String(code[1]) + PubLib.Hex2String(code[2]) +PubLib.Hex2String(code[3]);
                //获取接收的真实数据
                byte[] data = mybytes.Skip(4).Take(readBytes - 4).ToArray();
                //创建接收处理对象
                RecvObject o = new RecvObject()
                {
                    Code = RouteCode,
                    Data = data,
                    RemotePoint = tempRemoteEP
                };
                ProcessCommad(o);

                //Thread t = new Thread(new ParameterizedThreadStart(ProcessCommad));
                //t.IsBackground = true;
                //t.Name = code + "|" + tempRemoteEP.ToString() + "处理线程";
                //t.Start((object)o);
                //收到数据处理
                so.socket.BeginReceiveFrom(so.buffer, 0, StateObject.BUF_SIZE, SocketFlags.None, ref tempRemoteEP, new AsyncCallback(RecvCallBack), so);
            }
        }

        /// <summary>
        /// 数据接收处理线程
        /// </summary>
        /// <param name="o"></param>
        void ProcessCommad(object o)
        {
            RecvObject recv = o as RecvObject;
            if (recv != null)
            {
                Console.WriteLine("收到：" + PubLib.Hex2String(recv.Data));
                DateTime time = DateTime.Now;

                FrameData f = new FrameData(recv.Data);
                f.Code = recv.Code;
                DataAccess ac = new DataAccess();
                DataTable dt = ac.ExecuteQueryReturnTable("select * from Base_DeviceInfo where DeviceType='0' and Status<>2 and Token='" + f.Code + "'");  //绑定所有空闲或正常的控制设备
                if (dt.Rows.Count > 0)
                {
                    PubLib.当前总指令数++;
                    //控制器合法
                    switch (f.commandType)
                    {
                        case CommandType.机头网络状态报告:
                            {
                                Recv机头网络状态报告 cmd = new Recv机头网络状态报告(f);
                                SendData(cmd.SendData);
                            }
                            break;
                        case CommandType.机头地址动态分配:
                            {
                                Recv机头地址动态分配 cmd = new Recv机头地址动态分配(f);
                                SendData(cmd.SendData);
                            }
                            break;
                        case CommandType.游戏机参数申请:
                            {
                                Recv游戏机参数申请 cmd = new Recv游戏机参数申请(f);
                                SendData(cmd.SendData);
                            }
                            break;
                        //case CommandType.IC卡模式会员余额查询:
                        //    {
                        //        RecvIC卡模式会员余额查询 cmd = new RecvIC卡模式会员余额查询(f, time);
                        //        if (cmd.IsSuccess)
                        //        {
                        //            object obj = null;
                        //            int res = CheckRepeat(f.Code, cmd.机头地址, cmd.IC卡号码, CommandType.IC卡模式投币数据, ref obj, cmd.流水号);
                        //            if (res == 0)
                        //            {
                        //                //未处理指令
                        //                //处理指令
                        //                cmd.Process();
                        //                //加入重复性检查队列
                        //                InsertRepeat(f.Code, cmd.机头地址, cmd.IC卡号码, CommandType.IC卡模式投币数据, CommandType.IC卡模式投币数据应答, cmd.应答数据, cmd.流水号, time);
                        //            }
                        //            else if (res == 1)
                        //            {
                        //                //已处理过指令
                        //                cmd.应答数据 = (AskIC卡模式会员余额查询)obj;
                        //                PubLib.当前IC卡查询重复指令数++;
                        //            }
                        //            else
                        //            {
                        //                //遇到错误退出
                        //                return;
                        //            }
                        //        }
                        //    }
                        //    break;
                    }
                }
            }
        }

        public void SendConnect()
        {
            FrameData data = new FrameData();
            data.frameType = FrameType.命令帧;
            data.routeAddress = "0001";

            byte[] sDat  = PubLib.GetFrameDataBytes(data, null, CommandType.网络初始化连接);
            SendData(sDat);
        }

        public void SendData(byte[] data)
        {
            if (data != null)
            {
                IPEndPoint clients = new IPEndPoint(IPAddress.Parse(ServerIP), ServerPort);
                EndPoint epSender = (EndPoint)clients;
                client.BeginSendTo(data, 0, data.Length, SocketFlags.None, epSender, new AsyncCallback(SendCallBack), client);
            }
        }

        void SendCallBack(IAsyncResult ar)
        {
            try
            {
                Socket Client = (Socket)ar.AsyncState;
                Client.EndSend(ar);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public void RequestDeviceParamets(Info.HeadInfo.机头绑定信息 bind)
        {
 
        }
    }
}
