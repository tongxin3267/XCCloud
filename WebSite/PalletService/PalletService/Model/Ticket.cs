using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace PalletService.Model
{
    [DataContract]
    public class FoodSaleTicketModel
    {
        /// <summary>
        /// 套餐名称
        /// </summary>
        [DataMember(Name = "foodName", Order = 1)]
        public string FoodName { set; get; }

        /// <summary>
        /// 序号
        /// </summary>
        [DataMember(Name = "no", Order = 1)]
        public string No { set; get; }

        /// <summary>
        /// 有效日期
        /// </summary>
        [DataMember(Name = "validityDate", Order = 1)]
        public string ValidityDate { set; get; }

        /// <summary>
        /// 消费币数
        /// </summary>
        [DataMember(Name = "coins", Order = 1)]
        public string Coins { set; get; }
    }
}
