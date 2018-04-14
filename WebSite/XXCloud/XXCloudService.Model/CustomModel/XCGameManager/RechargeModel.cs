using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XCCloudService.Model.CustomModel.XCGameManager
{
    public class RechargeModel
    {
        public string SysId { get; set; }

        public string StoreId { get; set; }
        //充值类型
        public int RechargeType { get; set; }
        /// <summary>
        /// 卡号16字节，ASCII编码
        /// </summary>
        public string ICCardID { get; set; }
        /// <summary>
        /// 金额分，4字节
        /// </summary>
        public UInt32 Amount { get; set; }

        public UInt32 CoinAmount { get; set; }
        /// <summary>
        /// 订单编号32字节，ASCII编码
        /// </summary>
        public string OrderID { get; set; }

        public int FoodId { get; set; }

        /// <summary>
        /// 时间戳 uint64，8字节
        /// </summary>
        public UInt64 OrderTime { get; set; }
    }
}
