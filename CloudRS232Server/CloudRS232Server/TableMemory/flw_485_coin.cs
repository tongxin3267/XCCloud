using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace CloudRS232Server.TableMemory
{
    public class flw_485_coin
    {
        public class Table
        {
            public Int64 ID { get; set; }
            public String MerchID { get; set; }
            public String DeviceID { get; set; }
            public string ICCardID { get; set; }
            public Int32 Coins { get; set; }
            public String CoinType { get; set; }
            public Int32 Balance { get; set; }
            public DateTime RealTime { get; set; }
            public string SN { get; set; }
        }
        /// <summary>
        /// 添加新的流水纪录
        /// </summary>
        /// <param name="t"></param>
        public static UInt32 Add(Table t)
        {
            int d = 0;
            UInt32 id = 0;
            int.TryParse(t.ICCardID, out d);
            t.ICCardID = d.ToString();
            string sql = string.Format("insert into flw_485_coin values ('{0}','{1}','{2}','{3}','{4}','{5}','{6}'); select max(ID) as ID from flw_485_coin;", t.MerchID, t.DeviceID, t.ICCardID, t.Coins, t.CoinType, t.Balance, t.RealTime.ToString("yyyy-MM-dd HH:mm:ss"), t.SN);//, t.RealTime.ToShortDateString(), t.RealTime.ToLongTimeString());
            DataAccess ac = new DataAccess();
            DataTable dt = ac.ExecuteQueryReturnTable(sql);
            if (dt.Rows.Count > 0)
            {
                id = Convert.ToUInt32(dt.Rows[0]["ID"].ToString());
            }
            return id;
        }
    }
}
