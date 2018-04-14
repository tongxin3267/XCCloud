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
	public class Dict_BalanceTypeService : BaseService<Dict_BalanceType>, IDict_BalanceTypeService
	{
        public override void SetDal()
        {
        	
        }
        
        public Dict_BalanceTypeService()
        	: this(false)
        {
            
        }
        
        public Dict_BalanceTypeService(bool resolveNew)
        {
            Dal = DALContainer.Resolve<IDict_BalanceTypeDAL>(resolveNew: resolveNew);
        }
	} 
}