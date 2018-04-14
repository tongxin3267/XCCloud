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
	public class Flw_OrderService : BaseService<Flw_Order>, IFlw_OrderService
	{
		public override void SetDal()
        {

        }

        public Flw_OrderService()
            : this(false)
        {

        }

        public Flw_OrderService(bool resolveNew)
        {
            Dal = DALContainer.Resolve<IFlw_OrderDAL>(resolveNew: resolveNew);
        }
	} 
}