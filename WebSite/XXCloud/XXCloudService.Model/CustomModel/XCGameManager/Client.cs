using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace XCCloudService.Model.CustomModel.XCGameManager
{
    [DataContract]
    public class Client
    {
        [DataMember(Name = "id", Order = 11)]
        public int id { set; get; }
        [DataMember(Name = "store_password", Order = 12)]
        public string store_password { set; get; }
    }
}
