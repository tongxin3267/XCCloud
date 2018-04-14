using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XCCloudService.Model.CustomModel.XCGame
{
    public class XCGameUserTokenModel
    {
        public XCGameUserTokenModel(string storeId, int userId, string userName, string realName, string mobile)
        {
            this.StoreId = storeId;
            this.UserId = userId;
            this.UserName = userName;
            this.RealName = realName;
            this.Mobile = mobile;
        }

        public string StoreId { set; get; }

        public int UserId { set; get; }

        public string UserName { set; get; }

        public string RealName { set; get; }

        public string Mobile { set; get; }
    }
}
