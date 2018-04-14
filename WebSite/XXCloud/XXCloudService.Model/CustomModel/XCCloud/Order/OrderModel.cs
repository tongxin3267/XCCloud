using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace XCCloudService.Model.CustomModel.XCCloud.Order
{
    public class OrderModel
    {

    }

    [DataContract]
    public class OrderInfoModel
    {
        public OrderInfoModel(OrderMainModel orderMainModel, List<OrderDetailModel> orderDetailModel)
        {
            this.orderMainModel = orderMainModel;
            this.orderDetailModel = orderDetailModel;
        }

        [DataMember(Name = "main", Order = 1)]
        public OrderMainModel orderMainModel { set; get; }

        [DataMember(Name = "detail", Order = 2)]
        public List<OrderDetailModel> orderDetailModel { set; get; } 
    }

    [DataContract]
    public class OrderBuyDetailModel
    {
        [DataMember(Name = "foodId", Order = 1)]
        public int FoodId {set;get;}

        [DataMember(Name = "foodCount", Order = 2)]
        public int FoodCount {set;get;}

        [DataMember(Name = "payType", Order = 3)]
        public int PayType {set;get;}

        [DataMember(Name = "payNum", Order = 4)]
        public decimal PayNum { set; get; }
    }
}
