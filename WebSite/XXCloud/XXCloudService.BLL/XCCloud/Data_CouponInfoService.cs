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
	public class Data_CouponInfoService : BaseService<Data_CouponInfo>, IData_CouponInfoService
	{
		public override void SetDal()
        {

        }

        public Data_CouponInfoService()
            : this(false)
        {

        }

        public Data_CouponInfoService(bool resolveNew)
        {
            Dal = DALContainer.Resolve<IData_CouponInfoDAL>(resolveNew: resolveNew);
        }
	} 
}