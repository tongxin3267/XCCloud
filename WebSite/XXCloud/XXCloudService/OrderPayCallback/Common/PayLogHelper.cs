using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Text;

namespace XCCloudService.PayChannel.Common
{
    /// <summary>
    ///LogHelper 的摘要说明
    /// </summary>
    public class PayLogHelper
    {
        public static void WriteEvent(string context, string eventtext)
        {
            DateTime d = DateTime.Now;
            string filePath = "";
            string fileName = "";

            filePath = string.Format("C:\\WXPayLog\\{0}\\{1}", d.ToString("yyyy"), d.ToString("yyyy-MM"));
            fileName = d.ToString("yyyy-MM-dd") + ".txt";
            try
            {
                Directory.CreateDirectory(filePath);
                StreamWriter sw = new StreamWriter(filePath + "\\" + fileName, true, Encoding.GetEncoding("gb2312"));
                sw.WriteLine("执行时间：" + d.ToString());
                sw.WriteLine("事件：" + eventtext);
                sw.WriteLine("内容：" + context);
                sw.WriteLine();
                sw.Flush();
                sw.Close();
            }
            catch { }
        }

        public static void WriteError(Exception ex)
        {
            DateTime d = DateTime.Now;
            string filePath = "";
            string fileName = "";

            filePath = string.Format("C:\\WXPayLog\\{0}\\{1}", d.ToString("yyyy"), d.ToString("yyyy-MM"));
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

        public static void WriteError(Exception ex, string sql)
        {
            DateTime d = DateTime.Now;
            string filePath = "";
            string fileName = "";

            filePath = string.Format("C:\\WXPayLog\\{0}\\{1}", d.ToString("yyyy"), d.ToString("yyyy-MM"));
            fileName = d.ToString("yyyy-MM-dd") + ".txt";
            try
            {
                Directory.CreateDirectory(filePath);
                StreamWriter sw = new StreamWriter(filePath + "\\" + fileName, true, Encoding.GetEncoding("gb2312"));
                sw.WriteLine("错误时间：" + d.ToString());
                sw.WriteLine("SQL语句：" + sql);
                sw.WriteLine("错误内容：" + ex.Message);
                sw.WriteLine("堆栈信息：" + ex.StackTrace);
                sw.WriteLine();
                sw.Flush();
                sw.Close();
            }
            catch { }
        }
        public static void WritePayLog(string context)
        {
            DateTime d = DateTime.Now;
            string filePath = "";
            string fileName = "";

            filePath = string.Format("C:\\WXPayLog\\{0}\\{1}", d.ToString("yyyy"), d.ToString("yyyy-MM"));
            fileName = d.ToString("yyyy-MM-dd") + " Pay.txt";
            try
            {
                Directory.CreateDirectory(filePath);
                StreamWriter sw = new StreamWriter(filePath + "\\" + fileName, true, Encoding.GetEncoding("gb2312"));
                sw.WriteLine("执行时间：" + d.ToString());
                sw.WriteLine("内容：" + context);
                sw.WriteLine();
                sw.Flush();
                sw.Close();
            }
            catch { }
        }
    }
}