using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace XCCloudService.Model.Socket.UDP
{
    [DataContract]
    public class OutTicketQueryResultNotifyRequestModel
    {
        public OutTicketQueryResultNotifyRequestModel()
        {

        }

        [DataMember(Name = "result_code", Order = 1)]
        public string Result_Code { set; get; }

        [DataMember(Name = "result_msg", Order = 2)]
        public string Result_Msg { set; get; }

        [DataMember(Name = "signkey", Order = 3)]
        public string SignKey { set; get; }

        [DataMember(Name = "result_data", Order = 4)]
        public OutTicketQueryResultModel Result_Data { set; get; }

        [DataMember(Name = "sn", Order = 5)]
        public string SN { set; get; }
    }

    [DataContract]
    public class OutTicketQueryResultModel
    {
        public OutTicketQueryResultModel()
        {

        }

        [DataMember(Name = "id", Order = 1)]
        public string Id { set; get; }

        //项目名称
        [DataMember(Name = "coins", Order = 2)]
        public string Coins { set; get; }

        //门票状态
        [DataMember(Name = "gamename", Order = 3)]
        public string GameName { set; get; }

        //消费类型
        [DataMember(Name = "headinfo", Order = 4)]
        public string HeadInfo { set; get; }

        //剩余次数
        [DataMember(Name = "state", Order = 5)]
        public string State { set; get; }

        //截止时间
        [DataMember(Name = "printdate", Order = 6)]
        public string PrintDate { set; get; }
    }
}
