using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using XCCloudService.Common.Enum;

namespace XCCloudService.Model.CustomModel.XCCloud
{
    [DataContract]
    public class UserLogResponseModel
    {
        [DataMember(Name = "token", Order = 1)]
        public string Token { get; set; }

        [DataMember(Name = "logType", Order = 2)]
        public int LogType { get; set; }

        [DataMember(Name = "merchTag", Order = 3)]
        public int? MerchTag { get; set; }

        [DataMember(Name = "userType", Order = 4)]
        public int? UserType { get; set; }

        [DataMember(Name = "switchable", Order = 4)]
        public int? Switchable { get; set; }
    }    
}
