using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XCCloudService.Model.WeiXin.Message
{
    public class OrderFailSuccessDataModel
    {

        public OrderFailSuccessDataModel()
        {

        }

        public OrderFailSuccessDataModel(string productName, string buyDate, string BuyPrice)
        {
            this.ProductName = productName;
            this.BuyDate = buyDate;
            this.BuyPrice = BuyPrice;
  
        }

        public string ProductName { set; get; }

        public string BuyDate { set; get; }

        public string BuyPrice { set; get; }

    }
}
