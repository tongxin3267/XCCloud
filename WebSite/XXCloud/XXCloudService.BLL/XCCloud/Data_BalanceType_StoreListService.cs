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
	public class Data_BalanceType_StoreListService : BaseService<Data_BalanceType_StoreList>, IData_BalanceType_StoreListService
	{
        public override void SetDal()
        {
        	
        }
        
        public Data_BalanceType_StoreListService()
        	: this(false)
        {
            
        }
        
        public Data_BalanceType_StoreListService(bool resolveNew)
        {
            Dal = DALContainer.Resolve<IData_BalanceType_StoreListDAL>(resolveNew: resolveNew);
        }
	} 
}