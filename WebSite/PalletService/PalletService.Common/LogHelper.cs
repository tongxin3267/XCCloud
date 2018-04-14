using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Web;


namespace PalletService.Common
{
    public class LogHelper
    {
        public static void SaveLog(string strMsg)
        {
            string s = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "\n";
            SaveLogFile(s + strMsg + "\n");
        }

        private static void SaveLogFile(string strErrMsg)
        {
            string logRootDirectory = ("c:/Logs/");
            if (!Directory.Exists(logRootDirectory))
            {
                Directory.CreateDirectory(logRootDirectory);
            }

            string fileName = DateTime.Now.ToString("yyyyMMddHHmmss") + ".txt";
            FileInfo inf = new FileInfo(logRootDirectory + fileName);
            StreamWriter wri = new StreamWriter(logRootDirectory + fileName, true, Encoding.UTF8, 1024);
            wri.WriteLine(strErrMsg);
            wri.Close();
        }


        public static void SaveLog(TxtLogType txtLogType ,string logTxt)
        {
            StreamWriter wri = null;
            try
            {
                string logRootDirectory = CommonConfig.TxtLogPath + GetTextLogChildPath(txtLogType);
                if (!Directory.Exists(logRootDirectory))
                {
                    Directory.CreateDirectory(logRootDirectory);
                }

                string fileName = DateTime.Now.ToString("yyyyMMdd") + ".txt";
                FileInfo inf = new FileInfo(logRootDirectory + fileName);
                wri = new StreamWriter(logRootDirectory + fileName, true, Encoding.UTF8, 1024);
                string tip = string.Format("{0}{1}{2}", "***************************\n", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "***************************");
                wri.WriteLine(tip);
                wri.WriteLine(logTxt);
                wri.WriteLine("");
            }
            catch
            {

            }
            finally
            {
                if (wri != null)
                {
                    wri.Close();
                }
            }
        }

        public static void SaveLog(TxtLogType txtLogType, TxtLogContentType txtLogContentType, TxtLogFileType txtLogFileType, string logTxt)
        {
            StreamWriter wri = null;
            try
            {
                string logRootDirectory = string.Format("{0}{1}{2}",CommonConfig.TxtLogPath,GetTextLogChildPath(txtLogType),GetTextLogContentChildPath(txtLogContentType));
                if (!Directory.Exists(logRootDirectory))
                {
                    Directory.CreateDirectory(logRootDirectory);
                }

                string fileName = GetFileName(txtLogFileType);
                FileInfo inf = new FileInfo(logRootDirectory + fileName);
                wri = new StreamWriter(logRootDirectory + fileName, true, Encoding.UTF8, 1024);
                string tip = string.Format("{0}{1}{2}", "***************************", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "***************************");
                wri.WriteLine(tip);
                wri.WriteLine(logTxt);
                wri.WriteLine("");
            }
            catch
            {

            }
            finally
            {
                if (wri != null)
                {
                    wri.Close();
                }
            }
        }

        private static string GetTextLogChildPath(TxtLogType txtLogType)
        {
            switch (txtLogType)
            {
                case TxtLogType.SystemInit: return "Init/";
                case TxtLogType.UPDService: return "UPD/";
                case TxtLogType.TCPService: return "TCP/";
                case TxtLogType.WeiXin: return "WeiXin/";
                case TxtLogType.WeiXinPay: return "WeiXinPay/";
                default: return string.Empty;
            }
        }

        private static string GetTextLogContentChildPath(TxtLogContentType txtLogContentType)
        {
            switch (txtLogContentType)
            {
                case TxtLogContentType.Common: return "Common/";
                case TxtLogContentType.Exception: return "Exception/";
                case TxtLogContentType.Debug: return "Debug/";
                default: return string.Empty;
            }
        }

        private static string GetFileName(TxtLogFileType txtLogFileType)
        {
            switch (txtLogFileType)
            {
                case TxtLogFileType.Day: return DateTime.Now.ToString("yyyyMMdd") + ".txt";
                case TxtLogFileType.Time: return string.Format("{0}{1}{2}{3}",DateTime.Now.ToString("yyyyMMddHHmmss"),"_",Utils.Number(6,false),".txt");
                default: return DateTime.Now.ToString("yyyyMMdd") + ".txt";
            }
        }
    }
}