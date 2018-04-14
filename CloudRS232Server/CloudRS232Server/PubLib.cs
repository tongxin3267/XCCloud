using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Data;
using System.Windows.Forms;
using System.Xml;

namespace CloudRS232Server
{
    public static class PubLib
    {
        public static List<byte> PrintIndex = new List<byte>();
        public static List<string> PrintText = new List<string>();
        public static int indexVersion = 0;
        public static int textVerion = 0;
        public static bool isSetPrint = false;
        public static int intSetPrint = 0;

        public static bool SystemInitSuccess = false;

        public static string 程序版本号 = "2.0.0.1";
        public static Guid 我的程序标示 = Guid.NewGuid();
        public static string SQLConnectString = "";

        public static string 硬件版本号
        {
            set
            {
                XmlDocument doc = new XmlDocument();
                doc.Load("mysql.xml");
                if (doc.SelectSingleNode("Connection/Version") != null)
                {
                    doc.SelectSingleNode("Connection/Version").InnerText = value;
                }
                else
                {
                    XmlNode n = doc.CreateElement("Version");
                    n.InnerText = value;
                    doc.SelectSingleNode("Connection").AppendChild(n);
                }
                doc.Save("mysql.xml");
            }
            get
            {
                XmlDocument doc = new XmlDocument();
                doc.Load("mysql.xml");
                try
                {
                    return doc.SelectSingleNode("Connection/Version").InnerText;
                }
                catch { }
                return "";
            }
        }

        static int coinSN = 0;
        public static int 远程投币上分流水号
        {
            get
            {
                coinSN++;
                if (coinSN == 65535)
                    coinSN = 1;
                else if (coinSN == 0)
                    coinSN++;
                return coinSN;
            }
        }

        public static bool 串口数据调试打印 = true;
        public static bool 是否写数据库日志 = false;
        public static bool 是否下载配置文件 = false;
        public static bool 是否下载授权文件 = false;

        public static bool 是否打印接收头 = true;

        public static int 当前总指令数;
        public static int 当前查询指令数;
        public static int 当前币业务指令数;
        public static int 当前IC卡查询重复指令数;
        public static int 当前IC卡进出币指令重复数;
        public static int 当前小票指令数;
        public static int 当前错误指令数;
        public static int 当前返还指令数;

        public static int 会员余额上限 = 100000000;
        public static int 会员净退币数上限 = 100000;
        public static int 测试间隔时间 = 0;

        public static bool 是否允许继续发送机头指令 = false;
        public static int 当前发送机头序号 = 0;
        public static List<string> 当前待发送机头列表 = new List<string>();

        public enum PrintSettingType
        {
            LOGO图片 = 0,
            文字信息 = 1,
            打印顺序 = 2,
        }
        public const int MaxPacketSize = 200;
        /// <summary>
        /// 帧头 68
        /// </summary>
        public const byte FrameHead = 0x68;
        /// <summary>
        /// 帧尾 16
        /// </summary>
        public const byte FrameButtom = 0x16;
        /// <summary>
        /// 引导区字节 FE
        /// </summary>
        public const byte FrameBlankWord = 0xfe;

        public class 机头地址修改应答
        {
            public bool 机头在线状态 { get; set; }
            public bool 保留1 { get; set; }
            public bool 保留2 { get; set; }
            public bool 保留3 { get; set; }
            public bool 保留4 { get; set; }
            public bool 保留5 { get; set; }
            public bool 保留6 { get; set; }
            public bool 保留7 { get; set; }
        }

        public static void ShowAlerMessage(string msg)
        {
            MessageBox.Show(msg, "武汉莘宸科技", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        public static void ShowInfoMessage(string msg)
        {
            MessageBox.Show(msg, "武汉莘宸科技", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        public static void ShowErroMessage(string msg)
        {
            MessageBox.Show(msg, "武汉莘宸科技", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        public static bool ShowQuestMessage(string msg)
        {
            return (MessageBox.Show(msg, "武汉莘宸科技", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes);
        }

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
                value += " " + Hex2String(b);
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

        public static string Hex2BitString(UInt16 data)
        {
            string value = Convert.ToString(data, 2);
            int len = value.Length;
            for (int i = 0; i < 16 - len; i++)
            {
                value = "0" + value;
            }
            return value;
        }

        public static byte[] GetBytesByObject(object o)
        {
            List<byte> value = new List<byte>();
            Type t = o.GetType();
            foreach (PropertyInfo pi in t.GetProperties())
            {
                switch (pi.PropertyType.Name.ToLower())
                {
                    case "byte[]":
                        value.AddRange((byte[])pi.GetValue(o, null));
                        break;
                    case "byte":
                        value.Add((byte)pi.GetValue(o, null));
                        break;
                    case "uint16":
                        byte[] tuv = BitConverter.GetBytes((UInt16)pi.GetValue(o, null));
                        value.AddRange(tuv);
                        break;
                    case "int16":
                        byte[] tv = BitConverter.GetBytes((Int16)pi.GetValue(o, null));
                        value.AddRange(tv);
                        break;
                    case "uint32":
                        byte[] tul = BitConverter.GetBytes((UInt32)pi.GetValue(o, null));
                        value.AddRange(tul);
                        break;
                    case "int32":
                        byte[] tl = BitConverter.GetBytes((Int32)pi.GetValue(o, null));
                        value.AddRange(tl);
                        break;
                    case "uint64":
                        byte[] tl64 = BitConverter.GetBytes((UInt64)pi.GetValue(o, null));
                        value.AddRange(tl64);
                        if (pi.Name == "MCUID")
                        {
                            //芯片长地址移除最高位，转换成7位
                            value.RemoveAt(value.Count - 1);
                        }
                        break;
                    case "string":
                        switch (pi.Name)
                        {
                            case "本店卡校验密码":
                                string pv = pi.GetValue(o, null).ToString();
                                byte[] pvbytes = StringToByte(pv);
                                value.AddRange(pvbytes);
                                break;
                            case "IC卡号码":
                                string ic = pi.GetValue(o, null).ToString();
                                byte[] bytes = Encoding.ASCII.GetBytes(ic);
                                value.AddRange(bytes);
                                break;
                            case "游戏机机头编号":
                                string hid = pi.GetValue(o, null).ToString();
                                byte[] hidbytes = Encoding.ASCII.GetBytes(hid);
                                value.AddRange(hidbytes);
                                break;
                        }
                        break;
                    case "datetime":
                        DateTime dtd = Convert.ToDateTime(pi.GetValue(o, null).ToString());
                        byte[] dtdbytes = DateTimeBCD(dtd);
                        value.AddRange(dtdbytes);
                        break;
                }
            }
            return value.ToArray();
        }

        public static byte GetBitByObject(object o)
        {
            byte value = 0;

            Type t = o.GetType();
            foreach (PropertyInfo pi in t.GetProperties())
            {
                bool b = Convert.ToBoolean(pi.GetValue(o, null));
                if (b)
                {
                    value = (byte)((value << 1) + 0x01);
                }
                else
                {
                    value = (byte)((value << 1) + 0x00);
                }
            }

            return value;
        }

        public static UInt16 GetBit16ByObject(object o)
        {
            UInt16 value = 0;

            Type t = o.GetType();
            foreach (PropertyInfo pi in t.GetProperties())
            {
                bool b = Convert.ToBoolean(pi.GetValue(o, null));
                if (b)
                {
                    value = (UInt16)((value << 1) + 0x01);
                }
                else
                {
                    value = (UInt16)((value << 1) + 0x00);
                }
            }

            return value;
        }

        public static byte[] GetFrameDataBytes(FrameData dataFrame, byte[] dataBytes, CommandType cmdType)
        {
            List<byte> lstData = new List<byte>();
            lstData.Add(FrameHead);
            lstData.Add((byte)dataFrame.frameType);
            byte[] bs = BitConverter.GetBytes(Convert.ToInt16("0x" + dataFrame.routeAddress, 16));
            lstData.AddRange(bs);
            lstData.Add((byte)cmdType);
            if (dataBytes != null)
            {
                lstData.Add((byte)dataBytes.Length);
                lstData.AddRange(dataBytes);
            }
            else
            {
                lstData.Add(0);
            }
            bs = BitConverter.GetBytes((Int16)CRC.Crc16(lstData.ToArray(), (byte)lstData.Count));
            lstData.AddRange(bs);
            lstData.Add(FrameButtom);
            List<byte> value = new List<byte>();
            value.Add(FrameBlankWord);
            value.Add(FrameBlankWord);
            value.Add(FrameBlankWord);
            value.Add(FrameBlankWord);
            value.AddRange(lstData);
            value.Add(FrameBlankWord);
            value.Add(FrameBlankWord);

            return value.ToArray();
        }

        public static byte[] DateTimeBCD()
        {
            DateTime d = DateTime.Now;
            List<byte> res = new List<byte>();
            res.Add(0x00);
            res.Add((byte)Convert.ToInt32((d.Year - 2000).ToString(), 16));
            res.Add((byte)Convert.ToInt32(d.Month.ToString(), 16));
            res.Add((byte)Convert.ToInt32(d.Day.ToString(), 16));
            res.Add((byte)Convert.ToInt32(((int)d.DayOfWeek).ToString(), 16));
            res.Add((byte)Convert.ToInt32(d.Hour.ToString(), 16));
            res.Add((byte)Convert.ToInt32(d.Minute.ToString(), 16));
            res.Add((byte)Convert.ToInt32(d.Second.ToString(), 16));

            return res.ToArray();
        }
        public static byte[] DateTimeBCD(DateTime d)
        {
            List<byte> res = new List<byte>();
            res.Add(0x00);
            res.Add((byte)Convert.ToInt32((d.Year - 2000).ToString(), 16));
            res.Add((byte)Convert.ToInt32(d.Month.ToString(), 16));
            res.Add((byte)Convert.ToInt32(d.Day.ToString(), 16));
            res.Add((byte)Convert.ToInt32(((int)d.DayOfWeek).ToString(), 16));
            res.Add((byte)Convert.ToInt32(d.Hour.ToString(), 16));
            res.Add((byte)Convert.ToInt32(d.Minute.ToString(), 16));
            res.Add((byte)Convert.ToInt32(d.Second.ToString(), 16));

            return res.ToArray();
        }

        public static byte[] DateBCD()
        {
            DateTime d = DateTime.Now;
            List<byte> res = new List<byte>();
            res.Add(0x20);
            res.Add((byte)Convert.ToInt32((d.Year - 2000).ToString(), 16));
            res.Add((byte)Convert.ToInt32(d.Month.ToString(), 16));
            res.Add((byte)Convert.ToInt32(d.Day.ToString(), 16));

            return res.ToArray();
        }

        public static DateTime GetTimeBCD(byte[] data)
        {
            string value = string.Format("20{0}-{1}-{2} {3}:{4}:{5}", data[1], data[2], data[3], data[5], data[6], data[7]);
            DateTime d;
            if (!DateTime.TryParse(value, out d))
            {
                d = DateTime.Now;
            }
            return d;
        }


    }
}
