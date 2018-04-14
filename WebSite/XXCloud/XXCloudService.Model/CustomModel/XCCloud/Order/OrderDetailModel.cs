using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace XCCloudService.Model.CustomModel.XCCloud.Order
{
    [DataContract]
    public class OrderDetailModel
    {
        [DataMember(Name = "foodId", Order = 1)]
        public int FoodId {set;get;}

        [DataMember(Name = "foodName", Order = 2)]
        public string FoodName {set;get;}

        [DataMember(Name = "foodType", Order = 3)]
        public int FoodType { set; get; }

        [DataMember(Name = "allowPrint", Order = 4)]
        public int AllowPrint { set; get; }

        [DataMember(Name = "rechargeType", Order = 5)]
        public int RechargeType { set; get; }

        [DataMember(Name = "allowCoin", Order = 6)]
        public int AllowCoin { set; get; }

        [DataMember(Name = "coins", Order = 7)]
        public int Coins { set; get; }

        [DataMember(Name = "allowPoint", Order = 8)]
        public int AllowPoint { set; get; }

        [DataMember(Name = "points", Order = 9)]
        public int Points { set; get; }

        [DataMember(Name = "allowLottery", Order = 10)]
        public int AllowLottery { set; get; }

        [DataMember(Name = "lottery", Order = 11)]
        public int Lottery {set;get;}

        [DataMember(Name = "imageUrl", Order = 12)]
        public string ImageUrl {set;get;}

        [DataMember(Name = "foodPrice", Order = 13)]
        public decimal FoodPrice {set;get;}

        [DataMember(Name = "containCount", Order = 14)]
        public int ContainCount { set; get; }
        
        [DataMember(Name = "containName", Order = 15)]
        public string ContainName {set;get;}

        [DataMember(Name = "detailsCount", Order = 17)]
        public int DetailsCount { set; get; }

        [DataMember(Name = "goodsCount", Order = 18)]
        public int GoodsCount { set; get; }

        [DataMember(Name = "useCoin", Order = 19)]
        public int UseCoin { set; get; }

        [DataMember(Name = "usePoint", Order = 20)]
        public int UsePoint { set; get; }

        [DataMember(Name = "useLottery", Order = 21)]
        public int UseLottery {set;get;}

        [DataMember(Name = "deposit", Order = 22)]
        public decimal Deposit {set;get;}
    }
}
