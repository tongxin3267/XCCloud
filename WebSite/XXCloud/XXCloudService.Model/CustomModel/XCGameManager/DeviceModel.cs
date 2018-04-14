using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace XCCloudService.Model.CustomModel.XCGameManager
{
    [DataContract]
    public class DeviceModel
    {
        [DataMember(Name = "MCUID", Order = 1)]
        public string MCUID { set; get; }

        [DataMember(Name = "GameName", Order = 2)]
        public string GameName { set; get; }

        [DataMember(Name = "GameType", Order = 3)]
        public string GameType { set; get; }
    }
}
