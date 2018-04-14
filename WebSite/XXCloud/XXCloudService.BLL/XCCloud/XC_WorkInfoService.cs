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
    public partial class XC_WorkInfoService : BaseService<XC_WorkInfo>, IXC_WorkInfoService
    {
        public override void SetDal()
        {

        }

        public XC_WorkInfoService()
            : this(false)
        {

        }

        public XC_WorkInfoService(bool resolveNew)
        {

            Dal = DALContainer.Resolve<IXC_WorkInfoDAL>(resolveNew: resolveNew);

        }
    }
}
