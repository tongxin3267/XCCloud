using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace XCCloudService.Model.Socket.UDP
{
    [DataContract]
    public class DeviceStateResponseModel
    {
        public DeviceStateResponseModel(string result_code, string result_msg, string token, string signkey)
        {
            this.Result_Code = result_code;
            this.Result_Msg = result_msg;
            this.Token = token;
            this.SignKey = signkey;
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


        /// <summary>
        /// 结果消息（接口业务逻辑执行失败）
        /// </summary>
        [DataMember(Name = "token", Order = 2)]
        public string Token { set; get; }


        [DataMember(Name = "signkey", Order = 2)]
        public string SignKey { set; get; }
    }
}
