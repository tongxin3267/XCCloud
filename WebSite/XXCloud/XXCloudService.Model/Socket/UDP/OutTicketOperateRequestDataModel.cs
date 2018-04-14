using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace XCCloudService.Model.Socket.UDP
{
    [DataContract]
    public class OutTicketOperateRequestDataModel
    {
        public OutTicketOperateRequestDataModel(string sn,string barCode, string icCardId, string mobileName, string phone, decimal money, string operate)
        {
            this.SN = sn;
            this.BarCode = barCode;
            this.ICCardId = icCardId;
            this.MobileName = mobileName;
            this.Phone = phone;
            this.Money = money;
            this.Operate = operate;
            this.SignKey = "";
        }
        [DataMember(Name = "sn", Order = 1)]
        public string SN { set; get; }

        [DataMember(Name = "barcode", Order = 2)]
        public string BarCode { set; get; }

        [DataMember(Name = "iccardid", Order = 3)]
        public string ICCardId { set; get; }

        [DataMember(Name = "mobilename", Order = 4)]
        public string MobileName { set; get; }

        [DataMember(Name = "phone", Order = 5)]
        public string Phone { set; get; }

        [DataMember(Name = "money", Order = 6)]
        public decimal Money { set; get; }

        [DataMember(Name = "operate", Order = 7)]
        public string Operate { set; get; }

        [DataMember(Name = "signkey", Order = 8)]
        public string SignKey { set; get; }
    }
}
