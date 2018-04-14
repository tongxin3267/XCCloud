using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace XCCloudService.Model.CustomModel.XCGame
{
    [DataContract]
    public class MemberResponseModel
    {
        public MemberResponseModel()
        {
            ICCardID = 0;
            MemberName = string.Empty;
            Gender = string.Empty;
            Birthday = string.Empty;
            CertificalID = string.Empty;
            Mobile = string.Empty;
            Balance = 0;
            Point = 0;
            Deposit = 0;
            MemberState = string.Empty;
            Lottery = 0;
            Note = string.Empty;
            EndDate = string.Empty;
            MemberLevelName = string.Empty;
        }

        [DataMember(Name = "icCardID", Order = 2)]
        public int ICCardID { set; get; }

        [DataMember(Name = "memberName", Order = 3)]
        public string MemberName { set; get; }

        [DataMember(Name = "gender", Order = 4)]
        public string Gender { set; get; }

        [DataMember(Name = "birthday", Order = 5)]
        public string Birthday { set; get; }

        [DataMember(Name = "certificalID", Order = 6)]
        public string CertificalID {set;get;}

        [DataMember(Name = "mobile", Order = 7)]
        public string Mobile { set; get; }

        [DataMember(Name = "balance", Order = 8)]
        public int Balance { set; get; }

        [DataMember(Name = "point", Order = 9)]
        public int Point { set; get; }

        [DataMember(Name = "deposit", Order = 10)]
        public decimal Deposit { set; get; }

        [DataMember(Name = "memberState", Order = 11)]
        public string MemberState { set; get; }

        [DataMember(Name = "lottery", Order = 12)]
        public int Lottery { set; get; }

        [DataMember(Name = "note", Order = 13)]
        public string Note { set; get; }

        [DataMember(Name = "endDate", Order = 14)]
        public string EndDate { set; get; }

        [DataMember(Name = "memberLevelName", Order = 15)]
        public string MemberLevelName { set; get; }
    }

    [DataContract]
    public class RegisterMemberTokenResponseModel
    {
        public RegisterMemberTokenResponseModel(string storeId,string storeName, string memberToken, MemberResponseModel member)
        {
            this.StoreId = storeId;
            this.MemberToken = memberToken;
            this.Member = member;
            this.StoreName = storeName;
        }

        [DataMember(Name = "storeId", Order = 1)]
        public string StoreId { set; get; }

        [DataMember(Name = "storeName", Order = 2)]
        public string StoreName { set; get; }

        [DataMember(Name = "memberToken", Order = 3)]
        public string MemberToken { set; get; }

        [DataMember(Name = "member", Order = 4)]
        public MemberResponseModel Member { set; get; }
    }

    [DataContract]
    public class RegisterMemberResponseModel
    {
        [DataMember(Name = "storeId", Order = 1)]
        public string StoreId { set; get; }

        [DataMember(Name = "storeName", Order = 2)]
        public string StoreName { set; get; }

        [DataMember(Name = "icCardID", Order = 3)]
        public int ICCardID { set; get; }

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
        public int Balance { set; get; }

        [DataMember(Name = "point", Order = 10)]
        public int Point { set; get; }

        [DataMember(Name = "deposit", Order = 11)]
        public decimal Deposit { set; get; }

        [DataMember(Name = "memberState", Order = 12)]
        public string MemberState { set; get; }

        [DataMember(Name = "lottery", Order = 13)]
        public int Lottery { set; get; }

        [DataMember(Name = "note", Order = 14)]
        public string Note { set; get; }

        [DataMember(Name = "endDate", Order = 15)]
        public string EndDate { set; get; }

        [DataMember(Name = "memberLevelName", Order = 16)]
        public string MemberLevelName { set; get; }


    }


    [DataContract]
    public class CattleMemberCardModel
    {
        public CattleMemberCardModel(string storeId, string storeName, List<CattleMemberCardDetailModel> list)
        {
            this.StoreId = storeId;
            this.StoreName = storeName;
            this.CardDetail = list;
        }

        [DataMember(Name = "storeId", Order = 1)]
        public string StoreId { set; get; }

        [DataMember(Name = "storeName", Order = 2)]
        public string StoreName { set; get; }

        [DataMember(Name = "cardDetail", Order = 3)]
        List<CattleMemberCardDetailModel> CardDetail;
    }

    [DataContract]
    public class CattleMemberCardDetailModel
    {
        [DataMember(Name = "icCardID", Order = 1)]
        public int ICCardID { set; get; }

        [DataMember(Name = "memberName", Order = 2)]
        public string MemberName { set; get; }

        [DataMember(Name = "memberState", Order = 3)]
        public string MemberState { set; get; }

        [DataMember(Name = "mobile", Order = 4)]
        public string Mobile { set; get; }
    }

}