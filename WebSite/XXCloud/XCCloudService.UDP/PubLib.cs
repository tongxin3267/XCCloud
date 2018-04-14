using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XCCloudService.SocketService.UDP
{
    public class PubLib
    {
        public static string BytesToString(byte[] data)
        {
            StringBuilder sb = new StringBuilder();
            foreach (byte b in data)
            {
                sb.Append(Hex2String(b) + " ");
            }
            return sb.ToString();
        }

        public static byte[] StringToByte(string data)
        {
            List<byte> b = new List<byte>();
            if (data.Length % 2 != 0) return Encoding.ASCII.GetBytes("123456");

            for (int i = 0; i < data.Length; i += 2)
            {
                b.Add(Convert.ToByte(data.Substring(i, 2), 16));
            }
            return b.ToArray();
        }

        public static string UInt16ToHexString(UInt16 data)
        {
            string value = "";
            byte[] bytes = BitConverter.GetBytes(data);
            foreach (byte b in bytes)
            {
                value = Hex2String(b) + value;
            }
            return value;
        }

        public static string UInt32ToHexString(UInt32 data)
        {
            string value = "";
            byte[] bytes = BitConverter.GetBytes(data);
            foreach (byte b in bytes)
            {
                value = Hex2String(b) + value;
            }
            return value;
        }

        public static string UInt64ToHexString(UInt64 data)
        {
            string value = "";
            byte[] bytes = BitConverter.GetBytes(data);
            foreach (byte b in bytes)
            {
                value = Hex2String(b) + value;
            }
            return value;
        }

        public static string Hex2String(byte data)
        {
            string value = Convert.ToString(data, 16);
            if (value.Length == 1)
            {
                value = "0" + value;
            }
            return value;
        }
        public static string Hex2String(byte[] data)
        {
            string value = "";
            foreach (byte b in data)
            {
                value += Hex2String(b);
            }
            return value;
        }

        public static string Hex2BitString(byte data)
        {
            string value = Convert.ToString(data, 2);
            int len = value.Length;
            for (int i = 0; i < 8 - len; i++)
            {
                value = "0" + value;
            }
            return value;
        }

    }
}
