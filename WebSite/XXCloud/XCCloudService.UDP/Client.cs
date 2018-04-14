using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Threading;
using System.Net;
using XCCloudService.Common.Enum;
using XCCloudService.Business.Common;

namespace XCCloudService.SocketService.UDP
{
    public class Client
    {
        static Guid curGUID;
        static UDPSocketClientType curClientType;
        static Socket client;
        static ManualResetEvent allDone = new ManualResetEvent(false);
        static string ServerIP;
        static int ServerPort;
        static List<byte> recvBUF = new List<byte>();
        public static int ServiceClientBufCount { get { return queueRecv.Count; } }
        static Queue<UDPClientItemBusiness.ClientItem> queueRecv = new Queue<UDPClientItemBusiness.ClientItem>();     //待处理数据缓存
        static bool isPacket = false;
        static Thread tRun = null;
        static Thread tTick = null;
        static bool isRun = false;
        static DateTime ConnectSendTime;
        static DateTime ConnectRecvTime;
        static List<byte> downFile = new List<byte>();

        static Dictionary<int, List<byte>> fileBUF = new Dictionary<int, List<byte>>();

        public static void Init(string IP, int Port, Guid gID, UDPSocketClientType cType)
        {
            ConnectSendTime = DateTime.Now;
            ConnectRecvTime = DateTime.Now;
            curGUID = gID;
            curClientType = cType;

            if (!isRun)
            {
                try
                {
                    isRun = true;

                    tRun = new Thread(new ThreadStart(CommandProcess)) { IsBackground = true, Name = "客户端处理线程" };
                    tRun.Start();

                    tTick = new Thread(new ThreadStart(TickProcess)) { IsBackground = true, Name = "心跳处理线程" };
                    tTick.Start();

                    ServerIP = IP;
                    ServerPort = Port;
                    client = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                    byte[] optionInValue = { Convert.ToByte(false) };
                    byte[] optionOutValue = new byte[4];
                    uint IOC_IN = 0x80000000;
                    uint IOC_VENDOR = 0x18000000;
                    uint SIO_UDP_CONNRESET = IOC_IN | IOC_VENDOR | 12;
                    client.IOControl((int)SIO_UDP_CONNRESET, optionInValue, optionOutValue);
                    IPEndPoint serverPoint = new IPEndPoint(IPAddress.Parse(IP), Port);
                    client.Connect((EndPoint)serverPoint);

                    IPEndPoint sendP = new IPEndPoint(IPAddress.Any, 0);
                    EndPoint tempRemoteEP = (EndPoint)sendP;

                    StateObject so = new StateObject();
                    so.socket = client;
                    client.BeginReceiveFrom(so.buffer, 0, StateObject.BUF_SIZE, SocketFlags.None, ref tempRemoteEP, new AsyncCallback(RecvCallBack), so);

                    ConnectServer();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
        }

        static void ConnectServer()
        {
            isRun = true;
            List<byte> dataBUF = new List<byte>();
            dataBUF.Add((byte)curClientType);
            dataBUF.AddRange(curGUID.ToByteArray());
            Send(ServiceObjectConvert.协议编码(0xf0, dataBUF.ToArray()));
            ConnectSendTime = DateTime.Now;
        }

        public static void CloseServer()
        {
            Send(ServiceObjectConvert.协议编码(0xff, curGUID.ToByteArray()));
        }

        static void RecvCallBack(IAsyncResult ar)
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
                for (int i = 0; i < readBytes; i++)
                {
                    if (mybytes[i] == 0x7e)
                    {
                        if (!isPacket) isPacket = true;
                        else
                        {
                            isPacket = false;

                            UDPClientItemBusiness.ClientItem item = new UDPClientItemBusiness.ClientItem() { data = recvBUF.ToArray(), remotePoint = tempRemoteEP };
                            queueRecv.Enqueue(item);
                            recvBUF.Clear();//清除缓存
                        }
                    }
                    else
                    {
                        recvBUF.Add(mybytes[i]);
                    }
                }
                //收到数据处理
                so.socket.BeginReceiveFrom(so.buffer, 0, StateObject.BUF_SIZE, SocketFlags.None, ref tempRemoteEP, new AsyncCallback(RecvCallBack), so);
            }
        }
        public static void Send(byte[] data)
        {
            if (!isRun) return;
            IPEndPoint clients = new IPEndPoint(IPAddress.Parse(ServerIP), ServerPort);
            EndPoint epSender = (EndPoint)clients;
            client.BeginSendTo(data, 0, data.Length, SocketFlags.None, epSender, new AsyncCallback(SendCallBack), client);
        }

        public static void Send(TransmiteEnum cType, object objData)
        {
            if (!isRun) return;
            IPEndPoint clients = new IPEndPoint(IPAddress.Parse(ServerIP), ServerPort);
            EndPoint epSender = (EndPoint)clients;
            byte[] data = null;
            if (objData != null)
            {
                data = ServiceObjectConvert.SerializeObject(objData);
            }
            byte[] sendData = ServiceObjectConvert.协议编码((byte)cType, data);
            client.BeginSendTo(sendData, 0, sendData.Length, SocketFlags.None, epSender, new AsyncCallback(SendCallBack), client);
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
                Console.WriteLine(e);
            }
        }

        static void CommandProcess()
        {
            while (true)
            {
                if (queueRecv.Count > 0)
                {
                    try
                    {
                        UDPClientItemBusiness.ClientItem item = queueRecv.Dequeue();
                        List<byte> decodeData = new List<byte>(item.data);
                        ServiceObjectConvert.转定义解码(ref decodeData);
                        int len = BitConverter.ToUInt16(decodeData.ToArray(), 1);
                        
                        switch ((TransmiteEnum)decodeData[0])
                        {
                            case TransmiteEnum.雷达注册授权响应:
                                //心跳接收
                                ConnectRecvTime = DateTime.Now;
                                break;
                            default:
                                break;
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                    }
                }
                else
                {
                    Thread.Sleep(50);
                }
            }
        }

        static void TickProcess()
        {
            int i = 0;
            while (true)
            {
                if (isRun)
                {
                    if (ConnectRecvTime.AddSeconds(30) < ConnectSendTime)
                    {
                        //服务器断开
                        isRun = false;
                        CallBackEvent.ServerDisconnect();
                    }
                    if (i > 10)
                    {
                        i = 0;
                        ConnectServer();
                    }
                    i++;
                }
                else
                {
                    ConnectServer();
                }
                Thread.Sleep(1000);
            }
        }
    }
}
