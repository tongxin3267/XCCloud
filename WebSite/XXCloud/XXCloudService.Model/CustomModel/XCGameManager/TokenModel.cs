using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using XCCloudService.Model.CustomModel.XCGame;

namespace XCCloudService.Model.CustomModel.XCGameManager
{
    [DataContract] 
    public class SMSTokenModel
    {
        public SMSTokenModel(string mobile,string token)
        {
            this.Mobile = mobile;
            this.Token = token;
        }

        [DataMember(Name = "mobile", Order = 1)]
        public string Mobile { set; get; }

        [DataMember(Name = "token", Order = 2)]
        public string Token { set; get; }
    }

    [DataContract] 
    public class MobileTokenResponseModel
    {
        public MobileTokenResponseModel(string mobile, string token)
        {
            this.Mobile = mobile;
            this.Token = token;
        }

        [DataMember(Name = "mobile", Order = 1)]
        public string Mobile { set; get; }

        [DataMember(Name = "mobileToken", Order = 2)]
        public string Token { set; get; }
    }

    [DataContract] 
    public class MemberTokenResponseModel
    {
        public MemberTokenResponseModel(string storeId,string storeName, string memberToken, bool isMember, MemberResponseModel member)
        {
            this.StoreId = storeId;
            this.MemberToken = memberToken;
            this.IsMember = isMember;
            this.Member = member;
            this.StoreName = storeName;
        }

        [DataMember(Name = "storeId", Order = 1)]
        public string StoreId { set; get; }

        [DataMember(Name = "storeName", Order = 1)]
        public string StoreName { set; get; }

        [DataMember(Name = "memberToken", Order = 2)]
        public string MemberToken { set; get; }

        [DataMember(Name = "isMember", Order = 3)]
        public bool IsMember { set; get; }

        [DataMember(Name = "member", Order = 4)]
        public MemberResponseModel Member { set; get; }
    }

    [DataContract] 
    public class RadarTokenModel
    {
        public RadarTokenModel(string storeId, string segment, string routeDeviceToken)
        {
            this.StoreId = storeId;
            this.Segment = segment;
            this.RouteDeviceToken = routeDeviceToken;
        }

        [DataMember(Name = "storeId", Order = 1)]
        public string StoreId { set; get; }

        [DataMember(Name = "segment", Order = 2)]
        public string Segment { set; get; }

        [DataMember(Name = "routeDeviceToken", Order = 2)]
        public string RouteDeviceToken { set; get; }
    }
}
