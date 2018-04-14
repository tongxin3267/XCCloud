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
	public class Data_Member_Card_StoreService : BaseService<Data_Member_Card_Store>, IData_Member_Card_StoreService
	{
        public override void SetDal()
        {
        	
        }
        
        public Data_Member_Card_StoreService()
        	: this(false)
        {
            
        }
        
        public Data_Member_Card_StoreService(bool resolveNew)
        {
            Dal = DALContainer.Resolve<IData_Member_Card_StoreDAL>(resolveNew: resolveNew);
        }
	} 
}