using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace XCCloudService.Model.Socket.UDP
{
    [DataContract]
    public class StoreQueryResultNotifyResponseModel
    {
        public StoreQueryResultNotifyResponseModel(string result_code, string result_msg, string packId)
        {
            this.Result_Code = result_code;
            this.Result_Msg = result_msg;
            this.PackId = packId;
        }

        /// <summary>
        /// 结果代码（T/F;T-接口业务逻辑执行成功，F-接口业务逻辑执行失败）
        /// </summary>
        [DataMember(Name = "result_code", Order = 1)]
        public string Result_Code { set; get; }

        /// <summary>
        /// 结果消息（接口业务逻辑执行失败）
        /// </summary>
        [DataMember(Name = "result_msg", Order = 2)]
        public string Result_Msg { set; get; }


        ///// <summary>
        ///// 业务流水号
        ///// </summary>
        //[DataMember(Name = "sn", Order = 3)]
        //public string SN { set; get; }

        /// <summary>
        /// 帧序号
        /// </summary>
        [DataMember(Name = "packid", Order = 4)]
        public string PackId { set; get; }

        /// <summary>
        /// 签名
        /// </summary>
        [DataMember(Name = "signkey", Order = 5)]
        public string SignKey { set; get; }
    }
}
