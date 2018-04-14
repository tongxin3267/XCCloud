using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XXCloudService.RadarServer.Command.Recv
{
    public class Recv机头地址动态分配
    {
        public string MCUID = "";
        public FrameData RecvData;
        public string 机头地址;
        public byte[] SendData;
        public DateTime SendDataTime;

        public Recv机头地址动态分配(FrameData data)
        {
            try
            {
                RecvData = data;

                if (data.commandData.Length >= 8)
                {
                    List<byte> mid = new List<byte>(data.commandData);
                    UInt64 ui64 = BitConverter.ToUInt64(mid.ToArray(), 0);
                    MCUID = Convert.ToString((long)ui64, 16).ToUpper().Substring(0,14);

                    //if (!SecrityHeadInfo.CheckHead(MCUID)) return;
                    Ask.Ask机头地址动态分配 ask = new Command.Ask.Ask机头地址动态分配(data.Code, MCUID);
                    机头地址 = PubLib.Hex2String(ask.机头地址);
                    byte[] readyToSend = PubLib.GetBytesByObject(ask);
                    SendData = PubLib.GetFrameDataBytes(data, readyToSend, CommandType.机头地址动态分配应答);
                    SendDataTime = DateTime.Now;

                    LogHelper.WriteLogSpacail(string.Format("{0} 申请地址 {1}", MCUID, 机头地址));
                }
            }
            catch
            {
                throw;
            }
        }

        public string GetRecvData(DateTime printDate)
        {
            StringBuilder sb = new StringBuilder();
            //sb.Append("=============================================\r\n");
            sb.AppendFormat("{0:yyyy-MM-dd HH:mm:ss.fff}  收到数据\r\n", printDate);
            sb.Append(PubLib.BytesToString(RecvData.recvData) + Environment.NewLine);
            sb.AppendFormat("指令类别：{0}\r\n", RecvData.commandType);
            sb.AppendFormat("路由器地址：{0}\r\n", RecvData.routeAddress);
            sb.AppendFormat("长地址：{0}\r\n", MCUID);
            return sb.ToString();
        }

        public string GetSendData()
        {
            StringBuilder sb = new StringBuilder();
            sb = new StringBuilder();
            //sb.Append("=============================================\r\n");
            sb.AppendFormat("{0:yyyy-MM-dd HH:mm:ss.fff}  发送数据\r\n", SendDataTime);
            sb.Append(PubLib.BytesToString(SendData) + Environment.NewLine);
            sb.AppendFormat("指令类别：{0}\r\n", RecvData.commandType);
            sb.AppendFormat("机头地址：{0}\r\n", 机头地址);
            sb.AppendFormat("长地址：{0}\r\n", MCUID);
            return sb.ToString();
        }
    }
}
