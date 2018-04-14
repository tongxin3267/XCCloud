using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XCCloudService.Common.Extensions
{
    public static class StringExt
    {
        /// <summary>
        /// 判断字符串是否可以转换为Int类型
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static bool IsInt(this string content)
        {
            try
            {
                int var1 = Convert.ToInt32(content);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
