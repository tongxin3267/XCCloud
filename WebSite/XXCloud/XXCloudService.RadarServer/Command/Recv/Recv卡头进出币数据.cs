using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XCCloudService.Common.Extensions;

namespace XXCloudService.RadarServer.Command.Recv
{
    public class Recv卡头进出币数据
    {
        public string 机头地址 = "";
        public string IC卡号码 = "";
        public byte 动态密码 = 0;
        public CoinType 控制类型;
        public int 币数;
        public UInt16 流水号;
        public byte[] SendData = null;
        public FrameData RecvData;
        public UInt16 测试数据;
        public Ask.Ask卡头进出币数据应答 应答数据;
        public DateTime SendDataTime;
        public bool 高速投币标志 = false;
        public string 投币目标地址 = "";

        public Recv卡头进出币数据(FrameData f, DateTime RecvDateTime)
        {
            try
            {
                RecvData = f;
                if (f.commandData.Length >= 15)
                {
                    机头地址 = PubLib.Hex2String(f.commandData[0]);
                    for (int i = 0; i < 8; i++)
                    {
                        if (f.commandData[i + 1] < 0x30 || f.commandData[i + 1] > 0x39)
                        {
                            f.commandData[i + 1] = 32;
                        }
                    }
                    IC卡号码 = Encoding.ASCII.GetString(f.commandData, 1, 8);

                    IC卡号码 = IC卡号码.Trim();

                    动态密码 = f.commandData[9];
                    控制类型 = (CoinType)f.commandData[10];
                    币数 = (int)BitConverter.ToUInt16(f.commandData, 11);
                    测试数据 = BitConverter.ToUInt16(f.commandData, 13);
                    流水号 = BitConverter.ToUInt16(f.commandData, 15);

                    高速投币标志 = (测试数据 / 256 == 1);
                    投币目标地址 = PubLib.Hex2String((byte)(测试数据 % 256));

                    Info.HeadInfo.机头绑定信息 bind = new Info.HeadInfo.机头绑定信息();
                    bind.控制器令牌 = f.Code;
                    bind.短地址 = 机头地址;
                    Info.HeadInfo.机头信息 head = Info.HeadInfo.GetHeadInfoByShort(bind);
                    if (head != null)
                    {
                        if (IC卡号码 == "") IC卡号码 = head.常规.当前卡片号;
                        if (IC卡号码 == "") IC卡号码 = "0";
                        if (控制类型 == CoinType.实物投币) IC卡号码 = "0"; //实物投币过滤卡号，有可能是卡头没有清缓存导致
                        head.临时错误计数 = 测试数据;

                        head.状态.出币机或存币机正在数币 = false;

                        object obj = null;
                        int res = UDPServerHelper.CheckRepeat(f.Code, 机头地址, IC卡号码, CommandType.IC卡模式投币数据, ref obj, 流水号);
                        if (res == 0)
                        {
                            //if (高速投币标志)
                            //    Ask.Ask远程投币上分数据 ask = new Ask.Ask远程投币上分数据(投币目标地址, 币数, "刷卡", 流水号);
                            string msg = "";
                            应答数据 = new Ask.Ask卡头进出币数据应答(head, IC卡号码, 币数, 控制类型, 动态密码, (UInt16)流水号, f, 高速投币标志, 投币目标地址, ref msg);
                            if (f.commandType == CommandType.IC卡模式投币数据)
                            {
                                UDPServerHelper.InsertRepeat(f.routeAddress, 机头地址, IC卡号码, CommandType.IC卡模式投币数据, CommandType.IC卡模式投币数据应答, 应答数据, 流水号, RecvDateTime);
                                if (应答数据.脉冲数 == 0)
                                {
                                    LogHelper.WriteLog("IC卡进出币数据有误\r\n" + msg, f.commandData);
                                }
                            }
                            else
                                UDPServerHelper.InsertRepeat(f.routeAddress, 机头地址, IC卡号码, CommandType.IC卡模式退币数据, CommandType.IC卡模式退币数据应答, 应答数据, 流水号, RecvDateTime);
                        }
                        else if (res == 1)
                        {
                            应答数据 = (Ask.Ask卡头进出币数据应答)obj;
                            PubLib.当前IC卡进出币指令重复数++;
                        }
                        else
                        {
                            //重复性检查错误
                            return;
                        }
                        byte[] dataBuf = PubLib.GetBytesByObject(应答数据);
                        if (f.commandType == CommandType.IC卡模式投币数据)
                            SendData = PubLib.GetFrameDataBytes(f, dataBuf, CommandType.IC卡模式投币数据应答);
                        else
                        {
                            SendData = PubLib.GetFrameDataBytes(f, dataBuf, CommandType.IC卡模式退币数据应答);
                            LogHelper.WriteTBLog(PubLib.BytesToString(f.recvData));
                        }
                    }
                }
            }
            catch
            {
                //LogHelper.WriteLog("IC卡进出币数据有误", f.commandData);
                throw;
            }
        }

        public string GetRecvData(DateTime printDate)
        {
            StringBuilder sb = new StringBuilder();
            //sb.Append("=============================================\r\n");
            sb.AppendFormat("{0:yyyy-MM-dd HH:mm:ss.fff}  收到数据\r\n", DateTime.Now);
            sb.AppendFormat("{0}\r\n", PubLib.BytesToString(RecvData.recvData));
            sb.AppendFormat("指令类别：{0}\r\n", RecvData.commandType);
            sb.AppendFormat("IC卡号码：{0}\r\n", IC卡号码);
            sb.AppendFormat("动态密码：{0}\r\n", 动态密码);
            sb.AppendFormat("控制类型：{0}\r\n", 控制类型.ToDescription());
            sb.AppendFormat("币数：{0}\r\n", 币数);
            sb.AppendFormat("流水号：{0}\r\n", 流水号);
            sb.AppendFormat("机头地址：{0}\r\n", 机头地址);
            return sb.ToString();
        }

        public string GetSendData()
        {
            StringBuilder sb = new StringBuilder();
            if (SendData != null)
            {
                //sb.Append("=============================================\r\n");
                sb.AppendFormat("{0:yyyy-MM-dd HH:mm:ss.fff}  发送数据\r\n", SendDataTime);
                sb.Append(PubLib.BytesToString(SendData) + Environment.NewLine);
                sb.AppendFormat("指令类别：{0}\r\n", RecvData.commandType);
                //sb.AppendFormat("指令类别：{0}\r\n", CommandType.机头卡片报警指令应答);
                sb.AppendFormat("IC卡号码：{0}\r\n", IC卡号码);
                sb.AppendFormat("币数：{0}\r\n", 币数);
                sb.AppendFormat("流水号：{0}\r\n", 流水号);
                sb.AppendFormat("机头地址：{0}\r\n", 机头地址);
            }
            return sb.ToString();
        }
    }
}
