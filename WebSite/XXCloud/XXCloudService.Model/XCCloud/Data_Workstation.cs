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
    
    public partial class Data_Workstation
    {
        public int ID { get; set; }
        public string StoreID { get; set; }
        public string WorkStation { get; set; }
        public string MacAddress { get; set; }
        public string DiskID { get; set; }
        public Nullable<int> State { get; set; }
        public Nullable<int> UserID { get; set; }
        public Nullable<int> UserOnlineState { get; set; }
        public Nullable<int> ScheduleSender { get; set; }
        public string DogID { get; set; }
    }
}