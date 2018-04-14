using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CloudRS232Server
{
    public class CRC
    {
        static ushort[] crc_ta = new ushort[16] { 
            0x0000,0x1021,0x2042,0x3063,
            0x4084,0x50a5,0x60c6,0x70e7,
            0x8108,0x9129,0xa14a,0xb16b,
            0xc18c,0xd1ad,0xe1ce,0xf1ef
        };

        static public ushort Crc16(byte[] ptr, byte len)
        {
            ushort crc;
            byte da;
            int i = 0;

            crc = 0;
            while (len-- != 0)
            {
                da = (byte)(crc >> 12);
                crc <<= 4;
                crc ^= crc_ta[da ^ (ptr[i] / 16)];

                da = (byte)(crc >> 12);
                crc <<= 4;
                crc ^= crc_ta[da ^ (ptr[i] & 0x0f)];
                i++;
            }
            return (crc);
        }

        public static bool CRCCheck(string TxBuf)
        {
            ushort iCRC = 0;
            string sCRC = TxBuf.Substring(TxBuf.Length - 5, 4);
            TxBuf = TxBuf.Replace(sCRC + ">", "0000>");
            iCRC = Crc16(Encoding.ASCII.GetBytes(TxBuf), (byte)TxBuf.Length);
            if (sCRC == iCRC.ToString("X4"))		//对接收的数据进行CRC校验
            {
                return true;
            }
            return false;
        }

        public static bool CRCCheck(byte[] data)
        {
            byte[] a = data.Skip(4).Take(data.Length - 7).ToArray();
            //Array.Copy(data, a, data.Length - 3);
            int ncrc = Convert.ToInt32(Crc16(a, (byte)a.Length));
            int ocrc = BitConverter.ToUInt16(a, a.Length - 2);// (data[data.Length - 3] << 8) + data[data.Length - 2];
            if (ocrc == 0) return true;
            return (ocrc == ncrc);
        }

        public static byte[] CrcIntoPackage(byte[] cmd)
        {
            ushort iCRC = 0;
            byte[] crctmp = new byte[4];
            iCRC = Crc16(cmd, (byte)cmd.Length);	//获取发送CRC校验码
            crctmp = Encoding.ASCII.GetBytes(iCRC.ToString("X4"));
            cmd[cmd.Length - 5] = crctmp[0];
            cmd[cmd.Length - 4] = crctmp[1];
            cmd[cmd.Length - 3] = crctmp[2];
            cmd[cmd.Length - 2] = crctmp[3];
            return cmd;
        }
    }
}
