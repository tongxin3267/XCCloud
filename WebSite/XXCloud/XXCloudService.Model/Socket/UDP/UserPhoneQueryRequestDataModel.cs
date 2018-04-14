using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace XCCloudService.Model.Socket.UDP
{
    [DataContract]
    public class UserPhoneQueryRequestDataModel
    {
        public UserPhoneQueryRequestDataModel(string sn, string mobile)
        {
            this.SN = sn;
            this.Mobile = mobile;
            this.SignKey = "";
        }

        [DataMember(Name = "sn", Order = 1)]
        public string SN { set; get; }

        [DataMember(Name = "phone", Order = 2)]
        public string Mobile { set; get; }

        [DataMember(Name = "signkey", Order = 3)]
        public string SignKey { set; get; }
    }
}
