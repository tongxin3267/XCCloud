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
    
    public partial class Data_Message
    {
        public int ID { get; set; }
        public Nullable<int> Sender { get; set; }
        public Nullable<int> SenderType { get; set; }
        public Nullable<int> Receiver { get; set; }
        public Nullable<int> RecvType { get; set; }
        public Nullable<int> MsgType { get; set; }
        public Nullable<System.DateTime> SendTime { get; set; }
        public Nullable<int> ReadFlag { get; set; }
        public Nullable<System.DateTime> ReadTime { get; set; }
        public string MsgText { get; set; }
    }
}
