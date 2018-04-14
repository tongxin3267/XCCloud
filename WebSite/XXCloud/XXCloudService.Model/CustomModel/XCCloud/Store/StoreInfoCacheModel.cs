using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace XCCloudService.Model.CustomModel.XCCloud.Store
{
    public class StoreInfoCacheModel
    {
        public string MerchID { set; get; }

        public string ParentID { set; get; }

        public string StoreID { set; get; }

        public string StoreName { set; get; }

        public string Password { set; get; }
    }
}
