using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace XCCloudService.Model.Socket.UDP
{
    [DataContract]
    public class RadarNotifyRequestModel
    {
        /// <summary>
        /// 雷达令牌
        /// </summary>
        [DataMember(Name = "token", Order = 1)]
        public string Token {set;get;}

        /// <summary>
        /// 控制类别1 出币
        /// </summary>
        [DataMember(Name = "action", Order = 2)]
        public string Action {set;get;}

        /// <summary>
        /// 控制处理结果 
        /// </summary>
        [DataMember(Name = "result", Order = 3)]
        public string Result {set;get;}

        
        /// <summary>
        /// 订单号 
        /// </summary>
        [DataMember(Name = "orderid", Order = 4)]
        public string OrderId {set;get;}

        /// <summary>
        /// 雷达通知业务流水号
        /// </summary>
        [DataMember(Name = "sn", Order = 5)]
        public string SN {set;get;}


        /// <summary>
        /// 雷达通知业务流水号
        /// </summary>
        [DataMember(Name = "coins", Order = 6)]
        public string Coins { set; get; }

        /// <summary>
        /// 签名
        /// </summary>
        [DataMember(Name = "signkey", Order = 7)]
        public string SignKey {set;get;}

        public string StoreId { set; get; }
    }
}
