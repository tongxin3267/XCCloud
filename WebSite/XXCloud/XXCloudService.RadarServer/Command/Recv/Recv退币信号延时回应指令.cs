using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XXCloudService.RadarServer.Command.Recv
{
    public class Recv退币信号延时回应指令
    {
        public string 机头地址 = "";
        public int 退币信号超时时间 = 0;
        public UInt16 流水号;
        public byte[] SendData;

        public Recv退币信号延时回应指令(FrameData data)
        {
            //机头地址 = PubLib.Hex2String(data.commandData[0]);
            //退币信号超时时间 = (int)BitConverter.ToUInt16(data.commandData, 1);
            //流水号 = BitConverter.ToUInt16(data.commandData, 3);

            //StringBuilder sb = new StringBuilder();
            //sb.Append("=============================================\r\n");
            //sb.AppendFormat("{0:yyyy-MM-dd HH:mm:ss.fff}  收到数据\r\n", DateTime.Now);
            //sb.Append(PubLib.BytesToString(data.recvData) + Environment.NewLine);
            //sb.AppendFormat("指令类别：{0}\r\n", data.commandType);
            //sb.AppendFormat("机头地址：{0}\r\n", 机头地址);
            //sb.AppendFormat("退币信号超时时间：{0}\r\n", 退币信号超时时间);
            //sb.AppendFormat("流水号：{0}\r\n", 流水号);
            //UIClass.接收内容 = sb.ToString();
            //Ask.Ask退币信号延时应答指令应答 a = new Ask.Ask退币信号延时应答指令应答(机头地址, 流水号);
            //Info.HeadInfo.机头信息 head = Info.HeadInfo.GetHeadInfo(PubLib.路由器段号, 机头地址);
            //head.常规.退币保护启用标志 = true;
            //Info.GameInfo.CheckMDTimeout(机头地址, 退币信号超时时间);
            //byte[] dataBuf = PubLib.GetBytesByObject(a);
            //SendData = PubLib.GetFrameDataBytes(data, dataBuf, CommandType.退币信号延时应答指令应答);
            //TerminalDataProcess.SendData(SendData);
            //sb = new StringBuilder();
            //sb.Append("=============================================\r\n");
            //sb.AppendFormat("{0:yyyy-MM-dd HH:mm:ss.fff}  发送数据\r\n", DateTime.Now);
            //sb.Append(PubLib.BytesToString(SendData) + Environment.NewLine);
            //sb.AppendFormat("指令类别：{0}\r\n", CommandType.退币信号延时应答指令应答);
            //sb.AppendFormat("机头地址：{0}\r\n", 机头地址);
            //sb.AppendFormat("流水号：{0}\r\n", 流水号);
            //UIClass.发送内容 = sb.ToString();
        }
    }
}
