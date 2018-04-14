using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace XCCloudService.Model.Socket.UDP
{
    [DataContract]
    public class RadarRegisterResponseModel
    {
        public RadarRegisterResponseModel(string token,string signkey)
        {
            this.Token = token;
            this.SignKey = SignKey;
        }


        /// <summary>
        /// 结果消息（接口业务逻辑执行失败）
        /// </summary>
        [DataMember(Name = "token", Order = 1)]
        public string Token { set; get; }


        [DataMember(Name = "signkey", Order = 2)]
        public string SignKey { set; get; }
    }
}
