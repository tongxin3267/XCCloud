using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace XCCloudService.Model.Socket.UDP
{
    [DataContract]
    public class ParamQueryRequestDataModel
    {
        public ParamQueryRequestDataModel(string sn,string requestType)
        {
            this.SN = sn;
            this.RequestType = requestType;
            this.SignKey = "";
        }

        [DataMember(Name = "sn", Order = 1)]
        public string SN { set; get; }

        [DataMember(Name = "requesttype", Order = 2)]
        public string RequestType { set; get; }

        [DataMember(Name = "signkey", Order = 3)]
        public string SignKey { set; get; }
    }
}
