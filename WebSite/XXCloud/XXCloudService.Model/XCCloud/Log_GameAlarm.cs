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
    
    public partial class Log_GameAlarm
    {
        public int ID { get; set; }
        public Nullable<int> ICCardID { get; set; }
        public string Segment { get; set; }
        public string HeadAddress { get; set; }
        public Nullable<int> AlertType { get; set; }
        public Nullable<System.DateTime> HappenTime { get; set; }
        public Nullable<System.DateTime> EndTime { get; set; }
        public Nullable<int> State { get; set; }
        public Nullable<int> LockGame { get; set; }
        public Nullable<int> LockMember { get; set; }
        public string AlertContent { get; set; }
    }
}
