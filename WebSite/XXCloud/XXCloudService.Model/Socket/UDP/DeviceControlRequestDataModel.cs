using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace XCCloudService.Model.Socket.UDP
{    
    [DataContract]
    public class DeviceControlRequestDataModel
    {
        public DeviceControlRequestDataModel(string storeId, string mobile,string icCardId,string segment, string mcuid, string action, int count, string sn, string orderId, string storePassword,int foodId,string gameRuleId)
        {
            this.StoreId = storeId;
            this.Mobile = mobile;
            this.Segment = segment;
            this.MCUId = mcuid;
            this.Action = action;
            this.Coins = count;
            this.SN = sn;
            this.OrderId = orderId;
            this.StorePassword = storePassword;
            this.FoodId = foodId;
            this.IcCardId = icCardId;
            this.GameRuleId = gameRuleId;
        }

        /// <summary>
        /// 门店编号
        /// </summary>
        [DataMember(Name = "storeId", Order = 1)]
        public string StoreId { set; get; }

        /// <summary>
        /// 手机号码
        /// </summary>
        [DataMember(Name = "mobile", Order = 1)]
        public string Mobile { set; get; }

        /// <summary>
        /// 会员卡号
        /// </summary>
        [DataMember(Name = "icCardId", Order = 1)]
        public string IcCardId { set; get; }

        /// <summary>
        /// 段地址
        /// </summary>
        [DataMember(Name = "segment", Order = 2)]
        public string Segment { set; get; }

        /// <summary>
        /// 门店内雷达段号
        /// </summary>
        [DataMember(Name = "mcuid", Order = 3)]
        public string MCUId { set; get; }

        /// <summary>
        /// 控制类别1 出币
        /// </summary>
        [DataMember(Name = "action", Order = 4)]
        public string Action { set; get; }

        /// <summary>
        /// 控制计数
        /// </summary>
        [DataMember(Name = "count", Order = 5)]
        public int Coins { set; get; }


        /// <summary>
        /// 业务流水号
        /// </summary>
        [DataMember(Name = "sn", Order = 6)]
        public string SN { set; get; }


        /// <summary>
        /// 订单号
        /// </summary>
        [DataMember(Name = "orderid", Order = 7)]
        public string OrderId { set; get; }

        public string StorePassword { set; get; }

        public int FoodId { set; get; }

        [DataMember(Name = "ruleId", Order = 8)]
        public string GameRuleId { set; get; }
    }
}
