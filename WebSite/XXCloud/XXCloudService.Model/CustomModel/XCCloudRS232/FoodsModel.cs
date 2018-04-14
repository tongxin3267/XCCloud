using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace XCCloudService.Model.CustomModel.XCCloudRS232
{
    [DataContract]
    public class FoodsModellist {
        [DataMember(Name = "Lists", Order = 1)]
        public List<FoodsModel> Lists { set; get; }
    }

    public class FoodsModel
    {
        [DataMember(Name = "foodid", Order = 1)]
        public int foodid { set; get; }
        [DataMember(Name = "foodname", Order = 2)]
        public string foodname { set; get; }
        [DataMember(Name = "foodprice", Order = 3)]
        public decimal foodprice { set; get; }
        [DataMember(Name = "coinquantity", Order = 4)]
        public int coinquantity { set; get; }
        [DataMember(Name = "isquickfood", Order = 5)]
        public int isquickfood { set; get; }
        [DataMember(Name = "devicename", Order = 6)]
        public string devicename { set; get; }
    }
}
