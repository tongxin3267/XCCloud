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
    public partial class Base_UserGroupService : BaseService<Base_UserGroup>, IBase_UserGroupService
    {
        public override void SetDal()
        {

        }

        public Base_UserGroupService()
            : this(false)
        {

        }

        public Base_UserGroupService(bool resolveNew)
        {
            Dal = DALContainer.Resolve<IBase_UserGroupDAL>(resolveNew: resolveNew);

        }
    }
}
