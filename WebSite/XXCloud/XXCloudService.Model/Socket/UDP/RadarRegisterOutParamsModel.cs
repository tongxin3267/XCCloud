using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using XCCloudService.Common.Enum;

namespace XCCloudService.Model.Socket.UDP
{    
    /// <summary>
    /// 雷达参数输出模式
    /// </summary>
    [DataContract]
    public class RadarRegisterOutParamsModel
    {
        public RadarRegisterOutParamsModel(string storeId, string segment,string token,byte[] responsePackages,string responseJson)
        {
            this.StoreId = storeId;
            this.Segment = segment;
            this.Token = token;
            this.ResponsePackages = responsePackages;
            this.ResponseJson = responseJson;
        }

        /// <summary>
        /// 门店编号
        /// </summary>
        public string StoreId { set; get; }

        /// <summary>
        /// 门店内雷达段号
        /// </summary>
        public string Segment { set; get; }

        /// <summary>
        /// 雷达token
        /// </summary>
        public string Token { set; get; }

        /// <summary>
        /// 响应数据包
        /// </summary>
        public byte[] ResponsePackages { set; get; }


        public string ResponseJson { set; get; }
    }
}
