using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace XCCloudService.Model.CustomModel.XCGameManager
{
    [DataContract] 
    public class TerminalStoreReponseModel
    {
        public TerminalStoreReponseModel(string storeId,string storeName,string deviceName,string deviceType,string state,string stateName)
        {
            this.StoreId = storeId;
            this.StoreName = storeName;
            this.DeviceName = deviceName;
            this.DeviceType = deviceType;
            this.State = state;
            this.StateName = stateName;
        }

        [DataMember(Name = "storeId", Order = 1)]
        public string StoreId { set; get; }

        [DataMember(Name = "storeName", Order = 2)]
        public string StoreName { set; get; }

        [DataMember(Name = "deviceName", Order = 3)]
        public string DeviceName { set; get; }

        [DataMember(Name = "deviceType", Order = 4)]
        public string DeviceType { set; get; }

        [DataMember(Name = "state", Order = 5)]
        public string State { set; get; }

        [DataMember(Name = "stateName", Order = 6)]
        public string StateName { set; get; }
    }
}
