using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;

namespace XCCloudService.SocketService.UDP.Common
{
    public class JsonHelper
    {
        public static byte[] DataContractJsonSerializerToByteArray(object resObj)
        {
            //获取响应对象的类名和泛型类型名
            //string fullName = resObj.GetType().FullName;
            List<Type> list = new List<Type>();
            list.Add(resObj.GetType());
            IEnumerable<Type> iEnum = list;

            DataContractJsonSerializer serializer = new DataContractJsonSerializer(resObj.GetType(), iEnum);
            byte[] byteArr;
            using (MemoryStream ms = new MemoryStream())
            {
                serializer.WriteObject(ms, resObj);
                byteArr = ms.ToArray();
            }
            return byteArr;
        }


        /// <summary>
        /// 数据协议JSON序列华
        /// </summary>
        /// <param name="resObj"></param>
        /// <returns></returns>
        public static string DataContractJsonSerializer(object resObj)
        {
            //获取响应对象的类名和泛型类型名
            List<Type> list = new List<Type>();
            list.Add(resObj.GetType());
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

        public static T DataContractJsonDeserializer<T>(string json)
        {
            Type type = typeof(T);
            string fullName = type.FullName;
            List<Type> list = new List<Type>();
            list.Add(type);
            IEnumerable<Type> iEnum = list;

            DataContractJsonSerializer serializer = new DataContractJsonSerializer(type, iEnum);
            var mStream = new MemoryStream(Encoding.UTF8.GetBytes(json));
            return (T)(serializer.ReadObject(mStream));
        }
    }
}
