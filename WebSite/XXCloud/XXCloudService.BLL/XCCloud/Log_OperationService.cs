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
    public partial class Log_OperationService : BaseService<Log_Operation>, ILog_OperationService
    {
        public override void SetDal()
        {

        }

        public Log_OperationService()
            : this(false)
        {

        }

        public Log_OperationService(bool resolveNew)
        {

            Dal = DALContainer.Resolve<ILog_OperationDAL>(resolveNew: resolveNew);

        }
    }
}
