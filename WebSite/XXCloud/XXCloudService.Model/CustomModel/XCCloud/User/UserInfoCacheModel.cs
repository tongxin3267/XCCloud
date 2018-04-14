using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace XCCloudService.Model.CustomModel.XCCloud.User
{
    public class UserInfoCacheModel
    {
        public string OpenID { set; get; }

        public Nullable<int> UserID { set; get; }
        
    }
}
