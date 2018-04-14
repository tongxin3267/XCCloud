using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace CloudRS232Server
{
    public class LogHelper
    {
        public static void WriteLog(Exception ex)
        {
            DateTime d = DateTime.Now;
            string filePath = "";
            string fileName = "";

            filePath = string.Format("log\\{0}\\{1}", d.ToString("yyyy"), d.ToString("yyyy-MM"));
            fileName = d.ToString("yyyy-MM-dd") + ".txt";
            try
            {
                Directory.CreateDirectory(filePath);
                StreamWriter sw = new StreamWriter(filePath + "\\" + fileName, true, Encoding.GetEncoding("gb2312"));
                sw.WriteLine("错误时间：" + d.ToString());
                sw.WriteLine("错误内容：" + ex.Message);
                sw.WriteLine("堆栈信息：" + ex.StackTrace);
                sw.WriteLine();
                sw.Flush();
                sw.Close();
            }
            catch { }
        }

        public static void WriteLog(Exception ex, byte[] data)
        {
            DateTime d = DateTime.Now;
            string filePath = "";
            string fileName = "";

            filePath = string.Format("log\\{0}\\{1}", d.ToString("yyyy"), d.ToString("yyyy-MM"));
            fileName = d.ToString("yyyy-MM-dd") + ".txt";
            try
            {
                Directory.CreateDirectory(filePath);
                StreamWriter sw = new StreamWriter(filePath + "\\" + fileName, true, Encoding.GetEncoding("gb2312"));
                sw.WriteLine("错误时间：" + d.ToString());
                sw.WriteLine("错误内容：" + ex.Message);
                if (data != null)
                    sw.WriteLine("数据包内容：" + PubLib.BytesToString(data));
                sw.WriteLine("堆栈信息：" + ex.StackTrace);
                sw.WriteLine();
                sw.Flush();
                sw.Close();
            }
            catch { }
        }

        public static void WriteLog(string errorMsg, byte[] data)
        {
            DateTime d = DateTime.Now;
            string filePath = "";
            string fileName = "";

            filePath = string.Format("log\\{0}\\{1}", d.ToString("yyyy"), d.ToString("yyyy-MM"));
            fileName = d.ToString("yyyy-MM-dd") + "coin.txt";
            try
            {
                Directory.CreateDirectory(filePath);
                StreamWriter sw = new StreamWriter(filePath + "\\" + fileName, true, Encoding.GetEncoding("gb2312"));
                sw.WriteLine("错误时间：" + d.ToString());
                sw.WriteLine("错误内容：" + errorMsg);
                if (data != null)
                    sw.WriteLine("数据包内容：" + PubLib.BytesToString(data));
                sw.WriteLine();
                sw.Flush();
                sw.Close();
            }
            catch { }
        }

        public static void WriteLogSpacail(string msg)
        {
            DateTime d = DateTime.Now;
            string filePath = "";
            string fileName = "";

            filePath = string.Format("log\\{0}\\{1}", d.ToString("yyyy"), d.ToString("yyyy-MM"));
            fileName = d.ToString("yyyy-MM-dd") + "OFF.txt";
            try
            {
                Directory.CreateDirectory(filePath);
                StreamWriter sw = new StreamWriter(filePath + "\\" + fileName, true, Encoding.GetEncoding("gb2312"));
                sw.WriteLine("事件时间：" + d.ToString());
                sw.WriteLine("事件内容：" + msg);
                sw.WriteLine();
                sw.Flush();
                sw.Close();
            }
            catch { }
        }

        public static void WriteLog(string errorMsg)
        {
            DateTime d = DateTime.Now;
            string filePath = "";
            string fileName = "";

            filePath = string.Format("log\\{0}\\{1}", d.ToString("yyyy"), d.ToString("yyyy-MM"));
            fileName = d.ToString("yyyy-MM-dd") + ".txt";
            try
            {
                Directory.CreateDirectory(filePath);
                StreamWriter sw = new StreamWriter(filePath + "\\" + fileName, true, Encoding.GetEncoding("gb2312"));
                sw.WriteLine("事件时间：" + d.ToString());
                sw.WriteLine("事件内容：" + errorMsg);
                sw.WriteLine();
                sw.Flush();
                sw.Close();
            }
            catch { }
        }

        public static void WriteDBLog(string sql)
        {
            DateTime d = DateTime.Now;
            string filePath = "";
            string fileName = "";

            filePath = string.Format("log\\{0}\\{1}", d.ToString("yyyy"), d.ToString("yyyy-MM"));
            fileName = d.ToString("yyyy-MM-dd") + "db.txt";
            try
            {
                Directory.CreateDirectory(filePath);
                StreamWriter sw = new StreamWriter(filePath + "\\" + fileName, true, Encoding.GetEncoding("gb2312"));
                sw.WriteLine("执行时间：" + d.ToString());
                sw.WriteLine("执行语句：" + sql);
                sw.WriteLine();
                sw.Flush();
                sw.Close();
            }
            catch { }
        }
        public static void WriteDBLog(string sql, Exception ex)
        {
            DateTime d = DateTime.Now;
            string filePath = "";
            string fileName = "";

            filePath = string.Format("log\\{0}\\{1}", d.ToString("yyyy"), d.ToString("yyyy-MM"));
            fileName = d.ToString("yyyy-MM-dd") + "db.txt";
            try
            {
                Directory.CreateDirectory(filePath);
                StreamWriter sw = new StreamWriter(filePath + "\\" + fileName, true, Encoding.GetEncoding("gb2312"));
                sw.WriteLine("执行时间：" + d.ToString());
                sw.WriteLine("执行语句：" + sql);
                sw.WriteLine("发生错误：" + ex.Message);
                sw.WriteLine("调用堆栈：" + ex.StackTrace);
                sw.WriteLine();
                sw.Flush();
                sw.Close();
            }
            catch { }
        }

        public static void WriteTBLog(string txt)
        {
            DateTime d = DateTime.Now;
            string filePath = "";
            string fileName = "";

            filePath = string.Format("log\\{0}\\{1}", d.ToString("yyyy"), d.ToString("yyyy-MM"));
            fileName = d.ToString("yyyy-MM-dd HH") + " HourTB.txt";
            try
            {
                Directory.CreateDirectory(filePath);
                StreamWriter sw = new StreamWriter(filePath + "\\" + fileName, true, Encoding.GetEncoding("gb2312"));
                sw.WriteLine("接收时间：" + d.ToString());
                sw.WriteLine("原始数据：" + txt);
                sw.WriteLine();
                sw.Flush();
                sw.Close();
            }
            catch { }
        }
        public static void WritePush(byte[] data, string segment, string headAddress, string pushType, string coin, string sn)
        {
            DateTime d = DateTime.Now;
            string filePath = "";
            string fileName = "";

            filePath = string.Format("log\\SN\\{0}-{1}\\{2}\\{3}", segment, headAddress, d.ToString("yyyy"), d.ToString("yyyy-MM"));
            fileName = d.ToString("yyyy-MM-dd HH") + " PUSH.txt";
            try
            {
                Directory.CreateDirectory(filePath);
                StreamWriter sw = new StreamWriter(filePath + "\\" + fileName, true, Encoding.GetEncoding("gb2312"));
                sw.WriteLine("接收时间：" + d.ToString());
                sw.WriteLine("原始数据：" + PubLib.BytesToString(data));
                sw.WriteLine("投币类别：" + pushType);
                sw.WriteLine("投币数：" + coin);
                sw.WriteLine("投币流水：" + sn);
                sw.WriteLine();
                sw.Flush();
                sw.Close();
            }
            catch { }
        }

        public static void WriteSNLog(string type, string headAddress, string segment, string sn, byte[] data)
        {
            DateTime d = DateTime.Now;
            string filePath = "";
            string fileName = "";

            filePath = string.Format("log\\SN\\{0}-{1}\\{2}\\{3}", segment, headAddress, d.ToString("yyyy"), d.ToString("yyyy-MM"));
            fileName = d.ToString("yyyy-MM-dd HH") + " .txt";
            try
            {
                Directory.CreateDirectory(filePath);
                StreamWriter sw = new StreamWriter(filePath + "\\" + fileName, true, Encoding.GetEncoding("gb2312"));
                sw.WriteLine("接收时间：" + d.ToString());
                sw.WriteLine("类别：" + type);
                sw.WriteLine("流水号：" + sn);
                if (data != null)
                    sw.WriteLine("原始数据：" + PubLib.BytesToString(data));
                sw.WriteLine();
                sw.Flush();
                sw.Close();
            }
            catch { }
        }
    }
}
