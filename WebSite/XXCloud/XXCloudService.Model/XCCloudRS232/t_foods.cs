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
    
    public partial class t_foods
    {
        public int FoodID { get; set; }
        public Nullable<int> MerchID { get; set; }
        public Nullable<int> DeviceID { get; set; }
        public string FoodName { get; set; }
        public Nullable<decimal> FoodPrice { get; set; }
        public Nullable<int> CoinQuantity { get; set; }
        public Nullable<int> IsQuickFood { get; set; }
        public Nullable<int> FoodState { get; set; }
    }
}
