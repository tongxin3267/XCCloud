using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace XCCloudService.Model.CustomModel.XCCloud
{
    [DataContract]
    public class Data_MemberLevelModel
    {
        [DataMember(Name = "memberLevelId", Order = 1)]
        public int MemberLevelID { set; get; }

        [DataMember(Name = "memberLevelName", Order = 2)]
        public string MemberLevelName { set; get; }

        [DataMember(Name = "foodId", Order = 3)]
        public int FoodId { set; get; }

        [DataMember(Name = "foodName", Order = 4)]
        public string FoodName { set; get; }

        [DataMember(Name = "foodPrice", Order = 5)]
        public decimal FoodPrice { set; get; }

        [DataMember(Name = "cardUIURL", Order = 6)]
        public string CardUIURL { set; get; }
    }


}
