using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XCCloudService.Model.CustomModel.XCCloud
{
    public class Data_FoodInfoListModel
    {
        public int FoodID { get; set; }
        public string FoodName { get; set; }
        public string FoodTypeStr { get; set; }
        public string RechargeTypeStr { get; set; }
        public string AllowInternetStr { get; set; }
        public string AllowPrintStr { get; set; }
        public string ForeAuthorizeStr { get; set; }
        public string StartTimeStr { get; set; }
        public string EndTimeStr { get; set; }
    }
}
