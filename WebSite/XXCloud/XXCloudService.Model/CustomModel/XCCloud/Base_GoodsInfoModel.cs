using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace XCCloudService.Model.CustomModel.XCCloud
{
    public class Base_GoodsInfoList
    {
        public int ID { get; set; }
        public string GoodTypeStr { get; set; }
        public string GoodName { get; set; }
        public Nullable<decimal> Price { get; set; }
        public Nullable<int> Points { get; set; }
        public string Note { get; set; }        
    }
   
}
