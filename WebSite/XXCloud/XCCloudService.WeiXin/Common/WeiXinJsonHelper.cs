using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XCCloudService.Common;

namespace XCCloudService.WeiXin.Common
{
    public class WeiXinJsonHelper
    {
        /// <summary>
        /// 获取微信接口响应结果，如果微信响应结果代码包含errorcode返回false;否则返回true;
        /// </summary>
        /// <param name="json"></param>
        /// <param name="paramsObj"></param>
        /// <returns></returns>
        public static bool GetResponseJsonResult(string json,ref Dictionary<string,object> paramsObj)
        {
            object obj = Utils.DeserializeObject(json);
            if (Utils.JsonObjectExistProperty(obj, "errcode"))
            {
                //Json中含有"errcode"字段，结果代码为false
                paramsObj = (System.Collections.Generic.Dictionary<string, object>)obj;
                return false;
            }
            else
            {
                paramsObj = (System.Collections.Generic.Dictionary<string, object>)obj;
                return true;
            }
        }
    }
}
