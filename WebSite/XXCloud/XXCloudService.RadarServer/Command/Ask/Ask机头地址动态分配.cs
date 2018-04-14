using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XXCloudService.RadarServer.Command.Ask
{
    public class Ask机头地址动态分配
    {
        public byte 机头地址 { get; set; }
        public UInt64 MCUID { get; set; }
        /// <summary>
        /// 是否为有效地址
        /// </summary>
        public bool isSuccess = false;
        public Ask机头地址动态分配(string Code, string longAddress)
        {
            try
            {
                MCUID = Convert.ToUInt64(longAddress, 16);
                Info.HeadInfo.机头信息 机头 = Info.HeadInfo.GetHeadInfoByFull(longAddress);
                if (机头 == null)
                {
                    机头地址 = 0xfe;    //不属于该控制器下的设备，场地内有多个控制器
                    return;
                }
                if (机头.常规.路由器编号 == Code)
                    机头地址 = Convert.ToByte(机头.常规.机头地址);
                else
                    机头地址 = 0xfe;    //不属于该控制器下的设备，场地内有多个控制器
            }
            catch
            {
                throw;
            }
        }
    }
}
