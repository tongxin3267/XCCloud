using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace XCCloudService.Model.CustomModel.XCGameManager
{
    [DataContract]
    public class TmpmemberModel
    {
        [DataMember(Name = "Token", Order = 1)]
        public string Token { set; get; }

        [DataMember(Name = "StoreId", Order = 2)]
        public string StoreId { set; get; }

        [DataMember(Name = "Phone", Order = 3)]
        public string Phone { set; get; }

        [DataMember(Name = "ICCardID", Order = 4)]
        public string ICCardID { set; get; }

        [DataMember(Name = "MemberLevelName", Order = 5)]
        public string MemberLevelName { set; get; }

        [DataMember(Name = "StoreName", Order = 6)]
        public string StoreName { set; get; }

        [DataMember(Name = "EndTime", Order = 7)]
        public string EndTime { set; get; }

    }
}
