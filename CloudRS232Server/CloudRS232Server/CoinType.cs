using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CloudRS232Server
{
    public enum CoinType
    {
        实物投币 = 0,
        电子投币 = 1,
        实物退币 = 2,
        电子退币 = 3,
        电子存币 = 4,
        会员卡提币 = 5,
        电子碎票 = 6,
        实物彩票 = 7,
        IC退彩票 = 8,
        远程出币通知 = 9,
        远程存币通知 = 10,
        礼品掉落 = 11,
    }
}
