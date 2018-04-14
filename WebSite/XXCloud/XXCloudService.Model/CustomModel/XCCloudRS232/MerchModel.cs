using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace XCCloudService.Model.CustomModel.XCCloudRS232
{
    [DataContract]
    public class MerchModel
    {
        public MerchModel(string merchName, string opName, string token, Nullable<int> state)
        {
            this.MerchName = merchName;
            this.OPName = opName;
            this.Token = token;
            this.State = state == 1 ? "已激活" : "已禁用";
        }

        [DataMember(Name = "MerchName", Order = 1)]
        public string MerchName { get; set; }

        [DataMember(Name = "OPName", Order = 2)]
        public string OPName { get; set; }

        [DataMember(Name = "Token", Order = 3)]
        public string Token { get; set; }

        [DataMember(Name = "State", Order = 4)]
        public string State { get; set; }
    }

    [DataContract]
    public class GroupInfoModel
    {
        [DataMember(Name = "groupId", Order = 0)]
        public int groupId { get; set; }

        [DataMember(Name = "groupName", Order = 1)]
        public string groupName { get; set; }

        [DataMember(Name = "Peripherals", Order = 5)]
        public List<Terminal> Terminals { get; set; }
    }

    [DataContract]
    public class Terminal
    {
        [DataMember(Name = "terminalName", Order = 0)]
        public string terminalName { get; set; }

        [DataMember(Name = "terminalToken", Order = 1)]
        public string terminalToken { get; set; }

        [DataMember(Name = "sn", Order = 2)]
        public string sn { get; set; }

        [DataMember(Name = "deviceType", Order = 3)]
        public string deviceType { get; set; }

        [DataMember(Name = "headAddress", Order = 4)]
        public string headAddress { get; set; }

        [DataMember(Name = "status", Order = 5)]
        public string status { get; set; }
    }
}
