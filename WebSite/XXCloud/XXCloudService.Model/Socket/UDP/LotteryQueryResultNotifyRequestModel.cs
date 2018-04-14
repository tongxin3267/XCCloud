using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace XCCloudService.Model.Socket.UDP
{
    [DataContract]
    public class LotteryQueryResultNotifyRequestModel
    {
        public LotteryQueryResultNotifyRequestModel()
        {

        }

        [DataMember(Name = "result_code", Order = 1)]
        public string Result_Code { set; get; }

        [DataMember(Name = "result_msg", Order = 2)]
        public string Result_Msg { set; get; }

        [DataMember(Name = "sn", Order = 3)]
        public string SN { set; get; }

        [DataMember(Name = "signkey", Order = 4)]
        public string SignKey { set; get; }

        [DataMember(Name = "result_data", Order = 5)]
        public LotteryQueryResultModel Result_Data { set; get; }
    }

    [DataContract]
    public class LotteryQueryResultModel
    {
        public LotteryQueryResultModel()
        {

        }


        [DataMember(Name = "id", Order = 1)]
        public string Id { set; get; }

        //彩票数
        [DataMember(Name = "lottery", Order = 2)]
        public string Lottery { set; get; }

        //游戏机名
        [DataMember(Name = "gamename", Order = 3)]
        public string GameName { set; get; }

        //出票位置
        [DataMember(Name = "headinfo", Order = 4)]
        public string HeadInfo { set; get; }

        //剩余次数
        [DataMember(Name = "state", Order = 5)]
        public string State { set; get; }

        //出票时间
        [DataMember(Name = "printdate", Order = 6)]
        public string PrintDate { set; get; }
    }

}
