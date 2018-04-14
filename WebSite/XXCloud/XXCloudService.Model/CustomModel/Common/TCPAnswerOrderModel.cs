using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XCCloudService.Model.CustomModel.Common
{

    public class TCPAnswerOrderModel
    {
        public TCPAnswerOrderModel(string orderNo, string mobile, int coins,string action,string icCardId,string storeId)
        {
            this.OrderNo = orderNo;
            this.Mobile = mobile;
            this.Coins = coins;
            this.Action = action;
            this.ICCardId = icCardId;
            this.StoreId = storeId;
            this.CreateTime = System.DateTime.Now;
        }

        /// <summary>
        /// 订单号
        /// </summary>
        public string OrderNo { set; get; }

        /// <summary>
        /// 手机号
        /// </summary>
        public string Mobile { set; get; }

        /// <summary>
        /// 够币数
        /// </summary>
        public int Coins { set; get; }


        public string Action { set; get; }

        public string ICCardId { set; get; }

        public string StoreId { set; get; }

        public DateTime CreateTime { set; get; }
    }
}
