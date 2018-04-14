using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace XCCloudService.Model.CustomModel.XCCloudRS232
{
    [DataContract]
    public class BaseDeviceInfoModelList
    {
        [DataMember(Name = "Lists", Order = 1)]
        public List<BaseDeviceInfoModel> Lists { set; get; }
    }
    [DataContract]
    public class BaseDeviceInfoModel
    {
        [DataMember(Name = "SN", Order = 1)]
        public string SN { set; get; }
        [DataMember(Name = "Token", Order = 2)]
        public string Token { set; get; }

        [DataMember(Name = "DeviceName", Order = 3)]
        public string DeviceName { set; get; }

        [DataMember(Name = "Status", Order = 4)]
        public string Status { set; get; }

    }
}