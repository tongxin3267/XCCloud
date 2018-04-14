using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XCCloudService.Business.Common
{
    public class FilterMobileBusiness
    {
        private static List<string> mobileList = new List<string>();
        private static bool isTestSMS = false;
        public static void AddMobile(string mobile)
        { 
            mobileList.Add(mobile);
        }

        public static bool ExistMobile(string mobile)
        {
            return mobileList.Where(p => p.ToString().Equals(mobile)).Count() > 0;
        }

        public static bool IsTestSMS
        {
            get { return isTestSMS; }
            set { isTestSMS = value; }
        }
    }

    
}
