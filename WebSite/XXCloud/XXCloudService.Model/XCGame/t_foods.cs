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
    
    public partial class t_foods
    {
        public int FoodID { get; set; }
        public string FoodName { get; set; }
        public string RebateNote { get; set; }
        public string FoodType { get; set; }
        public Nullable<decimal> FoodPrice { get; set; }
        public Nullable<int> CoinQuantity { get; set; }
        public string SuitCoinType { get; set; }
        public Nullable<int> IsQuickFood { get; set; }
        public string FoodState { get; set; }
        public string BeginDate { get; set; }
        public string EndDate { get; set; }
        public string BackColor { get; set; }
        public string ForeColor { get; set; }
        public byte[] Icon { get; set; }
        public Nullable<int> ForeAuthorize { get; set; }
        public Nullable<int> day_sale_count { get; set; }
        public Nullable<int> member_day_sale_count { get; set; }
    }
}
