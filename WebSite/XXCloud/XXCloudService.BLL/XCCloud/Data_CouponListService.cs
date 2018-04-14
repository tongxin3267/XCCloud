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
	public class Data_CouponListService : BaseService<Data_CouponList>, IData_CouponListService
	{
		public override void SetDal()
        {

        }

        public Data_CouponListService()
            : this(false)
        {

        }

        public Data_CouponListService(bool resolveNew)
        {
            Dal = DALContainer.Resolve<IData_CouponListDAL>(resolveNew: resolveNew);
        }
	} 
}