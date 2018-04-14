using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace XCCloudService.Model.CustomModel.XCCloud
{
    [DataContract]
    public class OpenCardFoodInfoModel
    {
        [DataMember(Name = "foodId", Order = 1)]
        public int FoodID { get; set; }

        [DataMember(Name = "foodName", Order = 2)]
        public string FoodName { get; set; }

        [DataMember(Name = "allowPrint", Order = 3)]
        public int AllowPrint { get; set; }

        [DataMember(Name = "rechargeType", Order = 4)]
        public int RechargeType { get; set; }

        [DataMember(Name = "rechargeTypeName", Order = 5)]
        public string RechargeTypeName { get; set; }

        [DataMember(Name = "imageUrl", Order = 6)]
        public string ImageUrl { set; get; }

        [DataMember(Name = "foodPrice", Order = 7)]
        public decimal FoodPrice { set; get; }

        [DataMember(Name = "containName", Order = 8)]
        public string ContainName { set; get; }
    }

    [DataContract]
    public class FoodInfoModel
    {
        [DataMember(Name = "foodId", Order = 1)]
        public int FoodID { get; set; }

        [DataMember(Name = "foodName", Order = 2)]
        public string FoodName { get; set; }

        [DataMember(Name = "foodType", Order = 3)]
        public int FoodType { get; set; }

        [DataMember(Name = "allowPrint", Order = 4)]
        public int AllowPrint { get; set; }

        [DataMember(Name = "rechargeType", Order = 5)]
        public int RechargeType { get; set; }

        [DataMember(Name = "foodPrice", Order = 6)]
        public decimal FoodPrice { get; set; }

        [DataMember(Name = "imageUrl", Order = 7)]
        public string ImageUrl { get; set; }

        [DataMember(Name = "detailsCount", Order = 8)]
        public int DetailsCount { get; set; }

        public int AllowCoin {get;set;}

        public int Coins { get; set; }
        
        public int AllowPoint {set;get;}
        
        public int Points {set;get;}
        
        public int AllowLottery {set;get;}

        public int Lottery { set; get; }

        [DataMember(Name = "containCount", Order = 9)]
        public int ContainCount { set; get; }

        [DataMember(Name = "containName", Order = 10)]
        public string ContainName { set; get; }

        [DataMember(Name = "payList", Order = 11)]
        public List<FoodInfoPriceModel> priceListModel { set; get; }
    }

    [DataContract]
    public class FoodInfoNumModel
    {
        public FoodInfoNumModel(int payModel, int payNum)
        {
            this.PayModel = PayModel;
            this.PayNum = PayNum;
        }

        public int PayModel { set; get; }

        public int PayNum { set; get; }
    }

    [DataContract]
    public class FoodInfoPriceModel
    {
        public FoodInfoPriceModel(int payModel,decimal payPrice)
        {
            this.PayModel = payModel;
            this.PayPrice = payPrice;
        }

        [DataMember(Name = "payModel", Order = 1)]
        public int PayModel { set; get; }

        [DataMember(Name = "payNum", Order = 2)]
        public decimal PayPrice { set; get; }
    }

    [DataContract]
    public class FoodDetailModel
    {
        [DataMember(Name = "detailId", Order = 1)]
        public int DetailId { get; set; }

        [DataMember(Name = "detailFoodType", Order = 2)]
        public int DetailFoodType { get; set; }

        [DataMember(Name = "detailFoodTypeName", Order = 3)]
        public string DetailFoodTypeName { get; set; }

        [DataMember(Name = "containCount", Order = 4)]
        public int ContainCount { get; set; }

        [DataMember(Name = "containName", Order = 5)]
        public string ContainName { get; set; }
    }
}
