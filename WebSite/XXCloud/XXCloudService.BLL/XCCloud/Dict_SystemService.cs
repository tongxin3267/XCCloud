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
    public partial class Dict_SystemService : BaseService<Dict_System>, IDict_SystemService
    {
        public override void SetDal()
        {

        }

        public Dict_SystemService()
            : this(false)
        {

        }

        public Dict_SystemService(bool resolveNew)
        {
            Dal = DALContainer.Resolve<IDict_SystemDAL>(resolveNew: resolveNew);
        }
    }
}
