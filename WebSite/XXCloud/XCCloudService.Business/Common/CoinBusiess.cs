using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XCCloudService.Business.Common
{
    public class CoinBusiess
    {
        public static string GetCoinOpetionName(string action)
        {
            string actionName = string.Empty;
            switch (action)
            {
                case "1": actionName = "出币"; break;
                case "2": actionName = "存币"; break;
                default: actionName = ""; break;
            }
            return actionName;
        }
    }
}
