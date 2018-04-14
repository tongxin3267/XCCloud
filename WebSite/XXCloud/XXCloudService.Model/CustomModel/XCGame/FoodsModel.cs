using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace XCCloudService.Model.CustomModel.XCGame
{
    [DataContract]
    public class FoodsResponseModel
    {
        [DataMember(Name = "foodID", Order = 1)]
        public int FoodID { set; get; }

        [DataMember(Name = "foodName", Order = 2)]
        public string FoodName { set; get; }

        [DataMember(Name = "isQuickFood", Order = 3)]
        public int IsQuickFood { set; get; }

        [DataMember(Name = "coinQuantity", Order = 4)]
        public int CoinQuantity { set; get; }

        [DataMember(Name = "foodPrice", Order = 5)]
        public decimal FoodPrice { set; get; }

        [DataMember(Name = "sendCoinQuantity", Order = 6)]
        public int SendCoinQuantity { set; get; }
    }
}
