using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using XCCloudService.Common.Enum;

namespace XCCloudService.Business.Common
{
    public class UDPClientItemBusiness
    {
        static List<ClientItem> clientList = new List<ClientItem>();

        public static List<ClientItem> ClientList
        {
            get { return clientList; }
        }

        public class ClientItem
        {
            public byte[] data;             //最后一条数据
            public EndPoint remotePoint;    //远程节点
            public string gID;              //当前客户端唯一标识
            public UDPSocketClientType cType;        //当前客户端类别      
            public DateTime curTime;        //当前连接更新时间
            public DateTime HeatTime;     //雷达最后一次收到心跳的时间
            public string StoreID = "";     //当前连接店编号
            public string Segment = "";     //段地址（如果是雷达）
        }
    }
}
