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
    public partial class Base_MemberInfoService : BaseService<Base_MemberInfo>, IBase_MemberInfoService
    {
        public override void SetDal()
        {

        }

        public Base_MemberInfoService()
            : this(false)
        {

        }

        public Base_MemberInfoService(bool resolveNew)
        {

            Dal = DALContainer.Resolve<IBase_MemberInfoDAL>(resolveNew: resolveNew);

        }
    }
}
