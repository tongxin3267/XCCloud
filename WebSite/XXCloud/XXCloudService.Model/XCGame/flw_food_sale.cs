//------------------------------------------------------------------------------
// <auto-generated>
//     此代码已从模板生成。
//
//     手动更改此文件可能导致应用程序出现意外的行为。
//     如果重新生成代码，将覆盖对此文件的手动更改。
// </auto-generated>
//------------------------------------------------------------------------------

namespace XCCloudService.Model.XCGame
{
    using System;
    using System.Collections.Generic;
    
    public partial class flw_food_sale
    {
        public int ID { get; set; }
        public string FlowType { get; set; }
        public Nullable<int> ICCardID { get; set; }
        public Nullable<int> FoodID { get; set; }
        public Nullable<int> CoinQuantity { get; set; }
        public Nullable<int> Point { get; set; }
        public Nullable<int> Balance { get; set; }
        public Nullable<int> MemberLevelID { get; set; }
        public Nullable<decimal> Deposit { get; set; }
        public Nullable<decimal> OpenFee { get; set; }
        public Nullable<decimal> RenewFee { get; set; }
        public Nullable<decimal> ChangeFee { get; set; }
        public Nullable<decimal> CreditFee { get; set; }
        public Nullable<decimal> TotalMoney { get; set; }
        public string Note { get; set; }
        public string PayType { get; set; }
        public string BuyFoodType { get; set; }
        public Nullable<int> UserID { get; set; }
        public Nullable<int> ScheduleID { get; set; }
        public Nullable<int> AuthorID { get; set; }
        public Nullable<System.DateTime> RealTime { get; set; }
        public string WorkStation { get; set; }
        public string MacAddress { get; set; }
        public string DiskID { get; set; }
        public Nullable<System.DateTime> ExitRealTime { get; set; }
        public Nullable<int> ExitBalance { get; set; }
        public Nullable<int> ExitUserID { get; set; }
        public Nullable<int> ExitScheduleID { get; set; }
        public string ExitWorkStation { get; set; }
        public string OrderID { get; set; }
    }
}
