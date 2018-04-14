using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace XCCloudService.Model.Socket.UDP
{
    [DataContract]
    public class TicketQueryResultNotifyRequestModel
    {
        public TicketQueryResultNotifyRequestModel()
        {

        }

        [DataMember(Name = "result_code", Order = 1)]
        public string Result_Code { set; get; }

        [DataMember(Name = "result_msg", Order = 2)]
        public string Result_Msg { set; get; }

        [DataMember(Name = "signkey", Order = 3)]
        public string SignKey { set; get; }

        [DataMember(Name = "sn", Order = 4)]
        public string SN { set; get; }

        [DataMember(Name = "result_data", Order = 5)]
        public TicketQueryResultModel Result_Data { set; get; }
    }

    [DataContract]
    public class TicketQueryResultModel
    {
        public TicketQueryResultModel()
        {

        }


        [DataMember(Name = "id", Order = 1)]
        public string Id { set; get; }

        //项目名称
        [DataMember(Name = "projectname", Order = 2)]
        public string ProjectName { set; get; }

        //门票状态
        [DataMember(Name = "state", Order = 3)]
        public string State { set; get; }

        //消费类型
        [DataMember(Name = "projecttype", Order = 4)]
        public string ProjectType { set; get; }

        //剩余次数
        [DataMember(Name = "remaincount", Order = 5)]
        public string RemainCount { set; get; }

        //截止时间
        [DataMember(Name = "endtime", Order = 6)]
        public string endtime { set; get; }
    }

}
