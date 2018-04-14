using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace XCCloudService.Common
{
    public class XCGameDB
    {
        private static string xcGameDBDefaultConnStr = System.Configuration.ConfigurationManager.ConnectionStrings["XCGameDBDefault"].ToString();
        public static string GetConnString(string dbName)
        {
            return string.Format(xcGameDBDefaultConnStr, dbName);
        }
    }
}