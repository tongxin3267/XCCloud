using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XCCloudService.Model.WeiXin.SAppMessage
{
    public class MemberCoinsOperationNotifyDataModel
    {
        public MemberCoinsOperationNotifyDataModel(string coinsType,string storeName,string mobile,int icCardId,int coins,int lastBalance)
        {
            this.CoinsType = coinsType;
            this.StoreName = storeName;
            this.Mobile = mobile;
            this.ICCardId = icCardId;
            this.Coins = coins;
            this.LastBalance = lastBalance;
        }

        public string CoinsType {set;get;} 
        
        public string StoreName {set;get;} 
        
        public string Mobile {set;get;} 
        
        public int ICCardId {set;get;} 
        
        public int Coins {set;get;}

        public int LastBalance { set; get; }
    }

    public class MemberFoodSaleNotifyDataModel
    {
        public MemberFoodSaleNotifyDataModel(string coinsType, string storeName, string mobile, string foodName, int foodNum, int icCardId, decimal money, int coins)
        {
            this.CoinsType = coinsType;
            this.StoreName = storeName;
            this.Mobile = mobile;
            this.FoodName = foodName;
            this.FoodNum = foodNum;
            this.ICCardId = icCardId;
            this.Money = money;
            this.Coins = coins;
        }

        public string CoinsType { set; get; } 

        public string StoreName {set;get;}

        public string Mobile {set;get;}

        public string FoodName {set;get;}

        public int FoodNum {set;get;}

        public int ICCardId {set;get;}

        public decimal Money {set;get;}

        public int Coins {set;get;}
    }

}
