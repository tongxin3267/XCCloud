using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace XCCloudService.Model.CustomModel.XCGame
{
    [DataContract]
    public class MenberICListCardModel
    {
        [DataMember(Name = "Lists", Order = 1)]
        public List<MenberICCardModel> Lists { set; get; }
        [DataMember(Name = "Page", Order = 2)]
        public string Page { set; get; }
    }
    [DataContract]
    public class MenberICCardModel
    {
        [DataMember(Name = "NO", Order = 1)]
        public Int64 NO { set; get; }

        [DataMember(Name = "ICCardID", Order = 2)]
        public string ICCardID { set; get; }

        [DataMember(Name = "RealTimes", Order = 3)]
        public string RealTimes { set; get; }

        [DataMember(Name = "last_coins", Order = 4)]
        public decimal last_coins { set; get; }

        [DataMember(Name = "save_coins", Order = 5)]
        public int save_coins { set; get; }

        [DataMember(Name = "used_coins", Order = 6)]
        public int used_coins { set; get; }

        [DataMember(Name = "now_coins", Order = 7)]
        public int now_coins { set; get; }

        [DataMember(Name = "type", Order = 8)]
        public string type { set; get; }

        [DataMember(Name = "other_info", Order = 9)]
        public string other_info { set; get; }
    }
}
