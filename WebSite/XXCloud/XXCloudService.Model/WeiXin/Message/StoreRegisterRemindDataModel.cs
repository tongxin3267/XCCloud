using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XCCloudService.Model.WeiXin.Message
{
    public class StoreRegisterRemindDataModel
    {
        public StoreRegisterRemindDataModel()
        { 
            
        }

        public StoreRegisterRemindDataModel(string merchAccount, string registerTime, string storeName, string workId)
        {
            this.MerchAccount = merchAccount;
            this.RegisterTime = registerTime;
            this.StoreName = storeName;
            this.WorkId = workId;
        }

        public string MerchAccount { set; get; }

        public string RegisterTime { set; get; }

        public string StoreName { set; get; }

        public string WorkId { set; get; }

    }
}
