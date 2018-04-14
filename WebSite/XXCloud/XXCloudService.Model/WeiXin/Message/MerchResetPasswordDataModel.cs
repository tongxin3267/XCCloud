using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XCCloudService.Model.WeiXin.Message
{
    public class MerchResetPasswordDataModel
    {
        public MerchResetPasswordDataModel()
        { 
            
        }

        public MerchResetPasswordDataModel(string userName, string password)
        {
            this.UserName = userName;
            this.Password = password;
        }

        public string UserName { set; get; }

        public string Password { set; get; }

    }
}
