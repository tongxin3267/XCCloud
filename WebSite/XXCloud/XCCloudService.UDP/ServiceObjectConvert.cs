using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace XCCloudService.SocketService.UDP
{
    public class ServiceObjectConvert
    {
        /// <summary>
        /// 结构生成数组
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static byte[] SerializeObject(object obj)
        {
            if (obj == null) return null;
            MemoryStream ms = new MemoryStream();
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(ms, obj);
            ms.Position = 0;
            byte[] bytes = new byte[ms.Length];
            ms.Read(bytes, 0, bytes.Length);
            ms.Close();
            return bytes;
        }

        /// <summary>
        /// 数组生成对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static object DeserializeObject(byte[] bytes)
        {
            object obj = null;
            if (bytes == null) return obj;
            MemoryStream ms = new MemoryStream(bytes);
            ms.Position = 0;
            BinaryFormatter formatter = new BinaryFormatter();
            obj = formatter.Deserialize(ms);
            ms.Close();
            return obj;
        }

        /// <summary>
        /// 将7E转成7D 01
        /// 将7D转成7D 02
        /// </summary>
        /// <param name="recv"></param>
        public static void 转定义解码(ref List<byte> recv)
        {
            int iLen = recv.Count;
            for (int i = 0; i < recv.Count - 1; i++)
            {
                if (recv[i] == 0x7d)
                {
                    if (recv[i + 1] == 0x02)
                    {
                        recv.RemoveRange(i, 2);
                        recv.Insert(i, 0x7e);
                    }
                }
            }
            iLen = recv.Count;
            for (int i = 0; i < recv.Count - 1; i++)
            {
                if (recv[i] == 0x7d)
                {
                    if (recv[i + 1] == 0x01)
                    {
                        recv.RemoveRange(i, 2);
                        recv.Insert(i, 0x7d);
                    }
                }
            }
        }

        /// <summary>
        /// 将7D 01转成7E
        /// 将7D 02转成7D
        /// </summary>
        /// <param name="recv"></param>
        /// <param name="key"></param>
        public static void 转定义编码(ref List<byte> recv, byte key)
        {
            try
            {
                int startIndex = -1;

                while (startIndex < recv.Count)
                {
                    startIndex = recv.IndexOf(key, startIndex + 1);
                    if (startIndex < 0)
                    {
                        break;
                    }
                    if (startIndex <= (recv.Count - 1))
                    {
                        switch (recv[startIndex])
                        {
                            case 0x7e:
                                recv.RemoveRange(startIndex, 1);
                                recv.Insert(startIndex, 0x7d);
                                recv.Insert(startIndex + 1, 0x02);
                                break;
                            case 0x7d:
                                recv.Insert(startIndex + 1, 0x01);
                                break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        public static byte[] 协议编码(byte cmdType, byte[] data)
        {
            List<byte> dataBUF = new List<byte>();

            if (data != null)
            {
                dataBUF.AddRange(data);
            }
            dataBUF.InsertRange(0, BitConverter.GetBytes((UInt16)dataBUF.Count));
            dataBUF.Insert(0, cmdType);
            ServiceObjectConvert.转定义编码(ref dataBUF, 0x7D);
            ServiceObjectConvert.转定义编码(ref dataBUF, 0x7E);
            dataBUF.Insert(0, 0x7e);
            dataBUF.Add(0x7e);

            return dataBUF.ToArray();
        }
    }
}
