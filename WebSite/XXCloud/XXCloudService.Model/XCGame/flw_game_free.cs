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
    
    public partial class flw_game_free
    {
        public int id { get; set; }
        public string GameID { get; set; }
        public string HeadID { get; set; }
        public string ICCardID { get; set; }
        public Nullable<int> FreeCoin { get; set; }
        public Nullable<System.DateTime> RealTime { get; set; }
        public Nullable<int> RuleID { get; set; }
        public Nullable<int> Balance { get; set; }
    }
}
