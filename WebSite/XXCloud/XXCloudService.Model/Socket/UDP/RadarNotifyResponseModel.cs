using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace XCCloudService.Model.Socket.UDP
{
    [DataContract]
    public class RadarNotifyResponseModel
    {
        public RadarNotifyResponseModel(string sn, string signkey)
        {
            this.SN = sn;
            this.SignKey = signkey;
        }

        [DataMember(Name = "sn", Order = 1)]
        public string SN { set; get; }

        [DataMember(Name = "signkey", Order = 2)]
        public string SignKey { set; get; }
    }
}
