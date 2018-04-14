using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace XCCloudService.Model.Socket.UDP
{    
    [DataContract]
    public class RadarHeartbeatRequestDataModel
    {
        public RadarHeartbeatRequestDataModel(string token,string storeId)
        {
            this.Token = token;
            this.StoreId = storeId;
        }

        /// <summary>
        /// 雷达token
        /// </summary>
        [DataMember(Name = "token", Order = 1)]
        public string Token { set; get; }

        public string StoreId { set; get; }

        public string Segment { set; get; }
    }
}
