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
    
    public partial class Flw_Food_Exit
    {
        public int ExitID { get; set; }
        public string OrderID { get; set; }
        public string StoreID { get; set; }
        public Nullable<int> CardID { get; set; }
        public Nullable<int> Point { get; set; }
        public Nullable<int> CoinBalance { get; set; }
        public Nullable<int> PointBalance { get; set; }
        public Nullable<int> LotteryBalance { get; set; }
        public Nullable<decimal> TotalMoney { get; set; }
        public string Note { get; set; }
        public Nullable<int> UserID { get; set; }
        public Nullable<int> ScheduleID { get; set; }
        public Nullable<int> AuthorID { get; set; }
        public Nullable<System.DateTime> RealTime { get; set; }
        public string WorkStation { get; set; }
    }
}
