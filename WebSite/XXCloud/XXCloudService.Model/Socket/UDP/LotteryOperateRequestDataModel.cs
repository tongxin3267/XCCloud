using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace XCCloudService.Model.Socket.UDP
{
    [DataContract]
    public class LotteryOperateRequestDataModel
    {
        public LotteryOperateRequestDataModel(string sn, string barCode, string icCardId, string Operate, string mobileName, string phone)
        {
            this.SN = sn;
            this.BarCode = barCode;
            this.ICCardId = icCardId;
            this.Operate = Operate;
            this.MobileName = mobileName;
            this.Phone = phone;
        }

        [DataMember(Name = "barcode", Order = 1)]
        public string BarCode { set; get; }

        [DataMember(Name = "iccardid", Order = 2)]
        public string ICCardId { set; get; }

        //0 使用 1 解锁
        [DataMember(Name = "operate", Order = 3)]
        public string Operate { set; get; }

        [DataMember(Name = "sn", Order = 4)]
        public string SN { set; get; }

        [DataMember(Name = "mobileName", Order = 5)]
        public string MobileName { set; get; }

        [DataMember(Name = "phone", Order = 6)]
        public string Phone { set; get; }

        [DataMember(Name = "signkey", Order = 7)]
        public string SignKey { set; get; }

    }
}
