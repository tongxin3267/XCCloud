using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace PalletService.Model.SocketData
{
    [DataContract]
    public class MemberRegisterModel
    {
        [DataMember(Name = "userToken", Order = 1)]
        public string UserToken { set; get; }

        [DataMember(Name = "storeId", Order = 1)]
        public string StoreId { set; get; }

        [DataMember(Name = "mobile", Order = 1)]
        public string Mobile {set;get;}

        [DataMember(Name = "cardShape", Order = 2)]
        public string CardShape { set; get; }

        [DataMember(Name = "memberName", Order = 3)]
        public string MemberName { set; get; }

        [DataMember(Name = "birthday", Order = 4)]
        public string Birthday {set;get;}

        [DataMember(Name = "gender", Order = 5)]
        public string Gender {set;get;}

        [DataMember(Name = "identityCard", Order = 6)]
        public string IdentityCard { set; get; }

        [DataMember(Name = "note", Order = 7)]
        public string Note {set;get;}

        [DataMember(Name = "memberLevelId", Order = 8)]
        public string MemberLevelId {set;get;}

        [DataMember(Name = "foodId", Order = 9)]
        public string FoodId {set;get;}

        [DataMember(Name = "payCount", Order = 10)]
        public decimal PayCount { set; get; }

        [DataMember(Name = "realPay", Order = 11)]
        public decimal RealPay { set; get; }

        [DataMember(Name = "freePay", Order = 12)]
        public decimal FreePay { set; get; }

        [DataMember(Name = "repeatCode", Order = 12)]
        public string RepeatCode { set; get; }

        [DataMember(Name = "icCardId", Order = 13)]
        public string ICCardId {set;get;}

        [DataMember(Name = "deposit", Order = 14)]
        public decimal Deposit { set; get; }

        [DataMember(Name = "payType", Order = 15)]
        public int PayType { set; get; }

        [DataMember(Name = "saleCoinType", Order = 16)]
        public int SaleCoinType { set; get; }
    }
}
