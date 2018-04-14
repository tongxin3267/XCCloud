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
    public partial class Flw_Order_SerialNumberService : BaseService<Flw_Order_SerialNumber>, IFlw_Order_SerialNumberService
    {
        public override void SetDal()
        {

        }

        public Flw_Order_SerialNumberService()
            : this(false)
        {

        }

        public Flw_Order_SerialNumberService(bool resolveNew)
        {
            Dal = DALContainer.Resolve<IFlw_Order_SerialNumberDAL>(resolveNew: resolveNew);

        }
    }
 
}
