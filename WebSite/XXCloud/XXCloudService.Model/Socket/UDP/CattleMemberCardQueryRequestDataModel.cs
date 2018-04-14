using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace XCCloudService.Model.Socket.UDP
{
    [DataContract]
    public class CattleMemberCardQueryRequestDataModel
    {
        public CattleMemberCardQueryRequestDataModel(string sn,string requestType)
        {
            this.RequestType = requestType;
            this.SN = sn;
            this.SignKey = string.Empty;
        }

        /// <summary>
        /// 请求列表
        /// </summary>
        [DataMember(Name = "requesttype", Order = 1)]
        public string RequestType { set; get; }

        [DataMember(Name = "sn", Order = 2)]
        public string SN { set; get; }

        /// <summary>
        /// 手机号码
        /// </summary>
        [DataMember(Name = "signkey", Order = 3)]
        public string SignKey { set; get; }
    }
}
