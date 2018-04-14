using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace XCCloudService.Model.CustomModel.XCCloud.Order
{
    [DataContract]
    public class OrderMainModel
    {
        [DataMember(Name = "storeId", Order = 1)]
        public string StoreId { set; get; }

        [DataMember(Name = "storeName", Order = 2)]
        public string StoreName { set; get; }

        [DataMember(Name = "icCardId", Order = 3)]
        public string ICCardId { set; get; }

        [DataMember(Name = "payCount", Order = 4)]
        public decimal PayCount { set; get; }

        [DataMember(Name = "realPay", Order = 5)]
        public decimal RealPay { set; get; }

        [DataMember(Name = "freePay", Order = 6)]
        public decimal FreePay { set; get; }

        [DataMember(Name = "foodCount", Order = 7)]
        public int FoodCount { set; get; }

        [DataMember(Name = "detailGoodsCount", Order = 8)]
        public int DetailGoodsCount { set; get; }

        [DataMember(Name = "customerType", Order = 8)]
        public int CustomerType { set; get; }

        [DataMember(Name = "memberLevelId", Order = 10)]
        public int MemberLevelId { set; get; }

        [DataMember(Name = "memberLevelName", Order = 11)]
        public string MemberLevelName { set; get; }

        [DataMember(Name = "createTime", Order = 12)]
        public DateTime CreateTime { set; get; }
    }
}
