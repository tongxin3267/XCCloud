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
    public partial class Base_MerchantInfoService : BaseService<Base_MerchantInfo>, IBase_MerchantInfoService
    {
        public override void SetDal()
        {

        }

        public Base_MerchantInfoService()
            : this(false)
        {

        }

        public Base_MerchantInfoService(bool resolveNew)
        {

            Dal = DALContainer.Resolve<IBase_MerchantInfoDAL>(resolveNew: resolveNew);

        }
    }
}
