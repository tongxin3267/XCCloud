using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace XCCloudService.Model.CustomModel.XCCloud
{
    [DataContract]
    public partial class DictionaryResponseModel
    {
        [DataMember(Name = "name", Order = 1)]
        public string DictKey { get; set; }        

        [DataMember(Name = "children", Order = 2)]
        public List<DictionaryResponseModel> Children { get; set; }

        [DataMember(Name = "id", Order = 3)]
        public int ID { get; set; }

        [IgnoreDataMember]
        public Nullable<int> PID { get; set; }

        [DataMember(Name = "dictValue", Order = 4)]
        public string DictValue { get; set; }

        [IgnoreDataMember]
        public string Comment { get; set; }

        [IgnoreDataMember]
        public Nullable<int> OrderID { get; set; }

        [DataMember(Name = "enabled", Order = 5)]
        public Nullable<int> Enabled { get; set; }

        [DataMember(Name = "merchId", Order = 6)]
        public string MerchID { get; set; }

        [IgnoreDataMember]
        public Nullable<int> DictLevel { get; set; }
    }
}
