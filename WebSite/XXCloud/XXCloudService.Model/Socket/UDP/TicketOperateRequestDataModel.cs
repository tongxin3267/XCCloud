using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace XCCloudService.Model.Socket.UDP
{
    [DataContract]
    public class TicketOperateRequestDataModel
    {
        public TicketOperateRequestDataModel(string sn,string barCode,string operate)
        {
            this.SN = sn;
            this.BarCode = barCode;
            this.Operate = operate;
            this.SignKey = "";
        }


        [DataMember(Name = "barcode", Order = 1)]
        public string BarCode { set; get; }

        //0 使用 1 解锁
        [DataMember(Name = "operate", Order = 2)]
        public string Operate { set; get; }

        [DataMember(Name = "sn", Order = 3)]
        public string SN { set; get; }

        [DataMember(Name = "signkey", Order = 4)]
        public string SignKey { set; get; }
    }
}
