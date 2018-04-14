using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace XCCloudService.Model.CustomModel.XCCloud
{
    [DataContract]
    public class BaseMemberModel
    {
        [DataMember(Name = "icCardID", Order = 1)]
        public string ICCardID { set; get; }
        [DataMember(Name = "memberName", Order = 2)]
        public string MemberName { set; get; }

        [DataMember(Name = "gender", Order = 3)]
        public int Gender { set; get; }
        [DataMember(Name = "birthday", Order = 4)]
        public string Birthday { set; get; }
        [DataMember(Name = "certificalID", Order = 5)]
        public string IDCard { set; get; }
        [DataMember(Name = "mobile", Order = 6)]
        public string Mobile { set; get; }

        [DataMember(Name = "memberState", Order = 7)]
        public int MemberState { set; get; }

        [DataMember(Name = "note", Order = 8)]
        public string Note { set; get; }
        [DataMember(Name = "memberLevelName", Order = 9)]
        public string MemberLevelName { set; get; }

        [DataMember(Name = "endDate", Order = 10)]
        public string EndDate { set; get; }

        [DataMember(Name = "repeatCode", Order = 11)]
        public int RepeatCode { set; get; }


        [DataMember(Name = "storeId", Order = 12)]
        public string StoreId { set; get; }


        [DataMember(Name = "storeName", Order = 13)]
        public string StoreName { set; get; }

        [DataMember(Name = "storage", Order = 14)]
        public decimal Storage { set; get; }

        [DataMember(Name = "banlance", Order = 15)]
        public decimal Banlance { set; get; }

        [DataMember(Name = "point", Order = 16)]
        public decimal Point { set; get; }

        [DataMember(Name = "lottery", Order = 17)]
        public decimal Lottery { set; get; }

        [DataMember(Name = "deposit", Order = 18)]
        public decimal Deposit { set; get; }
        
    }
}
