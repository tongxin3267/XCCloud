using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;

namespace XCCloudService.SocketService.UDP
{
    public class ClientCall
    {
        public static void 机头参数设定(string rAddress, string hAddress)
        {
            TransmiteObject.后台设置机头参数结构 机头参数 = new TransmiteObject.后台设置机头参数结构()
            {
                rAddress = rAddress,
                hAddress = hAddress
            };

            Client.Send(TransmiteEnum.后台设置机头参数, 机头参数);
        }

        public static void 机头地址变更(string rAddress, string hAddress, string MCUID)
        {
            TransmiteObject.后台机头地址变更结构 地址变更 = new TransmiteObject.后台机头地址变更结构()
            {
                rAddress = rAddress,
                hAddress = hAddress,
                MCUID = MCUID
            };

            Client.Send(TransmiteEnum.后台机头地址变更, 地址变更);
        }

        public static void 设置LOGO图片(byte[] image, int version)
        {
            TransmiteObject.LOGO图片结构 logo图片 = new TransmiteObject.LOGO图片结构()
            {
                imageData = image,
                version = version
            };

            Client.Send(TransmiteEnum.后台设置LOGO图片, logo图片);
        }

        public static void 设置打印文字(string[] word, int version)
        {
            TransmiteObject.打印文字结构 打印文字 = new TransmiteObject.打印文字结构()
            {
                textValue = word,
                version = version
            };

            Client.Send(TransmiteEnum.后台设置打印文字, 打印文字);
        }

        public static void 设置打印顺序(byte[] index, int version)
        {
            TransmiteObject.打印顺序结构 打印顺序 = new TransmiteObject.打印顺序结构()
            {
                indexData = index,
                version = version
            };

            Client.Send(TransmiteEnum.后台设置打印顺序, 打印顺序);
        }

        public static void 充值(string ICCardID)
        {
            TransmiteObject.吧台充值结构 充值卡号 = new TransmiteObject.吧台充值结构()
            {
                ICCardID = ICCardID
            };

            Client.Send(TransmiteEnum.吧台充值, 充值卡号);
        }

        public static void 报警(TransmiteObject.机头报警结构 机头)
        {
            Client.Send(TransmiteEnum.机头报警, 机头);
        }

        public static void 机头状态汇报(TransmiteObject.机头状态结构[] 机头)
        {
            Client.Send(TransmiteEnum.机头状态汇报, 机头);
        }

        public static void 路由器时间管理(DateTime date, bool isSet, ClientType cType)
        {
            TransmiteObject.路由器时间管理结构 时间 = new TransmiteObject.路由器时间管理结构()
            {
                timeValue = date,
                IsSet = isSet,
                cType = cType
            };

            Client.Send(TransmiteEnum.路由器时间管理, 时间);
        }

        public static void 游戏机变更()
        {
            Client.Send(TransmiteEnum.游戏机变更, null);
        }

        public static void 互联网同步时间(DateTime date, bool isSet, ClientType cType)
        {
            TransmiteObject.路由器时间管理结构 时间 = new TransmiteObject.路由器时间管理结构()
            {
                timeValue = date,
                IsSet = isSet,
                cType = cType
            };
            Client.Send(TransmiteEnum.时间同步, 时间);
        }

        public static void 结账()
        {
            Client.Send(TransmiteEnum.结账, null);
        }

        public static void 机头锁定状态报告(string hAddress, string rAddress, LockTypeEnum lockTypeData)
        {
            TransmiteObject.机头解锁结构 obj = new TransmiteObject.机头解锁结构() { lockType = lockTypeData, HeadAdress = hAddress, RouteAddress = rAddress };
            Client.Send(TransmiteEnum.机头锁定状态报告, obj);
        }

        static string curFileName = "";
        static UInt16 curPacketIndex = 0;
        static UInt16 curPacketCount = 0;
        static int curPacketRemain = 0;
        const int PacketLength = 1024 * 8;

        public static void 上传文件(string fileName)
        {
            List<byte> data = new List<byte>();
            if (fileName != "")
            {
                using (FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read))
                {
                    curPacketCount = (UInt16)(fs.Length / PacketLength);
                    curPacketRemain = (int)(fs.Length % PacketLength);
                    if (curPacketRemain != 0) curPacketCount++;
                    fs.Close();
                }
                curPacketIndex = 0;
                data.AddRange(BitConverter.GetBytes(curPacketIndex));
                data.AddRange(BitConverter.GetBytes(curPacketCount));
                data.AddRange(Encoding.GetEncoding("gb2312").GetBytes(Path.GetFileName(fileName)));
                byte[] sendData = ServiceObjectConvert.协议编码((byte)TransmiteEnum.上传文件, data.ToArray());
                Client.Send(sendData);
                curFileName = fileName;
                curPacketIndex++;
            }
            else
            {

                using (FileStream fs = new FileStream(curFileName, FileMode.Open, FileAccess.Read))
                {
                    byte[] dataRead;
                    if (curPacketIndex > curPacketCount) return; //发送完毕
                    fs.Position = (curPacketIndex - 1) * PacketLength;
                    if (curPacketIndex == curPacketCount)   //最后一包数据
                    {
                        dataRead = new byte[curPacketRemain];
                        fs.Read(dataRead, 0, curPacketRemain);
                    }
                    else
                    {   //正常包
                        dataRead = new byte[PacketLength];
                        fs.Read(dataRead, 0, PacketLength);
                    }
                    fs.Close();
                    data.AddRange(BitConverter.GetBytes(curPacketIndex));
                    data.AddRange(BitConverter.GetBytes(curPacketCount));
                    data.AddRange(dataRead);
                    byte[] sendData = ServiceObjectConvert.协议编码((byte)TransmiteEnum.上传文件, data.ToArray());
                    Client.Send(sendData);
                    curPacketIndex++;
                }
            }
        }

        public static void 下载文件(string fileName)
        {
            byte[] sendData = ServiceObjectConvert.协议编码((byte)TransmiteEnum.下载文件, Encoding.GetEncoding("gb2312").GetBytes(fileName));
            Client.Send(sendData);
        }

        public static void 获取本店密码()
        {
            byte[] sendData = ServiceObjectConvert.协议编码((byte)TransmiteEnum.获取店密码, null);
            Client.Send(sendData);
        }

        public static void 启动远程强制退分检测(string ICCardID)
        {
            byte[] sendData = ServiceObjectConvert.协议编码((byte)TransmiteEnum.启动远程强制退分检测, Encoding.GetEncoding("gb2312").GetBytes(ICCardID));
            Client.Send(sendData);
        }

        public static void 远程强制退分应答(string 错误消息, string IC卡号, bool 是否允许退分)
        {
            TransmiteObject.远程强制退分应答结构 obj = new TransmiteObject.远程强制退分应答结构() { 错误消息 = 错误消息, IC卡号 = IC卡号, 是否允许退分 = 是否允许退分 };
            Client.Send(TransmiteEnum.远程强制退分应答, obj);
        }

        public static void 机头状态复位()
        {
            Client.Send(TransmiteEnum.系统复位, null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hAddress"></param>
        /// <param name="isLock"></param>
        /// <param name="lockType">1.常规解锁，2.超分忽略当天</param>
        public static void 机头锁定解锁指令(string rAddress, string hAddress, bool isLock, int lockType)
        {
            TransmiteObject.机头锁定解锁结构 obj = new TransmiteObject.机头锁定解锁结构()
            {
                路由器段号 = rAddress,
                机头地址 = hAddress,
                是否锁定 = isLock,
                解锁目的 = lockType,
            };
            Client.Send(TransmiteEnum.机头锁定解锁指令, obj);
        }

        public static void 软件版本请求指令(Guid clientID, string storeID, int softType)
        {
            TransmiteObject.请求软件版本号结构 obj = new TransmiteObject.请求软件版本号结构()
            {
                程序标识号 = clientID,
                店编号 = storeID,
                软件类别 = softType
            };
            Client.Send(TransmiteEnum.请求软件版本号, obj);
        }

        public static void 文件下载超时()
        {
            byte[] sendData = ServiceObjectConvert.协议编码((byte)TransmiteEnum.下载文件, null);
            Client.Send(sendData);
        }
    }
}
