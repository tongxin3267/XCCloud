using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using XCCloudService.BLL.CommonBLL;

namespace XXCloudService.RadarServer.Info
{
    public class CoinInfo
    {
        public class 会员卡信息
        {
            public string 会员卡号 { get; set; }
            public string 商户号 { get; set; }
            public string 微信号 { get; set; }
            public int 币余额 { get; set; }
            public int 票余额 { get; set; }
            public bool 机头上下分 { get; set; }
            public int 动态密码 { get; set; }
            public string 备注 { get; set; }
            public bool 锁会员 { get; set; }
            public bool 级别规则是否允许退币到卡里 { get; set; }
            public bool 锁机头 { get; set; }
            public DateTime? 退币时间 = null;
            public int 投币数 = 0;
            public int 退币数 = 0;

            public void Update()
            {
                string sql = string.Format("update t_member set Balance='{0}',Lottery='{1}' where ICCardID='{2}' and MerchID='{3}'", 币余额, 票余额, 会员卡号, 商户号);
                //DataAccess ac = new DataAccess();
                //ac.Execute(sql);
                XCCloudRS232BLL.ExecuteSql(sql);
            }
        }

        public 会员卡信息 GetMemberInfo(string ICCardID, string Code)
        {
            //DataAccess ac = new DataAccess();
            string sql = "select m.* from t_member m,Base_DeviceInfo d where m.MerchID=d.MerchID and d.Token='" + Code + "' and m.ICCardID='" + ICCardID + "'";
            //DataTable dt = ac.ExecuteQueryReturnTable(sql);
            DataTable dt = XCCloudRS232BLL.ExecuterSqlToTable(sql);
            if (dt.Rows.Count > 0)
            {
                DataRow row =dt.Rows[0];
                会员卡信息 card = new 会员卡信息();
                card.备注 = row["Note"].ToString();
                card.币余额 = Convert.ToInt32(row["Balance"].ToString());
                card.动态密码 = Convert.ToInt32(row["RepeatCode"].ToString());
                card.会员卡号 = row["ICCardID"].ToString();
                card.机头上下分 = true;
                card.级别规则是否允许退币到卡里 = true;
                card.票余额 = Convert.ToInt32(row["Lottery"].ToString());
                card.商户号 = row["MerchID"].ToString();
                card.锁会员 = (row["Lock"].ToString() == "1");
                card.锁机头 = false;
                card.微信号 = row["OpenID"].ToString();
                return card;
            }
            return null;
        }
    }
}
