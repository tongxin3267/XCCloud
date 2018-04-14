using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XCCloudService.Model.Socket.UDP
{
    public class StoreQueryNotifyOutParamsModel
    {
        public StoreQueryNotifyOutParamsModel(byte[] responsePackages, object responseModel, string responseJson)
        {
            this.ResponsePackages = responsePackages;
            this.ResponseModel = responseModel;
            this.ResponseJson = responseJson;
        }

        /// <summary>
        /// 响应数据包
        /// </summary>
        public byte[] ResponsePackages { set; get; }

        public object ResponseModel { set; get; }

        public string ResponseJson { set; get; }
    }
}
