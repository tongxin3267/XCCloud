using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace XCCloudService.Model.Socket.UDP
{
    [DataContract]
    public class CattleMemberCardQueryResultNotifyRequestModel
    {
        public CattleMemberCardQueryResultNotifyRequestModel()
        {

        }

        [DataMember(Name = "result_code", Order = 1)]
        public string Result_Code { set; get; }

        [DataMember(Name = "result_msg", Order = 2)]
        public string Result_Msg { set; get; }

        [DataMember(Name = "cardcount", Order = 3)]
        public string CardCount { set; get; }

        [DataMember(Name = "result_data", Order = 4)]
        public List<CattleMemberCardQueryResultNotifyRequestDetailModel> Result_Data { set; get; }

        [DataMember(Name = "signkey", Order = 5)]
        public string SignKey { set; get; }

        [DataMember(Name = "sn", Order = 6)]
        public string SN { set; get; }

    }

    [DataContract]
    public class CattleMemberCardQueryResultNotifyRequestDetailModel
    { 
        [DataMember(Name = "iccardid", Order = 1)]
        public string ICCardId { set; get; }

        [DataMember(Name = "name", Order = 2)]
        public string Name { set; get; }

        [DataMember(Name = "memberstate", Order = 3)]
        public string MemberState { set; get; }

        [DataMember(Name = "phone", Order = 4)]
        public string Phone { set; get; }

    }
}
