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
    public partial class Base_UserGrantService : BaseService<Base_UserGrant>, IBase_UserGrantService
    {
        public override void SetDal()
        {

        }

        public Base_UserGrantService()
            : this(false)
        {

        }

        public Base_UserGrantService(bool resolveNew)
        {

            Dal = DALContainer.Resolve<IBase_UserGrantDAL>(resolveNew: resolveNew);

        }
    }
}
