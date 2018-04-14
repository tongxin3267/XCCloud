//------------------------------------------------------------------------------
// <auto-generated>
//     此代码已从模板生成。
//
//     手动更改此文件可能导致应用程序出现意外的行为。
//     如果重新生成代码，将覆盖对此文件的手动更改。
// </auto-generated>
//------------------------------------------------------------------------------

namespace XCCloudService.Model.XCCloud
{
    using System;
    using System.Collections.Generic;
    
    public partial class Data_CouponInfo
    {
        public int ID { get; set; }
        public string MerchID { get; set; }
        public string StoreID { get; set; }
        public string CouponName { get; set; }
        public Nullable<int> CouponType { get; set; }
        public Nullable<int> EntryCouponFlag { get; set; }
        public Nullable<int> AuthorFlag { get; set; }
        public Nullable<int> OverUseCount { get; set; }
        public Nullable<int> PublishCount { get; set; }
        public Nullable<decimal> CouponValue { get; set; }
        public Nullable<decimal> CouponDiscount { get; set; }
        public Nullable<decimal> CouponThreshold { get; set; }
        public Nullable<System.DateTime> StartTime { get; set; }
        public Nullable<System.DateTime> EndTime { get; set; }
        public Nullable<int> SendType { get; set; }
        public Nullable<decimal> OverMoney { get; set; }
        public Nullable<int> FreeCouponCount { get; set; }
        public Nullable<int> JackpotCount { get; set; }
        public Nullable<int> JackpotID { get; set; }
        public Nullable<int> ChargeType { get; set; }
        public Nullable<int> ChargeCount { get; set; }
        public Nullable<int> GoodID { get; set; }
        public Nullable<int> ProjectID { get; set; }
        public Nullable<int> OpUserID { get; set; }
        public string Context { get; set; }
    }
}
