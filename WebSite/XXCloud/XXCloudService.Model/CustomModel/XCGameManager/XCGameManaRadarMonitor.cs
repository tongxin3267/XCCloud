using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace XCCloudService.Model.CustomModel.XCGameManager
{
    /// <summary>
    /// 注册雷达监控
    /// </summary>
    [DataContract]
    public class XCGameManaRadarMonitor
    {
        [DataMember(Name = "storeId", Order = 1)]
        public string StoreId { set; get; }

        [DataMember(Name = "storeName", Order = 2)]
        public string StoreName {set;get;}

        [DataMember(Name = "token", Order = 3)]
        public string Token { set; get; }

        [DataMember(Name = "segment", Order = 4)]
        public string Segment {set;get;}

        [DataMember(Name = "registerTime", Order = 5)]
        public string RegisterTime { set; get; }

        [DataMember(Name = "heatTime", Order = 6)]
        public string HeatTime { set; get; }

        [DataMember(Name = "address", Order = 7)]
        public string Address { set; get; }

        [DataMember(Name = "port", Order = 8)]
        public int Port { set; get; }

        [DataMember(Name = "stateName", Order = 9)]
        public string StateName { set; get; }
    }
}
