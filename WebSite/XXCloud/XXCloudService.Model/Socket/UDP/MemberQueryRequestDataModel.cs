using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace XCCloudService.Model.Socket.UDP
{
    [DataContract]
    public class MemberQueryRequestDataModel
    {
        public MemberQueryRequestDataModel(string sn,string icCardId)
        {
            this.SN = sn;
            this.ICCardId = icCardId;
            this.SignKey = "";
        }


        [DataMember(Name = "iccardid", Order = 1)]
        public string ICCardId { set; get; }

        [DataMember(Name = "sn", Order = 1)]
        public string SN { set; get; }

        [DataMember(Name = "signkey", Order = 3)]
        public string SignKey { set; get; }

    }
}
