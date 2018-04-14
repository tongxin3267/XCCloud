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
    public partial class Base_StoreInfoService : BaseService<Base_StoreInfo>, IBase_StoreInfoService
    {
        public override void SetDal()
        {

        }

        public Base_StoreInfoService()
            : this(false)
        {

        }

        public Base_StoreInfoService(bool resolveNew)
        {

            Dal = DALContainer.Resolve<IBase_StoreInfoDAL>(resolveNew: resolveNew);

        }
    }
}
