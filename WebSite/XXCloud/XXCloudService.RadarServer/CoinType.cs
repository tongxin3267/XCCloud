using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace XXCloudService.RadarServer
{
    public enum CoinType
    {
        [Description("实物投币")]
        实物投币 = 0,
        [Description("电子投币")]
        电子投币 = 1,
        [Description("实物退币")]
        实物退币 = 2,
        [Description("电子退币")]
        电子退币 = 3,
        [Description("电子存币")]
        电子存币 = 4,
        [Description("会员卡提币")]
        会员卡提币 = 5,
        [Description("电子碎票")]
        电子碎票 = 6,
        [Description("实物彩票")]
        实物彩票 = 7,
        [Description("IC退彩票")]
        IC退彩票 = 8,
        [Description("远程出币通知")]
        远程出币通知 = 9,
        [Description("远程存币通知")]
        远程存币通知 = 10,
        [Description("礼品掉落")]
        礼品掉落 = 11,
    }
}
