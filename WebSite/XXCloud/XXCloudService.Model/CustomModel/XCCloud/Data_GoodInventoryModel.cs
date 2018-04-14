using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XCCloudService.Model.CustomModel.XCCloud
{
    public class Data_GoodInventoryList
    {
        public int ID { get; set; }
        public Nullable<DateTime> InventoryTime { get; set; }
        public Nullable<int> UserID { get; set; }
        public string RealName { get; set; }
        public Nullable<int> InventoryCount { get; set; }
        public string Note { get; set; }
    }
}
