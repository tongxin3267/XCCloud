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
    
    public partial class Flw_Schedule
    {
        public int ID { get; set; }
        public string StoreID { get; set; }
        public Nullable<int> CheckDateID { get; set; }
        public Nullable<int> UserID { get; set; }
        public Nullable<System.DateTime> OpenTime { get; set; }
        public Nullable<System.DateTime> ShiftTime { get; set; }
        public Nullable<System.DateTime> CheckDate { get; set; }
        public Nullable<int> State { get; set; }
        public Nullable<decimal> RealCash { get; set; }
        public Nullable<decimal> RealCredit { get; set; }
        public Nullable<int> RealCoin { get; set; }
        public string WorkStation { get; set; }
        public string Note { get; set; }
    }
}