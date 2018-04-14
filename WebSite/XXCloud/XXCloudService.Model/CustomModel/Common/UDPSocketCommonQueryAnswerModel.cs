using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XCCloudService.Model.CustomModel.Common
{
    public class UDPSocketCommonQueryAnswerModel
    {
        public UDPSocketCommonQueryAnswerModel(string id, string storeId, string storePassword, int status, object result, string radarToken)
        {
            this.Id = id;
            this.StoreId = storeId;
            this.StorePassword = storePassword;
            this.Status = 0;
            this.Result = null;
            this.CreateTime = System.DateTime.Now;
            this.RadarToken = radarToken;
        }

        public string StoreId { set; get; }

        public string StorePassword { set; get; }

        public string Id { get; set; }

        public int Status { set; get; }//0-初始;1-完成查询指令;2-完成查询结果

        public object Result { set; get; }

        public DateTime CreateTime { set; get; }

        public string RadarToken { set; get; }
    }
}
