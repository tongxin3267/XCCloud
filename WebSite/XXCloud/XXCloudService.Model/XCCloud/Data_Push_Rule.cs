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
    
    public partial class Data_Push_Rule
    {
        public int ID { get; set; }
        public Nullable<int> GameIndexID { get; set; }
        public Nullable<int> MemberLevelID { get; set; }
        public string MemberLevelName { get; set; }
        public Nullable<int> Allow_Out { get; set; }
        public Nullable<int> Allow_In { get; set; }
        public Nullable<int> Week { get; set; }
        public Nullable<int> Coin { get; set; }
        public Nullable<int> Level { get; set; }
        public Nullable<System.TimeSpan> StartTime { get; set; }
        public Nullable<System.TimeSpan> EndTime { get; set; }
    }
}
