using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using XCCloudService.Common.Enum;

namespace XCCloudService.SocketService.UDP
{
    public class TransmiteObject
    {
        [Serializable]
        public class 后台设置机头参数结构
        {
            public string hAddress;
            public string rAddress;
        }

        [Serializable]
        public class 后台机头地址变更结构
        {
            public string hAddress;
            public string rAddress;
            public string MCUID;
        }

        [Serializable]
        public class 吧台充值结构
        {
            public string ICCardID;
        }

        [Serializable]
        public class 机头报警结构
        {
            public string hAddress;
            public string rAddress;
            public bool CO信号异常;
            public bool CO2信号异常;
            public bool SSR信号异常;
            public bool 打印机故障;
            public bool 打印设置错误;
            public bool 读币器故障;
            public bool 非法币或卡报警;
            public bool 高压干扰报警;
            public bool 高频干扰报警;
            public bool 存币箱满报警;
        }

        [Serializable]
        public class 机头状态结构
        {
            public string hAddress;
            public string rAddress;
            public bool CO信号异常;
            public bool CO2信号异常;
            public bool SSR信号异常;
            public bool 打印机故障;
            public bool 打印设置错误;
            public bool 读币器故障;
            public bool 非法币或卡报警;
            public bool 高压干扰报警;
            public bool 高频干扰报警;
            public bool 存币箱满报警;
            public bool IsLock;
            public bool IsOver;
            public int InCoins;
            public int OutCoins;
            public int WinCoins;
            public bool IsOnline;
            public bool IsInCoin;
            public bool IsOutCoin;
            public int GameInCoins;
            public int GameOutCoins;
            public int GameWinCoins;
        }

        [Serializable]
        public class LOGO图片结构
        {
            public int version;
            public byte[] imageData;
        }

        [Serializable]
        public class 打印文字结构
        {
            public int version;
            public string[] textValue;
        }

        [Serializable]
        public class 打印顺序结构
        {
            public int version;
            public byte[] indexData;
        }

        [Serializable]
        public class 路由器时间管理结构
        {
            public DateTime timeValue;
            public bool IsSet;
            public UDPSocketClientType cType;
        }

        [Serializable]
        public class 机头解锁结构
        {
            public string HeadAdress;
            public string RouteAddress;
            public LockTypeEnum lockType;
        }

        public class 文件传输结构
        {
            public string IP;
            public int PacketIndex;
            public int PacketCount;
            public List<byte> PacketBytes = new List<byte>();
        }

        public class 文件下载结构
        {
            public string IP;
            public UInt16 PacketIndex;
            public UInt16 PacketCount;
            public int PacketRemain;
            public string fileName;
        }

        [Serializable]
        public class 远程强制退分应答结构
        {
            public string 错误消息 = "";
            public string IC卡号 = "";
            public bool 是否允许退分;
        }

        [Serializable]
        public class 机头锁定解锁结构
        {
            public string 路由器段号 = "";
            public string 机头地址 = "";
            public bool 是否锁定 = false;
            public int 解锁目的 = 0;
        }
        [Serializable]
        public class 请求软件版本号结构
        {
            public Guid 程序标识号;
            public string 店编号 = "";
            /// <summary>
            /// 1.自助机
            /// </summary>
            public int 软件类别 = 0;    
        }
        [Serializable]
        public class 软件版本号应答结构
        {
            public string 软件版本号 = "";
            public string 升级包名 = "";
        }
    }
}
