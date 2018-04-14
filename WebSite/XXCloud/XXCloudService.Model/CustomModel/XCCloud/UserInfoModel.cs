using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using XCCloudService.Model.XCCloud;

namespace XCCloudService.Model.CustomModel.XCCloud
{
    [DataContract]
    public class UserInfoCollection<T>
    {
        [DataMember(Name = "user_info_list", Order = 1)]
        public List<T> UserInfoList;
    }

    [DataContract]
    public class UserInfoModel
    {
        [DataMember(Name = "subscribe", Order = 1)]
        public int Subscribe { get; set; }

        [DataMember(Name = "openid", Order = 2)]
        public string OpenID { get; set; }

        [DataMember(Name = "nickname", Order = 3)]
        public string NickName { get; set; }
    }

    [DataContract]
    public class UserInfoDetailModel : UserInfoModel
    {        
        [DataMember(Name = "sex", Order = 4)]
        public int Sex { get; set; }

        [DataMember(Name = "language", Order = 5)]
        public string Language { get; set; }

        [DataMember(Name = "city", Order = 6)]
        public string City { get; set; }

        [DataMember(Name = "province", Order = 7)]
        public string Province { get; set; }

        [DataMember(Name = "country", Order = 8)]
        public string Country { get; set; }

        [DataMember(Name = "headimgurl", Order = 9)]
        public string Headimgurl { get; set; }

        [DataMember(Name = "subscribe_time", Order = 10)]
        public long Subscribe_time { get; set; }

        [DataMember(Name = "unionid", Order = 11)]
        public string Unionid { get; set; }

        [DataMember(Name = "remark", Order = 12)]
        public string Remark { get; set; }

        [DataMember(Name = "groupid", Order = 13)]
        public int Groupid { get; set; }

        [DataMember(Name = "tagid_list", Order = 14)]
        public List<int> Tagid_list { get; set; }

        [DataMember(Name = "userId", Order = 15)]
        public Nullable<int> UserID { get; set; }
    }

    [DataContract]
    public class XcUserInfoModel
    {
        [DataMember(Name = "userId", Order = 1)]
        public int UserID { get; set; }

        [DataMember(Name = "logName", Order = 2)]
        public string LogName { get; set; }

        [DataMember(Name = "openId", Order = 3)]
        public string OpenID { get; set; }

        [DataMember(Name = "realName", Order = 4)]
        public string RealName { get; set; }

        [DataMember(Name = "mobile", Order = 5)]
        public string Mobile { get; set; }

        [DataMember(Name = "iCCardId", Order = 6)]
        public string ICCardID { get; set; }

        [DataMember(Name = "status", Order = 7)]
        public Nullable<int> Status { get; set; }

        [DataMember(Name = "unionId", Order = 8)]
        public string UnionID { get; set; }

        [DataMember(Name = "userGroupId", Order = 9)]
        public Nullable<int> UserGroupID { get; set; }
    }    
}
