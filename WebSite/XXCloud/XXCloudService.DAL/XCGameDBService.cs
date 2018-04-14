using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace XCCloudService.DAL
{
    public class XCGameDBService
    {
        public static string GetConnString(string connstirng, string dbName)
        {
            Regex reg = new Regex("catalog=[^;](.*?);", RegexOptions.IgnoreCase);
            connstirng = reg.Replace(connstirng, "catalog=" + dbName + ";");
            return connstirng;
        }
    }
}
