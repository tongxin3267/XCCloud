using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XCCloudService.Model.CustomModel.XCGame
{
    public class XCGameMemberTokenModel
    {
        public XCGameMemberTokenModel(string storeId, string mobile, string icCardId, string memberLevelName, string storeName,string EndTime)
        {
            this.StoreId = storeId;
            this.Mobile = mobile;
            this.ICCardId = icCardId;
            this.MemberLevelName = memberLevelName;
            this.StoreName = storeName;
            this.EndTime = EndTime;
        }

        public string StoreId { set; get; }

        public string Mobile { set; get; }

        public string ICCardId { set; get; }

        public string MemberLevelName { set; get; }

        public string StoreName { set; get; }
        public string EndTime { set; get; }
    }
}
