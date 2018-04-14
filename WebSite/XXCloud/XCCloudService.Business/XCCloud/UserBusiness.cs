using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XCCloudService.BLL.Container;
using XCCloudService.BLL.IBLL.XCCloud;
using XCCloudService.Common;
using XCCloudService.Common.Enum;
using XCCloudService.Model.CustomModel.XCCloud.User;
using XCCloudService.Model.XCCloud;

namespace XCCloudService.Business.XCCloud
{
    public class UserBusiness
    {
        private static List<UserInfoCacheModel> listXcUser = null;

        public static List<UserInfoCacheModel> XcUserInfoList 
        {
            get 
            {
                if (listXcUser == null) XcUserInit();
                return listXcUser; 
            }
        }

        public static void XcUserInit()
        {
            IBase_UserInfoService base_UserInfoService = BLLContainer.Resolve<IBase_UserInfoService>();
            var list = base_UserInfoService.GetModels(p => p.UserType == (int)UserType.Xc).ToList();
            listXcUser = Utils.GetCopyList<UserInfoCacheModel, Base_UserInfo>(list);
        }

        public static bool IsEffectiveXcUser(string openId, out UserInfoCacheModel userInfoCacheModel)
        {
            userInfoCacheModel = null;
            if (XcUserInfoList.Any<UserInfoCacheModel>(p => p.OpenID.Equals(openId, StringComparison.OrdinalIgnoreCase)))
            {
                userInfoCacheModel = XcUserInfoList.Where(p => p.OpenID.Equals(openId, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
                return true;
            }

            return false;
        }        
    }
}
