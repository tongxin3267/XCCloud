using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace CloudRS232Server.Cache
{
    public class MerchInfo
    {
        /// <summary>
        /// 商户令牌缓存列表
        /// </summary>
        Dictionary<string, string> MerchList = new Dictionary<string, string>();

        public void CheckMerchToken(string phone, string token)
        {
            DataAccess ac = new DataAccess();
            DataTable dt = ac.ExecuteQueryReturnTable("select * from Base_MerchInfo where Mobile='" + phone + "'");
            if (dt.Rows.Count > 0)
            {
                
            }
        }
    }
}
