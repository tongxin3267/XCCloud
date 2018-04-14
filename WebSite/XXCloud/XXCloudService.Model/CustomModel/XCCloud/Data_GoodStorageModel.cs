using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XCCloudService.Model.CustomModel.XCCloud
{
    public class Data_GoodStorageList
    {
        public int ID { get; set; }
        public Nullable<DateTime> RealTime { get; set; }
        public Nullable<int> UserID { get; set; }
        public string RealName { get; set; }
        public Nullable<int> StorageCount { get; set; }
        public string Note { get; set; }
    }
}
