using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XCCloudService.Model.WeiXin.Message
{
    public class OrderPaySuccessDataModel 
    {
        public OrderPaySuccessDataModel ()
        { 
            
        }

        public OrderPaySuccessDataModel(string productName, string buyDate, string BuyPrice,string Createtime,string OrderNumber)
        {
            this.ProductName = productName;
            this.BuyDate = buyDate;
            this.BuyPrice = BuyPrice;
            this.Createtime = Createtime;
            this.OrderNumber = OrderNumber;
        }

        public string ProductName { set; get; }

        public string BuyDate { set; get; }

        public string BuyPrice { set; get; }
        public string Createtime { set; get; }
        public string OrderNumber { set; get; }
    }
}
