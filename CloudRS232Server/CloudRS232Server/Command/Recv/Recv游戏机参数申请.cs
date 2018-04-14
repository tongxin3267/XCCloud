using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CloudRS232Server.Command.Recv
{
    public class Recv游戏机参数申请
    {
        public string 机头地址 = "";
        public byte[] SendData = null;
        public FrameData RecvData;
        public object 应答数据;
        public bool IsDevice = false;
        public DateTime SendDataTime;

        public Recv游戏机参数申请(FrameData data)
        {
            try
            {
                机头地址 = PubLib.Hex2String(data.commandData[0]);
                CloudRS232Server.Info.HeadInfo.机头绑定信息 bind = new Info.HeadInfo.机头绑定信息();
                bind.控制器令牌 = data.Code;
                bind.短地址 = 机头地址;

                Info.HeadInfo.机头信息 head = Info.HeadInfo.GetHeadInfoByShort(bind);
                if (head != null)
                {
                    switch (head.类型)
                    {
                        case Info.HeadInfo.设备类型.存币机:
                        case Info.HeadInfo.设备类型.提_售币机:
                            IsDevice = true;
                            Ask.Ask设备参数申请 askData1 = new Ask.Ask设备参数申请(bind);
                            应答数据 = askData1;
                            break;
                        default:
                            IsDevice = false;
                            Ask.Ask终端参数申请 askData2 = new Command.Ask.Ask终端参数申请(bind);
                            应答数据 = askData2;
                            break;
                    }
                    byte[] dataBuf = PubLib.GetBytesByObject(应答数据);
                    SendData = PubLib.GetFrameDataBytes(data, dataBuf, CommandType.游戏机参数申请应答);
                    RecvData = data;
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
            sb.Append("=============================================\r\n");
            sb.AppendFormat("{0:yyyy-MM-dd HH:mm:ss.fff}  收到数据\r\n", printDate);
            sb.Append(PubLib.BytesToString(RecvData.recvData) + Environment.NewLine);
            sb.AppendFormat("指令类别：{0}\r\n", RecvData.commandType);
            sb.AppendFormat("路由器地址：{0}\r\n", RecvData.routeAddress);
            sb.AppendFormat("机头地址：{0}\r\n", 机头地址);
            return sb.ToString();
        }

        public string GetSendData()
        {
            StringBuilder sb = new StringBuilder();
            sb = new StringBuilder();
            if (!IsDevice)
            {
                Ask.Ask终端参数申请 AskData = 应答数据 as Ask.Ask终端参数申请;
                sb.Append("=============================================\r\n");
                sb.AppendFormat("{0:yyyy-MM-dd HH:mm:ss.fff}  发送数据\r\n", SendDataTime);
                sb.Append(PubLib.BytesToString(SendData) + Environment.NewLine);
                sb.Append("当前是机头在获取参数\r\n");
                sb.AppendFormat("机头地址：{0}\r\n", PubLib.Hex2String(AskData.机头地址));
                sb.AppendFormat("单次退币限额：{0}\r\n", AskData.单次退币限额);
                sb.AppendFormat("退币时接收游戏机数币数：{0}\r\n", AskData.退币时给游戏机脉冲数比例因子);
                sb.AppendFormat("退币时卡上增加币数：{0}\r\n", AskData.退币时卡上增加币数比例因子);
                sb.AppendFormat("本店卡校验密码：{0}\r\n", AskData.本店卡校验密码);
                sb.AppendFormat("开关1：{0}\r\n", PubLib.Hex2BitString(AskData.开关1));
                sb.AppendFormat("开关2：{0}\r\n", PubLib.Hex2BitString(AskData.开关2));
                sb.AppendFormat("首次投币启动间隔：{0}\r\n", AskData.首次投币启动间隔);
                sb.AppendFormat("退币速度：{0}\r\n", AskData.退币速度);
                sb.AppendFormat("退币脉宽：{0}\r\n", AskData.退币脉宽);
                sb.AppendFormat("投币速度：{0}\r\n", AskData.投币速度);
                sb.AppendFormat("投币脉宽：{0}\r\n", AskData.投币脉宽);
                sb.AppendFormat("第二路上分线上分脉宽：{0}\r\n", AskData.第二路上分线上分脉宽);
                sb.AppendFormat("第二路上分线上分启动间隔：{0}\r\n", AskData.第二路上分线首次上分启动间隔);
                sb.AppendFormat("第二路上分线上分速度：{0}\r\n", AskData.第二路上分线上分速度);
            }
            else
            {
                Ask.Ask设备参数申请 AskData = 应答数据 as Ask.Ask设备参数申请;
                sb.Append("=============================================\r\n");
                sb.AppendFormat("{0:yyyy-MM-dd HH:mm:ss.fff}  发送数据\r\n", SendDataTime);
                sb.Append(PubLib.BytesToString(SendData) + Environment.NewLine);
                sb.Append("当前是存币机在获取参数\r\n");
                sb.AppendFormat("机头地址：{0}\r\n", PubLib.Hex2String(AskData.机头地址));
                sb.AppendFormat("马达配置：{0}\r\n", PubLib.Hex2BitString(AskData.马达配置));
                sb.AppendFormat("马达1比例：{0}\r\n", AskData.马达1比例);
                sb.AppendFormat("马达2比例：{0}\r\n", AskData.马达2比例);
                sb.AppendFormat("存币箱最大存币数：{0}\r\n", AskData.存币箱最大存币数);
            }
            return sb.ToString();
        }
    }
}
