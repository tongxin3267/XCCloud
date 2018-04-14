using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace XCCloudService.Model.Socket.UDP
{
    public class StoreQueryResultNotifyRequestModel
    {
        public StoreQueryResultNotifyRequestModel(string token, string signKey, string sn, List<StoreQueryResultNotifyTableData> list)
        {
            this.Token = token;
            this.SignKey = signKey;
            this.SN = sn;
            this.TDList = list;
            this.StoreId = "";
        }

        public string Token { set; get; }

        public string SignKey { set; get; }

        public string SN { set; get; }

        public List<StoreQueryResultNotifyTableData> TDList { set; get; }

        public string StoreId { set; get; }
    }

    public class StoreQueryResultNotifyTableData
    {
        public StoreQueryResultNotifyTableData(string key,string value)
        {
            this.Key = key;
            this.Value = value;
        }

        public string Key { set; get; }

        public string Value { set; get; }
    }
}
