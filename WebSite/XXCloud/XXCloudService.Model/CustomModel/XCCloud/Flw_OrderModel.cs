using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using XCCloudService.Common.Enum;

namespace XCCloudService.Model.CustomModel.XCCloud
{
    [DataContract]
    public class OrderViewModel
    {
        [DataMember]
        public string sysId { get; set; }
        [DataMember]
        public string versionNo { get; set; }
        [DataMember]
        public string orderId { get; set; }
        [DataMember]
        public List<Flw_OrderModel> buyDetails { get; set; }
    }

    [DataContract]
    public class Flw_OrderModel
    {
        [DataMember]
        public string orderId { set; get; }

        [DataMember]
        public string foodCount { set; get; }

        [DataMember]
        public string goodCount { set; get; }

        [DataMember]
        public string iccardID { set; get; }

        [DataMember]
        public string payType { set; get; }

        [DataMember]
        public string payCount { set; get; }

        [DataMember]
        public string realPay { set; get; }


        [DataMember]
        public string freePay { set; get; }


        [DataMember]
        public string userID { set; get; }
    }

    public class Flw_OrdersModel
    {
        public int ID { get; set; }
        public string StoreID { get; set; }
        public string StoreName { get; set; }
        public string OrderID { get; set; }
        public string FoodName { get; set; }
        public Nullable<int> FoodCount { get; set; }
        public Nullable<int> OrderSource { get; set; }
        public string OrderSourceStr { get; set; }
        public Nullable<System.DateTime> CreateTime { get; set; }
        public Nullable<int> PayType { get; set; }
        public string PayTypeStr { get; set; }
        public Nullable<int> OrderStatus { get; set; }
        public string OrderStatusStr { get; set; }
    }

    public class Data_FoodInfoModel
    {
        public int FoodID { get; set; }
        public string FoodName { get; set; }
        public string StoreID { get; set; }
        public string StoreName { get; set; }
        public string Note { get; set; }
        public string ImageURL { get; set; }
        public Nullable<int> FoodType { get; set; }
        public string FoodTypeStr { get; set; }
        public Nullable<int> RechargeType { get; set; }
        public string RechargeTypeStr { get; set; }
        public Nullable<int> AllowInternet { get; set; }
        public Nullable<int> AllowPrint { get; set; }
        public Nullable<int> FoodState { get; set; }
        public string FoodStateStr { get; set; }
        public Nullable<int> ForeAuthorize { get; set; }
        public Nullable<System.DateTime> StartTime { get; set; }
        public Nullable<System.DateTime> EndTime { get; set; }
        public Nullable<decimal> ClientPrice { get; set; }
        public Nullable<decimal> MemberPrice { get; set; }
        public Nullable<int> AllowCoin { get; set; }
        public Nullable<int> Coins { get; set; }
        public Nullable<int> AllowPoint { get; set; }
        public Nullable<int> Points { get; set; }
        public Nullable<int> AllowLottery { get; set; }
        public Nullable<int> Lottery { get; set; }
    }

    public class Flw_OrdersChartModel
    {
        public DateTime CreateTime { get; set; }
        public decimal RealPay { get; set; }
        public int RealCount { get; set; }
    }

    public class Store_CheckDateChartModel
    {
        public DateTime CheckDate { get; set; }
        public decimal AliPay { get; set; }
        public decimal Wechat { get; set; }
    }

    public class Flw_OrderCheckModel
    {
        //Flw_Schedule
        public int ID { get; set; } //Flw_CheckDate.ID 营业日期流水号
        public DateTime CheckDate { get; set; }
        public string ScheduleState { get; set; } //0:进行中 1:未审核 2:已审核
        public string StoreName { get; set; }

        //Flw_Order
        public decimal PayCount { get; set; } //应收现金
        public decimal AliRealPay { get; set; } //支付宝进账
        public decimal WechatRealPay { get; set; } //微信进账
        public decimal GroupBuyRealPay { get; set; } //团购金额 Flw_Order.OrderSource=2,3,4

        //Flw_Food_Sale
        public int OpenCount { get; set; } //开卡数量 FlowType=1
        public decimal OpenDeposit { get; set; } //开卡押金 FlowType=1
        public decimal OpenFee { get; set; } //开卡手续费 FlowType=1
        public int RefundCount { get; set; } //退卡数量 FlowType=7
        public decimal RefundDeposit { get; set; } //退卡押金 FlowType=7
        public decimal TokenRealPay { get; set; } //(代币)数字币 FlowType=2
        public decimal RechargeRealPay { get; set; } //充值金额 FlowType=0, BuyFoodType=1
        public decimal CoinRealPay { get; set; } //实物币金额 FlowType=0, BuyFoodType=2,3
        public decimal GoodRealPay { get; set; } //商品销售金额 FlowType=3
        public decimal TicketRealPay { get; set; } //门票销售金额 FlowType=4
        public int GroupBuyCount { get; set; } //团购币数 Flw_Order.OrderSource=2,3,4 Flw_Food_SaleDetail.SaleType=0        

        //Flw_Coin_Exit
        public decimal CoinMoney { get; set; } //过户币金额 FlowType=0

        //Flw_Ticket_Exit
        public int TicketCoins { get; set; } //条码兑币 State=1
        public decimal TicketCoinMoney { get; set; } //条码兑款 State=1

        //Flw_Coin_Sale
        public int FreeCoin { get; set; } //会员送币 WorkType=2,5,8
        public int SaveCoin { get; set; } //手工存币 WorkType=4

    }

    [DataContract]
    public class OrderPayCacheModel
    {
        public string OrderId { get; set; }

        public decimal PayAmount { get; set; }

        public string PayTime { get; set; }

        public OrderState PayState { get; set; }

        public SelttleType PayType { get; set; }
    }
}
