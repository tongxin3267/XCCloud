using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Net;
using XCCloudService.Common.Enum;
using XCCloudService.Business.Common;

namespace XCCloudService.SocketService.UDP
{
    public class ClientList
    {
        static List<UDPClientItemBusiness.ClientItem> clientList = UDPClientItemBusiness.ClientList;
        static Thread tCheck = null;
        public static bool IsRequestRouteTime = false;

        public static List<UDPClientItemBusiness.ClientItem> ClientListObj
        {
            set { clientList = value; }
            get { return clientList; }
        }

        //public class ClientItem
        //{
        //    public byte[] data;             //最后一条数据
        //    public EndPoint remotePoint;    //远程节点
        //    public string gID;              //当前客户端唯一标识
        //    public UDPSocketClientType cType;        //当前客户端类别      
        //    public DateTime curTime;        //当前连接更新时间
        //    public DateTime HeatTime;     //雷达最后一次收到心跳的时间
        //    public string StoreID = "";     //当前连接店编号
        //    public string Segment = "";     //段地址（如果是雷达）
        //}

        public class ForcBackItem
        {
            public string RouteID = "";         //路由器编号
            public string Message = "";         //消息
            public bool isCallBack = false;     //是否反馈
        }
        //当前下发强制退分指令缓存列表
        public static Dictionary<string, List<ForcBackItem>> ForcBackList = new Dictionary<string, List<ForcBackItem>>();

        static void CheckOnline(object obj)
        {
            while (true)
            {
                try
                {
                    UDPClientItemBusiness.ClientItem item = (UDPClientItemBusiness.ClientItem)obj;
                    //非雷达注册客户都清理了
                    var clients = clientList.Where(p => p.HeatTime < DateTime.Now.AddSeconds(0 - XCCloudService.Common.CommonConfig.RadarOffLineTimeLong * 2));
                    if (clients.Count() > 0)
                    {
                        foreach (UDPClientItemBusiness.ClientItem client in clients)
                        {
                            clientList.Remove(client);
                            CallBackEvent.ClientDisconnect(client.gID);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }

                Thread.Sleep(5000);
            }
        }

        public static void UpdateClientHeatTime(string radarToken)
        {
            UDPClientItemBusiness.ClientItem curClient = new UDPClientItemBusiness.ClientItem();
            var clients = clientList.Where(p => p.gID.Equals(radarToken));
            if (clients.Count() > 0)
            {
                var client = clients.First();
                client.HeatTime = DateTime.Now;
            }
        }

        public static void UpdateClient(UDPClientItemBusiness.ClientItem item)
        {
            if (tCheck == null)
            {
                tCheck = new Thread(new ParameterizedThreadStart(CheckOnline)) { IsBackground = true, Name = "在线检查线程" };
                tCheck.Start(item);
            }


            UDPClientItemBusiness.ClientItem curClient = new UDPClientItemBusiness.ClientItem();
            var clients = clientList.Where(p => p.Segment == item.Segment && p.StoreID == item.StoreID);
            if (clients.Count() > 0)
            {
                curClient = clients.First();
                curClient.data = item.data;
                curClient.gID = item.gID;
                curClient.remotePoint = item.remotePoint;
                curClient.curTime = DateTime.Now;
                curClient.HeatTime = DateTime.Now;
                curClient.StoreID = item.StoreID;
                curClient.Segment = item.Segment;
            }
            else
            {   
                curClient.data = item.data;
                curClient.gID = item.gID;
                curClient.remotePoint = item.remotePoint;
                curClient.curTime = DateTime.Now;
                curClient.HeatTime = DateTime.Now;
                curClient.StoreID = item.StoreID;
                curClient.Segment = item.Segment;
                clientList.Add(curClient);
            }
        }

        public static void CloseClient(UDPClientItemBusiness.ClientItem item)
        {
            UDPClientItemBusiness.ClientItem curClient = new UDPClientItemBusiness.ClientItem();
            var clients = clientList.Where(p => p.gID == item.gID);
            if (clients.Count() > 0)
            {
                curClient = clients.First();
                clientList.Remove(curClient);
            }
        }

        public static void SendCommand(string gID, TransmiteEnum cType, object data)
        {
            UDPClientItemBusiness.ClientItem curClient = new UDPClientItemBusiness.ClientItem();
            var clients = clientList.Where(p => p.gID == gID);
            if (clients.Count() > 0)
            {
                curClient = clients.First();
                byte[] dat = ServiceObjectConvert.SerializeObject(data);
                byte[] sendData = ServiceObjectConvert.协议编码((byte)cType, dat);
                Server.Send(((IPEndPoint)curClient.remotePoint).Address.ToString(), ((IPEndPoint)curClient.remotePoint).Port, sendData);
            }
        }
    }
}
