using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XCCloudService.BLL.Base;
using XCCloudService.BLL.IBLL.XCCloud;
using XCCloudService.DAL.Container;
using XCCloudService.DAL.IDAL.XCCloud;
using XCCloudService.Model.XCCloud;

namespace XCCloudService.BLL.XCCloud
{
    public partial class Base_UserInfoService : BaseService<Base_UserInfo>, IBase_UserInfoService
    {
        public override void SetDal()
        {

        }

        public Base_UserInfoService()
            : this(false)
        {

        }

        public Base_UserInfoService(bool resolveNew)
        {
            Dal = DALContainer.Resolve<IBase_UserInfoDAL>(resolveNew: resolveNew);
        }
    }
}
