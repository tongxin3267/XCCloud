//------------------------------------------------------------------------------
// <auto-generated>
//     此代码已从模板生成。
//
//     手动更改此文件可能导致应用程序出现意外的行为。
//     如果重新生成代码，将覆盖对此文件的手动更改。
// </auto-generated>
//------------------------------------------------------------------------------

namespace XCCloudService.Model.XCCloudRS232
{
    using System;
    using System.Collections.Generic;
    
    public partial class t_member
    {
        public int MemberID { get; set; }
        public Nullable<int> ICCardID { get; set; }
        public Nullable<int> MerchID { get; set; }
        public Nullable<long> Uid { get; set; }
        public Nullable<int> Type { get; set; }
        public string OpenID { get; set; }
        public string MemberName { get; set; }
        public string Birthday { get; set; }
        public string CertificalID { get; set; }
        public string Mobile { get; set; }
        public Nullable<int> Balance { get; set; }
        public Nullable<int> Lottery { get; set; }
        public Nullable<int> Point { get; set; }
        public Nullable<System.DateTime> JoinTime { get; set; }
        public Nullable<System.DateTime> EndDate { get; set; }
        public string MemberPassword { get; set; }
        public Nullable<int> RepeatCode { get; set; }
        public string Note { get; set; }
        public Nullable<int> Lock { get; set; }
        public Nullable<System.DateTime> LockDate { get; set; }
    }
}
