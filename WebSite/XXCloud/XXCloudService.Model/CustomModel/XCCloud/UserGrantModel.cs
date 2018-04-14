using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace XCCloudService.Model.CustomModel.XCCloud
{
    [DataContract]
    public class UserGrantModel
    {
        [DataMember(Name = "id", Order = 1)]
        public int ID { get; set; }

        [DataMember(Name = "pId", Order = 2)]
        public int PID { get; set; }

        [DataMember(Name = "dictKey", Order = 3)]
        public string DictKey { get; set; }

        [DataMember(Name = "dictValue", Order = 4)]
        public string DictValue { get; set; }

        [DataMember(Name = "grantEn", Order = 5)]
        public int GrantEN { get; set; }
        
    }
}
