using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace XCCloudService.Model.Socket.UDP
{
    [DataContract]
    public class LotteryQueryRequestDataModel
    {
        public LotteryQueryRequestDataModel(string sn,string barCode)
        {
            this.SN = sn;
            this.BarCode = barCode;
            this.SignKey = "";
        }

        [DataMember(Name = "sn", Order = 1)]
        public string SN { set; get; }

        [DataMember(Name = "barcode", Order = 2)]
        public string BarCode { set; get; }

        [DataMember(Name = "signkey", Order = 3)]
        public string SignKey { set; get; }
    }
}
