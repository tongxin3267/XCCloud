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
    
    public partial class Flw_Game_Watch
    {
        public int ID { get; set; }
        public Nullable<int> GameIndex { get; set; }
        public Nullable<int> HeadIndex { get; set; }
        public Nullable<System.DateTime> CreateTime { get; set; }
        public Nullable<int> UserID { get; set; }
        public Nullable<int> InCoin { get; set; }
        public Nullable<int> InCoinError { get; set; }
        public Nullable<int> PrizeCount { get; set; }
        public Nullable<int> PrizeError { get; set; }
        public Nullable<decimal> GoodPrice { get; set; }
        public Nullable<int> OutCoin { get; set; }
        public Nullable<int> OutCoinError { get; set; }
        public Nullable<int> OutLottery { get; set; }
        public Nullable<int> OutLotteryError { get; set; }
    }
}
