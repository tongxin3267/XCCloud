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
	public class Base_StorageInfoService : BaseService<Base_StorageInfo>, IBase_StorageInfoService
	{
        public override void SetDal()
        {
        	
        }
        
        public Base_StorageInfoService()
        	: this(false)
        {
            
        }
        
        public Base_StorageInfoService(bool resolveNew)
        {
            Dal = DALContainer.Resolve<IBase_StorageInfoDAL>(resolveNew: resolveNew);
        }
	} 
}