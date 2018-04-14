using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace XCCloudService.Model.Socket.UDP
{
    [DataContract]
    public class StoreQueryRequestDataModel
    {
        public StoreQueryRequestDataModel(string storeId,string date, string sn, string searchType,string icCardId)
        {
            this.StoreId = storeId;
            this.Date = date;
            this.SN = sn;
            this.SearchType = searchType;
            this.ICCardId = icCardId;
            this.SignKey = "";
        }


        [DataMember(Name = "date", Order = 1)]
        public string Date { set; get; }

        [DataMember(Name = "sn", Order = 2)]
        public string SN { set; get; }

        [DataMember(Name = "searchtype", Order = 3)]
        public string SearchType { set; get; }

        [DataMember(Name = "iccardid", Order = 4)]
        public string ICCardId { set; get; }

        [DataMember(Name = "signkey", Order = 5)]
        public string SignKey { set; get; }

        public string StoreId { set; get; }
    }
}
