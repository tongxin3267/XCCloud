using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace XCCloudService.Model.Socket.UDP
{
    [DataContract]
    public class ParamQueryResultNotifyRequestModel
    {
        public ParamQueryResultNotifyRequestModel()
        {
            this.Result_Code = "";
            this.Result_Msg = "";
            this.SignKey = "";
            this.Result_Data = new ParamQueryResultModel();
            this.SN = "";

        }

        [DataMember(Name = "result_code", Order = 1)]
        public string Result_Code { set; get; }

        [DataMember(Name = "result_msg", Order = 2)]
        public string Result_Msg { set; get; }

        [DataMember(Name = "signkey", Order = 3)]
        public string SignKey { set; get; }

        [DataMember(Name = "result_data", Order = 4)]
        public ParamQueryResultModel Result_Data { set; get; }

        [DataMember(Name = "sn", Order = 5)]
        public string SN { set; get; }
    }

    [DataContract]
    public class ParamQueryResultModel
    {
        public ParamQueryResultModel()
        {

        }


        [DataMember(Name = "txtCoinPrice", Order = 1)]
        public string TxtCoinPrice { set; get; }

        //项目名称
        [DataMember(Name = "txtTicketDate", Order = 2)]
        public string TxtTicketDate { set; get; }
    }

}
