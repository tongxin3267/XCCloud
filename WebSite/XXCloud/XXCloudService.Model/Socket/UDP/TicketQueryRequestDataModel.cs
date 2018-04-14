using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace XCCloudService.Model.Socket.UDP
{
    [DataContract]
    public class TicketQueryRequestDataModel
    {
        public TicketQueryRequestDataModel(string sn,string barCode)
        {
            this.BarCode = barCode;
            this.SN = sn;
            this.SignKey = "";
        }


        [DataMember(Name = "barcode", Order = 1)]
        public string BarCode { set; get; }

        [DataMember(Name = "sn", Order = 2)]
        public string SN { set; get; }

        [DataMember(Name = "signkey", Order = 3)]
        public string SignKey { set; get; }
    }
}
