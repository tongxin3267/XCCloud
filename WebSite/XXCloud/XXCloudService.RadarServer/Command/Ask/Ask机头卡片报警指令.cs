using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XXCloudService.RadarServer.Command.Ask
{
    public class Ask机头卡片报警指令
    {
        public byte 机头地址 { get; set; }
        public UInt16 流水号 { get; set; }
        public Ask机头卡片报警指令(string hAddress, UInt16 SN)
        {
            机头地址 = Convert.ToByte(hAddress, 16);
            流水号 = SN;
        }
    }
}
