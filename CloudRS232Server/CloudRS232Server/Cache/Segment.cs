using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace CloudRS232Server.Cache
{
    public class Segment
    {
        /// <summary>
        /// 判断接入控制器是否合法
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool GetSegment(string key)
        {
            DataAccess ac = new DataAccess();
            DataTable dt = ac.ExecuteQueryReturnTable("select * from Base_DeviceInfo where DeviceType='0' and Status<>2 and Token='" + key + "'");  //绑定所有空闲或正常的控制设备
            return dt.Rows.Count > 0;
        }

    }
}
