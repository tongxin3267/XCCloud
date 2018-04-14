using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace PalletService.Model.Member
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

        [DataMember(Name = "banlance", Order = 7)]
        public decimal Banlance { set; get; }

        [DataMember(Name = "point", Order = 8)]
        public decimal Point { set; get; }

        [DataMember(Name = "deposit", Order = 9)]
        public decimal Deposit { set; get; }

        [DataMember(Name = "memberState", Order = 10)]
        public int MemberState { set; get; }

        [DataMember(Name = "lottery", Order = 11)]
        public decimal Lottery { set; get; }

        [DataMember(Name = "note", Order = 12)]
        public string Note { set; get; }
        [DataMember(Name = "memberLevelName", Order = 13)]
        public string MemberLevelName { set; get; }

        [DataMember(Name = "endDate", Order = 14)]
        public string EndDate { set; get; }

        [DataMember(Name = "repeatCode", Order = 15)]
        public int RepeatCode { set; get; }


        [DataMember(Name = "storeId", Order = 16)]
        public string StoreId { set; get; }


        [DataMember(Name = "StoreName", Order = 17)]
        public string StoreName { set; get; }

        [DataMember(Name = "storage", Order = 18)]
        public decimal Storage { set; get; }

    }
}
