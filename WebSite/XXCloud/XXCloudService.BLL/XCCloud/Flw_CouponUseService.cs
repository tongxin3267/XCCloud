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
	public class Flw_CouponUseService : BaseService<Flw_CouponUse>, IFlw_CouponUseService
	{
        public override void SetDal()
        {
        	
        }
        
        public Flw_CouponUseService()
        	: this(false)
        {
            
        }
        
        public Flw_CouponUseService(bool resolveNew)
        {
            Dal = DALContainer.Resolve<IFlw_CouponUseDAL>(resolveNew: resolveNew);
        }
	} 
}