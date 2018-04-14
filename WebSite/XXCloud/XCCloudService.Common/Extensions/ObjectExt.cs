using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XCCloudService.Common.Extensions
{
    /// <summary>
    /// Object扩展类
    /// </summary>
    public static class ObjectExt
    {
        /// <summary>
        /// 检查对象是否为空或字符串是否为空字符串
        /// </summary>
        /// <param name="o">对象</param>
        /// <returns></returns>
        public static bool IsNull(this object o)
        {
            return ((o == null) || (((o.GetType() == typeof(string)) && string.IsNullOrEmpty(o.ToString().Trim())) || (o.GetType() == typeof(DBNull))));
        }
    }
}
