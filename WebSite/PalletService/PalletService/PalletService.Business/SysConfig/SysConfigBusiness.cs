
using PalletService.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PalletService.Business.SysConfig
{
    public class SysConfigBusiness
    {
        public static void Init(string appStartPath)
        {
            ApplicationStartPath = appStartPath.Replace("\\bin","\\config");
            PrinterName = GetValue("/SysConfig/Printer/Name");
        }


        public static string ApplicationStartPath = string.Empty;

        public static string GetValue(string xmlPath)
        {
            string xmlFilePath = string.Format("{0}//{1}", ApplicationStartPath, "SysConfig.xml");
            return Utils.GetXmlNodeValue(xmlFilePath,xmlPath);
        }

        public static void SetValue(string xmlPath,string value)
        {
            string xmlFilePath = string.Format("{0}//{1}", ApplicationStartPath, "SysConfig.xml");
            Utils.SetXmlNodeValue(xmlFilePath,xmlPath, value);
        }

        public static string TCPHost = System.Configuration.ConfigurationSettings.AppSettings["TCPSocketServiceHost"].ToString();

        public static int TCPPort = int.Parse(System.Configuration.ConfigurationSettings.AppSettings["TCPSocketServicePort"].ToString());

        public static string PrinterName = "";

        public static string StoreId = "100025420106001";

        public static string XCCloudHost = "http://localhost:3288";
    }
}
