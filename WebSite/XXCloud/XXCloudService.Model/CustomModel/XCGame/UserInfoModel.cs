using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XCCloudService.Model.CustomModel.XCGame
{
  public  class UserInfoModel
    {
        public UserInfoModel(string storeId,  string mobile)
        {
            this.StoreId = storeId;
    
            this.Mobile = mobile;
        }

        public string StoreId { set; get; }


        public string Mobile { set; get; }
    }
}
