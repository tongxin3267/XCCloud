using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace XCCloudService.Model.CustomModel.XCGameManager
{
    [DataContract]
    public class PromotionModel
    {
        [DataMember(Name = "ID", Order = 1)]
        public int ID { set; get; }
        [DataMember(Name = "StoreName", Order = 1)]
        public string StoreName { set; get; }

        [DataMember(Name = "StoreID", Order = 2)]
        public string StoreID { set; get; }

        [DataMember(Name = "Time", Order = 3)]
        public string Time { set; get; }

        [DataMember(Name = "ReleaseTime", Order = 4)]
        public string ReleaseTime { set; get; }

        [DataMember(Name = "PicturePath", Order = 5)]
        public string PicturePath { set; get; }

        [DataMember(Name = "Title", Order = 6)]
        public string Title { set; get; }

        [DataMember(Name = "PagePath", Order = 7)]
        public string PagePath { set; get; }
        [DataMember(Name = "Publisher", Order = 8)]
        public string Publisher { set; get; }
        [DataMember(Name = "PublishType", Order = 9)]
        public int PublishType { set; get; }
        [DataMember(Name = "PromotionType", Order = 10)]
        public int PromotionType { set; get; }
    }
}
