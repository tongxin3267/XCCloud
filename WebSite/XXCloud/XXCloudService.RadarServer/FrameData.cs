using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XXCloudService.RadarServer
{
    public class FrameData
    {
        /// <summary>
        /// 是否完整
        /// </summary>
        public bool isComplite = false;
        public FrameType frameType;
        public string routeAddress = "";
        public CommandType commandType;
        public byte[] commandData;
        public int commandLength;
        public byte[] recvData;
        public string Code = "";

        public FrameData()
        { }

        public FrameData(byte[] data)
        {
            recvData = data;
            try
            {
                if (CRC.CRCCheck(data))
                {
                    isComplite = true;
                    frameType = (FrameType)data[5];
                    routeAddress = PubLib.Hex2String(data[7]) + PubLib.Hex2String(data[6]);
                    commandType = (CommandType)data[8];
                    commandLength = (int)data[9];
                    commandData = data.Skip(10).Take(commandLength).ToArray();
                }
                else
                {
                    LogHelper.WriteLog("数据接收错误", data);
                    PubLib.当前错误指令数++;
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("数据接收错误", data);
                LogHelper.WriteLog(ex);
            }
        }
    }
}
