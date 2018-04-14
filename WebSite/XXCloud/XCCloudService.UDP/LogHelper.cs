using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace XCCloudService.SocketService.UDP
{
    public class LogHelper
    {
        public static void WriteLog(Exception ex)
        {
            DateTime d = DateTime.Now;
            string filePath = "";
            string fileName = "";

            filePath = string.Format("log\\{0}\\{1}", d.ToString("yyyy"), d.ToString("yyyy-MM"));
            fileName = "ServiceDLL [" + d.ToString("yyyy-MM-dd") + "].txt";
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

        public static void WriteLog(string msg)
        {
            DateTime d = DateTime.Now;
            string filePath = "";
            string fileName = "";

            filePath = string.Format("log\\{0}\\{1}", d.ToString("yyyy"), d.ToString("yyyy-MM"));
            fileName = "ServiceDLL [" + d.ToString("yyyy-MM-dd") + "].txt";
            try
            {
                Directory.CreateDirectory(filePath);
                StreamWriter sw = new StreamWriter(filePath + "\\" + fileName, true, Encoding.GetEncoding("gb2312"));
                sw.WriteLine("错误时间：" + d.ToString());
                sw.WriteLine("错误内容：" + msg);
                sw.WriteLine();
                sw.Flush();
                sw.Close();
            }
            catch { }
        }
    }
}
