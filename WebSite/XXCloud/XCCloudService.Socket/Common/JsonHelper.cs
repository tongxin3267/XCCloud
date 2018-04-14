using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;

namespace XCCloudService.SocketService.TCP.Common
{
    public class JsonHelper
    {
        public static string DataContractJsonSerializer(object resObj)
        {
            //获取响应对象的类名和泛型类型名
            string fullName = resObj.GetType().FullName;
            List<Type> list = new List<Type>();
            list.Add(Type.GetType(fullName, true, true));
            IEnumerable<Type> iEnum = list;

            DataContractJsonSerializer serializer = new DataContractJsonSerializer(resObj.GetType(), iEnum);
            byte[] byteArr;
            using (MemoryStream ms = new MemoryStream())
            {
                serializer.WriteObject(ms, resObj);
                byteArr = ms.ToArray();
            }
            string jsonStr = Encoding.UTF8.GetString(byteArr);
            return jsonStr;
        }
    }
}
