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
    public class UDPSocketAnswerModel
    {
        public UDPSocketAnswerModel(string ip, int port, byte[] data,string orderId,DateTime createTime,string mobile,string storeId,string segment,string mcuId,int coins,string sn)
        {
            this.IP = ip;
            this.Port = port;
            this.Data = data;
            this.OrderId = orderId;
            this.CreateTime = createTime;
            this.Mobile = mobile;
            this.StoreId = storeId;
            this.Segment = segment;
            this.MCUID = mcuId;
            this.Coins = coins;
            this.SN = sn;
        }

        /// <summary>
        /// ip地址
        /// </summary>
        public string IP { set; get; }

        /// <summary>
        /// 端口号
        /// </summary>
        public int Port { set; get; }

        /// <summary>
        /// 数据
        /// </summary>
        public byte[] Data { set; get; }

        public string OrderId { set; get; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { set;get;}

        public string Mobile { set; get; }

        public string StoreId { set; get; }

        public string Segment {set;get;}

        public string MCUID { set; get; }

        public int Coins { get; set; }

        public string SN { get; set; }
    }
}
