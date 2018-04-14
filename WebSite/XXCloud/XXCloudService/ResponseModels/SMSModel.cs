using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace XCCloudService.ResponseModels
{
    /// <summary>
    /// 短信余额响应模式
    /// </summary>
    [DataContract]
    public class SMSBalanceResponseModel
    {
        public SMSBalanceResponseModel(int remain)
        {
            this.Remain = remain;
        }

        [DataMember(Name = "remain", Order = 1)]
        public int Remain { set; get; }
    }
}