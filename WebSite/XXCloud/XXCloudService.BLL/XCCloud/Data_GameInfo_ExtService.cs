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
	public class Data_GameInfo_ExtService : BaseService<Data_GameInfo_Ext>, IData_GameInfo_ExtService
	{
        public override void SetDal()
        {
        	
        }
        
        public Data_GameInfo_ExtService()
        	: this(false)
        {
            
        }
        
        public Data_GameInfo_ExtService(bool resolveNew)
        {
            Dal = DALContainer.Resolve<IData_GameInfo_ExtDAL>(resolveNew: resolveNew);
        }
	} 
}