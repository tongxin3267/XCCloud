using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace XCCloudService.Model.CustomModel.XCCloud.Member
{
    [DataContract]
    public class MemberRegisterResponseModel
    {
        [DataMember(Name = "userName", Order = 1)]
        public string UserName {set;get;}

        [DataMember(Name = "mobile", Order = 2)]
        public string Mobile {set;get;}

        [DataMember(Name = "icCardId", Order = 3)]
        public string ICCardID {set;get;}

        [DataMember(Name = "registerTime", Order = 4)]
        public string RegisterTime {set;get;}

        [DataMember(Name = "cardBeginDate", Order = 5)]
        public string CardBeginDate {set;get;}

        [DataMember(Name = "cardEndDate", Order = 6)]
        public string CardEndDate {set;get;}
    }
}
