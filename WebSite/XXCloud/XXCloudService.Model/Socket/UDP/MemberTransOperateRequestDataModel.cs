using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace XCCloudService.Model.Socket.UDP
{
    [DataContract]
    public class MemberTransOperateRequestDataModel
    {
        public MemberTransOperateRequestDataModel(string sn,string mobilename,string phone,string iniccardid,string outiccardid,int coins)
        {
            this.SN = sn;
            this.MobileName = mobilename;
            this.Phone = phone;
            this.InICCardId = iniccardid;
            this.OutICCardId = outiccardid;
            this.Coins = coins;
            this.SignKey = "";
        }

        [DataMember(Name = "mobilename", Order = 1)]
        public string MobileName { set; get; }

        [DataMember(Name = "phone", Order = 2)]
        public string Phone { set; get; }

        //转出会员卡号
        [DataMember(Name = "iniccardid", Order = 3)]
        public string InICCardId { set; get; }


        //转入会员卡号
        [DataMember(Name = "outiccardid", Order = 5)]
        public string OutICCardId { set; get; }

        //过户币数
        [DataMember(Name = "coins", Order = 6)]
        public int Coins { set; get; }

        [DataMember(Name = "sn", Order = 7)]
        public string SN { set; get; }

        [DataMember(Name = "signkey", Order = 8)]
        public string SignKey { set; get; }
    }
}
