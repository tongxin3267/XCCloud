using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XCCloudService.Model.CustomModel.Common
{
    /// <summary>
    /// UDPSocket应答对象
    /// </summary>
    public class UDPSocketStoreQueryAnswerModel
    {
        public UDPSocketStoreQueryAnswerModel(string sn,string storeId,string radarToken)
        {
            this.SN = sn;
            this.StoreId = storeId;
            this.Status = 0;
            this.Result = "";
            this.CreateTime = System.DateTime.Now;
            this.RadarToken = radarToken;
        }


        public string StoreId { set; get; }

        public string SN { get; set; }

        public int Status { set; get; }//0-初始;1-完成查询指令;2-完成查询结果

        public object Result { set; get; }

        public DateTime CreateTime { set; get; }

        public string RadarToken { set; get; }
    }
}
