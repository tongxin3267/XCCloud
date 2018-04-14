using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XCCloudService.Model.CustomModel.XCGame
{
    public class XCGameRadarDeviceTokenModel
    {
        public XCGameRadarDeviceTokenModel(string storeId, string segment)
        {
            this.StoreId = storeId;
            this.Segment = segment;
        }

        public string StoreId { set; get; }

        public string Segment { set; get; }
    }
}
