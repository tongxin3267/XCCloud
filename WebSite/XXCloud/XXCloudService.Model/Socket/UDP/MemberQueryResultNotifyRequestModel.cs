using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace XCCloudService.Model.Socket.UDP
{
    [DataContract]
    public class MemberQueryResultNotifyRequestModel
    {
        public MemberQueryResultNotifyRequestModel()
        {

        }

        [DataMember(Name = "result_code", Order = 1)]
        public string Result_Code { set; get; }

        [DataMember(Name = "result_msg", Order = 2)]
        public string Result_Msg { set; get; }

        [DataMember(Name = "signkey", Order = 3)]
        public string SignKey { set; get; }

        [DataMember(Name = "result_data", Order = 3)]
        public MemberQueryResultModel Result_Data { set; get; }

        [DataMember(Name = "sn", Order = 4)]
        public string SN { set; get; }

        public string StoreId { set; get; }
    }

    [DataContract]
    public class MemberQueryResultModel
    {
        public MemberQueryResultModel()
        { 
            
        }

        [DataMember(Name = "storeId", Order = 1)]
        public string StoreId { set; get; }

        [DataMember(Name = "storeName", Order = 2)]
        public string StoreName { set; get; }

        [DataMember(Name = "icCardID", Order = 3)]
        public string ICCardID { set; get; }

        [DataMember(Name = "memberName", Order = 4)]
        public string MemberName { set; get; }

        [DataMember(Name = "gender", Order = 5)]
        public string Gender { set; get; }

        [DataMember(Name = "birthday", Order = 6)]
        public string Birthday { set; get; }

        [DataMember(Name = "certificalID", Order = 7)]
        public string CertificalID { set; get; }

        [DataMember(Name = "mobile", Order = 8)]
        public string Mobile { set; get; }

        [DataMember(Name = "balance", Order = 9)]
        public decimal Balance { set; get; }

        [DataMember(Name = "point", Order = 10)]
        public decimal Point { set; get; }

        [DataMember(Name = "deposit", Order = 11)]
        public decimal Deposit { set; get; }

        [DataMember(Name = "memberState", Order = 12)]
        public string MemberState { set; get; }

        [DataMember(Name = "lottery", Order = 13)]
        public decimal Lottery { set; get; }

        [DataMember(Name = "note", Order = 14)]
        public string Note { set; get; }

        [DataMember(Name = "memberLevelName", Order = 15)]
        public string MemberLevelName { set; get; }

        [DataMember(Name = "endDate", Order = 16)]
        public string EndDate { set; get; }
    }
}
