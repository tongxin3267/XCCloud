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
	public class Data_Coupon_StoreListService : BaseService<Data_Coupon_StoreList>, IData_Coupon_StoreListService
	{
        public override void SetDal()
        {
        	
        }
        
        public Data_Coupon_StoreListService()
        	: this(false)
        {
            
        }
        
        public Data_Coupon_StoreListService(bool resolveNew)
        {
            Dal = DALContainer.Resolve<IData_Coupon_StoreListDAL>(resolveNew: resolveNew);
        }
	} 
}