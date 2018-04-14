using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XCCloudService.Model.WeiXin.SAppMessage
{
    public class MemberFoodSaleDataModel
    {
        public MemberFoodSaleDataModel()
        { 
            
        }

        public string BuyType { set; get; }

        public string StoreName { set; get; }

        public string OrderId { set; get; }

        public int Balance { set; get; }

        public int ICCardId { set; get; }

        public string BuyDate { set; get; }

        public decimal BuyAmmount { set; get; }

        public int BuyCoins { set; get; }

        public string FoodName { set; get; }

        public int FoodNum { set; get; }

        public string BuyMobile { set; get; }

        public string Remark { set; get; }
    }
}
