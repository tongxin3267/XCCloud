using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Threading;
using System.Net;
using System.Data;
using CloudRS232Server.Command.Recv;
//using CloudRS232Server.Command.Ask;

namespace CloudRS232Server
{
    public class StateObject
    {
        public const int BUF_SIZE = 1024 * 128;
        public Socket socket;
        public byte[] buffer = new byte[BUF_SIZE];
    }

    public class RecvObject
    {
        public string Code { get; set; }
        public byte[] Data { get; set; }
        public EndPoint RemotePoint { get; set; }
    }

    /// <summary>
    /// 接收服务器类
    /// </summary>
    public class UDPServerHelper
    {
        #region UDP收发数据处理
        Socket client;
        ManualResetEvent allDone = new ManualResetEvent(false);

        public bool isRun = false;

        public delegate void 消息显示(string msg);
        public event 消息显示 OnShowMsg;
        protected void ShowMsg(string msg)
        {
            if (OnShowMsg != null)
            {
                OnShowMsg(msg);
            }
        }

        /// <summary>
        /// 启动绑定端口
        /// </summary>
        /// <param name="Port"></param>
        /// <returns></returns>
        public bool Init(int Port)
        {
            try
            {
                if (!isRun)
                {
                    isRun = true;

                    client = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                    byte[] optionInValue = { Convert.ToByte(false) };
                    byte[] optionOutValue = new byte[4];
                    uint IOC_IN = 0x80000000;
                    uint IOC_VENDOR = 0x18000000;
                    uint SIO_UDP_CONNRESET = IOC_IN | IOC_VENDOR | 12;
                    client.IOControl((int)SIO_UDP_CONNRESET, optionInValue, optionOutValue);
                    IPEndPoint p = new IPEndPoint(IPAddress.Any, Port);
                    client.Bind((EndPoint)p);

                    IPEndPoint sendP = new IPEndPoint(IPAddress.Any, 0);
                    EndPoint tempRemoteEP = (EndPoint)sendP;

                    StateObject so = new StateObject();
                    so.socket = client;
                    client.BeginReceiveFrom(so.buffer, 0, StateObject.BUF_SIZE, SocketFlags.None, ref tempRemoteEP, new AsyncCallback(ReceiveCallback), so);

                    //开启重复性检查线程
                    if (repeatProcT == null)
                    {
                        repeatProcT = new Thread(new ThreadStart(CheckRepeatTimeOut));
                        repeatProcT.IsBackground = true;
                        repeatProcT.Name = "重复性超时检查线程";
                        repeatProcT.Start();
                    }
                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("初始化失败");
                Console.WriteLine(ex);
                LogHelper.WriteLog(ex);
            }
            return false;
        }

        /// <summary>
        /// 异步接收回调
        /// </summary>
        /// <param name="ar"></param>
        void ReceiveCallback(IAsyncResult ar)
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
                LogHelper.WriteLog(oe);
                throw oe;
            }
            catch (SocketException se)
            {
                Console.WriteLine(se);
                LogHelper.WriteLog(se);
                throw se;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                LogHelper.WriteLog(e);
                // 获得接收失败信息 
                throw e;
            }

            if (readBytes > 0)
            {
                byte[] mybytes = new byte[readBytes];
                Array.Copy(so.buffer, mybytes, readBytes);
                //获取DTU设备的定义码
                byte[] code = mybytes.Take(4).ToArray();
                string RouteCode = PubLib.Hex2String(code[0]) + PubLib.Hex2String(code[1]) + PubLib.Hex2String(code[2]) + PubLib.Hex2String(code[3]);
                //获取接收的真实数据
                byte[] data = mybytes.Skip(4).Take(readBytes - 4).ToArray();
                //创建接收处理对象
                RecvObject o = new RecvObject()
                {
                    Code = RouteCode,
                    Data = data,
                    RemotePoint = tempRemoteEP
                };

                ShowMsg(string.Format("[ {0} ] 来自:{1} 编号: {2} : {3}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"), tempRemoteEP.ToString(), RouteCode, PubLib.BytesToString(data)));
                ProcessCommad(o);
                //Thread t = new Thread(new ParameterizedThreadStart(ProcessCommad));
                //t.IsBackground = true;
                //t.Name = code + "|" + tempRemoteEP.ToString() + "处理线程";
                //t.Start((object)o);
                //创建新的数据接收线程
                so.socket.BeginReceiveFrom(so.buffer, 0, StateObject.BUF_SIZE, SocketFlags.None, ref tempRemoteEP, new AsyncCallback(ReceiveCallback), so);
            }
        }

        public void SendData(byte[] data, EndPoint point)
        {
            try
            {
                ShowMsg(string.Format("[ {0} ] 发送:{1} : {2} ", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"), point.ToString(), PubLib.BytesToString(data)));
                client.SendTo(data, point);
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog(ex);
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
                SetRouteInfo(recv.Code, f.routeAddress, recv.RemotePoint);
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
                                SendData(cmd.SendData, recv.RemotePoint);
                            }
                            break;
                        case CommandType.机头地址动态分配:
                            {
                                Recv机头地址动态分配 cmd = new Recv机头地址动态分配(f);
                                SendData(cmd.SendData, recv.RemotePoint);
                            }
                            break;
                        case CommandType.游戏机参数申请:
                            {
                                Recv游戏机参数申请 cmd = new Recv游戏机参数申请(f);
                                SendData(cmd.SendData, recv.RemotePoint);
                            }
                            break;
                        case CommandType.远程投币上分指令应答:
                            {
                                int SN = BitConverter.ToUInt16(f.recvData, 6);  //用于清除重发队列
                            }
                            break;
                        case CommandType.远程被动退分解锁指令应答:
                            {
                                //远程退分指令应答
                            }
                            break;
                        case CommandType.机头卡片报警指令:
                            {
                                Recv机头卡片报警指令 cmd = new Recv机头卡片报警指令(f, DateTime.Now);
                                SendData(cmd.SendData, recv.RemotePoint);
                            }
                            break;
                        case CommandType.IC卡模式退币数据:
                        case CommandType.IC卡模式投币数据:
                            {
                                Recv卡头进出币数据 cmd = new Recv卡头进出币数据(f, DateTime.Now);
                                SendData(cmd.SendData, recv.RemotePoint);
                            }
                            break;
                    }
                }
            }
        }
        #endregion

        #region 重复性检查
        //重复性检查需要的结构
        class CRepeat
        {
            /// <summary>
            /// 路由器地址
            /// </summary>
            public string rAddress;
            /// <summary>
            /// 机头地址
            /// </summary>
            public string hAddress;
            /// <summary>
            /// 应答对象
            /// </summary>
            public object askObject;
            /// <summary>
            /// 应答类别
            /// </summary>
            public CommandType askType;
            /// <summary>
            /// 接收指令类别
            /// </summary>
            public CommandType recvType;
            /// <summary>
            /// 重复性流水号
            /// </summary>
            public UInt16 repeatID;
            /// <summary>
            /// 记录产生时间
            /// </summary>
            public DateTime createTime;
            /// <summary>
            /// 最后检查时间
            /// </summary>
            public DateTime checkTime;
            /// <summary>
            /// 发送记录时间
            /// </summary>
            public List<DateTime> SendDateTimeList = new List<DateTime>();
            /// <summary>
            /// IC卡号码
            /// </summary>
            public string ICCard;
        }
        //重复性检查超时检查线程
        static Thread repeatProcT = null;
        /// <summary>
        /// 重复性检查队列
        /// </summary>
        static List<CRepeat> repeatQueue = new List<CRepeat>();
        /// <summary>
        /// 重复性队列超时时间，秒
        /// </summary>
        static int repeatTimeoutSecond = 30;
        /// <summary>
        /// 指令重复性检查
        /// </summary>
        /// <param name="rAddress">路由器段号</param>
        /// <param name="hAddress">机头地址</param>
        /// <param name="SN">流水号</param>
        /// <returns>查找到重复性指令返回1,否则返回0,出错返回-1</returns>
        public static int CheckRepeat(string rAddress, string hAddress, string ICCard, CommandType ctype, ref object askObj, UInt16 SN)
        {
            try
            {
                lock (repeatQueue)
                {
                    //Console.WriteLine("rAddress：" + rAddress + "    hAddress：" + hAddress + "     ICCard：" + ICCard + "   SN：" + SN);
                    foreach (CRepeat item in repeatQueue)
                    {
                        if (item.repeatID == SN && item.rAddress == rAddress && item.hAddress == hAddress && item.ICCard == ICCard && item.recvType == ctype)
                        {
                            switch (item.recvType)
                            {
                                case CommandType.IC卡模式投币数据:
                                    //Command.Ask.AskIC卡模式进出币数据 ask = item.askObject as Command.Ask.AskIC卡模式进出币数据;
                                    //if (ask == null) continue;
                                    break;
                            }
                            askObj = item.askObject;
                            item.checkTime = DateTime.Now;
                            return 1;
                        }
                    }
                    return 0;
                }
            }
            catch (Exception ex)
            {
                //goto continueline;
                throw ex;
            }
        }

        public static void UpdateRepeatTime(string rAddress, string hAddress, UInt16 SN)
        {
            try
            {
                lock (repeatQueue)
                {
                    foreach (CRepeat item in repeatQueue)
                    {
                        if (item.repeatID == SN && item.rAddress == rAddress && item.hAddress == hAddress)
                        {
                            DateTime curDate = DateTime.Now;
                            item.SendDateTimeList.Add(curDate);
                        }
                    }
                }
            }
            catch
            {
                throw;
            }
        }
        public static void InsertRepeat(string rAddress, string hAddress, string ICCard, CommandType recvType, CommandType askType, object askObj, UInt16 SN, DateTime recvDate)
        {
            CRepeat c = new CRepeat();
            c.askObject = askObj;
            c.askType = askType;
            c.checkTime = DateTime.Now;
            c.createTime = recvDate;
            c.hAddress = hAddress;
            c.rAddress = rAddress;
            c.repeatID = SN;
            c.ICCard = ICCard;
            c.recvType = recvType;

            lock (repeatQueue)
            {
                repeatQueue.Add(c);

            }
        }

        static void CheckRepeatTimeOut()
        {
            while (true)
            {

                lock (repeatQueue)
                {
                    if (repeatQueue.Count > 0)
                    {
                        try
                        {
                            foreach (CRepeat item in repeatQueue)
                            {
                                if (item.checkTime.AddSeconds(repeatTimeoutSecond) < DateTime.Now)
                                {
                                    repeatQueue.Remove(item);
                                    break;
                                }
                            }
                            Thread.Sleep(2);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex);
                        }
                    }
                    else
                    {
                        Thread.Sleep(10);
                    }
                }
            }
        }
        #endregion

        #region 控制器信息

        class RouteInfo
        {
            public EndPoint RemotePoint { get; set; }
            public string Segment { get; set; }
            public bool IsOnline { get; set; }
        }

        Dictionary<string, RouteInfo> RouteList = new Dictionary<string, RouteInfo>();

        void SetRouteInfo(string Code, string Segment, EndPoint point)
        {
            if (RouteList.ContainsKey(Code))
            {
                RouteList[Code].RemotePoint = point;
                RouteList[Code].IsOnline = true;
                RouteList[Code].Segment = Segment;
            }
            else
            {
                RouteInfo info = new RouteInfo();
                info.RemotePoint = point;
                info.Segment = Segment;
                info.IsOnline = true;
                RouteList.Add(Code, info);
            }
        }
        bool GetRouteInfo(string Code, out RouteInfo Route)
        {
            Route = null;
            if (RouteList.ContainsKey(Code))
            {
                Route = RouteList[Code];
                return true;
            }
            return false;
        }
        #endregion

        #region 公共事件
        public void 机头锁定解锁指令(string Code, string Address, bool isLock)
        {
            try
            {
                RouteInfo route;
                if (GetRouteInfo(Code, out route))
                {
                    if (route.IsOnline)
                    {
                        //当前路由器在线
                        FrameData data = new FrameData();
                        data.commandType = CommandType.机头锁定解锁指令;
                        data.routeAddress = route.Segment;
                        List<byte> dataList = new List<byte>();
                        dataList.Add((byte)Convert.ToByte(Address, 16));
                        dataList.Add((byte)(isLock ? 1 : 0));
                        byte[] Send = PubLib.GetFrameDataBytes(data, dataList.ToArray(), CommandType.机头锁定解锁指令);

                        SendData(Send, route.RemotePoint);
                    }
                }
            }
            catch
            {
                throw;
            }
        }

        public void 设置机头长地址(string Code, string MCUID)
        {
            RouteInfo route;
            if (GetRouteInfo(Code, out route))
            {
                if (route.IsOnline)
                {
                    List<byte> send = new List<byte>();
                    send.AddRange(PubLib.StringToByte(MCUID));
                    List<byte> sendRe = new List<byte>();
                    //位置调换，低位在前
                    for (int i = send.Count - 1; i >= 0; i--)
                    {
                        sendRe.Add(send[i]);
                    }
                    FrameData data = new FrameData();
                    data.routeAddress = "FFFF";
                    byte[] Senddata = PubLib.GetFrameDataBytes(data, sendRe.ToArray(), CommandType.设置机头长地址);

                    SendData(Senddata, route.RemotePoint);
                }
            }
        }

        public void 指定路由器复位(string Code)
        {
            RouteInfo route;
            if (GetRouteInfo(Code, out route))
            {
                if (route.IsOnline)
                {
                    byte[] routeBytes = BitConverter.GetBytes(Convert.ToUInt16(route.Segment, 16));
                    FrameData data = new FrameData();
                    data.commandType = CommandType.设置路由器地址;
                    data.routeAddress = "FFFF";
                    byte[] Senddata = PubLib.GetFrameDataBytes(data, routeBytes, CommandType.设置路由器地址);

                    SendData(Senddata, route.RemotePoint);
                }
            }
        }

        public void 远程投币上分(string Code, string Address,string ICCardID, int Coins)
        {
            RouteInfo route;
            if (GetRouteInfo(Code, out route))
            {
                if (route.IsOnline)
                {

                    Info.HeadInfo.机头绑定信息 bind = new Info.HeadInfo.机头绑定信息();
                    bind.控制器令牌 = Code;
                    bind.短地址 = Address;
                    Info.HeadInfo.机头信息 h = Info.HeadInfo.GetHeadInfoByShort(bind);
                    if (h != null)
                    {
                        h.常规.当前卡片号 = ICCardID;
                        FrameData data = new FrameData();
                        data.commandType = CommandType.远程投币上分指令;
                        data.routeAddress = route.Segment;
                        List<byte> dataList = new List<byte>();
                        dataList.Add((byte)Convert.ToByte(Address, 16));
                        dataList.AddRange(BitConverter.GetBytes((UInt16)Coins));
                        dataList.AddRange(BitConverter.GetBytes((UInt16)h.远程投币上分流水号));
                        byte[] Send = PubLib.GetFrameDataBytes(data, dataList.ToArray(), CommandType.远程投币上分指令);

                        SendData(Send, route.RemotePoint);
                    }
                }
            }
        }

        public void 远程退币(string Code, string Address)
        {
            RouteInfo route;
            if (GetRouteInfo(Code, out route))
            {
                if (route.IsOnline)
                {
                    Info.HeadInfo.机头绑定信息 bind = new Info.HeadInfo.机头绑定信息();
                    bind.控制器令牌 = Code;
                    bind.短地址 = Address;
                    Info.HeadInfo.机头信息 h = Info.HeadInfo.GetHeadInfoByShort(bind);
                    if (h != null)
                    {
                        FrameData data = new FrameData();
                        data.commandType = CommandType.远程被动退分解锁指令;
                        data.routeAddress = route.Segment;
                        List<byte> dataList = new List<byte>();
                        dataList.Add((byte)Convert.ToByte(Address, 16));
                        dataList.Add(0x01);
                        byte[] Send = PubLib.GetFrameDataBytes(data, dataList.ToArray(), CommandType.远程被动退分解锁指令);

                        SendData(Send, route.RemotePoint);
                    }
                }
            }
        }

        public void 退币信号延时检测指令(string Code, string Address)
        {
            RouteInfo route;
            if (GetRouteInfo(Code, out route))
            {
                if (route.IsOnline)
                {
                    Info.HeadInfo.机头绑定信息 bind = new Info.HeadInfo.机头绑定信息();
                    bind.控制器令牌 = Code;
                    bind.短地址 = Address;
                    Info.HeadInfo.机头信息 h = Info.HeadInfo.GetHeadInfoByShort(bind);
                    if (h != null)
                    {
                        FrameData data = new FrameData();
                        data.commandType = CommandType.远程被动退分解锁指令;
                        data.routeAddress = route.Segment;
                        List<byte> dataList = new List<byte>();
                        dataList.Add((byte)Convert.ToByte(Address, 16));
                        dataList.Add(0x01);
                        byte[] Send = PubLib.GetFrameDataBytes(data, dataList.ToArray(), CommandType.远程被动退分解锁指令);

                        SendData(Send, route.RemotePoint);
                    }
                }
            }
        }
        #endregion
    }
}
