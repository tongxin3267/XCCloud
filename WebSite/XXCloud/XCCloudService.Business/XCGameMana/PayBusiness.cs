using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XCCloudService.Business.XCGameMana
{
    public class PayBusiness
    {
        public static int GetPaymentTypeId(string paymentTypeName)
        {
            switch (paymentTypeName)
            {
                case "现金":return 0;
                case "微信":return 1;
                case "支付宝":return 2;
                case "银联":return 3;
                default: return -1;
            }
        }
    }
}
