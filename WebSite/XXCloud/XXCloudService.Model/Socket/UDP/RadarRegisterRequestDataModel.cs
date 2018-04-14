using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace XCCloudService.Model.Socket.UDP
{    
    [DataContract]
    public class RadarRegisterRequestDataModel
    {
        public RadarRegisterRequestDataModel(string storeId, string segment)
        {
            this.StoreId = storeId;
            this.Segment = segment;
            this.SignKey = "";
        }

        /// <summary>
        /// 门店编号
        /// </summary>
        [DataMember(Name = "storeid", Order = 1)]
        public string StoreId { set; get; }

        /// <summary>
        /// 门店内雷达段号
        /// </summary>
        [DataMember(Name = "segment", Order = 2)]
        public string Segment { set; get; }

        /// <summary>
        /// 签名
        /// </summary>
        [DataMember(Name = "signkey", Order = 3)]
        public string SignKey { set; get; }
    }
}
