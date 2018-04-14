using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XCCloudService.Model.CustomModel.XCCloud
{
    public class Data_CouponInfoModel
    {
        public int ID { get; set; }
        public string CouponName { get; set; }
        public string CouponTypeStr { get; set; }
        public int PublishCount { get; set; }
        public int LeftCount { get; set; }        
        public string StartTime { get; set; }
        public string EndTime { get; set; }        
        public string Context { get; set; }
    }
}
