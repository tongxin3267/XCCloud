using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XCCloudService.DAL.Container;
using XCCloudService.DAL.IDAL.XCCloud;
using XCCloudService.BLL.Base;
using XCCloudService.BLL.IBLL.XCCloud;
using XCCloudService.Model.XCCloud;
namespace XCCloudService.BLL.XCCloud
{
	public class Flw_Order_DetailService : BaseService<Flw_Order_Detail>, IFlw_Order_DetailService
	{
		public override void SetDal()
        {

        }

        public Flw_Order_DetailService()
            : this(false)
        {

        }

        public Flw_Order_DetailService(bool resolveNew)
        {
            Dal = DALContainer.Resolve<IFlw_Order_DetailDAL>(resolveNew: resolveNew);
        }
	} 
}