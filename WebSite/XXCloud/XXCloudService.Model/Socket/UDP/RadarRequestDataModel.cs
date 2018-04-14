using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace XCCloudService.Model.Socket.UDP
{    
    [DataContract]
    public class RadarRequestDataModel
    {
        public RadarRequestDataModel(string token, string mcuid, string action, int count, string sn, string orderId,string icCardId,string signkey,string storeId)
        {
            this.Token = token;
            this.MCUId = mcuid;
            this.Action = action;
            this.Count = count;
            this.SN = sn;
            this.OrderId = orderId;
            this.ICCardId = icCardId;
            this.SignKey = signkey;
            this.StoreId = storeId;
        }

        /// <summary>
        /// 门店编号
        /// </summary>
        [DataMember(Name = "token", Order = 1)]
        public string Token { set; get; }


        /// <summary>
        /// 门店内雷达段号
        /// </summary>
        [DataMember(Name = "mcuid", Order = 2)]
        public string MCUId { set; get; }

        /// <summary>
        /// 控制类别1 出币
        /// </summary>
        [DataMember(Name = "action", Order = 3)]
        public string Action { set; get; }

        /// <summary>
        /// 控制计数
        /// </summary>
        [DataMember(Name = "count", Order = 4)]
        public int Count { set; get; }


        /// <summary>
        /// 业务流水号
        /// </summary>
        [DataMember(Name = "sn", Order = 5)]
        public string SN { set; get; }


        /// <summary>
        /// 订单号
        /// </summary>
        [DataMember(Name = "orderid", Order = 6)]
        public string OrderId { set; get; }

        /// <summary>
        /// 订单号
        /// </summary>
        [DataMember(Name = "iccardid", Order = 7)]
        public string ICCardId { set; get; }

        /// <summary>
        /// 订单号
        /// </summary>
        [DataMember(Name = "signkey", Order = 10)]
        public string SignKey { set; get; }

        public string StoreId { set; get; }
    }
}
