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
    
    public partial class t_goods
    {
        public string Barcode { get; set; }
        public string GoodsName { get; set; }
        public decimal Price { get; set; }
        public decimal Cost { get; set; }
        public string Units { get; set; }
        public int Quantity { get; set; }
        public int AllowPointFee { get; set; }
        public int Point { get; set; }
        public int AllowCoinFee { get; set; }
        public int Coin { get; set; }
        public string Note { get; set; }
        public int UserID { get; set; }
        public string UpdateTime { get; set; }
        public string State { get; set; }
        public int AllowLottery { get; set; }
        public int Lottery { get; set; }
        public int WindowID { get; set; }
        public int AllowCashFee { get; set; }
        public byte[] Picture { get; set; }
    }
}