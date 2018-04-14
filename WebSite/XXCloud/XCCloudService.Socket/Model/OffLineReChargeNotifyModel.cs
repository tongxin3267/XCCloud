using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace XCCloudService.SocketService.TCP.Model
{
    /// <summary>
    /// 消息推送发送方对象模式
    /// </summary>
    [DataContract]
    public class OffLineReChargeNotifySendModel
    {
        public OffLineReChargeNotifySendModel(string storeId, string mobile)
        {
            this.StoreId = storeId;
            this.mobile = mobile;
        }
        /// <summary>
        /// 门店ID
        /// </summary>
        [DataMember(Name = "storeId", Order = 1)]
        public string StoreId { set; get; }

        /// <summary>
        /// 用户ID
        /// </summary>
        [DataMember(Name = "mobile", Order = 2)]
        public string mobile { set; get; }
    }


    /// <summary>
    /// 消息接收方对象模式
    /// </summary>
    [DataContract]
    public class OffLineReChargeNotifyReceiveModel
    {
        public OffLineReChargeNotifyReceiveModel(string storeId, string segment)
        {
            this.StoreId = storeId;
            this.Segment = segment;
        }

        /// <summary>
        /// 门店ID
        /// </summary>
        [DataMember(Name = "storeId", Order = 1)]
        public string StoreId { set; get; }

        /// <summary>
        /// 用户ID
        /// </summary>
        [DataMember(Name = "segment", Order = 2)]
        public string Segment { set; get; }
    }
}
