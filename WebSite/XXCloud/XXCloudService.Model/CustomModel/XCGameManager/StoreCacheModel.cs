using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace XCCloudService.Model.CustomModel.XCGameManager
{
    public class StoreCacheModel
    {
        public int StoreID { set; get; }

        public string StorePassword { set; get; }

        public string StoreDBName { set; get; }

        public string StoreName { set; get; }

        public int StoreType { set; get; }

        public int StoreDBDeployType { set; get; }
    }

    public class StoreDogCacheModel
    {
        public string StoreID { set; get; }

        public string DogId { set; get; }
    }
}
